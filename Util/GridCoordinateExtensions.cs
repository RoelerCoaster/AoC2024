using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Util;
internal static class GridCoordinateExtensions
{
    public static GridCoordinate CoordinateInDirection(this GridCoordinate current, CardinalDirection direction, int offset = 1)
    {
        return direction switch
        {
            CardinalDirection.North => current with { Row = current.Row - offset },
            CardinalDirection.South => current with { Row = current.Row + offset },
            CardinalDirection.West => current with { Col = current.Col - offset },
            CardinalDirection.East => current with { Col = current.Col + offset },
            _ => throw new NotSupportedException()
        };
    }

    public static bool IsInsideGrid<TGrid>(this GridCoordinate coord, TGrid[][] grid)
    {
        return coord.Row >= 0 && coord.Col >= 0 && coord.Row < grid.Length && coord.Col < grid.Length;
    }

    public static int ManhattanDistance(this GridCoordinate source, GridCoordinate target)
    {
        return Math.Abs(source.Row - target.Row) + Math.Abs(source.Col - target.Col);
    }
}
