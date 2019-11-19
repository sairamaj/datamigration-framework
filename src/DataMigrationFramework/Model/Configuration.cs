using System;
using System.Reflection;

namespace DataMigrationFramework.Model
{
    /// <summary>
    /// Configuration used for migration types.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configuration"/> class.
        /// </summary>
        /// <param name="name">
        /// Unique name of the data migration.
        /// </param>
        /// <param name="sourceTypeName">
        /// Type name (with name,assembly format) which implements <see cref="ISource{T}"/> for providing the data.
        /// </param>
        /// <param name="destinationTypeName">
        /// Type name (with name,assembly format) which implements <see cref="IDestination{T}"/> for consuming the data.
        /// </param>
        /// <param name="modelTypeName">
        /// Type name which is shared between source and destinations.
        /// </param>
        /// <param name="settings">
        /// A <see cref="Settings"/> used by the data migration during migration process.
        /// </param>
        public Configuration(
            string name,
            string sourceTypeName,
            string destinationTypeName,
            string modelTypeName,
            Settings settings)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.SourceTypeName = ValidTypeName(sourceTypeName, "sourceTypeName");
            this.DestinationTypeName = ValidTypeName(destinationTypeName, "destinationTypeName");
            this.ModelTypeName = ValidTypeName(modelTypeName, "modelTypeName");
            this.Settings = settings;
        }

        /// <summary>
        /// Gets name of the migration.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets source type name.
        /// </summary>
        public string SourceTypeName { get; }

        /// <summary>
        /// Gets destination type name.
        /// </summary>
        public string DestinationTypeName { get; }

        /// <summary>
        /// Gets model type name.
        /// </summary>
        public string ModelTypeName { get; }

        /// <summary>
        /// Gets settings.
        /// </summary>
        public Settings Settings { get; }

        /// <summary>
        /// Gets source type.
        /// </summary>
        public Type SourceType => ParseForType(this.SourceTypeName);

        /// <summary>
        /// Gets destination type.
        /// </summary>
        public Type DestinationType => ParseForType(this.DestinationTypeName);

        /// <summary>
        /// Gets model type.
        /// </summary>
        public Type ModelType => ParseForType(this.ModelTypeName);

        /// <summary>
        /// Valid type name for correct format which should be in name,assemblyName format.
        /// </summary>
        /// <param name="typeName">
        /// Type name with name,assemblyName format.
        /// </param>
        /// <param name="property">
        /// Property name to be used in exception to inform the property being failed.
        /// </param>
        /// <returns>
        /// Given type name echoed back if successful.
        /// </returns>
        private static string ValidTypeName(string typeName, string property)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentException($"TypeName cannot be null or empty.", property);
            }

            var parts = typeName.Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException($"{typeName} Type name is not well formed. It should be in type,assembly format.");
            }

            return typeName;
        }

        /// <summary>
        /// Parse the type name in to actual instance of type.
        /// </summary>
        /// <param name="typeName">
        /// Type name containing name and assembly name.
        /// </param>
        /// <returns>
        /// A <see cref="Type"/> from the assembly.
        /// </returns>
        private static Type ParseForType(string typeName)
        {
            var parts = typeName.Split(',');
            var asm = Assembly.LoadFrom(parts[1]);
            return asm.GetType(parts[0], true);
        }
    }
}