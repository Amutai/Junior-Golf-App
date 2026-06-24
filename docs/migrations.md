# EF Core Migrations Workflow

## Overview

Database schema changes are managed via EF Core migrations. Every change is:
- **Versioned** — timestamped migration files in source control
- **Auditable** — reviewed in PRs like any other code
- **Automatic** — applied on app startup (no manual intervention)
- **Idempotent** — safe to run multiple times (skips already-applied)

---

## How to Create a Migration

### 1. Modify an entity or add a new one

```csharp
// src/JuniorGolf.Core/Entities/Event.cs
public class Event : BaseEntity
{
    public required string Name { get; set; }
    public DateTime StartDate { get; set; }
    // ...
}
```

### 2. Register in AppDbContext (if new entity)

```csharp
// src/JuniorGolf.Infrastructure/Data/AppDbContext.cs
public DbSet<Event> Events => Set<Event>();
```

### 3. Generate the migration

```bash
dotnet ef migrations add AddEvents \
  --project src/JuniorGolf.Infrastructure \
  --startup-project src/JuniorGolf.Api \
  --output-dir Data/Migrations
```

### 4. Review the generated files

- `Data/Migrations/<timestamp>_AddEvents.cs` — the Up/Down methods
- `Data/Migrations/<timestamp>_AddEvents.Designer.cs` — snapshot metadata
- `Data/Migrations/AppDbContextModelSnapshot.cs` — current model state

### 5. Commit and push

```bash
git add .
git commit -m "feat(database): add Events migration"
```

---

## How Migrations Are Applied

### Development (local)
On app startup in `Program.cs`:
```csharp
await db.Database.MigrateAsync();
```
This runs automatically — no manual steps needed.

### CI Pipeline
Migrations are validated by compiling the Infrastructure project. If a migration has errors, the build fails.

### Production
Same `MigrateAsync()` call on startup. For multi-instance deployments, use a pre-deploy job instead:
```bash
dotnet ef database update \
  --project src/JuniorGolf.Infrastructure \
  --startup-project src/JuniorGolf.Api
```

---

## How to Revert a Migration

### Remove the last migration (if not yet applied to DB):
```bash
dotnet ef migrations remove \
  --project src/JuniorGolf.Infrastructure \
  --startup-project src/JuniorGolf.Api
```

### Revert to a specific migration (if already applied):
```bash
dotnet ef database update <PreviousMigrationName> \
  --project src/JuniorGolf.Infrastructure \
  --startup-project src/JuniorGolf.Api
```

---

## Rules

1. **Never edit a migration that's been merged** — create a new one instead
2. **One migration per logical change** — don't combine unrelated schema changes
3. **Always review the generated SQL** — use `dotnet ef migrations script` to inspect
4. **Name migrations descriptively** — `AddEvents`, `AddPaymentStatus`, not `Update1`
