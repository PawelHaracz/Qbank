using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ProtoBuf;
using ProtoBuf.Meta;
using Qbank.Core.Serialization;

namespace Qbank.Core
{
    public static class EventSerializer
    {
        static readonly Dictionary<Guid, Type> EventTypeLookup = new Dictionary<Guid, Type>();
        static readonly RuntimeTypeModel TypeModel = RuntimeTypeModel.Default;

        static EventSerializer()
        {
            DateTimeOffsetSurrogate.Install(TypeModel);

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Register(asm);
            }
        }

        public static void Register(Assembly asm)
        {
            var values = asm.GetTypes()
                .Where(t => typeof(IEvent).IsAssignableFrom(t) && t != typeof(IEvent))
                .ToDictionary(t => t.GetCustomAttribute<EventTypeIdAttribute>().Id, t => t);

            foreach (var kvp in values)
            {
                if (EventTypeLookup.ContainsKey(kvp.Key) == false)
                {
                    EventTypeLookup.Add(kvp.Key, kvp.Value);

                    var metaType = TypeModel.Add(kvp.Value, true);
                    metaType.UseConstructor = false;
                }
            }
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
