# TrappyKeepy  

A Simple Document Storage API

## Database  

The database is PostgreSQL.  

Connectivity to the PostgreSQL database from .NET is provided by the Npgsql.EntityFrameworkCore.PostgreSQL libarary.  

For development, a Vagrant box is setup to create a fresh PostgreSQL database instance that is ready to go. Read about [installing Vagrant](https://www.vagrantup.com/docs/installation) if needed. Once installed, run `vagrant up` from the root directory of this code repository where the `Vagrantfile` is located, which will create and configure the guest machine using the `Vagrant-setup/bootstrap.sh` shell script. Some helpful details for accessing this database are available in `Vagrant-setup/access.md`.  

## Secrets  

Secrets are managed using the [Secrets Manager tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=linux#secret-manager).  

To enable secret storage for the project, run the following command:  

```bash
dotnet user-secrets init --project TrappyKeepy.Api
```

This adds the secret storage ID to the project's `csproj` file.  

As an example, the database connection string for development is stored as a secret by running the following command.  

```bash
dotnet user-secrets set ConnectionStrings:keepydb "Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass" --project TrappyKeepy.Api
```

That secret connection string is accessed in the `Program.cs` file like this:  

```csharp
var connectionString = builder.Configuration["ConnectionStrings.keepydb"];
```

## Makefile  

For common actions like clean, restore, migrate, build, and run, a `Makefile` exists in the source code root directory so these could be easily organized as `Makefile` recipes.  

The `make all` command will run a combination of a clean, restore, migrate, build, and run.  

The `make flyway` command will download and extract the Flyway applition. This is used for the database migrations.  

The `make clean` command runs the `dotnet clean TrappyKeepy.Api` command.  

The `make restore` command runs the `dotnet restore TrappyKeepy.Api` command.  

The `make migrate` command migrates the database using the migration scripts located in the `Migrations` directory.  

The `make build` command reverse-engineers the database context and model classes from the PostgreSQL database into .NET, overwriting existing classes with the current database structure, then formats the source code of the project to match the `.editorconfig` file settings by running the `dotnet format TrappyKeepy.Api` command, and then builds the project by running `dotnet build TrappyKeepy.Api`.  

The `make test` command runs the `dotnet test TrappyKeepy.Api` command.  

The `make run` command runs the `dotnet run --project TrappyKeepy.Api` command.  
