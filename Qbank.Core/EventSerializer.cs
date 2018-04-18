using System;

namespace Qbank.Core
{
    public class EventSerializer
    {
        static readonly IReadOnlyDictionary<Guid, Type> EventTypeLookup;

        static EventSerializer()
        {
            EventTypeLookup = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IEvent).IsAssignableFrom(t) && t != typeof(IEvent))
                .ToDictionary(t => t.GetCustomAttribute<EventTypeIdAttribute>().Id, t => t);
        }

        public static byte[] Serialize(IEvent e)
        {
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, e);
                return ms.ToArray();
            }
        }

        public static IEvent Deserialize(byte[] data, Guid eventTypeId)
        {
            return (IEvent)Serializer.Deserialize(EventTypeLookup[eventTypeId], new MemoryStream(data));
        }
    }
}
