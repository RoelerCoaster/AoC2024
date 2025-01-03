using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day04;

internal class Day04 : DayBase
{
    public override int Day => 4;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();

        (int, int)[] directions = [
            (-1, -1),
            (-1, 0),
            (-1, 1),
            (0, -1),
            (0, 1),
            (1, -1),
            (1, 0),
            (1, 1),
        ];

        var counter = 0;
        for (var row = 0; row < grid.Length; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                foreach (var direction in directions)
                {
                    if (CanSpellXmas(grid, row, col, direction))
                    {
                        counter++;
                    }
                }
            }
        }

        return counter.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();
        var counter = 0;

        // Note: no need to check the edge
        for (var row = 1; row < grid.Length - 1; row++)
        {
            for (var col = 1; col < grid[row].Length - 1; col++)
            {
                if (IsXShapedMas(grid, row, col))
                {
                    counter++;
                }
            }
        }

        return counter.ToString();

    }

    private bool CanSpellXmas(char[][] grid, int row, int col, (int, int) direction)
    {
        var currentRow = row;
        var currentCol = col;

        foreach (var c in "XMAS")
        {
            if (currentRow < 0 || currentCol < 0 || currentRow >= grid.Length || currentCol >= grid[0].Length)
            {
                return false;
            }

            var currentChar = grid[currentRow][currentCol];
            if (currentChar != c)
            {
                return false;
            }

            currentRow += direction.Item1;
            currentCol += direction.Item2;
        }

        return true;
    }
    private bool IsXShapedMas(char[][] grid, int row, int col)
    {
        if (grid[row][col] != 'A')
        {
            return false;
        }

        if ((grid[row - 1][col - 1], grid[row + 1][col + 1]) is not ('M', 'S') and not ('S', 'M'))
        {
            return false;

        }

        if ((grid[row + 1][col - 1], grid[row + -1][col + 1]) is not ('M', 'S') and not ('S', 'M'))
        {
            return false;

        }

        return true;
    }
}
