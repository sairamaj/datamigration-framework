using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Autofac;
using MediatR;

namespace DataMigrationFramework.Console
{
    internal class ServiceContainer
    {
        public static IContainer Initialize()
        {
            var builder = new ContainerBuilder();
            builder
                .RegisterType<Mediator>()
                .As<IMediator>()
                .InstancePerLifetimeScope();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            var migrationFactory = new DefaultMigrationFactory(File.ReadAllText("migrationinfo.json"));
            builder.RegisterInstance(migrationFactory).As<IMigrationFactory>();
            builder.RegisterAssemblyTypes(typeof(ServiceContainer).GetTypeInfo().Assembly).AsImplementedInterfaces();
            return builder.Build();
        }
    }
}
