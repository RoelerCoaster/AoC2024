using QuikGraph;
using QuikGraph.Algorithms.ShortestPath;
using RoelerCoaster.AdventOfCode.Year2024.Internals.Model;
using RoelerCoaster.AdventOfCode.Year2024.Util;
using RoelerCoaster.AdventOfCode.Year2024.Util.Model;

namespace RoelerCoaster.AdventOfCode.Year2024.Solutions.Day16;

internal class Day16 : DayBase
{
    public override int Day => 16;

    public override bool UseTestInput => false;

    protected override PartToRun PartsToRun => PartToRun.Both;

    /*
     * General idea:
     * 
     * We turn the maze into a graph
     * Each corridor position will become 4 nodes, one for each cardinal direaction.
     * These will be connected as:
     *    
     *    N
     *   / \
     *  W   E
     *   \ /
     *    S
     *    
     * "Rotating" will simply be moving between these nodes, with the appopriate cost.
     * 
     * Then we connect the nodes to the adjacent locations based on the possible movements.
     */
    protected override async Task<string> SolvePart1(string input)
    {
        var (potentialSolutions, _) = FindShortestPaths(input);
        return potentialSolutions.Min(s => s.Distance).ToString();
    }

    protected override async Task<string> SolvePart2(string input)
    {
        var (potentialSolutions, allPredecessors) = FindShortestPaths(input);

        var min = potentialSolutions.Min(n => n.Distance);

        // Starting from the end nodes, we wal back trough all paths back to the start using the recorded predecessors
        // Since we record all visited positions in a set, get all unique visited locations.
        var allVisited = new HashSet<GridCoordinate>();
        var queue = new Queue<MazeNode>(potentialSolutions.Where(n => n.Distance == min).Select(n => n.Node));

        while (queue.TryDequeue(out var next))
        {
            allVisited.Add(next.Position);

            if (allPredecessors.ContainsKey(next))
            {
                allPredecessors[next].ForEach(queue.Enqueue);
            }
        }

        return allVisited.Count.ToString();
    }

    private (List<NodeDistance> EndNodeDistances, Dictionary<MazeNode, List<MazeNode>> AllPredecessors) FindShortestPaths(string input)
    {
        var grid = input.Grid();

        var cardinalDirections = Enum.GetValues<CardinalDirection>();
        var graph = GetGraph(grid, cardinalDirections);


        var start = grid.FindFirst(c => c == 'S');
        var end = grid.FindFirst(c => c == 'E');

        var dijkstra = new DijkstraShortestPathAlgorithm<MazeNode, Edge<MazeNode>>(graph, EdgeCost);
        var recorder = new AllPredecessorRecorder<MazeNode, Edge<MazeNode>>(dijkstra);

        dijkstra.Compute(new(start, CardinalDirection.East));

        var potentialSolutions = cardinalDirections.Select(d =>
        {
            var node = new MazeNode(end, d);

            return new NodeDistance(node, dijkstra.TryGetDistance(node, out var distance) ? distance : double.PositiveInfinity);
        });

        return (potentialSolutions.ToList(), recorder.AllPredecessors);
    }

    private static double EdgeCost(Edge<MazeNode> edge) => edge.Source.Direction != edge.Target.Direction ? 1000 : 1;

    private static AdjacencyGraph<MazeNode, Edge<MazeNode>> GetGraph(char[][] grid, CardinalDirection[] cardinalDirections)
    {
        var graph = new AdjacencyGraph<MazeNode, Edge<MazeNode>>();

        grid.ForEachGridElement((c, pos) =>
        {
            if (c is '#')
            {
                return;
            }

            foreach (var direction in cardinalDirections)
            {
                graph.AddVerticesAndEdge(new Edge<MazeNode>(new(pos, direction), new(pos, direction.RotateClockwise())));
                graph.AddVerticesAndEdge(new Edge<MazeNode>(new(pos, direction), new(pos, direction.RotateCounterClockwise())));

                var adjacent = pos.CoordinateInDirection(direction);
                if (grid.Get(adjacent) is not '#')
                {
                    graph.AddVerticesAndEdge(new Edge<MazeNode>(new(pos, direction), new(adjacent, direction)));
                }
            }
        });
        return graph;
    }

    private record struct MazeNode(GridCoordinate Position, CardinalDirection Direction);
    private record struct NodeDistance(MazeNode Node, double Distance);


    private class AllPredecessorRecorder<TVertex, TEdge>
        where TEdge : IEdge<TVertex> where TVertex : notnull
    {
        public Dictionary<TVertex, List<TVertex>> AllPredecessors { get; } = new();

        public AllPredecessorRecorder(DijkstraShortestPathAlgorithm<TVertex, TEdge> algorithm)
        {
            // The TreeEdge event is fired when the distance on a vertex has decreased.
            // We therefore have a new shortest paths, and reset the list of predecessors
            algorithm.TreeEdge += edge =>
            {
                AllPredecessors[edge.Target] = new List<TVertex> { edge.Source };
            };

            /*
             * The EdgeNotRelaxed event is fired when a vertex was reached, but the distance was not decreased.
             * It may be the case that the distance was equal, which would introduce an alternated shortest path.
             * We check that case here
             */
            algorithm.EdgeNotRelaxed += edge =>
            {
                var targtDist = algorithm.GetDistance(edge.Target);
                var srcDist = algorithm.GetDistance(edge.Source);
                var newDist = algorithm.DistanceRelaxer.Combine(srcDist, algorithm.Weights(edge));

                if (targtDist == newDist)
                {
                    AllPredecessors[edge.Target].Add(edge.Source);
                }
            };
        }


    }
}
