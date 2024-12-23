using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using Spectre.Console;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day17;

internal class Day17 : DayBase
{
    public override int Day => 17;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var registers = sections[0].Lines()
            .Select(l => l.Substring(l.IndexOf(':') + 1).Trim().ToNumber<int>())
            .ToArray();

        var program = sections[1].Substring(sections[1].IndexOf(':') + 1).Trim().NumbersBySeparator<byte>(",");

        var state = new ProgramState
        {
            A = registers[0],
            B = registers[1],
            C = registers[2],
            IP = 0
        };

        ThreeBitComputerEmulator.Run(state, program);


        return state.Out.StringJoin(",");

    }

    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        var registers = sections[0].Lines()
            .Select(l => l.Substring(l.IndexOf(':') + 1).Trim().ToNumber<int>())
            .ToArray();

        var program = sections[1].Substring(sections[1].IndexOf(':') + 1).Trim().NumbersBySeparator<byte>(",");


        var results = new List<string>();
        for (long a = 0; a <= 4096; a++)
        {
            var state = new ProgramState
            {
                A = a,
                B = registers[1],
                C = registers[2],
                IP = 0
            };

            ThreeBitComputerEmulator.Run(state, program);

            results.Add(state.Out.StringJoin(","));
        }

        var dir = Path.Join("./Outputs", Day.ToString());
        Directory.CreateDirectory(dir);

        File.WriteAllLines(Path.Join(dir, "output.txt"), results);

        return string.Empty;

    }
}
