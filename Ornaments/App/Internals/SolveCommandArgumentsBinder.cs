﻿using System.CommandLine;
using System.CommandLine.Binding;

namespace Ornaments.App.Internals;

internal class SolveCommandArgumentsBinder : BinderBase<SolveCommandArguments>
{
    private readonly Option<int> year;
    private readonly Option<int> day;
    private readonly Option<bool> dryRun;
    private readonly Option<IEnumerable<TokenType>> tokenTypes;
    private readonly Option<int> timeout;

    public SolveCommandArgumentsBinder(Option<int> year, Option<int> day, Option<bool> dryRun, Option<IEnumerable<TokenType>> tokenTypes, Option<int> timeout)
    {
        this.year = year;
        this.day = day;
        this.dryRun = dryRun;
        this.tokenTypes = tokenTypes;
        this.timeout = timeout;
    }

    protected override SolveCommandArguments GetBoundValue(BindingContext bindingContext)
    {
        return new(bindingContext.ParseResult.GetValueForOption(year),
            bindingContext.ParseResult.GetValueForOption(day),
            bindingContext.ParseResult.GetValueForOption(dryRun),
            bindingContext.ParseResult.GetValueForOption(tokenTypes) ?? Enumerable.Empty<TokenType>(),
            bindingContext.ParseResult.GetValueForOption(timeout));
    }
}
