using System;

namespace Qbank.Core
{
    /// <summary>
    ///     Identifies event type with a Guid. Guids are much easier to preserve than a non-colliding names across modules.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EventTypeIdAttribute : Attribute
    {
        public readonly Guid Id;

        public EventTypeIdAttribute(string id)
        {
            Id = new Guid(id);
        }
    }
}