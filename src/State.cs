namespace Search;

/// <summary>
/// The <c>IState</c> interface provides properties on a discrete configuration of the salient features of an <see
/// cref="IProblem{TState}"/>.
/// </summary>
public interface IState
{
    /// <summary>
    /// Indicates whether this <c>IState</c> is an <see cref="ISolution"/> to an <see cref="IProblem{TState}"/>.
    /// </summary>
    public bool IsGoal { get; }
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
