using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day06;

internal class Day06 : DayBase
{
    public override int Day => 6;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();

        var initialPosition = grid.FindFirst(c => c is '^');
        var initialDirection = CardinalDirection.North;
        MarkGrid(grid, initialPosition, initialDirection);

        var count = grid.SelectMany(x => x).Count(c => c is 'X');

        return count.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();

        var initialPosition = grid.FindFirst(c => c is '^');
        var initialDirection = CardinalDirection.North;
        MarkGrid(grid, initialPosition, initialDirection);

        var allMarked = grid.FindAll(c => c is 'X').Where(p => p != initialPosition);

        var count = 0;

        foreach (var markedPosition in allMarked)
        {
            var currentSymbol = grid[markedPosition.Row][markedPosition.Col];
            grid[markedPosition.Row][markedPosition.Col] = '#';
            if (HasLoop(grid, initialPosition, initialDirection))
            {
                count++;
            }
            grid[markedPosition.Row][markedPosition.Col] = currentSymbol;
        }

        return count.ToString();
    }

    private void MarkGrid(char[][] grid, GridCoordinate initialPosition, CardinalDirection initialDirection)
    {
        var currentPosition = initialPosition;
        var currentDirection = initialDirection;

        for (; ; )
        {
            grid[currentPosition.Row][currentPosition.Col] = 'X';

            var nextPosition = currentPosition.CoordinateInDirection(currentDirection);

            if (!nextPosition.IsInsideGrid(grid))
            {
                // Walk off the grid, finish marking
                return;
            }

            if (grid[nextPosition.Row][nextPosition.Col] == '#')
            {
                currentDirection = currentDirection.RotateClockwise();
            }
            else
            {
                currentPosition = nextPosition;
            }
        }
    }

    private bool HasLoop(char[][] grid, GridCoordinate initialPosition, CardinalDirection initialDirection)
    {
        var currentPosition = initialPosition;
        var currentDirection = initialDirection;

        var visited = new HashSet<(GridCoordinate, CardinalDirection)>();
        for (; ; )
        {
            visited.Add((currentPosition, currentDirection));
            var nextPosition = currentPosition.CoordinateInDirection(currentDirection);

            if (!nextPosition.IsInsideGrid(grid))
            {
                // Walk off the grid, no loop
                return false;
            }

            if (visited.Contains((nextPosition, currentDirection)))
            {
                // visited next position and direction already, so we are in a loop
                return true;
            }

            if (grid[nextPosition.Row][nextPosition.Col] == '#')
            {
                currentDirection = currentDirection.RotateClockwise();
            }
            else
            {
                currentPosition = nextPosition;
            }
        }
    }
}
