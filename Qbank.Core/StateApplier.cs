namespace Qbank.Core
{
    public class StateApplier
    {
        /// <summary>
        /// Applies events using poorly performing 'dynamic' approach.
        /// One could IL Emit proper applier ;-)
        /// </summary>
        public static void Apply<TState>(TState state, IEvent @event)
            where TState : BaseState
        {
            ((dynamic)state).Apply((dynamic)@event);
        }
    }
}