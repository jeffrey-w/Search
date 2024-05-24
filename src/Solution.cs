namespace Search;

/// <summary>
/// The <c>ISolution</c> interface provides properties on a sequence of <see cref="IState"/>s that together constitute a
/// means for solving a problem.
/// </summary>
public interface ISolution
{
    /// <summary>
    /// Indicates whether this <see cref="ISolution"/>  cannot be found or infeasible.
    /// </summary>
    public bool IsFailure { get; }
    /// <summary>
    /// The <see cref="IState"/>s that constitute this <c>ISolution</c>.
    /// </summary>
    public IEnumerable<IState> States { get; }
}

internal sealed class Solution : ISolution
{
    public static ISolution Failure = new Solution([]);

    public static ISolution Success(BaseAlgorithm.Node node)
    {
        var states = new Stack<IState>([node.State]);
        while (node.Parent is not null)
        {
            node = node.Parent;
            states.Push(node.State);
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
