using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using Spectre.Console;
using System.Collections.Concurrent;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day07;

internal class Day07 : DayBase
{
    public override int Day => 7;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var operators = new[] { Add, Multiply };
        return FindCalibrationResult(input, operators);
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var operators = new[] { Add, Multiply, Concat };
        return FindCalibrationResult(input, operators);
    }

    private static string FindCalibrationResult(string input, Func<long, long, long>[] operators)
    {
        var equations = input.Lines()
                    .Select(line => line.Split(": "))
                    .Select(split => new UnfilledEquation(
                        split[0].ToNumber<long>(),
                        new LinkedList<long>(split[1].NumbersBySeparator<long>(" "))
                    ))
                    .ToList();

        var testValues = new ConcurrentBag<long>();

        Parallel.ForEach(equations, e =>
        {
            if (CanBeMadeTrue(e, operators))
            {
                testValues.Add(e.TestValue);
            }
        });

        return testValues.Sum().ToString();
    }

    private static bool CanBeMadeTrue(UnfilledEquation equation, IReadOnlyCollection<Func<long, long, long>> operators)
    {
        var left = equation.Operands.First!;

        var right = left.Next;

        if (right == null)
        {
            return left.Value == equation.TestValue;
        }

        equation.Operands.Remove(left);
        equation.Operands.Remove(right);

        var result = false;
        foreach (var op in operators)
        {
            equation.Operands.AddFirst(op(left.Value, right.Value));
            result = CanBeMadeTrue(equation, operators);
            equation.Operands.RemoveFirst();

            if (result)
            {
                break;
            }
        }

        equation.Operands.AddFirst(right);
        equation.Operands.AddFirst(left);

        return result;
    }

    private static long Add(long a, long b) => a + b;
    private static long Multiply(long a, long b) => a * b;
    private static long Concat(long a, long b) => a * (int)Math.Pow(10, b.NumberOfDigits()) + b;


    private record UnfilledEquation(long TestValue, LinkedList<long> Operands);
}
