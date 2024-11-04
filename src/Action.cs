namespace Search;

/// <summary>
/// The <c>IAction</c> interface provides properties on a transition from one <see cref="IState"/> to another.
/// </summary>
/// <typeparam name="TState">The type of <see cref="IState"/> induced by this <c>IAction</c>.</typeparam>
public interface IAction<out TState> where TState : IState
{
    /// <summary>
    /// The degree to which this <c>IAction</c> frustrates progress towards an <see cref="ISolution"/>.
    /// </summary>
    public int Cost { get; }
    /// <summary>
    /// The <see cref="IState"/> induced by this <c>IAction</c>.
    /// </summary>
    public TState Successor { get; }
}

/// <summary>
/// The <c>Action</c> class provides a minimal implementation of the <see cref="IAction{TState}"/> interface.
/// </summary>
public static class Action
{
    /// <summary>
    /// Creates a new <see cref="IAction{TState}"/> with the specified <paramref name="cost"/> and <paramref
    /// name="successor"/>.
    /// </summary>
    /// <param name="cost">The degree to which the new <see cref="IAction{TState}"/> frustrates progress towards an <see
    /// cref="ISolution"/>.</param>
    /// <param name="successor">The <see cref="IState"/> induced by the new <see cref="IAction{TState}"/>.</param>
    /// <returns>A new <see cref="IAction{TState}"/>.</returns>
    public static IAction<TState> Make<TState>(int cost, TState successor) where TState : IState
    {
        return new Action<TState>(cost, successor);
    }
}

/// <inheritdoc cref="Action"/>
/// <typeparam name="TState">The type of <see cref="IState"/> induced by this <c>IAction</c>.</typeparam>
public sealed class Action<TState> : IAction<TState> where TState : IState
{
    /// <inheritdoc/> 
    public int Cost { get; }
    /// <inheritdoc/> 
    public TState Successor { get; }

    internal Action(int cost, TState successor)
    {
        Cost = cost;
        Successor = successor;
    }
}
