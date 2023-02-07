using System;
using MongoDB.Bson;
using MongoDB.Driver.Core.Events;
using StackExchange.Profiling;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

// ReSharper disable UnusedParameter.Local
// ReSharper disable UnusedMember.Local
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter

namespace MiniProfiler.MongoDB
{
    public class MiniProfilerMongoDbEventSubscriber : IEventSubscriber
    {
        private readonly ConcurrentDictionary<int, (CustomTiming Profiler, Stopwatch Timming)> _commands = new();
        private readonly IEventSubscriber _subscriber;

        public MiniProfilerMongoDbEventSubscriber()
        {
            _subscriber = new ReflectionEventSubscriber(this, bindingFlags: BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public bool TryGetEventHandler<TEvent>(out Action<TEvent> handler)
        {
            var result = _subscriber.TryGetEventHandler(out handler);
            return result;
        }

        private void Handle(CommandStartedEvent @event)
        {
            var profiler = StackExchange.Profiling.MiniProfiler.Current.CustomTiming("mongodb", @event.Command.ToJson(), @event.CommandName);
            if (profiler is not null)
            {
                _commands[@event.RequestId] = (profiler, new Stopwatch());
            }
        }

        private void Handle(ConnectionSentMessagesEvent @event)
        {
            foreach (var requestId in @event.RequestIds)
            {
                if (_commands.TryGetValue(requestId, out var data))
                {
                    var cmdStr = $@"
Request:
                       Size: {@event.Length:N0} bytes
            NetworkDuration: {@event.NetworkDuration:ss\.fffff}  Speed:{@event.Length / @event.NetworkDuration.TotalMilliseconds / 1000:N3} MBps
      SerializationDuration: {@event.SerializationDuration:ss\.fffff}
              TotalDuration: {@event.Duration:ss\.fffff}

-----------------------------------------------------------

";
                    data.Profiler.CommandString = $"{cmdStr}{data.Profiler.CommandString}";
                    data.Timming.Start();
                }
            }
        }

        private void Handle(ConnectionReceivingMessageEvent @event)
        {
            if (_commands.TryGetValue(@event.ResponseTo, out var data))
            {
                data.Timming.Stop();
            }
        }

        private void Handle(ConnectionReceivedMessageEvent @event)
        {
            if (_commands.TryRemove(@event.ResponseTo, out var data))
            {
                data.Profiler.CommandString += $@"

-----------------------------------------------------------

Response:
                       Size: {@event.Length:N0} bytes
                       TTFB: {data.Timming.Elapsed:ss\.fffffff}
            NetworkDuration: {@event.NetworkDuration:ss\.fffff}  Speed:{@event.Length / @event.NetworkDuration.TotalMilliseconds / 1000:N3} MBps
    DeserializationDuration: {@event.DeserializationDuration:ss\.fffff}
              TotalDuration: {@event.Duration:ss\.fffff}";

                data.Profiler.Stop();
            }
        }

        private void Handle(CommandSucceededEvent @event)
        {
        }

        private void Handle(CommandFailedEvent @event)
        {
            if (_commands.TryRemove(@event.RequestId, out var data))
            {
                data.Profiler.Errored = true;
                data.Profiler.Stop();
            }
        }
    }
}


