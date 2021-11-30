# TrappyKeepy  

A Simple Document Storage [Web API](https://dotnet.microsoft.com/apps/aspnet/apis)  

Users, user roles, user groups, and documents (aka keepers).  

Written using [.NET6](https://dotnet.microsoft.com/download/dotnet/6.0) with the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/), [C# 10](https://devblogs.microsoft.com/dotnet/welcome-to-csharp-10/), and [PostgreSQL](https://www.postgresql.org/).  

## Using the `Makefile`  

For common actions like clean, restore, migrate, build, and run, a `Makefile` exists in the source code root directory so these could be easily organized as `Makefile` recipes. You can run any of the following commands from the root directory of the project's source code.  

Command|Description
--|--
`make all` | Run a combination of a clean, restore, migrate, build, and run.
`make flyway` | Download and extract the [Flyway](https://flywaydb.org) application. This is used for the database migrations, so this command must be run prior to running the `make migrate` command for the first time.
`make clean` | Clean the outputs (both the intermediate (obj) and final output (bin) folders) bu running the `dotnet clean` command for the TrappyKeepy.Api project.
`make restore` | Restore the dependencies and tools by running the `dotnet restore` command for all projects.
`make migrate` | Migrate the database using the SQL migration scripts located in the `TrappyKeepy.Data/Migrations` directory, using the Flyway application to apply and track the migrations. For more information such as the file naming patterns used by Flyway, see their [SQL-based migrations](https://flywaydb.org/documentation/concepts/migrations#sql-based-migrations) documentation page.
`make dbscaffold` | Reverse-engineer the database context and model classes from the PostgreSQL database into .NET, overwriting existing classes with the current database structure, by running the `dotnet ef dbcontext scaffold` command with proper arguments. This command reads the database connection string from the Microsoft Secrets Manager (see the Secrets section above).
`make format` | Format the source code of  all projects to match the `.editorconfig` file settings by running the `dotnet format` command.
`make build` | Build the project by running `dotnet build` command for the TrappyKeepy.Api project.
`make test` | Execute the unit tests, but running the `dotnet test` command for the TrappyKeepy.Test project.
`make run` | Start the TrappyKeepy application by running the `dotnet run --project TrappyKeepy.Api` command.

## Env vars  

The following development environment variables with development values provide an example of the environment variables required in production. Environment variable values can be set in the `/etc/environment` file on a Linux host system:  

```bash
export TKDB_URL="jdbc:postgresql://localhost:15432/keepydb"
export TKDB_USER="dbuser"
export TKDB_OWNER="dbowner"
export TKDB_PASSWORD="dbpass"
export TKDB_MIGRATIONS="filesystem:./TrappyKeepy.Data/Migrations"
export TKDB_CONN_STRING="Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass"
export TK_JWT_SECRET="devTestEnvironment"
export TK_CRYPTO_KEY="MqSm0P5dMgFSZhEBKpCv4dVKgDrsgrmT"
```

## Database  

The database is PostgreSQL.  

There are a few environment variables that must be set related to the database (see the Env vars section above).  

For development, a Vagrant box is setup to create a fresh PostgreSQL database instance that is ready to go. Read about [installing Vagrant](https://www.vagrantup.com/docs/installation) if needed. Once installed, run `vagrant up` from the root directory of this code repository where the `Vagrantfile` is located, which will create and configure the guest machine using the `Vagrant-setup/bootstrap.sh` shell script. Some helpful details for accessing this database are available in `Vagrant-setup/access.md`.  

## Secret Manager  

This only applies to the Database-first reverse-engineering / scaffolding to turn the PostgreSQL table types into .NET model types. This is only done in a development environment after making changes to the database.  

When running the `make dbscaffold` command, the `dotnet ef` tool reads the database connection string from the Microsoft [Secret Manager tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=linux#secret-manager). Before running the `make dbscaffold` command for the first time, the Secret Manager tool must be initialized and the development database connection string must be saved to the secret storage.  

WARNING: The Secret Manager tool doesn't encrypt the stored secrets and shouldn't be treated as a trusted store. It's for development purposes only. The keys and values are stored in a JSON configuration file in the user profile directory (`~/.microsoft/usersecrets/`).  

To enable secret storage for the project, and add the secret storage ID to the project's `csproj` file, run the following command:  

```bash
dotnet user-secrets init --project TrappyKeepy.Api
```

The development database connection string for development is stored as a secret by running the following command.  

```bash
dotnet user-secrets set ConnectionStrings:TKDB_CONN_STRING "Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass" --project TrappyKeepy.Api
```

To view all currently stored development secrets, run the following command:  

```bash
dotnet user-secrets list --project TrappyKeepy.Api
```

## First Admin  

To create the first administrator user, connect to the database and insert the user by running the tk.users_create function. Here is an example using development values:  

```sql
SELECT * FROM tk.users_create('foo', 'passwordfoo', 'foo@example.com', '2');
```
