using MoreLinq;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day21;

internal class Day21 : DayBase
{
    public override int Day => 21;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;


    /*
     * We have the following configuration
     * 
     * Numpad (robot3) - directional pad (robot2) - directional pad (robot1) - directional pad (you)
     * 
     */
    protected override async Task<string> SolvePart1(string input)
    {
        var useOriginal = false;

        if (useOriginal)
        {
            return Part1OriginalAttempt(input);
        }

        return Part2Solution(input, 2);
    }

    /*
     * Original algorithm for part 1. Part 1 can be solved much quicker using the part 2 algorithm;
     */
    protected string Part1OriginalAttempt(string input)
    {
        var directionalPadGraph = GetDirectionalPadGraph();
        var numPadGraph = GetNumPadGraph();


        var graph = new AdjacencyGraph<RobotsState, TaggedEdge<RobotsState, char>>();

        var queue = new Queue<RobotsState>();


        queue.Enqueue(new('A', 'A', 'A'));
        var visited = new HashSet<RobotsState>();

        while (queue.TryDequeue(out var current))
        {
            visited.Add(current);


            // Own press
            // Direction buttons
            foreach (var button in directionalPadGraph.Vertices.Except(['A']))
            {
                directionalPadGraph.TryGetOutEdges(current.Robot1, out var edges);
                var edge = edges.FirstOrDefault(e => e.Tag == button);
                if (edge is null)
                {
                    continue;
                }
                var nextVertex = current with { Robot1 = edge.Target };
                graph.AddVerticesAndEdge(new(current, nextVertex, button));

                if (!visited.Contains(nextVertex) && !queue.Contains(nextVertex))
                {
                    queue.Enqueue(nextVertex);
                }
            }

            // Press A
            // Handle press by Robot 1
            // Robot 1 presses a directional button
            if (current.Robot1 is not 'A')
            {
                directionalPadGraph.TryGetOutEdges(current.Robot2, out var edges);
                var edge = edges.FirstOrDefault(e => e.Tag == current.Robot1);
                if (edge is null)
                {
                    continue;
                }
                var nextVertex = current with { Robot2 = edge.Target };
                graph.AddVerticesAndEdge(new(current, nextVertex, 'A'));

                if (!visited.Contains(nextVertex) && !queue.Contains(nextVertex))
                {
                    queue.Enqueue(nextVertex);
                }
            }
            // Robot 1 presses A
            else
            {
                // Handle press by Robot2
                // Robot 2 presses a directional button
                numPadGraph.TryGetOutEdges(current.Robot3, out var edges);
                var edge = edges.FirstOrDefault(e => e.Tag == current.Robot2);
                if (edge is null)
                {
                    continue;
                }
                var nextVertex = current with { Robot3 = edge.Target };
                graph.AddVerticesAndEdge(new(current, nextVertex, 'A'));

                if (!visited.Contains(nextVertex) && !queue.Contains(nextVertex))
                {
                    queue.Enqueue(nextVertex);
                }
            }
        }

        var algo = new FloydWarshallAllShortestPathAlgorithm<RobotsState, TaggedEdge<RobotsState, char>>(graph, _ => 1);

        algo.Compute();

        var codes = input.Lines();

        var total = 0;
        foreach (var code in codes)
        {
            var complexity = 0;

            foreach (var (src, tar) in ('A' + code).Pairwise((a, b) => (a, b)))
            {
                var dist = algo.TryGetDistance(new('A', 'A', src), new('A', 'A', tar), out var d) ? d : throw new InvalidOperationException();

                // number op presses to move all the robots, and then one additional press on A
                complexity += (int)dist + 1;
            }

            var num = Regex.Match(code, @"\d+").Value.ToNumber<int>();

            total += (complexity * num);
        }
        return total.ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        return Part2Solution(input, 25);
    }

