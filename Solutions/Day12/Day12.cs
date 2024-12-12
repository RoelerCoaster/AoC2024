using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day12;

internal class Day12 : DayBase
{
    public override int Day => 12;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.CustomGrid(c => c.ToString());
        MakeUniqueNames(grid);

        var regionDataMap = new Dictionary<string, RegionData>();
        var cardinalDirections = Enum.GetValues<CardinalDirection>();

        grid.ForEachGridElement((regionName, coord) =>
        {
            regionDataMap.TryAdd(regionName, new());
            var regionData = regionDataMap[regionName];

            regionData.Area++;
            regionData.Perimeter += 4;

            foreach (var dir in cardinalDirections)
            {
                var adjacent = coord.CoordinateInDirection(dir);
                if (adjacent.IsInsideGrid(grid) && grid.Get(adjacent) == regionName)
                {
                    regionData.Perimeter--;
                }
            }
        });

        var price = regionDataMap.Values.Sum(d => d.Perimeter * d.Area);


        return price.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    /**
     * We create a new grid where each region has a unique name.
     * 
     * This is done by essentially doing a BFS and adding an appropriate index to
     * the current region name.
     */
    private void MakeUniqueNames(string[][] grid)
    {
        var visited = new HashSet<GridCoordinate>();

        var counts = new Dictionary<string, int>();
        var cardinalDirections = Enum.GetValues<CardinalDirection>();


        grid.ForEachGridElement((regionName, coord) =>
        {
            if (visited.Contains(coord))
            {
                return;
            }

            counts.TryAdd(regionName, 0);
            var index = counts[regionName];

            var queue = new Queue<GridCoordinate>();
            queue.Enqueue(coord);

            // Perform BFS to update all the names  
            while (queue.TryDequeue(out var current))
            {

                grid.Set(current, regionName + index);
                visited.Add(current);

                foreach (var dir in cardinalDirections)
                {
                    var adjacent = current.CoordinateInDirection(dir);
                    if (adjacent.IsInsideGrid(grid) && !visited.Contains(adjacent) && grid.Get(adjacent) == regionName && !queue.Contains(adjacent))
                    {

                        queue.Enqueue(adjacent);
                    }
                }
            }

            counts[regionName]++;
        });

    }

    private sealed class RegionData
    {
        public int Area { get; set; } = 0;
        public int Perimeter { get; set; } = 0;
    }
}
