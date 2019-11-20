using System;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Console.Requests;
using MediatR;

namespace DataMigrationFramework.Console.Handlers
{
    public class StartMigrationHandler : IRequestHandler<StartMigrationRequest, Guid>
    {
        private readonly IMigrationManager _manager;

        public StartMigrationHandler(IMigrationManager manager)
        {
            _manager = manager;
        }

        public Task<Guid> Handle(StartMigrationRequest request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            var migration = _manager.Get(id,  request.Name, request.Parameters);
            migration.Subscribe(s =>
            {
                System.Console.WriteLine($"[Notification] {s.Id}: {s.Status}");
            });
            migration.StartAsync();
            return Task.FromResult(id);
        }
    }
}
