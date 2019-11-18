using System;
using Autofac;
using DataMigrationFramework.Model;

namespace DataMigrationFramework.Extension
{
    internal static class BuilderExtension
    {
        public static void Register(this ContainerBuilder builder, Configuration config)
        {
            builder.RegisterAssemblyTypes(config.SourceType.Assembly).AsClosedTypesOf(typeof(ISource<>));
            builder.RegisterAssemblyTypes(config.DestinationType.Assembly).AsClosedTypesOf(typeof(IDestination<>));
            builder.RegisterType(config.ModelType);
            builder.RegisterGeneric(typeof(DefaultDataMigration<>));
            Console.WriteLine($"Registering module.{config.SourceType.Assembly.FullName}");
            builder.RegisterAssemblyModules(config.SourceType.Assembly);
        }
    }
}