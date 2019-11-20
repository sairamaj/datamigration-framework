using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    internal class DataMigrationCreator
    {
        public DataMigrationCreator()
        {
            var mockSource = MockRepository.GenerateMock<ISource<string>>();
            var mockDestination = MockRepository.GenerateMock<IDestination<string>>();
            var settings = new Settings() { BatchSize = 1 };
            mockSource.Stub(source => source.PrepareAsync(null)).IgnoreArguments().Return(Task.FromResult(0));
            mockDestination.Stub(dest => dest.PrepareAsync(null)).IgnoreArguments().Return(Task.FromResult(0));
            mockSource.Stub(source => source.CleanupAsync(MigrationStatus.Completed)).IgnoreArguments().Return(Task.FromResult(0));
            mockDestination.Stub(dest => dest.CleanupAsync(MigrationStatus.Completed)).IgnoreArguments().Return(Task.FromResult(0));
            mockSource.Stub(source => source.ProduceAsync(settings.BatchSize))
                .Return(Task.FromResult<IEnumerable<string>>(new string[] { "item1" })).Repeat.Once();
            mockSource.Stub(source => source.ProduceAsync(settings.BatchSize))
                .Return(Task.FromResult<IEnumerable<string>>(new string[] { })).Repeat.Once();
            mockDestination.Stub(dest => dest.ConsumeAsync(null)).IgnoreArguments().Return(Task.FromResult(0));

            DefaultDataMigration = new DefaultDataMigration<string>(
                Guid.NewGuid(),
                "testing",
                mockSource,
                mockDestination,
                settings,
                new Dictionary<string, string>());
        }


        public IDataMigration DefaultDataMigration { get; }
    }
}