    private string Part2Solution(string input, int rounds)
    {
        var keypadMap = GetButtonPressMap("789\n456\n123\n#0A".ReplaceLineEndings());
        var dirpadMap = GetButtonPressMap("#^A\n<v>".ReplaceLineEndings());

        var codes = input.Lines();

        var complexityMemoized = RecursionUtil.Memoize<ComplexityArgs, long>(Complexity);

        var expanded = codes.Select(c => ExpandInput(c, keypadMap)).ToList();

        var complexities = expanded
            .Select(e => complexityMemoized(new(e, rounds, dirpadMap)))
            .ToList();

        var total = codes.Select((code, i) =>
        {
            var num = Regex.Match(code, @"\d+").Value.ToNumber<int>();
            return (complexities[i] * num);
        }).Sum();

        return total.ToString();
    }

    private static AdjacencyGraph<char, TaggedEdge<char, char>> GetDirectionalPadGraph()
    {
        return new List<TaggedEdge<char, char>>
        {
            new ('^', 'A', '>'),
            new ('^', 'v', 'v'),

            new ('A', '^', '<'),
            new ('A', '>', 'v'),

            new ('<', 'v', '>'),

            new ('v', '<', '<'),
            new ('v', '^', '^'),
            new ('v', '>', '>'),

            new ('>', 'A', '^'),
            new ('>', 'v', '<'),
        }.ToAdjacencyGraph<char, TaggedEdge<char, char>>();
    }

    private static AdjacencyGraph<char, TaggedEdge<char, char>> GetNumPadGraph()
    {
        return new List<TaggedEdge<char, char>>
        {
            new ('7', '8', '>'),
            new ('7', '4', 'v'),

            new ('8', '7', '<'),
            new ('8', '5', 'v'),
            new ('8', '9', '>'),

            new ('9', '8', '<'),
            new ('9', '6', 'v'),

            new ('4', '7', '^'),
            new ('4', '5', '>'),
            new ('4', '1', 'v'),

            new ('5', '8', '^'),
            new ('5', '4', '<'),
            new ('5', '6', '>'),
            new ('5', '2', 'v'),

            new ('6', '9', '^'),
            new ('6', '5', '<'),
            new ('6', '3', 'v'),

            new ('1', '4', '^'),
            new ('1', '2', '>'),

            new ('2', '5', '^'),
            new ('2', '1', '<'),
            new ('2', '3', '>'),
            new ('2', '0', 'v'),

            new ('3', '6', '^'),
            new ('3', '2', '<'),
            new ('3', 'A', 'v'),

            new ('0', '2', '^'),
            new ('0', 'A', '>'),

            new ('A', '3', '^'),
            new ('A', '0', '<'),

        }.ToAdjacencyGraph<char, TaggedEdge<char, char>>();
    }

