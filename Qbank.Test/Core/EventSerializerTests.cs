using System;
using NUnit.Framework;
using Qbank.Core;
using Qbank.Core.Event;

namespace Qbank.Test.Core
{
    public class EventSerializerTests
    {
        [Test]
        public void Test()
        {
            var e = new ActionOccured("a");
            var serialized = EventSerializer.Serialize(e);
            var deserialized = EventSerializer.Deserialize(serialized, new Guid(ActionOccured.TypeId));

            Assert.AreEqual(e.ToString(), deserialized.ToString());
        }
    }
}