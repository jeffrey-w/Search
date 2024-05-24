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
    /// The degree to which this <c>IState</c> constitutes progress towards an <see cref="ISolution"/>.
    /// </summary>
    public int Fitness { get; }
    /// <summary>
    /// The <see cref="IAction"/>s executable from this <c>IState</c> to transition to neighboring <c>IState</c>s.
    /// </summary>
    public IEnumerable<IAction> Actions { get; }
}

internal sealed class DummyState : IState
{
    public static IState Instance = new DummyState();

    public bool IsGoal => throw new NotImplementedException();
    public int Fitness => throw new NotImplementedException();
    public IEnumerable<IAction> Actions => throw new NotImplementedException();

    private DummyState()
    {
    }
}
