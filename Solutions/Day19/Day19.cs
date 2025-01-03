using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day19;

internal class Day19 : DayBase
{
    public override int Day => 19;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var patterns = sections[0].Split(", ").ToHashSet();
        var designs = sections[1].Lines();

        var memoizedIsPatternPossible = RecursionUtil.Memoize<ComputationArgs, bool>(IsPatternPossible, new KeyEqualityComparer<ComputationArgs, string>(a => a.RemainingPattern));

        var possible = designs.Where(d => memoizedIsPatternPossible(new(d, patterns))).ToList();

        return possible.Count.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        var patterns = sections[0].Split(", ").ToHashSet();
        var designs = sections[1].Lines();

        var memoizedNumberOfDesigns = RecursionUtil.Memoize<ComputationArgs, long>(NumberOfDesigns, new KeyEqualityComparer<ComputationArgs, string>(a => a.RemainingPattern));

        var numberOfDesigns = designs.Select(d => memoizedNumberOfDesigns(new(d, patterns))).ToList();

        return numberOfDesigns.Sum().ToString();
    }

    private static bool IsPatternPossible(Func<ComputationArgs, bool> recurse, ComputationArgs args)
    {
        if (args.RemainingPattern.Length == 0)
        {
            return true;
        }

        for (var i = 1; i <= args.RemainingPattern.Length; i++)
        {
            var (left, right) = args.RemainingPattern.SplitBefore(i);
            if (args.Patterns.Contains(left) && recurse(args with { RemainingPattern = right }))
            {
                return true;
            }
        }

        return false;
    }
    private static long NumberOfDesigns(Func<ComputationArgs, long> recurse, ComputationArgs args)
    {
        if (args.RemainingPattern.Length == 0)
        {
            return 1;
        }

        var total = 0L;
        for (var i = 1; i <= args.RemainingPattern.Length; i++)
        {
            var (left, right) = args.RemainingPattern.SplitBefore(i);
            if (args.Patterns.Contains(left))
            {
                total += recurse(args with { RemainingPattern = right });
            }
        }

        return total;
    }

    private record struct ComputationArgs(string RemainingPattern, HashSet<string> Patterns);

}
