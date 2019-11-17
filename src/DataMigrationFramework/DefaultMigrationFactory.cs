using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Core;
using DataMigrationFramework.Model;
using DataMigrationFramework.Extension;
using Newtonsoft.Json;

namespace DataMigrationFramework
{
    public class DefaultMigrationFactory : IMigrationFactory
    {
        private readonly IContainer _container;
        private readonly IEnumerable<Configuration> _configs;

        public DefaultMigrationFactory(string configValue)
        {
            var builder = new Autofac.ContainerBuilder();
            this._configs = JsonConvert.DeserializeObject<IEnumerable<Configuration>>(configValue).ToList();
            foreach (var config in this._configs)
            {
                builder.Register(config);
            }

            _container = builder.Build();
        }

        public IDataMigration Get(string name, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(nameof(name));
            }

            var config = _configs.FirstOrDefault(c => c.Name == name);
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

        public IEnumerable<Configuration> GetInfo()
        {
            return this._configs;
        }
    }
}
