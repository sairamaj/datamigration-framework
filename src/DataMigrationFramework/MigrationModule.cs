using System.IO;
using Autofac;

namespace DataMigrationFramework
{
    /// <summary>
    /// Migration module for registering in to the container.
    /// </summary>
    public class MigrationModule : Module
    {
        /// <summary>
        /// Configuration JSON data containing migration information.
        /// </summary>
        private readonly string _configData;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationModule"/> class.
        /// </summary>
        /// <param name="configData">
        /// Configuration data.
        /// </param>
        public MigrationModule(string configData)
        {
            this._configData = configData;
        }

        /// <summary>
        /// Registers migration types in to container.
        /// </summary>
        /// <param name="builder">
        /// A <see cref="ContainerBuilder"/> instance.
        /// </param>
        protected override void Load(ContainerBuilder builder)
        {
            var migrationFactory = new DefaultMigrationFactory(this._configData);
            builder.RegisterInstance(migrationFactory).As<IMigrationFactory>().SingleInstance();
            builder.RegisterType<DefaultMigrationManager>().As<IMigrationManager>().SingleInstance();
            base.Load(builder);
        }
    }
}