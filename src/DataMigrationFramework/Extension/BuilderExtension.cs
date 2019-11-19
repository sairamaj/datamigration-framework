using System;
using Autofac;
using DataMigrationFramework.Model;

namespace DataMigrationFramework.Extension
{
    /// <summary>
    /// Autofac builder extension.
    /// </summary>
    internal static class BuilderExtension
    {
        /// <summary>
        /// Register types containing in migration configuration.
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ContainerBuilder"/> instance.
        /// </param>
        /// <param name="config">
        /// A <see cref="Configuration"/> where types were defined.
        /// </param>
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