using QuikGraph;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day23;

internal class Day23 : DayBase
{
    public override int Day => 23;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var graph = GetGraph(input);

        var foundTriangles = new HashSet<(string, string, string)>();

        foreach (var vertex in graph.Vertices)
        {
            var adjacent = graph.AdjacentEdges(vertex).Select(e => e.GetOtherVertex(vertex)).ToList();

            foreach (var (a, b) in adjacent.UnorderedPairs(true))
            {
                if (graph.ContainsEdge(a, b))
                {
                    var triangle = new[] { vertex, a, b }.Order().ToList();
                    foundTriangles.Add((triangle[0], triangle[1], triangle[2]));
                }
            }
        }

        return foundTriangles
            .Where(triangle => triangle.Item1.StartsWith('t') || triangle.Item2.StartsWith('t') || triangle.Item3.StartsWith('t'))
            .Count()
            .ToString();
    }


    /*
     * We basically have to find the max clique...
     */
    protected override async Task<string> SolvePart2(string input)
    {
        var graph = GetGraph(input);


        return string.Empty;
    }


    private static UndirectedGraph<string, Edge<string>> GetGraph(string input)
    {
        return input.Lines()
                    .Select(l => l.Split('-'))
                    .Select(split => new Edge<string>(split[0], split[1]))
                    .ToUndirectedGraph<string, Edge<string>>(false);
    }
}
