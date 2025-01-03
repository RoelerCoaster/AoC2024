using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day10;

internal class Day10 : DayBase
{
    public override int Day => 10;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.DigitGrid();

        var computeTrailScore = RecursionUtil.Memoize<ComputationArgs, HashSet<GridCoordinate>>(ComputeReachableTopsTemplate);

        var potentialStarts = grid.FindAll(h => h == 0);

        var reachable = potentialStarts.Select(start =>

            computeTrailScore.Invoke(new(0, start, grid)));

        return reachable.Sum(r => r.Count).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.DigitGrid();

        var computeTrailScore = RecursionUtil.Memoize<ComputationArgs, int>(ComputeTrailRatingTemplate);

        var potentialStarts = grid.FindAll(h => h == 0);

        var scores = potentialStarts.Select(start =>

            computeTrailScore.Invoke(new(0, start, grid)));

        return scores.Sum().ToString();
    }

    private int ComputeTrailRatingTemplate(Func<ComputationArgs, int> recurse, ComputationArgs args)
    {
        var currentHeight = args.Grid.Get(args.Position);

        if (args.Height != currentHeight)
        {
            return 0;
        }

        if (currentHeight == 9)
        {
            return 1;
        }

        var validSurroundingCoordinates = Enum.GetValues<CardinalDirection>()
            .Select(dir => args.Position.CoordinateInDirection(dir))
            .Where(pos => pos.IsInsideGrid(args.Grid));

        return validSurroundingCoordinates.Sum(pos =>
            recurse.Invoke(args with { Position = pos, Height = currentHeight + 1 }));
    }


    private HashSet<GridCoordinate> ComputeReachableTopsTemplate(Func<ComputationArgs, HashSet<GridCoordinate>> recurse, ComputationArgs args)
    {
        var currentHeight = args.Grid.Get(args.Position);

        if (args.Height != currentHeight)
        {
            return new();
        }

        if (currentHeight == 9)
        {
            return new() { args.Position };
        }

        var validSurroundingCoordinates = Enum.GetValues<CardinalDirection>()
            .Select(dir => args.Position.CoordinateInDirection(dir))
            .Where(pos => pos.IsInsideGrid(args.Grid));

        var result = new HashSet<GridCoordinate>();

        foreach (var coord in validSurroundingCoordinates)
        {
            result.UnionWith(recurse.Invoke(args with { Position = coord, Height = currentHeight + 1 }));
        }

        return result;
    }


    private record ComputationArgs(int Height, GridCoordinate Position, int[][] Grid);
}
