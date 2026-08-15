# Database updates

Revert to migration name:

```
dotnet ef database update <previous-migration-name>
```

Remove initial update / drop database:

```
dotnet ef database drop --force
```

# Migrations

Remove last migration:

```
dotnet ef migrations remove
```

