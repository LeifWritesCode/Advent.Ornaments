using System.Diagnostics;

namespace Ornaments.Solutions.Structures;

public interface IMonkey
{
    long Inspections { get; }

    void Throw(IDictionary<int, IMonkey> monkeys, bool isPart1);
}

internal sealed class Monkey : IMonkey
{
    private readonly Queue<long> items;
    private readonly string operation;
    private readonly long rhsOperand;
    private readonly long test;
    private readonly int outcomeIfTrue;
    private readonly int outcomeIfFalse;

    public long Inspections { get; private set; } = 0;

    public Monkey(string operation, long rhsOperand, long test, int outcomeIfTrue, int outcomeIfFalse, params long[] items)
    {
        this.operation = operation;
        this.rhsOperand = rhsOperand;
        this.items = new Queue<long>(items);
        this.test = test;
        this.outcomeIfTrue = outcomeIfTrue;
        this.outcomeIfFalse = outcomeIfFalse;
    }

    private long Operate(long item)
    {
        return operation switch
        {
            "*" => item * rhsOperand,
            "+" => item + rhsOperand,
            "^" => item * item,
            "-" => item - rhsOperand,
            _ => throw new UnreachableException()
        };
    }

    public void Throw(IDictionary<int, IMonkey> monkeys, bool isPart1)
    {
        while (items.Any())
        {
            var item = items.Dequeue();
            var worry = isPart1 switch
            {
                true => Operate(item) / 3,
                // if part 2, no relief -- use chinese remainder theorem.
                false => Operate(item) % monkeys.Select(x => x.Value).Aggregate(1L, (a, v) => a * ((Monkey)v).test)
            };

            var where = worry % test == 0;
            var monkey = where switch
            {
                true => (Monkey)monkeys[outcomeIfTrue],
                false => (Monkey)monkeys[outcomeIfFalse],
            };
            monkey.Catch(worry);

            Inspections++;
        }
    }

    private void Catch(long item)
    {
        items.Enqueue(item);
    }
}
