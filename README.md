# MiniOrm 
A custom-built C# ORM for PostgreSQL


### ️Configure Database Connection

Add your connection string to `launchSettings.json` (inside the `Properties` folder of both projects):

### Run Database Migrations

Use the CLI to automatically generate and apply SQL tables based on your C# models.

**1. Start the CLI**

```bash
cd MiniOrm.Migrations
dotnet run

```

**2. Available Commands**
Once the CLI is running, type any of the following:

* `add <Name>` : Generates a new `.sql` script with Up/Down logic.
* `apply` : Executes pending migrations safely.
* `list` : Shows the execution history of all migrations.
* `rollback` : Reverses the most recently applied migration.
```
  MiniOrm
├──  MiniOrm.sln
├── 📁 MiniOrm
│   ├──  MiniOrm.csproj
│   ├──  Dependencies
│   ├──  Properties
│   │   └──  launchSettings.json
│   ├──  Attributes
│   │   ├── 📄 ColumnAttribute.cs
│   │   ├── 📄 PrimaryKeyAttribute.cs
│   │   └── 📄 TableAttribute.cs
│   ├──  Data
│   │   ├── 📄 AppDbContext.cs
│   │   ├── 📄 DbContext.cs
│   │   ├── 📄 DbSet.cs
│   │   ├── 📄 EntityMetadata.cs
│   │   └── 📄 TypeMapper.cs
│   ├──  Models
│   │   ├── 📄 Order.cs
│   │   └── 📄 Product.cs
│   └── 📄 Program.cs
└── 📁 MiniOrm.Migrations
    ├──  MiniOrm.Migrations.csproj
    ├──  Dependencies
    ├──  Properties
    │   └──  launchSettings.json
    ├──  SqlMigrations
    ├── 📄 MigrationRunner.cs
    └── 📄 Program.cs
```
###  Validate CRUD Operations

Use the core project to test your CRUD (Create, Read, Update, Delete) operations and ensure your ORM is communicating correctly with PostgreSQL.

**1. Run the Project**
```bash
cd MiniOrm
dotnet run
```


