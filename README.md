# datamigration-framwork

Any data migration needs to get data from a __source__ and move to __destination__. Along the way it needs some throttling ( when running in production not to consume the resources) and some features like monitoring , stop and start the process etc. This framework written to take care of all other features 

## Installing / Getting started

You should install using nuget here.

### Motivation
Any data migration needs to get data from a __source__ and move to __destination__. Along the way it needs some throttling ( when running in production not to consume the resources) and some features like monitoring , stop and start the process etc. This framework written to take care of all other features 

### Features
* Configure with Source and Destination and Model types
* Throttling feature where one can slow down the process in control fashion if needed in production scenarios.
* Monitoring the progress

### Develop migration source  and destination
See the samples for developing a migration.

