using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day20;

internal class Day20 : DayBase
{
    public override int Day => 20;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();
        var cardinalDirections = Enum.GetValues<CardinalDirection>();


        var (path, pathIndexLookup) = GetPath(grid);

        /*
         * To find shortcuts, we check for each position of the path if there is a
         * position 2 spots away, where we are further on the path, and there is a non-path space in between.
         * Then we can check the distance we skipped.
         */
        var cheatCounter = 0;

        foreach (var pos in path)
        {
            foreach (var dir in cardinalDirections)
            {
                var oneAdjacent = pos.CoordinateInDirection(dir);

                if (pathIndexLookup.ContainsKey(oneAdjacent))
                {
                    continue;
                }

                var twoAdjacent = oneAdjacent.CoordinateInDirection(dir);

                if (!pathIndexLookup.ContainsKey(twoAdjacent))
                {
                    continue;
                }

                var currentIndex = pathIndexLookup[pos];
                var twoAdjacentIndex = pathIndexLookup[twoAdjacent];

                if (twoAdjacentIndex - currentIndex > 100)
                {
                    cheatCounter++;
                }

            }
        }

        return cheatCounter.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();
        var minSkip = 100;

        var (path, _) = GetPath(grid);

        var cheatCounter = 0;

        for (var i = 0; i < path.Count; i++)
        {
            for (var j = i + minSkip; j < path.Count; j++)
            {
                var maxDist = Math.Min(j - i - minSkip, 20);
                if (path[i].ManhattanDistance(path[j]) <= maxDist)
                {
                    cheatCounter++;
                }
            }
        }

        return cheatCounter.ToString();
    }

    public (List<GridCoordinate> Path, Dictionary<GridCoordinate, int> PathIndexLookup) GetPath(char[][] grid)
    {
        GridCoordinate? current = grid.FindFirst(c => c is 'S');

        var path = new List<GridCoordinate>();
        var pathIndexLookup = new Dictionary<GridCoordinate, int>();
        var cardinalDirections = Enum.GetValues<CardinalDirection>();

        while (current.HasValue)
        {
            path.Add(current.Value);
            pathIndexLookup.Add(current.Value, path.Count - 1);

            current = cardinalDirections.Select(d => current.Value.CoordinateInDirection(d))
                .Where(next => !pathIndexLookup.ContainsKey(next) && next.IsInsideGrid(grid) && grid.Get(next) is not '#')
                .Cast<GridCoordinate?>()
                .FirstOrDefault();
        }

        return (path, pathIndexLookup);

    }
}
