using Ornaments.Data.Models;
using Ornaments.Extensions;
using Ornaments.Graphs;
using Ornaments.IO.InMemory;
using Ornaments.Solutions;
using Ornaments.Solutions.Structures;
using System.Text.RegularExpressions;

namespace Ornaments.Text;

/// <summary>
/// <see cref="String"/> extension methods for parsing inputs.
/// </summary>
public static class StringParsingExtensions
{
    /// <summary>
    /// Splits the input into lines, respecting OS line endings.
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>The original string, trimmed, with line endings replaced, and split on the OS default line ending.</returns>
    public static string[] AsDefaultStringArray(this string self)
    {
        return self.Trim().ReplaceLineEndings().Split(Environment.NewLine);
    }

    /// <summary>
    /// Parses the input string for 2022.12.1, "Calorie Counting."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>An enumerable of blocks of integers, themselves as enumerables.</returns>
    public static IEnumerable<IEnumerable<int>> AsCalorieCountingInput(this string self)
    {
        return self
            .Trim()
            .ReplaceLineEndings()
            .Split($"{Environment.NewLine}{Environment.NewLine}", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(Environment.NewLine).Select(x => x.ToInt()));
    }

    /// <summary>
    /// Parses the input string for 2022.12.2, "Rock Paper Scissors."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>The original string, trimmed, with line endings replaced, spaces removed, and split on the OS default line ending.</returns>
    /// <remarks>
    /// The resulting format is conducive to lookup table usage.
    /// </remarks>
    public static string[] AsRockPaperScissorsInput(this string self)
    {
        return self.Trim().ReplaceLineEndings().Replace(" ", string.Empty).Split(Environment.NewLine);
    }

    /// <summary>
    /// Parses the input string for 2022.12.5, "Supply Stacks."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>The original input, split into lines, with "blank" ([-]) stack items inserted to normalise each line.</returns>
    public static string[] AsSupplyStacksInput(this string self)
    {
        return self
            .Replace("    ", "[-] ")       // replace groups of four spaces with a blank stack item so that we have even height inputs
            .Replace("  ", " ")            // cleanup double spaces
            .Replace("][", "] [")          // cleanup none spaces
            .ReplaceLineEndings()          // then replace the line endings as normal
            .Split(Environment.NewLine);
    }

    /// <summary>
    /// Parses the input string for 2022.12.7, "No Space Left On Device."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>An in-memory directory structure representing the input string.</returns>
    public static INode AsNoSpaceLeftOnDeviceInput(this string self)
    {
        var isFilePattern = "^(?<size>\\d+)\\s(?<name>[\\w\\.]+)$";
        var real = self.AsDefaultStringArray();
        var enumerator = real.GetEnumerator();
        DirectoryNode? pwd = default; // at end of execution, this will be the root
        while (enumerator.MoveNext())
        {
            var node = (string)enumerator.Current;
            if (node.StartsWith("$ cd "))
            {
                var where = node["$ cd ".Length..];
                switch (where)
                {
                    case "..":
                        // we're traversing up to the parent
                        // parent is always a directory
                        ArgumentNullException.ThrowIfNull(pwd);
                        ArgumentNullException.ThrowIfNull(pwd.Parent);
                        pwd = pwd.Parent as DirectoryNode;
                        break;
                    default:
                        // we're traversing down, so change the current directory
                        // if this is the root directory, it's parent is null
                        var directory = new DirectoryNode(where, pwd!);
                        pwd?.Children.Add(directory);
                        pwd = directory;
                        break;
                }
            }
            else
            {
                var match = Regex.Match(node, isFilePattern);
                if (!match.Success)
                {
                    continue;
                }

                var name = match.Groups["name"].Value;
                var size = match.Groups["size"].Value.ToInt();
                ArgumentNullException.ThrowIfNull(pwd);
                var child = new FileNode(name, size, pwd);
                pwd?.Children.Add(child);
            }
        }

        // quick traversal back to the top
        var result = pwd;
        while (result?.Parent is not null)
        {
            result = result.Parent as DirectoryNode;
        }

        ArgumentNullException.ThrowIfNull(result);
        return result;
    }

    /// <summary>
    /// Parses the input string for 2022.12.8, "Treetop Treehouse."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>A two dimensional grid of trees, where the value of each cell is its height.</returns>
    public static Grid<int> AsTreetopTreehouseInput(this string self)
    {
        return new Grid<int>(self.AsDefaultStringArray());
    }

    /// <summary>
    /// Parses the input string for 2022.12.11, "Monkey In The Middle."
    /// </summary>
    /// <param name="self">An input string.</param>
    /// <returns>An indexed collection of monkeys.</returns>
    public static IDictionary<int, IMonkey> AsMonkeyInTheMiddleInput(this string self)
    {
        var blocks = self.ReplaceLineEndings().Split("Monkey ").Where(x => !string.IsNullOrWhiteSpace(x));
        var counter = 0;
        var dictionary = new Dictionary<int, IMonkey>();
        foreach (var block in blocks)
        {
            var operation = string.Empty;
            var rhsOperand = 0L;
            var test = 0L;
            var outcomeIfTrue = 0;
            var outcomeIfFalse = 0;
            var items = Array.Empty<long>();

            var enumerator = block.Split(Environment.NewLine).GetEnumerator();
            while (enumerator.MoveNext())
            {
                var line = ((string)enumerator.Current).Trim();
                if (line.StartsWith("Monkey"))
                {
                    continue;
                }
                else if (line.StartsWith("Starting items: "))
                {
                    items = line["Starting items: ".Length..].Split(',').Select(x => long.Parse(x)).ToArray();
                }
                else if (line.StartsWith("Operation: new = old "))
                {
                    var parts = line["Operation: new = old ".Length..].Split(' ');
                    operation = parts.First();
                    if (!long.TryParse(parts.Last(), out rhsOperand))
                    {
                        operation = "^";
                        rhsOperand = 0L;
                    }
                }
                else if (line.StartsWith("Test: divisible by "))
                {
                    test = long.Parse(line["Test: divisible by ".Length..]);
                }
                else if (line.StartsWith("If true: throw to monkey "))
                {
                    outcomeIfTrue = int.Parse(line["If true: throw to monkey ".Length..]);
                }
                else if (line.StartsWith("If false: throw to monkey "))
                {
                    outcomeIfFalse = int.Parse(line["If false: throw to monkey ".Length..]);
                }
            }

            dictionary.Add(counter, new Monkey(operation, rhsOperand, test, outcomeIfTrue, outcomeIfFalse, items));
            counter++;
        }

        return dictionary;
    }
}
