using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day15;

internal class Day15 : DayBase
{
    public override int Day => 15;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var grid = sections[0].Grid();

        var moves = sections[1].Lines().SelectMany(l => l).ToArray();

        var robotPosition = grid.FindFirst(c => c is '@');

        foreach (var move in moves)
        {
            (_, robotPosition) = TryMove(grid, robotPosition, TranslateMove(move));
        }

        var GPS = grid.FindAll(c => c is 'O')
            .Select(c => 100 * c.Row + c.Col);

        return GPS.Sum().ToString();

    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        var grid = sections[0].Lines()
            .Select(l => l.SelectMany(c => c switch
            {
                'O' => ['[', ']'],
                '@' => ['@', '.'],
                _ => new[] { c, c }
            }).ToArray())
            .ToArray();


        var moves = sections[1].Lines().SelectMany(l => l).ToArray();

        var robotPosition = grid.FindFirst(c => c is '@');

        foreach (var move in moves)
        {
            robotPosition = TryMoveRobotPart2(grid, robotPosition, TranslateMove(move));
        }

        var GPS = grid.FindAll(c => c is '[')
            .Select(c => 100 * c.Row + c.Col);

        return GPS.Sum().ToString();
    }

    private (bool Moved, GridCoordinate NewPos) TryMove(char[][] grid, GridCoordinate pos, CardinalDirection direction)
    {
        var nextPos = pos.CoordinateInDirection(direction);
        var nextSymbol = grid.Get(nextPos);

        if (nextSymbol is '#')
        {
            // Wall. No movement
            return (false, pos);
        }

        if (nextSymbol is '.' || TryMove(grid, nextPos, direction).Moved)
        {
            // Empty spot, or spot can be made empty. Move object
            grid.Set(nextPos, grid.Get(pos));
            grid.Set(pos, '.');
            return (true, nextPos);
        }

        return (false, pos);
    }

    private GridCoordinate TryMoveRobotPart2(char[][] grid, GridCoordinate pos, CardinalDirection direction)
    {
        if (CanMovePart2(grid, pos, direction))
        {
            DoMovePart2(grid, pos, direction);
            return pos.CoordinateInDirection(direction);
        }
        return pos;
    }

    private bool CanMovePart2(char[][] grid, GridCoordinate pos, CardinalDirection direction)
    {
        var nextPos = pos.CoordinateInDirection(direction);
        var nextSymbol = grid.Get(nextPos);

        if (nextSymbol is '#')
        {
            // Wall. No movement
            return false;
        }

        if (nextSymbol is '.')
        {
            // Empty space. Movement possible.
            return true;
        }

        // Symbol is either [ or ]
        if (direction.IsHorizontal())
        {
            // Horizontal movement. We have to check if the (partial) object in the adjacent cell can be moved
            return CanMovePart2(grid, nextPos, direction);
        }

        // Vertical movement. We have to try to move the 2-wide box in one go
        var otherSideOfTheBox = nextSymbol is '['
            ? nextPos.CoordinateInDirection(CardinalDirection.East)
            : nextPos.CoordinateInDirection(CardinalDirection.West);


        return CanMovePart2(grid, nextPos, direction) && CanMovePart2(grid, otherSideOfTheBox, direction);
    }

    private void DoMovePart2(char[][] grid, GridCoordinate pos, CardinalDirection direction)
    {
        var nextPos = pos.CoordinateInDirection(direction);
        var nextSymbol = grid.Get(nextPos);
        var currentSymbol = grid.Get(pos);

        if (nextSymbol is '#')
        {
            throw new InvalidOperationException("Trying to move through a wall");
        }

        if (nextSymbol is '[' or ']')
        {
            DoMovePart2(grid, nextPos, direction);

            if (direction.IsVertical())
            {
                // Vertical movement. We have to try to move the 2-wide box in one go
                var otherSideOfTheBox = nextSymbol is '['
                    ? nextPos.CoordinateInDirection(CardinalDirection.East)
                    : nextPos.CoordinateInDirection(CardinalDirection.West);
                DoMovePart2(grid, otherSideOfTheBox, direction);
            }
        }

        grid.Set(nextPos, currentSymbol);
        grid.Set(pos, '.');
    }

    private static CardinalDirection TranslateMove(char move)
    {
        return move switch
        {
            '^' => CardinalDirection.North,
            '>' => CardinalDirection.East,
            '<' => CardinalDirection.West,
            'v' => CardinalDirection.South,
            _ => throw new NotSupportedException(),
        };
    }
}
