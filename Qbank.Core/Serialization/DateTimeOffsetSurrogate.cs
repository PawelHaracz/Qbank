﻿using System;
using ProtoBuf;
using ProtoBuf.Meta;

namespace Qbank.Core.Serialization
{
    [ProtoContract]
    public class DateTimeOffsetSurrogate
    {
        [ProtoMember(1)]
        public long DateTimeTicks { get; set; }
        [ProtoMember(2)]
        public short OffsetMinutes { get; set; }

        public static implicit operator DateTimeOffsetSurrogate(DateTimeOffset value)
        {
            return new DateTimeOffsetSurrogate
            {
                DateTimeTicks = value.Ticks,
                OffsetMinutes = (short)value.Offset.TotalMinutes
            };
        }

        public static implicit operator DateTimeOffset(DateTimeOffsetSurrogate value)
        {
            return new DateTimeOffset(value.DateTimeTicks, TimeSpan.FromMinutes(value.OffsetMinutes));
        }

        public static void Install(RuntimeTypeModel model)
        {
            model.Add(typeof(DateTimeOffset), false).SetSurrogate(typeof(DateTimeOffsetSurrogate));
        }
    }
}