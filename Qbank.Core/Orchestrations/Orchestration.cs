using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qbank.Core.Event;
using Qbank.Core.Orchestrations.Impl;

namespace Qbank.Core.Orchestrations
{
    public abstract class Orchestration<TInput> : IOrchestration
    {
        IOrchestrationContext _context;
        Queue<IEvent> _recorded;
        ICallSerializer _serializer;

        async Task IOrchestration.Execute(object input, IOrchestrationContext context, IEnumerable<IEvent> events, ICallSerializer serializer)
        {
            this._serializer = serializer;
            this._context = context;
            _recorded = new Queue<IEvent>(events);

            await Execute((TInput)input).ConfigureAwait(false);

            OrchestrationEnded end;
            if (TryPop(out end) == false)
            {
                Append(OrchestrationEnded.Instance);
            }
        }

        protected abstract Task Execute(TInput input);

        protected NowOrNever Delay(TimeSpan delay)
        {
            var date = GetDateTimeUtcNow();
            var scheduleAt = date + delay;

            ScheduledAt existingDelay;
            if (TryPop(out existingDelay))
            {
                if (existingDelay.Value > _context.DateTimeUtcNow())
                {
                    EndCurrentExecution();
                    return NowOrNever.Never;
                }

                return NowOrNever.Now;
            }

            if (scheduleAt <= date)
            {
                return NowOrNever.Now;
            }

            Append(new ScheduledAt(scheduleAt));
            EndCurrentExecution();
            return NowOrNever.Never;
        }

        protected Guid NewGuid()
        {
            GuidGenerated generated;
            if (TryPop(out generated))
            {
                return generated.Value;
            }

            generated = new GuidGenerated(_context.NewGuid());
            Append(generated);
            return generated.Value;
        }

        protected DateTimeOffset GetDateTimeUtcNow()
        {
            DateTimeRetrieved generated;
            if (TryPop(out generated))
            {
                return generated.Value;
            }

            generated = new DateTimeRetrieved(_context.DateTimeUtcNow());
            Append(generated);
            return generated.Value;
        }

        protected async Task<TResult> Call<TResult>(CallDelegate<TResult> call)
        {
            var callId = NewGuid();
            CallRecorded callRecorded;

            if (TryPop(out callRecorded))
            {
                if (callRecorded.ExceptionMessage == null)
                {
                    return _serializer.Deserialize<TResult>(callRecorded.Payload);
                }

                throw new TaskFailedException(callRecorded.ExceptionMessage, callRecorded.ExceptionStackTrace);
            }

            // flush before calling external system
            await FlushEvents().ConfigureAwait(false);

            var result = default(TResult);
            TaskFailedException exceptionToThrow = null;
            try
            {
                result = await call(callId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                exceptionToThrow = new TaskFailedException(ex.Message, ex.StackTrace);
            }

            if (exceptionToThrow == null)
            {
                var payload = _serializer.Serialize(result);
                Append(new CallRecorded(payload));

                // don't flush after call, it's assumed that it's idempotent
                return result;
            }

            Append(new CallRecorded(exceptionToThrow.Message, exceptionToThrow.StackTrace));
            throw exceptionToThrow;
        }

        public Task FlushEvents()
        {
            return _context.Flush();
        }

        void EndCurrentExecution()
        {
            _context.EndCurrentExecution();
        }

        void Append<TEvent>(TEvent @event) where TEvent : IEvent
        {
            _context.Append(@event);
        }

        bool TryPop<TEvent>(out TEvent @event)
             where TEvent : IEvent
        {
            if (_recorded.Count > 0)
            {
                @event = (TEvent)_recorded.Dequeue();
                return true;
            }

            @event = default(TEvent);
            return false;
        }
    }
}