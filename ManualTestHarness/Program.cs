using Ornaments;
using Ornaments.Algorithms;
using Ornaments.Algorithms.PathFinding;
using Ornaments.App;
using Ornaments.Solutions;
using System.Collections.Concurrent;

namespace ManualTestHarness;

[RegisterOrnament("SomeChallenge", 2022, 1)]
internal sealed class SomeSolution : ISolution
{
    public SomeSolution() {}

    public async Task<object> DoPartOneAsync(ISolutionContext solutionContext)
    {
        var grid = solutionContext.As<Grid<char>>();
        var start = grid.First('S');
        var end = grid.First('E');
        var astar = Algorithm.Create(PathFindingAlgorithm.AStar);
        var path = astar.Find(grid, (a, b) =>
        {
            var a2 = a.Value == 'S' ? 'a' : a.Value;
            var b2 = b.Value == 'E' ? 'z' : b.Value;
            var delta = b2 - a2;
            return delta < 2;
        }, (start.X, start.Y), (end.X, end.Y));
        return path.Count();
    }

    public async Task<object> DoPartTwoAsync(ISolutionContext solutionContext)
    {
        var grid = solutionContext.As<Grid<char>>();
        var starts = grid
            .Where(x => x.Value == 'a');
            //.Where(x => grid.Adjacents(x).Any(x => x.Value == 'b'));
        var end = grid.First('E');
        var astar = Algorithm.Create(PathFindingAlgorithm.AStar);

        var counts = new ConcurrentBag<int>();
        var then = DateTime.UtcNow;
        Parallel.For(0, starts.Count(), index =>
        {
            var start = starts.ElementAt(index);
            Console.WriteLine($"testing start #{index+1} -- ({start.X}, {start.Y})");
            try
            {
                var path = astar.Find(grid, (a, b) =>
                {
                    var a2 = a.Value == 'S' ? 'a' : a.Value;
                    var b2 = b.Value == 'E' ? 'z' : b.Value;
                    var delta = b2 - a2;
                    return delta < 2;
                }, (start.X, start.Y), (end.X, end.Y));
                counts.Add(path.Count());
            }
            catch (Exception)
            {
                Console.WriteLine($"no viable path from start #{index + 1}");
            }
        });
        var now = DateTime.UtcNow;
        var delta = now - then;
        Console.WriteLine($"codethulu took {delta.TotalSeconds} to assess {starts.Count()} candidate positions.");

        return counts.Min();
    }

