using Autofac;

namespace DataMigrationFramework.Samples
{
    public class SampleAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(ctx => new DataAccess(@"TestFiles\personsdata.txt", @"TestFiles\personsout.txt"))
                .As<IDataAccess>().InstancePerLifetimeScope();
        }
    }
}
