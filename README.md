# datamigration-framework ( work in progress)

Any data migration needs to get data from a __source__ and move to __destination__. Along the way it needs some throttling ( when running in production not to consume the resources) and some features like monitoring , stop and start the process etc. This framework written to take care of all other features 


### Installing DataMigrationFramework

You should install [DataMigrationFramework with NuGet](https://www.nuget.org/packages/DataMigrationFramework):

    Install-Package DataMigrationFramework
    
Or via the .NET Core command line interface:

    dotnet add package DataMigrationFramework

Either commands, from Package Manager Console or .NET Core CLI, will download and install DataMigrationFramework and all required dependencies.


### Motivation
Any data migration needs to get data from a __source__ and move to __destination__. Along the way it needs some throttling ( when running in production not to consume the resources) and some features like monitoring , stop and start the process etc. This framework written to take care of all other features 

### Features
* Configure with Source and Destination and Model types
* Throttling feature where one can slow down the process in control fashion if needed in production scenarios.
* Monitoring the progress

### Developing a migration work flow
#### Step1
Create a POCO class which will be shared between source and destination
```csharp
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
```

#### Step2
Implement __ISource<T>__ for producing the data
```csharp
    public class PersonFileSource : ISource<Person>
    {
        public async Task<IEnumerable<Person>> ProduceAsync(int batchSize)
        {
            // produce the data here.
        }
    }
```

#### Step3
Implement __IDestination<T>__ for consuming the data.
```csharp
    public class PersonFileDestination :IDestination<Person>
    {
        public async Task<int> ConsumeAsync(IEnumerable<Person> persons)
        {
            // consume and return number of successful records updated (used for tracking errors.)
        }
    }
```

#### Step4
Define a configuration file to wire up all source and destination
```json
[
  {
    "name": "personDataMigration",
    "sourceTypeName": "DataMigrationFramework.Samples.PersonFileSource,DataMigrationFramework.Samples.dll",
    "destinationTypeName": "DataMigrationFramework.Samples.PersonFileDestination,DataMigrationFramework.Samples.dll",
    "ModelTypeName": "DataMigrationFramework.Samples.Model.Person,DataMigrationFramework.Samples.dll",
    "settings": {
      "batchSize": 1,
      "delayBetweenBatches": 2000,
      "errorThresholdBeforeExit": 100,
      "notifyStatusRecordSizeFrequency": 100,
      "maxNumberOfRecords":  100000,
      "numberOfProducers": 4, 
      "numberOfConsumers": 4,
    },
    "parameters": {
      "inputFileName": "",
      "ouputFileName": ""
    }
  },
  {
    "name": "sampleDataMigration",
    "sourceTypeName": "DataMigrationFramework.Samples.SampleDataSource,DataMigrationFramework.Samples.dll",
    "destinationTypeName": "DataMigrationFramework.Samples.SampleDataDestination,DataMigrationFramework.Samples.dll",
    "ModelTypeName": "DataMigrationFramework.Samples.Model.SampleData,DataMigrationFramework.Samples.dll"
  }

]
```
#### Step5 (Registering Autofac module)
```csharp
    builder.RegisterModule(new MigrationModule(File.ReadAllText("migrationConfig.json")));
```

#### Step6 (Subscribe for progress)
```csharp
    migration.Subscribe(s =>
    {
        System.Console.WriteLine($"[Notification] {s.Id}: {s.Status}");
    });

```

#### Step7 (Starting migration)
```csharp
    // unique id through which one can get the status and stop the existing migration
    // Name of the migration task (should be one of value defined in configuration)
    // Parameters which will be passed to source and destination (through PrepareAsync method).
    var migrationTask = _manager.Create(uniqueId,  "personDataMigration", new Dictionary<string,string>{});
    await migrationTask.StartAsync();
```


### Coniguration
| Name                                      |  Description                                       | Default  |
|------------------------------------------|----------------------------------------------------|----------|
| BatchSize                                |  Number of items produced by the source            |   5      | 
| DelayBetweenBatches                      |  Delay between each batch produce (throttling)     |   10(ms) | 
| NumberOfProducers                        |  Number of producers (throttling)                  |   1      | 
| NumberOfConsumers                        |  Number of consumers (throttling)                  |   1      | 
| ErrorThresholdBeforeExit                 |  Errors allowed before migration terminated        |   10     | 
| MaxNumberOfRecords                       |  Maximum records allowed in migration              | 1000000  | 
| NotifyStatusRecordSizeFrequency          |  Notifications freqency to notify observers        |   100    | 


### Samples
See the samples which does person data migration from file to file.



