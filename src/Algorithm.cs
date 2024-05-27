namespace Search;

/// <summary>
/// The <c>IAlgorithm</c> interface provides operations for finding a sequence of <see cref="IState"/>s that constitute
/// an <see cref="ISolution"/> to a <see cref="Search"/> problem.
/// </summary>
/// <typeparam name="TState">The type of <see cref="IState"/> over which this <c>IAlgorithm</c> provides <see
/// cref="ISolution"/>s.</typeparam>
public interface IAlgorithm<TState> where TState : IState
{
    /// <summary>
    /// <see cref="Search"/>s for the <see cref="ISolution"/> to the problem initiating at the specified <paramref
    /// name="state"/>.
    /// </summary>
    /// <param name="state">The initial <see cref="IState"/> of the problem being solved.</param>
    /// <returns>An <see cref="ISolution"/> to the problem initiating at the specfied <paramref
    /// name="state"/>.</returns>
    /// <exception cref="InvalidOperationException">If the specified <paramref name="state"/> provides <see
    /// cref="IAction"/>s that exhibit <see cref="IState">successors</see> of a different type.</exception>
    /// <exception cref="OutOfMemoryException">If the search for an <see cref="ISolution"/> exhausts its runtime
    /// environment's memory.</exception>
    public ISolution Run(TState state);
}

/// <summary>
/// The <c>Algorithm</c> class provides a means for obtaining different search strategies.
/// </summary>
/// <remarks>The <see cref="IAlgorithm{TState}"/>s provided herein are thread safe.</remarks>
public static class Algorithm
{
    internal abstract class BaseSolver<TState> where TState : IState
    {
        internal sealed class Node
        {
            public static Node MakeRoot(TState state)
            {
                return new Node(null, 0, 0, state);
            }

            public Node? Parent { get; }
            public int Depth { get; }
            public int Cost { get; }
            public TState State { get; }

            private Node(Node? parent, int depth, int cost, TState state)
            {
                Parent = parent;
                Depth = depth;
                Cost = cost;
                State = state;
            }

            public Node MakeChild(IAction action)
            {
                try
                {
                    return new Node(this, Depth + 1, Cost + action.Cost, (TState)action.Successor);
                }
                catch (InvalidCastException e)
                {
                    throw new InvalidOperationException(string.Empty, e);
                }
            }
        }

        protected readonly Dictionary<TState, Node> visited = [];

        private readonly PriorityQueue<Node, int> frontier = new();

        public ISolution Solve(TState state)
        {
            var root = Node.MakeRoot(state);
            frontier.Enqueue(root, Evaluate(root));
            visited.Add(state, root);
            while (frontier.Count > 0)
            {
                var node = frontier.Dequeue();
                if (node.State.IsGoal)
                {
                    return Solution.Success(node);
                }
                foreach (var action in node.State.Actions)
                {
                    if (TryGetChild(node, action, out var child))
                    {
                        frontier.Enqueue(child, Evaluate(child));
                    }
                }
            }
            return Solution.Failure;
        }

        protected abstract int Evaluate(Node node);
        protected abstract bool TryGetChild(Node node, IAction action, out Node child);

    }

    private abstract class Solver : BaseSolver<IState>
    {
        protected override bool TryGetChild(Node node, IAction action, out Node child)
        {
            child = node.MakeChild(action);
            if (visited.ContainsKey(child.State))
            {
                return false;
            }
            visited[child.State] = child;
            return true;
        }
    }

    private abstract class HeuristicSolver : BaseSolver<IHeuristicState>
    {
        protected override bool TryGetChild(Node node, IAction action, out Node child)
        {
            if (IsToVisit(child = node.MakeChild(action)))
            {
                visited[child.State] = child;
                return true;
            }
            return false;
        }

        private bool IsToVisit(Node child)
        {
            return !visited.TryGetValue(child.State, out var other) || child.Cost < other.Cost;
        }
    }

    private sealed class BreadthFirstSolver : Solver
    {
        protected override int Evaluate(Node node)
        {
            return node.Depth;
        }
    }

    private sealed class DepthFirstSolver : Solver
    {
        protected override int Evaluate(Node node)
        {
            return -node.Depth;
        }
    }

    private sealed class UniformCostSolver : Solver
    {
        protected override int Evaluate(Node node)
        {
            return node.Cost;
        }
    }

    private sealed class GreedySolver : HeuristicSolver
    {
        protected override int Evaluate(Node node)
        {
            return node.State.Fitness;
        }
    }

    private sealed class AStarSolver : HeuristicSolver
    {
        protected override int Evaluate(Node node)
        {
            return node.State.Fitness + node.Cost;
        }
    }

    private sealed class AlgorithmImpl<TState>(Func<BaseSolver<TState>> factory) : IAlgorithm<TState> where TState : IState
    {
        private readonly Func<BaseSolver<TState>> factory = factory;

        public ISolution Run(TState state)
        {
            return this.factory()
                       .Solve(state);
        }
    }

    /// <summary>
    /// Provides a breadth-first search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm<IState> BreadthFirstSearch = new AlgorithmImpl<IState>(() => new BreadthFirstSolver());
    /// <summary>
    /// Provides a depth-first search over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm<IState> DepthFirstSearch = new AlgorithmImpl<IState>(() => new DepthFirstSolver());
    /// <summary>
    /// Provides Djikstra's search algorithm over <see cref="IState"/>s.
    /// </summary>
    public static readonly IAlgorithm<IState> UniformCostSearch = new AlgorithmImpl<IState>(() => new UniformCostSolver());
    /// <summary>
    /// Provides a greedy best-first search over <see cref="IHeuristicState"/>s.
    /// </summary>
    public static readonly IAlgorithm<IHeuristicState> GreedySearch = new AlgorithmImpl<IHeuristicState>(() => new GreedySolver());
    /// <summary>
    /// Provides an A* search over <see cref="IHeuristicState"/>s.
    /// </summary>
    public static readonly IAlgorithm<IHeuristicState> AStarSearch = new AlgorithmImpl<IHeuristicState>(() => new AStarSolver());
}
