using System;
using System.Collections;
using Qbank.Core;

namespace Qbank.Test.Core.Orchestrations
{
    public class EventComparer : IComparer
    {
        private readonly Func<IEvent, IEvent, int> _comparer;

        public EventComparer()
            : this(Compare)
        { }

        public EventComparer(Func<IEvent, IEvent, int> comparer)
        {
            this._comparer = comparer;
        }

        public int Compare(object x, object y)
        {
            if (x == null && y == null)
                return 0;
            if (x == null)
                return -1;
            if (y == null)
                return 1;

            var typeComparison = TypeAsLong(x).CompareTo(TypeAsLong(y));

            if (typeComparison != 0)
                return typeComparison;

            return _comparer((IEvent)x, (IEvent)y);
        }

        static int Compare(IEvent xe, IEvent ye)
        {
            var xBytes = EventSerializer.Serialize(xe);
            var yBytes = EventSerializer.Serialize(ye);

            var lengthComparison = xBytes.Length.CompareTo(yBytes.Length);
            if (lengthComparison != 0)
                return lengthComparison;

            for (var i = 0; i < xBytes.Length; i++)
            {
                var comparison = xBytes[i].CompareTo(yBytes[i]);
                if (comparison != 0)
                    return comparison;
            }

            return 0;
        }

        static long TypeAsLong(object x)
        {
            return x.GetType().TypeHandle.Value.ToInt64();
        }
    }
}