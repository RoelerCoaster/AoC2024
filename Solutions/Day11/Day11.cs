using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day11;

internal class Day11 : DayBase
{
    public override int Day => 11;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var stones = input.NumbersBySeparator<long>(" ");

        var memoizedCountStonesGeneratedByValue = RecursionUtil.Memoize<StoneCountArgs, long>(CountStonesGeneratedByValue);

        var result = 0L;

        foreach (var stone in stones)
        {
            result += memoizedCountStonesGeneratedByValue.Invoke(new(stone, 25));
        }

        return result.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var stones = input.NumbersBySeparator<long>(" ");

        var memoizedCountStonesGeneratedByValue = RecursionUtil.Memoize<StoneCountArgs, long>(CountStonesGeneratedByValue);

        var result = 0L;

        foreach (var stone in stones)
        {
            result += memoizedCountStonesGeneratedByValue.Invoke(new(stone, 75));
        }

        return result.ToString();
    }

    private static long CountStonesGeneratedByValue(Func<StoneCountArgs, long> recurse, StoneCountArgs args)
    {
        if (args.LoopsLeft == 0)
        {
            return 1;
        }

        if (args.Value == 0)
        {
            return recurse.Invoke(new(1, args.LoopsLeft - 1));
        }
        var numDigits = args.Value.NumberOfDigits();

        if (numDigits % 2 == 0)
        {
            var (div, rem) = Math.DivRem(args.Value, (int)Math.Pow(10, numDigits / 2));
            return recurse.Invoke(new(div, args.LoopsLeft - 1))
                + recurse.Invoke(new(rem, args.LoopsLeft - 1));
        }


        return recurse.Invoke(new(args.Value * 2024, args.LoopsLeft - 1));

    }

    private record struct StoneCountArgs(long Value, int LoopsLeft);
}
