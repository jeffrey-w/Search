namespace Search;

/// <summary>
/// The <c>ISolution</c> interface provides properties on a sequence of <see cref="IState"/>s that together constitute a
/// means for solving a problem.
/// </summary>
public interface ISolution
{
    /// <summary>
    /// Indicates whether this <see cref="ISolution"/>  cannot be found or is infeasible.
    /// </summary>
    public bool IsFailure { get; }
    /// <summary>
    /// The <see cref="IState"/>s that constitute this <c>ISolution</c>.
    /// </summary>
    public IEnumerable<IState> States { get; }
}

internal sealed class Solution : ISolution
{
    public static readonly ISolution Failure = new Solution([]);

    public static ISolution Success<TState>(Algorithm.BaseSolver<TState>.Node? node) where TState : IState
    {
        var states = new Stack<IState>();
        while (node is not null)
        {
            states.Push(node.State);
            node = node.Parent;
        }
        return new Solution(states);
    }

    public bool IsFailure => !States.Any();
    public IEnumerable<IState> States { get; }

    private Solution(IEnumerable<IState> states)
    {
        States = states.ToList();
    }
}
