using System;
using System.Reflection;

namespace DataMigrationFramework.Model
{
    public class Configuration
    {
        public Configuration(string name, string sourceTypeName, string destinationTypeName, string modelTypeName)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.SourceTypeName = ValidTypeName(sourceTypeName,"sourceTypeName");
            this.DestinationTypeName = ValidTypeName(destinationTypeName, "destinationTypeName");
            this.ModelTypeName = ValidTypeName(modelTypeName, "modelTypeName");
        }

        public string Name { get;  }
        public string SourceTypeName { get; }
        public string DestinationTypeName { get; }
        public string ModelTypeName { get;}

        public Type SourceType => ParseForType(this.SourceTypeName);
        public Type DestinationType => ParseForType(this.DestinationTypeName);
        public Type ModelType => ParseForType(this.ModelTypeName);

        private static Type ParseForType(string typeName)
        {
            var parts = typeName.Split(',');
            var asm = Assembly.LoadFrom(parts[1]);
            return asm.GetType(parts[0], true);
        }

        private string ValidTypeName(string typeName, string property)
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
    }
}