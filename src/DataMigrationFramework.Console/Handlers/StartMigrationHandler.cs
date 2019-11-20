using System;
using System.Threading;
using System.Threading.Tasks;
using DataMigrationFramework.Console.Requests;
using MediatR;

namespace DataMigrationFramework.Console.Handlers
{
    public class StartMigrationHandler : IRequestHandler<StartMigrationRequest, Guid>
    {
        private readonly IMigrationFactory _factory;

        public StartMigrationHandler(IMigrationFactory factory)
        {
            _factory = factory;
        }

        public Task<Guid> Handle(StartMigrationRequest request, CancellationToken cancellationToken)
        {
            var migration = _factory.Get(request.Name, request.Parameters);
            migration.Subscribe(s =>
            {
                System.Console.WriteLine($"[Notification] {s.Id}: {s.Status}");
            });
            migration.StartAsync(Guid.NewGuid());
            return Task.FromResult(Guid.NewGuid());
        }
    }
}
