using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day03;

internal class Day03 : DayBase
{
    public override int Day => 3;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var matches = Regex.Matches(input, @"mul\((?<left>\d{1,3}),(?<right>\d{1,3})\)");

        var result = matches.Select(m => m.Groups["left"].Value.ToNumber<int>() * m.Groups["right"].Value.ToNumber<int>())
            .Sum();

        return result.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var disabledSectionsRemoved = Regex.Replace(input, @"don't\(\).*?((do\(\))|$)", "", RegexOptions.Singleline);

        return await SolvePart1(disabledSectionsRemoved);
    }
}
