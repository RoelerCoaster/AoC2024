using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day25;

internal class Day25 : DayBase
{
    public override int Day => 25;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var keys = new List<int[]>();
        var locks = new List<int[]>();

        foreach (var section in sections)
        {
            var grid = section.Grid();

            var heights = new int[grid[0].Length];

            grid.ForEachGridElement((el, pos) =>
            {
                if (el is '#')
                {
                    heights[pos.Col]++;
                }
            });

            for (var i = 0; i < heights.Length; i++)
            {
                heights[i]--;
            }

            if (grid[0][0] is '#')
            {
                locks.Add(heights);
            }
            else
            {
                keys.Add(heights);
            }
        }

        var count = 0;

        foreach (var @lock in locks)
        {
            foreach (var key in keys)
            {
                if (@lock.Select((h, i) => h + key[i]).All(x => x <= 5))
                {
                    count++;
                }
            }
        }

        return count.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }
}
