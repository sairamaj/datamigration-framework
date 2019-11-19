using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Console.Requests;
using DataMigrationFramework.Model;
using MediatR;

namespace DataMigrationFramework.Console.Handlers
{
    public class ListMigrationInfoHandler : IRequestHandler<ListMigrationInfo, IEnumerable<Configuration>>
    {
        private readonly IMigrationFactory _factory;

        public ListMigrationInfoHandler(IMigrationFactory factory)
        {
            _factory = factory;
        }

        public Task<IEnumerable<Configuration>> Handle(ListMigrationInfo request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_factory.Configuration);
        }
    }
}
