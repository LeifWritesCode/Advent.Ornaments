﻿using Microsoft.Extensions.Options;
using Ornaments.Data;
using Ornaments.Data.Models;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Ornaments.Internals;

internal partial class AdventOfCodeClient
{
    private const string uriPrefixFragment = "{0}/day/{1}";
    private const string getInputUriFragment = uriPrefixFragment + "/input";
    private const string postAnswerUriFragment = uriPrefixFragment + "/answer";

    private static readonly Regex title = GetTitleRegex();
    private static readonly Regex rightAnswer = GetRightAnswerRegex();
    private static readonly Regex wrongAnswer = GetWrongAnswerRegex();

    private readonly IHttpClientFactory httpClientFactory;
    private readonly OrnamentsContext ornamentsContext;
    private readonly TokenOptions tokenOptions;

    public AdventOfCodeClient(IHttpClientFactory httpClientFactory, OrnamentsContext ornamentsContext, IOptions<TokenOptions> tokenOptions)
    {
        ArgumentNullException.ThrowIfNull(httpClientFactory, nameof(httpClientFactory));
        ArgumentNullException.ThrowIfNull(tokenOptions, nameof(tokenOptions));
        ArgumentNullException.ThrowIfNull(tokenOptions.Value, nameof(tokenOptions.Value));

        this.httpClientFactory = httpClientFactory;
        this.tokenOptions = tokenOptions.Value;
        this.ornamentsContext = ornamentsContext;
    }

    public async Task<Challenge> GetChallengeAsync(int year, int day)
    {
        // Check if we've already seen this one before.
        var dbChallenge = ornamentsContext.Challenges.FirstOrDefault(x => x.Year == year && x.Day == day);
        if (dbChallenge is not null)
            return dbChallenge;

        // Scrape!
        var httpClient = httpClientFactory.CreateClient(nameof(AdventOfCodeClient));
        var endpoint = string.Format(uriPrefixFragment, year, day);
        var response = await httpClient.GetAsync(endpoint);

        // Handle the response and return to sender.
        return response.StatusCode switch
        {
            HttpStatusCode.OK => await ParseChallengeAsync(response.Content, year, day),
            HttpStatusCode.NotFound => throw new ArgumentException($"the challenge {day}.12.{year} doesn't exist."),
            _ => throw new InvalidOperationException("bad response from server")
        };
    }

    public async Task<Input> GetInputAsync(TokenType tokenType, int year, int day)
    {

        // Ensure matching challenge + identity entities have been created.
        var dbChallenge = await GetChallengeAsync(year, day);
        var dbIdentity = await GetIdentityAsync(tokenType);

        // Scrape!
        var httpClient = httpClientFactory.CreateClient(dbIdentity.Provider.ToString());
        var endpoint = string.Format(getInputUriFragment, year, day);
        var response = await httpClient.GetAsync(endpoint);

        // Handle the response and return to sender.
        return response.StatusCode switch
        {
            HttpStatusCode.OK => await AddInputToDatabaseAsync(new Input
            {
                Identity = dbIdentity,
                Challenge = dbChallenge,
                Value = await response.Content.ReadAsStringAsync()
            }),
            _ => throw new InvalidOperationException("bad response from server")
        };
    }

