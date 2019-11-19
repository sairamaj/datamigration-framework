using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using DataMigrationFramework.Extension;
using DataMigrationFramework.Model;
using Newtonsoft.Json;

namespace DataMigrationFramework
{
    /// <summary>
    /// Default implementation of migration. Uses the JSON value to represent the migration information.
    /// </summary>
    public class DefaultMigrationFactory : IMigrationFactory
    {
        /// <summary>
        /// Autofac container used for registering and resolving the instances.
        /// </summary>
        private readonly IContainer _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultMigrationFactory"/> class.
        /// </summary>
        /// <param name="configValue">
        /// JSON Configuration value representing the migration information.
        /// </param>
        public DefaultMigrationFactory(string configValue)
        {
            var builder = new Autofac.ContainerBuilder();
            this.Configuration = JsonConvert.DeserializeObject<IEnumerable<Configuration>>(configValue).ToList();
            foreach (var config in this.Configuration)
            {
                builder.Register(config);
            }

            this._container = builder.Build();
        }

        /// <summary>
        /// Gets configuration representing data migrations.
        /// </summary>
        public IEnumerable<Configuration> Configuration { get; }

        /// <summary>
        /// Get data migration reference for the given name.
        /// </summary>
        /// <remarks>
        /// Name is defined in the data migration configuration JSON file.
        /// </remarks>
        /// <param name="name">
        /// Name of the data migration defined in configuration file.
        /// </param>
        /// <param name="parameters">
        /// Parameters passed to source and destination while creating the data migration.
        /// </param>
        /// <returns>
        /// A <see cref="IDataMigration"/> reference which can be used to start and stop the data migration.
        /// </returns>
        public IDataMigration Get(string name, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            var config = this.Configuration.FirstOrDefault(c => c.Name == name);
            if (config == null)
            {
                throw new ArgumentException($"{name} not found in configuration.");
            }

            var dataMigrationType = typeof(DefaultDataMigration<>).MakeGenericType(config.ModelType);
            return (IDataMigration)this._container.Resolve(
                dataMigrationType,
                new NamedParameter("settings", config.Settings ?? Settings.Default),
                new NamedParameter("parameters", parameters));
        }
    }
}