    private Dictionary<(char, char), string> GetButtonPressMap(string buttonGridString)
    {
        var buttonPressMap = new Dictionary<(char, char), string>();

        /* 
         * General idea: For each button on the keypad, we create 5 vertices. One "start/end" vertex, and one for eachcardinal direction.
         * 
         * We add edges by connecting vertices by the correct cardinal direction.
         * Moving between cardinal directions will inflict a very minor but non-zero cost.
         * In this way, if we get shortest paths with the same number button presses, the one with
         * the fewest change of turns will win.
         * 
         * Moving back to an end-vertex will simulate an A-press
         * 
         * Then we can list all paths between all pairs of vertices.
         * 
         */
        var grid = buttonGridString.Grid();

        var graph = new AdjacencyGraph<(char Button, CardinalDirection?), TaggedEdge<(char, CardinalDirection?), string>>();

        grid.ForEachGridElement((el, pos) =>
        {
            if (el is '#')
            {
                return;
            }

            (char, CardinalDirection?) start = (el, null);

            foreach (var dir in Enum.GetValues<CardinalDirection>())
            {
                var vertexWithDir = (el, dir);
                graph.AddVerticesAndEdge(new(start, vertexWithDir, string.Empty));
                graph.AddVerticesAndEdge(new(vertexWithDir, start, "A"));
                graph.AddVerticesAndEdge(new(vertexWithDir, (el, dir.RotateClockwise()), string.Empty));
                graph.AddVerticesAndEdge(new(vertexWithDir, (el, dir.RotateCounterClockwise()), string.Empty));

                var adjacent = pos.CoordinateInDirection(dir);
                if (adjacent.IsInsideGrid(grid) && grid.Get(adjacent) is not '#')
                {
                    var label = dir switch
                    {
                        CardinalDirection.North => "^",
                        CardinalDirection.East => ">",
                        CardinalDirection.West => "<",
                        CardinalDirection.South => "v",
                        _ => throw new NotImplementedException()
                    };

                    graph.AddVerticesAndEdge(new(vertexWithDir, (grid.Get(adjacent), dir), label));
                }
            }
        });

        double EdgeCost(TaggedEdge<(char, CardinalDirection?), string> edge)
        {
            if (edge.Source.Item1 == edge.Target.Item1)
            {
                if (edge.Source.Item2.HasValue == edge.Target.Item2.HasValue)
                {
                    // Inflict a small cost when changing direction
                    // in order to minimize the number of turns we get in the final path
                    return edge.Target.Item2 switch
                    {
                        // When viewing from the A-button on the directional pad, it is cheaper to first
                        // press the up/right button compared to the down or left button
                        // than it is the other way around.
                        // So we add slightly higher costs for buttons further away from the A-button.
                        CardinalDirection.North => 0.01,
                        CardinalDirection.East => 0.01,
                        CardinalDirection.West => 0.03,
                        CardinalDirection.South => 0.02,
                        _ => throw new NotImplementedException(),
                    };
                }
                else
                {
                    // Add a very high cost to move to/from the start/end vertices, to make sure
                    // they do not introduce a shortcut.
                    return 100000;
                }
            }
            return 1;
        }

        var algo = new FloydWarshallAllShortestPathAlgorithm<(char, CardinalDirection?), TaggedEdge<(char, CardinalDirection?), string>>(graph, EdgeCost);
        algo.Compute();

        var startEndVertices = graph.Vertices.Where(v => !v.Item2.HasValue).ToList();
        foreach (var source in startEndVertices)
        {
            foreach (var target in startEndVertices)
            {
                if (algo.TryGetPath(source, target, out var path))
                {
                    var buttons = path.Select(e => e.Tag).StringJoin("");
                    buttonPressMap.Add((source.Item1, target.Item1), buttons);
                }
                else if (source.Equals(target))
                {
                    buttonPressMap.Add((source.Item1, target.Item1), "A");
                }
            }
        }

        return buttonPressMap;
    }

    private string ExpandInput(string input, Dictionary<(char, char), string> buttonPressMap)
    {
        var stringbuilder = new StringBuilder();

        ("A" + input).Pairwise((a, b) => (a, b)).ForEach(pair => stringbuilder.Append(buttonPressMap[(pair.a, pair.b)]));

        return stringbuilder.ToString();
    }

    private long Complexity(Func<ComplexityArgs, long> recurse, ComplexityArgs args)
    {
        if (args.RoundsLeft == 0)
        {
            return args.Input.Length;
        }

        /*
         * Observation: in order to move from one button to another we have to enter a
         * sequence of buttons in the current directional pad. However, in order to press the button
         * we always have to end a seqence with an "A".
         * 
         * So when considering adjacent subsequences, we can always assume the previous subsequence ended on A.
         */
        var complexity = ("A" + args.Input)
            .Pairwise((a, b) => recurse(new(args.ButtonPressMap[(a, b)], args.RoundsLeft - 1, args.ButtonPressMap)))
            .Sum();


        return complexity;
    }

    private record struct RobotsState(char Robot1, char Robot2, char Robot3);
    private record ComplexityArgs(string Input, int RoundsLeft, Dictionary<(char, char), string> ButtonPressMap);
}