    public bool TryParse(string input, out object parsed)
    {
        input = "abccccccccaaaaccccaaacaccccaaaaaacccccccccccaaaccccccccccaaaaaaacccccccccccccccccccccccccccccacaaaccccccccccccccccccccccccccccccccccccccccaaaaa\r\nabccccccccaaaaccaaaaaaaacccaaaaaacccccccccccaaaacccccccccaaaaaaaaaacccccccccccccccaaccccccccaaaaaccaaaccaacccccccccccccccccccccccccccccccaaaaaa\r\nabcccccccccaacccaaaaaaaaccccaaaaacccccccccccaaaacccccccaaaaaaaaaaaaaccccccccccaaaaaaccccccccaaaaaaaaaaaaaaccccccccccccccccaaaccccccccccccaaaaaa\r\nabcccccccccccccccaaaaaccccccaacaaccccaacccccaaacccccccaaaaaaaaaaaaaaccccccccccaaaaaaacccccccccaaaaacaaaaaaccccccccccccccccaaccccccccccccccccaaa\r\nabccccccccccccccccaaaaaccccccccccaaccaacccccccccccccccaaaaacaaaacacacccccaacccaaaaaaaacccccccaaaaacaaaaaaaccccccccccccccccaaacccccccccccccccaaa\r\nabcccccccccccccccaaaaaaccccccccccaaaaaaccccccccccccccccaaaaaaaacaaaaacaaaaaccccaaaaaaacccccccaacaacaaaaaaaaccccccccaaaaccaakcccccccccccccccccaa\r\nabcccccccccccccccaaaccacccccccccccaaaaaaacccccccaaaccccccaaaaaacaaaaaccaaaaaccaaaaaaccccccccccccaacaaaaaaaacccccccccaaaakkkklllcccccccccccccccc\r\nabcccccaaaccccccccccccccccccccccccaaaaaaacccccccaaacacccaaaaaaaaaaaaacaaaaaaccaaaaaacccccccccccccccccaaaccccccccccccaaakkkkkllllcccccccaacccccc\r\nabccccaaaacccccccccccccccccccccccaaaaaacccccccaccaaaaaccaaaaaaaaaaaaacaaaaccccccccaaccccccccccccccccccaaccccccccccccckkkkkkpllllccccaaaaaaccccc\r\nabccccaaaacccccccccccccccccaaacccaaaaaacccccccaaaaaaaacccaaaaacaaaaaacccaaaccccccccccccccccccccccccccccccccccccccccckkkkpppppplllcccaaaaacccccc\r\nabcccccaaaccccccccccccccccaaaacccccccaaccccccccaaaaacccccaaaccccaaacccccccccccccccccccccccccaaccccccccccccccccccjjjkkkkpppppppplllcccaaaaaacccc\r\nabccccccccccccccccccccccccaaaaccccccccccccccccccaaaaacccccccccccccccccccccccccccccccccccccccaaaaaccccccccccccjjjjjjkkkrppppuppplllccccaaaaacccc\r\nabccccccccccccccaaaccccccccaaaccccccccccccccccccaacaaccccccccccccccccccccccaaaccccccccaacccaaaaaccccccccccccjjjjjjjjrrrpuuuuuppplllcccccaaacccc\r\nabcccccaaccaacccaaacacccccccccccccccccccccccccacaaaaccccccccccccccccccccccaaaaaaccccaaaacccaaaaaaccaccccccccjjjrrrrrrrrruuuuuppplllmcccddcccccc\r\nabcccccaaaaaacaaaaaaaaccccccccccccccccccccccccaacaaaccccccccccccccccccccccaaaaaaccccaaaaaacccaaaaccaaacaaacjjjrrrrrrrrruuuxuuupqqlmmmmddddccccc\r\nabcccccaaaaaccaaaaaaaaccccccccccccccccccccccccaaaaaccccaacccccccccccccccccaaaaaacccccaaaacccaacccccaaaaaaacjjjrrrrtuuuuuuxxyuvqqqqmmmmmddddcccc\r\nabaacccaaaaaaccaaaaaccccccccccccccccccaaaaccccaaaaaaccaaaccccccccccccccccccaaaaaccccaaaaaccccccccccaaaaaaccjjjrrrtttuuuuuxxyvvvqqqqqmmmmdddcccc\r\nabaaccaaaaaaaaccaaaaaccccccccccccccccaaaaaaaaaaaaaaaacaaacaaaccccccccccccccaacaacaacaacaaccccccccaaaaaaaaccijjqqrtttxxxxxxyyvvvvvqqqqmmmmdddccc\r\nabaaccaaaaaaaacaaaaaaccccccccccccccccaaaaaaaaaaaaaaaaaaaaaaaacccccccccccccccaaacaaaccccccccccaaccaaaaaaaaaciiiqqqttxxxxxxxyyvvvvvvvqqqmmmdddccc\r\nabaaaccccaaccccaaaccacccccccccccccccccaaaaaaccccaacaaaaaaaaaaccaaaccccccccccaaaaaaaccccccccccaaaaaaaaaaaaaaiiiqqqtttxxxxxxyyyyyyvvvqqqmmmdddccc\r\nSbaaaccccaacccccccccccccccccccccccccaaaaaaaaccccaacccaaaaaaccaaaaaaccccccccccaaaaaacccccccccccaaaaacaaacaaaaiiiqqqttxxxxEzzyyyyyvvvqqqmmmdddccc\r\nabaaaccccccccccccccccccccccccccccccaaaaaaaaaaccccccccaaaaaaccaaaaaaccccccccccaaaaaaaaccccccccaaaaaacaaaccaaaiiiqqqtttxxxyyyyyyvvvvqqqmmmdddcccc\r\nabaccccccaacccccccccccccccccccccccccaaaaaaaaaaaacccccaaaaaaacaaaaaacccccccccaaaaaaaaacccccccaaaaaaaacaaaaaaaiiiqqqtttxxyyyyyyvvvvqqqqnnmeddcccc\r\nabccccccaaaaccccccccccccaaaccccccccccccaaaaaaaaaaacccaaacaaacaaaaacccccccccaaaaaaaaaacccccccaaaaaaaaccaaaaaaaiiiqqtttxxyyyyyywvrrrrnnnneeeccccc\r\nabccccccaaaacccccaacccccaaaacccccccccccaaaccaaaaaacccacccccccaaaaacccccccccaaacaaacccccccccccccaacccccccaaaaaiiqqqttxxwywwyyywwrrrnnnneeeeccccc\r\nabccccccaaaaccaacaaaccccaaaaccccccaacccaacccaaaaaccccccccccccccccccccccccccccccaaacccccccccccccaaccccccaaaaaaiiqqqttwwwwwwwwywwrrrnnneeeecccccc\r\nabccccccccccccaaaaacccccaaaccccacaaacccccccccaaaaacccccccccccccccccccccccccccccaaacccccccccccccccccccccaaaaaaiiqqpsswwwwsswwwwwrrnnneeeeccccccc\r\nabcccccccccccccaaaaaacccccccccaaaaacaacccccccaacaacccccaaccccccccccccccccccccccccccccccccccccccccccccccaccaahhhpppssssssssswwwwrrnnneeeaccccccc\r\nabcccccccccccaaaaaaaacccccccccaaaaaaaacccccaaccccccaaacaaccccccccccccccccccccccccccccccccccccaaaccaccccccccchhhpppsssssssssswwrrrnnneeeaaaacccc\r\nabcccccccccccaaaaacaacccccccccccaaaaaccaaaaaaccccccaaaaaaccccccccccccccccccccccccccccaaccaaccaaaaaacccccccccchhpppppsspppossrrrrrnnneeeaaaacccc\r\nabccccccccccccacaaaccccccccccccaaaaacccaaaaaaaacccccaaaaaaaccaaaccccccccaaaacccccccccaaaaaacccaaaaacccccccccchhhpppppppppoosrrrroonffeaaaaacccc\r\nabccccccccccccccaaaccccccccccccaacaaaccaaaaaaaaccccaaaaaaaaacaaaccccccccaaaacccccccccaaaaaccaaaaaaacccccccccchhhhpppppppoooooooooonfffaaaaccccc\r\nabcccccccccccccccccccccccaaaccccccaaccccaaaaaaaccccaaaaaaaaaaaaaaaccccccaaaacccccccccaaaaaacaaaaaaaacccccaacchhhhhhhhgggggoooooooofffaaaaaacccc\r\nabcccccccccccccccccccccccaaaaacccaacccccaaaaaccccccaaaaaacaaaaaaaaccccccaaacccccccccaaaaaaaaaaaaaaaaccccaaacccchhhhhgggggggooooooffffaaaaaacccc\r\nabccaaacccccccccccccccccaaaaaaccaaaccccaaaaaacccccccccaaacccaaaaacccccccccccccccccccaaaaaaaaccaaacacccccaaacaaachhhggggggggggfffffffcaacccccccc\r\nabcaaaaccccccccccaacccccaaaaaaccaaacaaaccccaaccccccccccccccaaaaacccccccccccccccccccccccaacccccaaaccccaaaaaaaaaacccccccccaagggffffffcccccccccccc\r\nabcaaaaccccccccccaaccccccaaaaaaaaaaaaaaccccccccccccccccccccaaaaaaccccccccccccccccccccccaaccccccccccccaaaaaaaaaccccccccccaaacgfffffcccccccccccaa\r\nabccaaacccccccaaaaaaaacccaaaaaaaaaaaaaaccccccccaaaaaccaaaaaaaaaaaaaacaacccccccccaaaccacccccccccccccccccaaaaacccccccccccaaaaccccccccccccccccccaa\r\nabccccccccccccaaaaaaaacccccccccaaaaaaccccccccccaaaaaccaaaaaaaaaaaaaacaaaaaacccccaaaaaacccccccccccccccccaaaaaacccccccccccaacccccccccccccccacacaa\r\nabccccccccccccccaaaacccccccccccaaaaaacccccccccaaaaaacccaaaaaaaaaaaaacaaaaaacccccaaaaaacccccccccccccccccaaaaaaaccccccccccaacccccccccccccccaaaaaa\r\nabcccccccccccccaaaaaaccccccccccaaaaaaaccccccccaaaaaaccccccaaaaaacccaaaaaaaaccccaaaaaaaaccccccccccccccccaaacaaacccccccccccccccccccccccccccaaaaaa";
        //input = "Sabqponm\r\nabcryxxl\r\naccszExk\r\nacctuvwj\r\nabdefghi";
        var lines = input.ReplaceLineEndings().Split(Environment.NewLine);
        parsed = new Grid<char>(lines);
        return true;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        OrnamentsApp.CreateDefault().RunAsync(args).GetAwaiter().GetResult();
    }
}