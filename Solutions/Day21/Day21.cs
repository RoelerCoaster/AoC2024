using MoreLinq;
using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using System.Text.RegularExpressions;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day21;

internal class Day21 : DayBase
{
    public override int Day => 21;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Part1;


    /*
     * We have the following configuration
     * 
     * Numpad (robot3) - directional pad (robot2) - directional pad (robot1) - directional pad (you)
     * 
     */
    protected override async Task<string> SolvePart1(string input)
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
        throw new NotImplementedException();
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

    private record struct RobotsState(char Robot1, char Robot2, char Robot3);
}
