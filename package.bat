echo off
md artifacts\staging\lib\net45
echo 'staging'
copy src\DataMigrationFramework\bin\Debug\netstandard2.0\DataMigrationFramework.dll artifacts\staging\lib\net45  
copy src\DataMigrationFramework\DataMigrationFramework.nuspec artifacts\staging
toolset\nuget\nuget pack -version 1.0.0 src\DataMigrationFramework\DataMigrationFramework.csproj -IncludeReferencedProjects -OutputDirectory artifacts\packages
