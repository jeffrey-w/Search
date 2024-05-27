namespace Search;

/// <summary>
/// The <c>IAction</c> interface provides properties on a transition from one <see cref="IState"/> to another.
/// </summary>
public interface IAction
{
    /// <summary>
    /// The degree to which this <c>IAction</c> frustrates progress towards an <see cref="ISolution"/>.
    /// </summary>
    public int Cost { get; }
    /// <summary>
    /// The <see cref="IState"/> induced by this <c>IAction</c>.
    /// </summary>
    public IState Successor { get; }
}

/// <summary>
/// The <c>Action</c> class provides a concreate implemenation of the <see cref="IAction"/> interface.
/// </summary>
public sealed class Action : IAction
{
    /// <summary>
    /// Creates a new <see cref="IAction"/> with the specified <paramref name="cost"/> and <paramref name="successor"/>.
    /// </summary>
    /// <param name="cost">The degree to which the new <see cref="IAction"/> frustrates progress towards an <see
    /// cref="ISolution"/>.</param>
    /// <param name="successor">The <see cref="IState"/> induced by the new <see cref="IAction"/>.</param>
    /// <returns>A new <see cref="IAction"/>.</returns>
    public static IAction Make(int cost, IState successor)
    {
        return new Action(cost, successor);
    }

    /// <inheritdoc/> 
    public int Cost { get; }
    /// <inheritdoc/> 
    public IState Successor { get; }

    private Action(int cost, IState successor)
    {
        Cost = cost;
        Successor = successor;
    }
}
