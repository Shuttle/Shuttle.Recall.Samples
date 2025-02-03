## Running

Once you have opened the `Shuttle.TenPinBowling.sln` solution in Visual Studio set the following at the startup project:

- Shuttle.TenPinBowling.Shell

## Database

The sample project uses the `Shuttle.Recall.Sql.Storage` and `Shuttle.Recall.Sql.EventProcessing` packages to store and process events.

In order to create the necessary databases and tables you will need to execute the relevant scripts.  The easiest is to clone the following repositories:

```cmd
git clone https://github.com/Shuttle/Shuttle.Recall.EFCore.SqlServer.Storage
git clone https://github.com/Shuttle/Shuttle.Recall.EFCore.SqlServer.EventProcessing
```

The execute the following commands Entity Framwork Core commands in solution folder:

```
dotnet ef database update --project .\Shuttle.Recall.EFCore.SqlServer.Storage\Shuttle.Recall.EFCore.SqlServer.Storage.csproj --connection "Server=.;Database=RecallSamples;User Id=sa;Password=Pass!000;TrustServerCertificate=true" -- --SchemaOverride="TenPinBowling"
dotnet ef database update --project .\Shuttle.Recall.EFCore.SqlServer.EventProcessing\Shuttle.Recall.EFCore.SqlServer.EventProcessing.csproj --connection "Server=.;Database=RecallSamples;User Id=sa;Password=Pass!000;TrustServerCertificate=true" -- --SchemaOverride="TenPinBowling"
```

There is also a `.script` folder for the ten pin bowling project:

- `.\Shuttle.TenPinBowling\.script`
