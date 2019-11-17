using System.Collections.Generic;
using DataMigrationFramework.Model;
using MediatR;

namespace DataMigrationFramework.Console.Requests
{
    public class ListMigrationInfo : IRequest<IEnumerable<Configuration>>
    {
    }
}
