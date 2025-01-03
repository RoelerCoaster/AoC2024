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

        /*
         * Observation:
         * The program produces an output number
         * for every 3 bits of A, and keeps looping until all bits are consumed.
         * At the end of every loop, the 3 least significant bits are removed.
         * 
         * Since the program consists of 16 numbers, we need to find a 48 bit number for A
         * 
         * We know that the 3 most significant bits determine the last number of the program,
         * the second to last number is determined by 3 additional bits etc.
         * 
         * So we can start at the back and work our way backward.
         */

        var finalResult = FindValue(program, registers, program.Length - 1, 0);

        return finalResult.ToString();

    }

    private long? FindValue(byte[] program, int[] registers, int i, long resultSoFar)
    {
        if (i == -1)
        {
            return resultSoFar;
        }

        // Bruteforce the next 3 bits
        for (var guess = 0L; guess < 8; guess++)
        {
            var a = (resultSoFar << 3) + guess;
            var state = new ProgramState
            {
                A = a,
                B = registers[1],
                C = registers[2],
                IP = 0
            };
            ThreeBitComputerEmulator.Run(state, program);

            if (state.Out.SequenceEqual(program[i..]))
            {
                var next = FindValue(program, registers, i - 1, a);
                if (next.HasValue)
                {
                    return next;
                }
            }
        }

        return null;
    }
}
