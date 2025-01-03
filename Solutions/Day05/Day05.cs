using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day05;

internal class Day05 : DayBase
{
    public override int Day => 5;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        ParseInput(input, out var rules, out var updates);

        var validUpdates = updates
            .Where(u => IsUpdateCorrectlyOrdered(u, rules));

        var result = validUpdates.Select(u => u[u.Length / 2])
            .Sum();

        return result.ToString();
    }



    protected override async Task<string> SolvePart2(string input)
    {
        ParseInput(input, out var rules, out var updates);

        var invalidUpdates = updates
            .Where(u => !IsUpdateCorrectlyOrdered(u, rules))
            .ToList();

        invalidUpdates.ForEach(u => FixUpdate(u, rules));

        var result = invalidUpdates.Select(u => u[u.Length / 2])
            .Sum();

        return result.ToString();
    }

    private bool IsUpdateCorrectlyOrdered(int[] update, ISet<(int, int)> rules)
    {
        for (var i = 0; i < update.Length; i++)
        {
            for (var j = i + 1; j < update.Length; j++)
            {
                if (rules.Contains((update[j], update[i])))
                {
                    // There is a a rule that puts the elements in the opposite order
                    return false;
                }
            }
        }

        return true;
    }

    private void FixUpdate(int[] update, ISet<(int, int)> rules)
    {
        for (var i = 0; i < update.Length; i++)
        {
            for (var j = i + 1; j < update.Length; j++)
            {
                if (rules.Contains((update[j], update[i])))
                {
                    (update[i], update[j]) = (update[j], update[i]);
                }
            }
        }
    }

    private static void ParseInput(string input, out HashSet<(int Left, int Right)> rules, out List<int[]> updates)
    {
        var sections = input.Sections();

        rules = sections[0]
            .Lines()
            .Select(x => x.NumbersBySeparator<int>("|"))
            .Select(numbers => (Left: numbers[0], Right: numbers[1]))
            .ToHashSet();
        updates = sections[1]
            .Lines()
            .Select(x => x.NumbersBySeparator<int>(","))
            .ToList();
    }
}
