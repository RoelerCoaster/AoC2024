namespace RoelerCoaster.AdventOfCode.Year2024.Util;
internal static class JaggedArrayGridExtensions
{
    public static T[][] CreateCopy<T>(this T[][] grid)
    {
        return grid.Select(x => x.ToArray()).ToArray();
    }

    public static string CreateString<T>(this T[][] grid)
    {
        return grid.Select(x => x.StringJoin("")).StringJoin(Environment.NewLine);
    }

    public static T[][] Transpose<T>(this T[][] grid)
    {
        if (grid.Length == 0)
        {
            return grid;
        }

        var transposed = new T[grid[0].Length][];

        for (var j = 0; j < transposed.Length; j++)
        {
            transposed[j] = new T[grid.Length];

            for (var i = 0; i < grid.Length; i++)
            {
                transposed[j][i] = grid[i][j];
            }
        }

        return transposed;
    }
}
