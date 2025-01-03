using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day12;

internal class Day12 : DayBase
{
    public override int Day => 12;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

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

            var regionEdges = new List<(GridCoordinate Coordinate, CardinalDirection Direction)>();

            while (queue.TryDequeue(out var current))
            {
                regionData.Area++;
                visited.Add(current);

                foreach (var dir in cardinalDirections)
                {
                    var adjacent = current.CoordinateInDirection(dir);
                    if (adjacent.IsInsideGrid(grid) && grid.Get(adjacent) == regionName)
                    {
                        if (!visited.Contains(adjacent) && !queue.Contains(adjacent))
                        {
                            queue.Enqueue(adjacent);
                        }
                    }
                    else
                    {
                        // Edge of the region in this direction
                        regionEdges.Add((current, dir));
                    }
                }
            }

            // determine sides
            new[] { CardinalDirection.North, CardinalDirection.South }.ForEach(dir =>
            {
                regionEdges.Where(edge => edge.Direction == dir)
                    .GroupBy(edge => edge.Coordinate.Row)
                    .ForEach(group =>
                    {
                        regionData.Perimeter += group
                            .Select(edge => edge.Coordinate.Col)
                            .Order()
                            .Pairwise((a, b) => (a != b - 1) ? 1 : 0)
                            .Sum() + 1;
                    });
            });

            new[] { CardinalDirection.East, CardinalDirection.West }.ForEach(dir =>
            {
                regionEdges.Where(edge => edge.Direction == dir)
                    .GroupBy(edge => edge.Coordinate.Col)
                    .ForEach(group =>
                    {
                        regionData.Perimeter += group
                            .Select(edge => edge.Coordinate.Row)
                            .Order()
                            .Pairwise((a, b) => (a != b - 1) ? 1 : 0)
                            .Sum() + 1;
                    });
            });


            regionDatas.Add(regionData);
        });

        var price = regionDatas.Sum(d => d.Perimeter * d.Area);


        return price.ToString();
    }

    private sealed class RegionData
    {
        public int Area { get; set; } = 0;
        public int Perimeter { get; set; } = 0;
    }
}
