using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ShortestPath;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day18;

internal class Day18 : DayBase
{
    public override int Day => 18;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    protected override async Task<string> SolvePart1(string input)
    {
        var gridSize = 70;
        var corruption = input.Lines()
            .Select(l => l.NumbersBySeparator<int>(","))
            .Select(pair => new XYCoordinate(pair[0], pair[1]))
            .ToList();

        // Interpret the grid as a graph
        var graph = CreateGraph(gridSize);

        foreach (var corruptedSpot in corruption[..1024])
        {
            graph.RemoveVertex(corruptedSpot);
        }

        var dijkstra = new UndirectedDijkstraShortestPathAlgorithm<XYCoordinate, Edge<XYCoordinate>>(graph, _ => 1);

        dijkstra.Compute(new(0, 0));

        return dijkstra.GetDistance(new(gridSize, gridSize)).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var gridSize = 70;
        var corruption = input.Lines()
            .Select(l => l.NumbersBySeparator<int>(","))
            .Select(pair => new XYCoordinate(pair[0], pair[1]))
            .ToList();

        // Interpret the grid as a graph
        var graph = CreateGraph(gridSize);

        // From part 1 we know we can safely remove the first 1024 coordinates without destroying reachability 
        foreach (var corruptedSpot in corruption[..1024])
        {
            graph.RemoveVertex(corruptedSpot);
        }

        var root = new XYCoordinate(0, 0);
        var dest = new XYCoordinate(gridSize, gridSize);
        HashSet<XYCoordinate>? pathVertices = null;
        foreach (var corruptedSpot in corruption.Skip(1024))
        {
            graph.RemoveVertex(corruptedSpot);
            if (pathVertices == null || pathVertices.Contains(corruptedSpot))
            {
                // We removed a vertex from the shortest path (or we haven't computed a path yet). Recompute.
                var tryGetPath = graph.ShortestPathsDijkstra(_ => 1, root);

                if (!tryGetPath(dest, out var edges))
                {
                    // Path is broken. Return result
                    return corruptedSpot.X + "," + corruptedSpot.Y;
                }

                pathVertices = edges.Select(e => e.Target).ToHashSet();
                pathVertices.Add(root);
            }
        }

        throw new InvalidOperationException();
    }

    private static UndirectedGraph<XYCoordinate, Edge<XYCoordinate>> CreateGraph(int gridSize)
    {
        var edges = Enumerable.Range(0, gridSize + 1)
                    .SelectMany(x =>
                        Enumerable.Range(0, gridSize)
                            .SelectMany(y => new[]
                            {
                        new Edge<XYCoordinate>(new(x, y), new(x, y + 1)),
                        new Edge<XYCoordinate>(new(y, x), new(y + 1, x))
                            })
                        );

        var graph = edges.ToUndirectedGraph<XYCoordinate, Edge<XYCoordinate>>(false);
        return graph;
    }

    private record struct XYCoordinate(int X, int Y);
}
