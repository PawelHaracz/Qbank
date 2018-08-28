using System.Runtime.Serialization;
using Qbank.Core;
using Qbank.Core.Event;

namespace Qbank.Test.Core
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class ActionOccured : IEvent
    {
        public const string TypeId = "97289C97-ACDC-49D8-8A0F-CD8A829A0DD1";

        ActionOccured()
        {
        }

        public ActionOccured(string name)
        {
            Name = name;
        }

        [DataMember(Order = 1)]
        public string Name { get; private set; }

        public override string ToString()
        {
            return $"{nameof(ActionOccured)} with Name:'{Name}'";
        }
    }
}