using System.IO;
using ProtoBuf;
using Qbank.Core.Orchestrations.Impl;

namespace Qbank.Test.Core.Orchestrations
{
    class CallSerializer : ICallSerializer
    {
        public byte[] Serialize<T>(T result)
        {
            var ms = new MemoryStream();
            Serializer.Serialize(ms, result);
            return ms.ToArray();
        }

        public T Deserialize<T>(byte[] payload)
        {
            return Serializer.Deserialize<T>(new MemoryStream(payload));
        }
    }
}