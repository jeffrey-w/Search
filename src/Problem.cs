namespace Search;

/// <summary>
/// The <c>IProblem</c> interface provides properties and operations on a <see cref="Search"/> space over <see
/// cref="IState"/>s. 
/// </summary>
/// <typeparam name="TState">The type of <see cref="IState"/> that is exhibited by this <c>IProblem</c>.</typeparam>
public interface IProblem<TState> where TState : IState
{
    /// <summary>
    /// The initial <see cref="IState"/> of this <c>IProblem</c>.
    /// </summary>
    public TState Start { get; }

    /// <summary>
    /// Provides the <see cref="IAction{TState}"/>s executable from the specified <paramref name="state"/> to transition
    /// to neighboring <see cref="IState"/>s.
    /// </summary>
    /// <param name="state">The current <see cref="IState"/> of this <c>IProblem</c>.</param>
    /// <returns>The <see cref="IAction{TState}"/>s that may be taken from the specified <paramref name="state"/>.</returns>
    public IEnumerable<IAction<TState>> GetActions(TState state);
}