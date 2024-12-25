using MoreLinq;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day24;

internal class Day24 : DayBase
{
    public override int Day => 24;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;

    protected override async Task<string> SolvePart1(string input)
    {
        var sections = input.Sections();

        var wireValues = new Dictionary<string, long>();

        sections[0].Lines().ForEach(line =>
        {
            var split = line.Split(": ");

            wireValues.Add(split[0], split[1].ToNumber<long>());
        });

        var gates = sections[1].Lines()
            .Select(line =>
                {
                    var split = line.Split(" ");
                    return new Gate(split[0], split[2], split[1], split[4]);
                })
            .ToList();

        var gateQueue = new Queue<Gate>(gates);

        while (gateQueue.TryDequeue(out var gate))
        {
            if (!wireValues.TryGetValue(gate.InA, out var a) || !wireValues.TryGetValue(gate.InB, out var b))
            {
                gateQueue.Enqueue(gate);
                continue;
            }

            var o = gate.Operator switch
            {
                "AND" => a & b,
                "OR" => a | b,
                "XOR" => a ^ b,
                _ => throw new NotSupportedException()
            };

            wireValues[gate.Out] = o;
        }

        var result = wireValues
            .Where(entry => entry.Key.StartsWith('z'))
            .Select(entry =>
            {
                var shift = entry.Key.Substring(1).ToNumber<int>();

                return entry.Value << shift;
            })
            .Sum();

        return result.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        throw new NotImplementedException();
    }

    private record struct Gate(string InA, string InB, string Operator, string Out);
}
