using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day02;

internal class Day02 : DayBase
{
    public override int Day => 2;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var reports = GetReports(input);

        var safeCount = reports.Count(IsSafe);

        return safeCount.ToString();
    }



    protected override async Task<string> SolvePart2(string input)
    {
        var reports = GetReports(input);

        var safeCount = reports.Count(IsDampenedSafe);

        return safeCount.ToString();
    }

    private static bool IsSafe(ICollection<int> report)
    {
        var differences = report.Pairwise((a, b) => a - b).ToList();

        return differences.All(d => Math.Abs(d) is (>= 1) and (<= 3) && Math.Sign(d) == Math.Sign(differences[0]));
    }


    private static bool IsDampenedSafe(int[] report)
    {
        if (IsSafe(report))
        {
            return true;
        }

        for (var i = 0; i < report.Length; i++)
        {
            var oneRemoved = report.ToList();
            oneRemoved.RemoveAt(i);

            if (IsSafe(oneRemoved))
            {
                return true;
            }
        }

        return false;
    }

    private static List<int[]> GetReports(string input)
    {
        return input.Lines()
                    .Select(l => l.NumbersBySeparator<int>(" "))
                    .ToList();
    }
}
