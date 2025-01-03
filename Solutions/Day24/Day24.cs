using MoreLinq;
using QuikGraph;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day24;

internal class Day24 : DayBase
{
    public override int Day => 24;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

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

    /*
     * Obeservation from analysing the input and creating an image of the gates and wires:
     * - We have a 45 bit binary adder
     * - It consists of one half-adder and 44 full adders
     * - The half adder appears to be wired up correctly
     * - We assume that the swaps happen within a single halfadder
     * 
     */
    protected override async Task<string> SolvePart2(string input)
    {
        var sections = input.Sections();

        // Build a graph of the input. Each gate and each wire is a node
        var graph = sections[1].Lines().SelectMany(line =>
        {
            var split = line.Split(" ");

            return new Edge<string>[]
            {
                new(split[0], line),
                new(split[2], line),
                new(line, split[4]),
            };
        }).ToAdjacencyGraph<string, Edge<string>>();

        /* Each fulladder has the following structure
         *  
         *  
         *  x  -  AND --------
         *     X               \
         *  y  -  XOR -- AND --  OR -> Cout
         *             X
         *  Cin  ------- XOR -> z
         * 
         */

        var invalidWires = new HashSet<string>();
        for (var i = 1; i <= 44; i++)
        {
            var x = $"x{i:D2}";

            var gates = new HashSet<string>();

            var xyGates = graph.OutEdges(x).Select(e => e.Target).ToList();

            gates.UnionWith(xyGates);

            xyGates.ForEach(gate =>
            {
                var wire = graph.OutEdge(gate, 0).Target;
                gates.UnionWith(graph.OutEdges(wire).Select(e => e.Target));
            });


            // Check each wire
            foreach (var gate in gates)
            {
                var wire = graph.OutEdge(gate, 0).Target;

                if (gate.Contains(" AND "))
                {
                    // AND-gate, should lead to a single OR-gate
                    var outEdges = graph.OutEdges(wire).ToList();
                    if (outEdges.Count != 1 || !outEdges.Single().Target.Contains(" OR "))
                    {
                        invalidWires.Add(wire);
                    }
                }
                else if (gate.Contains(" XOR ") && gate.Contains(x))
                {
                    // Input XOR-gate, should lead to an AND and an OR-gate
                    var outEdges = graph.OutEdges(wire).ToList();
                    if (outEdges.Count != 2 || !outEdges.Any(e => e.Target.Contains(" AND ")) || !outEdges.Any(e => e.Target.Contains(" XOR ")))
                    {
                        invalidWires.Add(wire);
                    }
                }
                else if (gate.Contains(" XOR "))
                {
                    // Input XOR-gate, before output
                    if (!wire.StartsWith("z"))
                    {
                        invalidWires.Add(wire);
                    }
                }
                else if (gate.Contains(" OR "))
                {
                    // OR-gate before carry-out. should lead to an AND and an OR-gate, or to z45
                    if (wire == "z45")
                    {
                        continue;
                    }

                    var outEdges = graph.OutEdges(wire).ToList();
                    if (outEdges.Count != 2 || !outEdges.Any(e => e.Target.Contains(" AND ")) || !outEdges.Any(e => e.Target.Contains(" XOR ")))
                    {
                        invalidWires.Add(wire);
                    }
                }
            }
        }

        return invalidWires.Order().StringJoin(",");
    }

    private record struct Gate(string InA, string InB, string Operator, string Out);
}
