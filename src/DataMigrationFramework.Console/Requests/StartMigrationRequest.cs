using System;
using System.Collections.Generic;
using MediatR;

namespace DataMigrationFramework.Console.Requests
{
    public class StartMigrationRequest : IRequest<Guid>
    {
        public StartMigrationRequest(string name, IDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null.", nameof(name));
            }

            this.Name = name;
            Parameters = parameters;
        }

        public string Name { get; }
        public IDictionary<string, string> Parameters { get; }
    }
}