    public async Task<Submission> PostSubmissionAsync(TokenType tokenType, int year, int day, int level, object answer)
    {
        // If the token isn't present in the config then we can't/won't attempt the submission.
        // We need the token in the http context, and its absence from the config means we are absent an appropriately configured http client.
        if (!tokenOptions.TryGet(tokenType, out string token))
            throw new InvalidOperationException();

        // Get the matching input. If it doesn't exist, it's an error.
        var dbChallenge = ornamentsContext.Challenges.FirstOrDefault(x => x.Year == year && x.Day == day);
        if (dbChallenge is null)
            throw new InvalidOperationException("can't push to an unknown challenge");
        var dbIdentity = ornamentsContext.Identities.FirstOrDefault(x => x.Provider == tokenType && x.Token == token);
        if (dbIdentity is null)
            throw new InvalidOperationException("can't push with an unknown identity");
        var dbInput = ornamentsContext.Inputs.FirstOrDefault(x => x.Challenge == dbChallenge && x.Identity == dbIdentity);
        if (dbInput is null)
            throw new InvalidOperationException("can't push with no matching input");

        // Submit!
        var httpClient = httpClientFactory.CreateClient(token.ToString());
        var endpoint = string.Format(postAnswerUriFragment, year, day);
        var response = await httpClient.PostAsJsonAsync(endpoint, new { level, answer });

        // Handle the response and return to sender.
        return response.StatusCode switch
        {
            HttpStatusCode.OK => await ParseSubmissionResponseAsync(response.Content, dbChallenge, dbInput, answer),
            _ => throw new InvalidOperationException("bad response from server")
        };
    }

    private async Task<Challenge> ParseChallengeAsync(HttpContent httpContent, int year, int day)
    {
        var content = await httpContent.ReadAsStringAsync();
        var name = title.Match(content);
        if (!name.Success)
            throw new UnreachableException("the website format has changed, how unexpected...");

        return await AddChallengeToDatabaseAsync(new Challenge
        {
            Year = year,
            Day = day,
            Name = name.Groups["name"].Value,
            Description = "nothing here yet"
        });
    }

    private async Task<Submission> ParseSubmissionResponseAsync(HttpContent httpContent, Challenge challenge, Input input, [NotNull] object answer)
    {
        return await AddSubmissionToDatabaseAsync(new Submission
        {
            Challenge = challenge,
            Input = input,
            Response = rightAnswer.IsMatch(await httpContent.ReadAsStringAsync()) ? Response.Correct : Response.Incorrect,
            DateTime = DateTime.UtcNow,
            Answer = (string)answer
        });
    }

    private async Task<Identity> GetIdentityAsync(TokenType tokenType)
    {
        // If the token isn't present in the config then we can't/won't attempt the scrape.
        // We need the token in the http context, and its absence from the config means we are absent an appropriately configured http client.
        if (!tokenOptions.TryGet(tokenType, out string token))
            throw new InvalidOperationException();

        var dbIdentity = ornamentsContext.Identities.FirstOrDefault(x => x.Provider == tokenType && x.Token == token);
        dbIdentity ??= await AddIdentityToDatabaseAsync(new Identity()
        {
            Provider = tokenType,
            Token = token,
        });

        return dbIdentity;
    }

    private async Task<Input> AddInputToDatabaseAsync(Input input)
    {
        ornamentsContext.Inputs.Add(input);
        await ornamentsContext.SaveChangesAsync();
        return input;
    }

    private async Task<Challenge> AddChallengeToDatabaseAsync(Challenge challenge)
    {
        ornamentsContext.Challenges.Add(challenge);
        await ornamentsContext.SaveChangesAsync();
        return challenge;
    }

    private async Task<Identity> AddIdentityToDatabaseAsync(Identity identity)
    {
        ornamentsContext.Identities.Add(identity);
        await ornamentsContext.SaveChangesAsync();
        return identity;
    }

    private async Task<Submission> AddSubmissionToDatabaseAsync(Submission submission)
    {
        ornamentsContext.Submissions.Add(submission);
        await ornamentsContext.SaveChangesAsync();
        return submission;
    }

    [GeneratedRegex("---\\sDay\\s\\d{1,2}:\\s(?<name>[\\w\\s]+)\\s---", RegexOptions.Compiled)]
    private static partial Regex GetTitleRegex();

    [GeneratedRegex("That's the", RegexOptions.Compiled)]
    private static partial Regex GetRightAnswerRegex();

    [GeneratedRegex("That's not", RegexOptions.Compiled)]
    private static partial Regex GetWrongAnswerRegex();
}