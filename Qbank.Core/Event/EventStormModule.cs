using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Qbank.Core.Event
{
    public class EventStormModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EventStoreConnectionProvider>().As<IEventStoreConnectionProvider>().InstancePerLifetimeScope();
        }
    }
}
