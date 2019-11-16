using System;
using System.Reflection;
using Autofac;
using Autofac.Core;
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
            builder.RegisterGeneric(typeof(DefaultDataMigration<>))
                .WithParameter(
                    new ResolvedParameter(
                        (pi, ctx) => pi.ParameterType == typeof(Settings),
                        (pi, ctx) =>
                        {
                            if (config.Settings == null)
                            {
                                return Settings.Default;
                            }
                            return config.Settings;
                        }));
        }
    }
}