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
    public class Factory
    {
        private readonly IContainer _container;
        private readonly IEnumerable<Configuration> _configs;

        public Factory(string configValue)
        {
            var builder = new Autofac.ContainerBuilder();
            this._configs = JsonConvert.DeserializeObject<IEnumerable<Configuration>>(configValue).ToList();
            foreach (var config in this._configs)
            {
                builder.Register(config);
            }

            _container = builder.Build();
        }

        public IDataMigration Get(string name)
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
            var dataMigrationType = typeof(DefaultDataMigration<>).MakeGenericType(new Type[] { config.ModelType });
            return (IDataMigration)this._container.Resolve(dataMigrationType, new Parameter[] { });
        }
    }
}
