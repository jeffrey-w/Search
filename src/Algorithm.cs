namespace Search;

/// <summary>
/// The <c>IAlgorithm</c> interface provides operations for finding a sequence of <see cref="IState"/>s that constitute
/// an <see cref="ISolution"/> to a problem.
/// </summary>
public interface IAlgorithm
{
    /// <summary>
    /// Searchs for an <see cref="ISolution"/> to the problem initiating at the specified <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The initial <see cref="IState"/> of the problem being solved..</param>
    /// <returns>An <see cref="ISolution"/> to the problem initiating at the specfied <paramref
    /// name="state"/>.</returns>
    /// <exception cref="OutOfMemoryException">If the search for an <see cref="ISolution"/> exhausts its runtime
    /// environment's memory.</exception>
    public ISolution Run(IState state);
}

internal abstract class BaseAlgorithm : IAlgorithm
{
    internal sealed class Node
    {
        public static Node Max = new(null, 0, int.MaxValue, DummyState.Instance);

        public static Node MakeRoot(IState state)
        {
            return new Node(null, 0, 0, state);
        }

        public Node? Parent { get; }
        public int Depth { get; }
        public int Cost { get; }
        public IState State { get; }

        private Node(Node? parent, int depth, int cost, IState state)
        {
            Parent = parent;
            Depth = depth;
            Cost = cost;
            State = state;
        }

        public Node MakeChild(IAction action)
        {
            return new Node(this, Depth + 1, Cost + action.Cost, action.Successor);
        }
    }

    public ISolution Run(IState state)
    {
        var root = Node.MakeRoot(state);
        var frontier = MakeFrontier(root);
        var visited = MakeVisited(root);
        while (frontier.Count > 0)
        {
            var node = frontier.Dequeue();
            if (node.State.IsGoal)
            {
                return Solution.Success(node);
            }
            foreach (var action in node.State.Actions)
            {
                var child = node.MakeChild(action);
                var other = visited.GetValueOrDefault(child.State, Node.Max);
                if (child.Cost < other.Cost)
                {
                    frontier.Enqueue(child, Evaluate(child));
                    visited[child.State] = child;
                }
            }
        }
        return Solution.Failure;
    }

    private PriorityQueue<Node, int> MakeFrontier(Node root)
    {
        var frontier = new PriorityQueue<Node, int>();
        frontier.Enqueue(root, Evaluate(root));
        return frontier;
    }

    private static Dictionary<IState, Node> MakeVisited(Node root)
    {
        return new Dictionary<IState, Node>{ { root.State, root } };
    }

    protected abstract int Evaluate(Node node);

}

internal sealed class BreadthFirstSearchAlgorithm : BaseAlgorithm
{
    protected override int Evaluate(Node node)
    {
        return node.Depth;
    }
}

internal sealed class DepthFirstSearchAlgorithm : BaseAlgorithm
{
    protected override int Evaluate(Node node)
    {
        return -node.Depth;
    }
}

internal sealed class UniformCostSearchAlgorithm : BaseAlgorithm
{
    protected override int Evaluate(Node node)
    {
        return node.Cost;
    }
}

internal sealed class GreedySearchAlgorithm : BaseAlgorithm
{
    protected override int Evaluate(Node node)
    {
        return node.State.Fitness;
    }
}

internal sealed class AStarSearchAlgorithm : BaseAlgorithm
{
    protected override int Evaluate(Node node)
    {
        return node.State.Fitness + node.Cost;
    }
}

/// <summary>
/// The <c>Algorithm</c> class provides a means for obtaining different search strategies.
/// </summary>
public static class Algorithm
{
    /// <summary>
    /// Provides a breadth-first search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm BreadthFirstSearch = new BreadthFirstSearchAlgorithm();
    /// <summary>
    /// Provides a depth-first search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm DepthFirstSearch = new DepthFirstSearchAlgorithm();
    /// <summary>
    /// Provides Djikstra's search algorithm over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm UniformCostSearch = new UniformCostSearchAlgorithm();
    /// <summary>
    /// Provides a greedy best-first search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm GreedySearch = new GreedySearchAlgorithm();
    /// <summary>
    /// Provides an A* search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm AStarSearch = new AStarSearchAlgorithm();
}
