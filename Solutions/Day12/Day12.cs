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
        var grid = input.Grid();

        var visited = new HashSet<GridCoordinate>();
        var regionDatas = new List<RegionData>();
        var cardinalDirections = Enum.GetValues<CardinalDirection>();

        grid.ForEachGridElement((regionName, coord) =>
        {
            if (visited.Contains(coord))
            {
                return;
            }

            var regionData = new RegionData();

            var queue = new Queue<GridCoordinate>();
            queue.Enqueue(coord);

            // Perform BFS to compute the area and perimeter of this region
            while (queue.TryDequeue(out var current))
            {
                regionData.Area++;
                regionData.Perimeter += 4;
                visited.Add(current);

                foreach (var dir in cardinalDirections)
                {
                    var adjacent = current.CoordinateInDirection(dir);
                    if (adjacent.IsInsideGrid(grid) && grid.Get(adjacent) == regionName)
                    {
                        regionData.Perimeter--;
                        if (!visited.Contains(adjacent) && !queue.Contains(adjacent))
                        {
                            queue.Enqueue(adjacent);
                        }
                    }
                }
            }

            regionDatas.Add(regionData);
        });

        var price = regionDatas.Sum(d => d.Perimeter * d.Area);


        return price.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    private sealed class RegionData
    {
        public int Area { get; set; } = 0;
        public int Perimeter { get; set; } = 0;
    }
}
