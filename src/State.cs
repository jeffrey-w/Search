namespace Search;

/// <summary>
/// The <c>IState</c> interface provides properties on a discrete configuration of the salient features of a search
/// problem.
/// </summary>
public interface IState
{
    /// <summary>
    /// Indicates whether this <c>IState</c> is the terminal of an <see cref="ISolution"/>.
    /// </summary>
    public bool IsGoal { get; }
    /// <summary>
    /// The <see cref="IAction"/>s executable from this <c>IState</c> to transition to neighboring <c>IState</c>s.
    /// </summary>
    public IEnumerable<IAction> Actions { get; }
}

/// <summary>
/// The <c>IHeuristicState</c> interface provides properties on an <see cref="IState"/> for which evaluative information
/// may be obtained.
/// </summary>
public interface IHeuristicState : IState
{
    /// <summary>
    /// The degree to which this <c>IState</c> constitutes progress towards an <see cref="ISolution"/>.
    /// </summary>
    public int Fitness { get; }
}
