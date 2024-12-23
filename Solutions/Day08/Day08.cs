using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day08;

internal class Day08 : DayBase
{
    public override int Day => 8;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var grid = input.Grid();
        var antennas = GetAntennas(grid);

        var antinodes = GetAllAntinodes(antennas, grid, GetAntinodesPair);

        return antinodes.Count.ToString();

    }

    protected override async Task<string> SolvePart2(string input)
    {
        var grid = input.Grid();
        var antennas = GetAntennas(grid);

        var antinodes = GetAllAntinodes(antennas, grid, GetAntinodesWithHarmonics);

        return antinodes.Count.ToString();
    }

    private List<Antenna> GetAntennas(char[][] grid)
    {
        var antennas = new List<Antenna>();

        grid.ForEachGridElement((el, r, c) =>
        {
            if (el != '.')
            {
                antennas.Add(new(el, new(r, c)));
            }
        });

        return antennas;
    }

    private static List<GridCoordinate> GetAllAntinodes(List<Antenna> antennas, char[][] grid, AntinodesForPairGetter getter)
    {
        return antennas.GroupBy(a => a.Type)
            .SelectMany(grp => GetAntinodesForGroup(grp, grid, getter))
            .Distinct()
            .ToList();
    }

    private static IEnumerable<GridCoordinate> GetAntinodesForGroup(IEnumerable<Antenna> group, char[][] grid, AntinodesForPairGetter getter)
    {
        return group
            .UnorderedPairs(true)
            .SelectMany(pair => getter(pair.Item1, pair.Item2, grid));
    }

    private static IEnumerable<GridCoordinate> GetAntinodesPair(Antenna antenna1, Antenna antenna2, char[][] grid)
    {
        var rowDif = antenna1.Location.Row - antenna2.Location.Row;
        var colDif = antenna1.Location.Col - antenna2.Location.Col;

        var firstAntinode = new GridCoordinate(antenna1.Location.Row + rowDif, antenna1.Location.Col + colDif);
        if (firstAntinode.IsInsideGrid(grid))
        {
            yield return firstAntinode;
        }

        var secondAntinode = new GridCoordinate(antenna2.Location.Row - rowDif, antenna2.Location.Col - colDif);
        if (secondAntinode.IsInsideGrid(grid))
        {
            yield return secondAntinode;
        }
    }

    private static IEnumerable<GridCoordinate> GetAntinodesWithHarmonics(Antenna antenna1, Antenna antenna2, char[][] grid)
    {
        var rowDif = antenna1.Location.Row - antenna2.Location.Row;
        var colDif = antenna1.Location.Col - antenna2.Location.Col;

        var gcd = MathUtil.GCD(rowDif, colDif);

        var rowDifNormalized = rowDif / gcd;
        var colDifNormalized = colDif / gcd;

        var current = antenna1.Location;

        while (current.IsInsideGrid(grid))
        {
            yield return current;
            current = new GridCoordinate(current.Row + rowDifNormalized, current.Col + colDifNormalized);
        }

        current = antenna1.Location;

        while (current.IsInsideGrid(grid))
        {
            if (current != antenna1.Location)
            {
                yield return current;
            }
            current = new GridCoordinate(current.Row - rowDifNormalized, current.Col - colDifNormalized);
        }


    }

    private record Antenna(char Type, GridCoordinate Location);

    private delegate IEnumerable<GridCoordinate> AntinodesForPairGetter(Antenna antenna1, Antenna antenna2, char[][] grid);
}
