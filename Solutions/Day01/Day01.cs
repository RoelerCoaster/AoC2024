using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day01;

internal class Day01 : DayBase
{
    public override int Day => 1;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var (first, second) = GetLists(input);

        var secondOrdered = second.Order().ToArray();

        var differenceSum = first.Order()
            .Select((id, index) => Math.Abs(id - secondOrdered[index]))
            .Sum();

        return differenceSum.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var lists = GetLists(input);

        var secondListCounts = lists.Second.CountBy(x => x).ToDictionary();

        var similarity = lists.First
            .Select(id => secondListCounts.TryGetValue(id, out var count) ? id * count : 0)
            .Sum();

        return similarity.ToString();
    }

    private static (int[] First, int[] Second) GetLists(string input)
    {
        var arrays = input.Lines()
            .Select(line => line.NumbersBySeparator<int>("   "))
            .ToArray()
            .Transpose();

        return (arrays[0], arrays[1]);
    }
}
