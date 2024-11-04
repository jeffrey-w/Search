namespace Search;

/// <summary>
/// The <c>IAlgorithm</c> interface provides operations for finding a sequence of <see cref="IState"/>s that constitute
/// an <see cref="ISolution"/> to a <see cref="Search"/> <see cref="IProblem{TState}"/>.
/// </summary>
/// <typeparam name="TState">The type of <see cref="IState"/> over which this <c>IAlgorithm</c> provides <see
/// cref="ISolution"/>s.</typeparam>
public interface IAlgorithm<TState> where TState : IState
{
    /// <summary>
    /// <see cref="Search"/>s for the <see cref="ISolution"/> to the specified <paramref name="problem"/>.
    /// </summary>
    /// <param name="problem">The <see cref="IProblem{TState}"/> being solved.</param>
    /// <returns>An <see cref="ISolution"/> to the specified <paramref name="problem"/>.</returns>
    /// <exception cref="OutOfMemoryException">If the search for an <see cref="ISolution"/> exhausts its runtime
    /// environment's memory.</exception>
    public ISolution Run(IProblem<TState> problem);
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

            public Node MakeChild(IAction<TState> action)
            {
                return new Node(this, Depth + 1, Cost + action.Cost, action.Successor);
            }
        }

        protected readonly Dictionary<TState, Node> Visited = [];

        private readonly PriorityQueue<Node, int> _frontier = new();

        public ISolution Solve(IProblem<TState> problem)
        {
            var root = Node.MakeRoot(problem.Start);
            _frontier.Enqueue(root, Evaluate(root));
            Visited.Add(problem.Start, root);
            while (_frontier.Count > 0)
            {
                var node = _frontier.Dequeue();
                if (node.State.IsGoal)
                {
                    return Solution.Success(node);
                }
                foreach (var action in problem.GetActions(node.State))
                {
                    var child = node.MakeChild(action);
                    if (IsToVisit(child))
                    {
                        Visited.Add(child.State, child);
                        _frontier.Enqueue(child, Evaluate(child));
                    }
                }
            }
            return Solution.Failure;
        }

        protected abstract int Evaluate(Node node);
        protected abstract bool IsToVisit(Node node);

    }

    private abstract class Solver<TState> : BaseSolver<TState> where TState : IState
    {
        protected override bool IsToVisit(Node node)
        {
            return !Visited.ContainsKey(node.State);
        }
    }

    private abstract class CostSensitiveSolver<TState> : BaseSolver<TState> where TState : IState
    {
        protected override bool IsToVisit(Node node)
        {
            return !Visited.TryGetValue(node.State, out var other) || node.Cost < other.Cost;
        }
    }

    private sealed class BreadthFirstSolver<TState> : Solver<TState> where TState : IState
    {
        protected override int Evaluate(Node node)
        {
            return node.Depth;
        }
    }

    private sealed class DepthFirstSolver<TState> : Solver<TState> where TState : IState
    {
        protected override int Evaluate(Node node)
        {
            return -node.Depth;
        }
    }

    private sealed class UniformCostSolver<TState> : CostSensitiveSolver<TState> where TState : IState
    {
        protected override int Evaluate(Node node)
        {
            return node.Cost;
        }
    }

    private sealed class GreedySolver<TState> : CostSensitiveSolver<TState> where TState : IHeuristicState
    {
        protected override int Evaluate(Node node)
        {
            return node.State.Fitness;
        }
    }

    private sealed class AStarSolver<TState> : CostSensitiveSolver<TState> where TState : IHeuristicState
    {
        protected override int Evaluate(Node node)
        {
            return node.State.Fitness + node.Cost;
        }
    }

    private sealed class AlgorithmImpl<TState>(Func<BaseSolver<TState>> factory) : IAlgorithm<TState> where TState : IState
    {
        public ISolution Run(IProblem<TState> problem)
        {
            return factory.Invoke()
                          .Solve(problem);
        }
    }

    /// <summary>
    /// Provides a breadth-first search over <typeparamref name="TState"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of <see cref="IState"/> over which the new <see cref="IAlgorithm{TState}"/>
    /// searches.</typeparam>
    /// <returns>A new <see cref="IAlgorithm{TState}"/></returns>
    public static IAlgorithm<TState> BreadthFirstSearch<TState>()where TState : IState => new AlgorithmImpl<TState>(() => new BreadthFirstSolver<TState>());
    /// <summary>
    /// Provides a depth-first search over <typeparamref name="TState"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of <see cref="IState"/> over which the new <see cref="IAlgorithm{TState}"/>
    /// searches.</typeparam>
    /// <returns>A new <see cref="IAlgorithm{TState}"/></returns>
    public static IAlgorithm<TState> DepthFirstSearch<TState>() where TState : IState => new AlgorithmImpl<TState>(() => new DepthFirstSolver<TState>());
    /// <summary>
    /// Provides Dijkstra's search algorithm over <typeparamref name="TState"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of <see cref="IState"/> over which the new <see cref="IAlgorithm{TState}"/>
    /// searches.</typeparam>
    /// <returns>A new <see cref="IAlgorithm{TState}"/></returns>
    public static IAlgorithm<TState> UniformCostSearch<TState>() where TState : IState => new AlgorithmImpl<TState>(() => new UniformCostSolver<TState>());
    /// <summary>
    /// Provides a greedy best-first search over <typeparamref name="TState"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of <see cref="IState"/> over which the new <see cref="IAlgorithm{TState}"/>
    /// searches.</typeparam>
    /// <returns>A new <see cref="IAlgorithm{TState}"/></returns>
    public static IAlgorithm<TState> GreedySearch<TState>() where TState : IHeuristicState => new AlgorithmImpl<TState>(() => new GreedySolver<TState>());
    /// <summary>
    /// Provides an A* search over <typeparamref name="TState"/>s.
    /// </summary>
    /// <typeparam name="TState">The type of <see cref="IState"/> over which the new <see cref="IAlgorithm{TState}"/>
    /// searches.</typeparam>
    /// <returns>A new <see cref="IAlgorithm{TState}"/></returns>
    public static IAlgorithm<TState> AStarSearch<TState>() where TState : IHeuristicState => new AlgorithmImpl<TState>(() => new AStarSolver<TState>());
}
