using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataMigrationFramework.Model;
using Rhino.Mocks;

namespace DataMigrationFramework.Unit.Test
{
    internal class DataMigrationCreator<T>
    {
        public DataMigrationCreator(Settings settings)
        {
            this.MockSource = MockRepository.GenerateMock<ISource<T>>();
            this.MockDestination = MockRepository.GenerateMock<IDestination<T>>();
            this.MockSource.Stub(source => source.PrepareAsync(null)).IgnoreArguments().Return(Task.FromResult(0));
            this.MockDestination.Stub(dest => dest.PrepareAsync(null)).IgnoreArguments().Return(Task.FromResult(0));
            this.MockSource.Stub(source => source.CleanupAsync(MigrationStatus.Completed)).IgnoreArguments().Return(Task.FromResult(0));
            this.MockDestination.Stub(dest => dest.CleanupAsync(MigrationStatus.Completed)).IgnoreArguments().Return(Task.FromResult(0));

            DefaultDataMigration = new DefaultDataMigration<T>(
                Guid.NewGuid(),
                "testing",
                this.MockSource,
                this.MockDestination,
                settings,
                new Dictionary<string, string>());
        }


        public IDataMigration DefaultDataMigration { get; }
        public ISource<T> MockSource { get; }
        public IDestination<T> MockDestination { get; }
    }
}
