# Running

When using Visual Studio 2015+ the NuGet packages should be restored automatically.  If you find that they do not or if you are using an older version of Visual Studio please execute the following in a Visual Studio command prompt:

~~~
cd {extraction-folder}\Shuttle.Esb.Samples\Shuttle.RequestResponse
nuget restore
~~~

If that does not work try executing the following in the **Package Manager Console**:

~~~
update-package -reinstall
~~~

Once you have opened the `Shuttle.RequestResponse.sln` solution in Visual Studio set the following at the startup project:

- Shuttle.TenPinBowling.Shell

# Database

When you reference the `Shuttle.Recall.SqlServer` package a number of scripts are included in the relevant package folder:

- `.\Shuttle.TenPinBowling\packages\Shuttle.Recall.SqlServer.{version}\scripts`

The `{version}` bit will be in a `semver` format.

> Create a new database called **Shuttle** and execute script `EventStoreCreate.sql` in the newly created database.
> Create a new database called **ShuttleProjection** and execute script `ProjectionCreate.sql` in the newly created database.

There is also a `.script` folder for the sample project:

- `.\Shuttle.TenPinBowling\.script`

> Execute the 'ten-pin-bowling.sql' script against the **ShuttleProjection** database.