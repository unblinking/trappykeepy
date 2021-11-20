# TrappyKeepy  

A Simple Document Storage API

## Database  

The database is PostgreSQL.  

Connectivity to the PostgreSQL database from .NET is provided by the Npgsql libarary. Set the database connection string using the environment variable `TKDB`.  

For development, a Vagrant box is setup to create a fresh PostgreSQL database instance that is ready to go. Read about [installing Vagrant](https://www.vagrantup.com/docs/installation) if needed. Once installed, run `vagrant up` from the root directory of this code repository where the `Vagrantfile` is located, which will create and configure the guest machine using the `Vagrant-setup/bootstrap.sh` shell script. Some helpful details for accessing this database are available in `Vagrant-setup/access.md`.  

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
