using MoreLinq;
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

        var cliques = new HashSet<HashSet<string>>(HashSet<string>.CreateSetComparer());

        cliques.UnionWith(graph.Edges.Select(e => new HashSet<string> { e.Source, e.Target }));

        for (; ; )
        {
            var newCliques = new HashSet<HashSet<string>>(HashSet<string>.CreateSetComparer());

            foreach (var clique in cliques)
            {
                var allAdjacent = clique.Select(vertex =>
                    graph.AdjacentEdges(vertex).Select(e => e.GetOtherVertex(vertex))
                    .Except(clique)
                    .ToHashSet());

                var intersection = allAdjacent.Aggregate((a, b) =>
                {
                    a.IntersectWith(b);
                    return a;
                });

                foreach (var vertex in intersection)
                {
                    newCliques.Add(clique.Append(vertex).ToHashSet());
                }
            }

            if (newCliques.Count == 0)
            {
                break;
            }
            cliques = newCliques;
        }

        return cliques.Single().Order().StringJoin(",");
    }


    private static UndirectedGraph<string, Edge<string>> GetGraph(string input)
    {
        return input.Lines()
                    .Select(l => l.Split('-'))
                    .Select(split => new Edge<string>(split[0], split[1]))
                    .ToUndirectedGraph<string, Edge<string>>(false);
    }
}
