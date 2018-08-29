using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Qbank.Core.Event
{
    public class EventStoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventStoreConnectionProvider>().As<IEventStoreConnectionProvider>().InstancePerLifetimeScope();
        }
    }
}
