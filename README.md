# TrappyKeepy  

[![Build & Test](https://github.com/jmg1138/TrappyKeepy/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/jmg1138/TrappyKeepy/actions/workflows/build.yml) [![codecov](https://codecov.io/gh/jmg1138/trappykeepy/branch/main/graph/badge.svg?token=ARrGqDcKhD)](https://codecov.io/gh/jmg1138/trappykeepy)  

A Simple Document Storage Web API  

## Features  

- Users
- Groups
- Document storage
- Document access control

Users may be created and may sign in using their credentials to receive a session token. Groups may be created, and users may be given memberships to groups. Downloading documents requires permission per-document, and document access permits may be issued directly to users, or to groups. Users may be given roles of basic, manager, or admin. Users with the role of manager or admin may upload documents. Administrators may access all API endpoints so that they may manage users, groups, and documents, including user/group memberships/permits.  

## Endpoints  

Endpoints are organized into CRUD operations by HTTP methods of `GET`, `POST`, `PUT`, and `DELETE`. Resources are named with plural nouns (e.g. `groups`).  

Relationships are represented through nested endpoints. To `GET` all memberships of a specific group `id`, the format is `GET /v1/groups/{id}/memberships`.  

Endpoints that retrieve many records will return simple objects. So, `GET /v1/users` will return an array of simple user objects with basic information for each user record, but no nested objects. Endpoints that retrieve a specific record may return complex objects. So, `GET /v1/users/{id}` will return a single complex user object including nested objects that may contain arrays of relational data such as the user's posted documents, group memberships, and document access permits.  

You can [review the Swagger/OpenApi style documentation on SwaggerHub](https://app.swaggerhub.com/apis/nothingworksright/trappykeepy/v0.1.0).  

Below is a simplified list of the available endpoints. 

### Groups  

```
POST /v1/groups
GET /v1/groups
POST /v1/groups/{id}/permits
GET /v1/groups/{id}/permits
DELETE /v1/groups/{id}/permits
GET /v1/groups/{id}
PUT /v1/groups/{id}
DELETE /v1/groups/{id}
GET /v1/groups/{id}/memberships
DELETE /v1/groups/{id}/memberships
DELETE /v1/groups/{gid}/permits/{pid}
```

### Keepers  

```
POST /v1/keepers
GET /v1/keepers
GET /v1/keepers/{id}
PUT /v1/keepers/{id}
DELETE /v1/keepers/{id}
GET /v1/keepers/{id}/permits
```

### Sessions  

```
POST /v1/sessions
```

### Users  

```
POST /v1/users
GET /v1/users
POST /v1/users/{id}/memberships
GET /v1/users/{id}/memberships
DELETE /v1/users/{id}/memberships
POST /v1/users/{id}/permits
GET /v1/users/{id}/permits
DELETE /v1/users/{id}/permits
GET /v1/users/{id}
PUT /v1/users/{id}
DELETE /v1/users/{id}
PUT /v1/users/{id}/password
DELETE /v1/users/{uid}/memberships/{mid}
DELETE /v1/users/{uid}/permits/{pid}
```

### Requests  

Review the [controller classes](https://github.com/jmg1138/TrappyKeepy/tree/main/TrappyKeepy.Api/Controllers) to find example requests in the comment blocks for every controller endpoint/method.  

Here is one example, sending a `POST` request to `/v1/groups`:

```bash
curl --location --request POST 'https://api.trappykeepy.com/v1/groups' \
--header 'Authorization: Bearer <token>' \
--header 'Content-Type: application/json' \
--data-raw '{
    "name": "foo",
    "description": "bar"
}'
```

### Responses  

Standard Http response status codes are used in responses, such as 200 OK, 400 Bad Reqeust, 401 Unauthorized, and 500 Internal Server Error. When responding with a status 400 there will be helpful information included so that the client may make corrections and try again.  

Responses are formatted as [JSend](https://github.com/omniti-labs/jsend). The JSON response will include a `status` key that will hold a value of success, fail, or error. Responses may also include key/values of `data` (the requested data), `message` (user-readable message), or `code` (an application code corresponding to the error, distinct from the Http status code).  

## Production  

After deploying the API into a production environment, there are some things that will need to be setup.  

### Env vars  

The following development environment variables with development values provide an example of the environment variables required in production. Environment variable values may be set in the `/etc/environment` file on a Linux host system:  

```bash
export TKDB_URL="jdbc:postgresql://localhost:15432/keepydb"
export TKDB_USER="dbuser"
export TKDB_OWNER="dbowner"
export TKDB_PASSWORD="dbpass"
export TKDB_MIGRATIONS="filesystem:./TrappyKeepy.Data/Migrations"
export TKDB_CONN_STRING="Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass"
export TK_CRYPTO_KEY="MqSm0P5dMgFSZhEBKpCv4dVKgDrsgrmT"
```

### First Admin  

To create the first administrator user in the system, connect to the database as `dbowner` and insert the user by running the tk.users_create function. Here is an example using development values:  

```sql
SELECT * FROM tk.users_create('foo', 'passwordfoo', 'foo@trappykeepy.com', 'admin');
```

## Development  

Written using [.NET6](https://dotnet.microsoft.com/download/dotnet/6.0) with the [.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/), [C# 10](https://devblogs.microsoft.com/dotnet/welcome-to-csharp-10/), and [PostgreSQL](https://www.postgresql.org/).  

### Using the `Makefile`  

For common actions like clean, restore, migrate, build, and run, a `Makefile` exists in the source code root directory so these could be easily organized as `Makefile` recipes. You can run any of the following commands from the root directory of the project's source code.  

&nbsp;&nbsp;&nbsp;&nbsp;Makefile&nbsp;Recipe&nbsp;&nbsp;&nbsp;&nbsp;|Explanation
--|--
`make all`| Run a combination of a clean, restore, migrate, build, and run.
`make flyway` | Download and extract the [Flyway](https://flywaydb.org) application. This is used for the database migrations, so this command must be run prior to running the `make migrate` command for the first time.
`make clean` | Clean the outputs (both the intermediate (obj) and final output (bin) folders) bu running the `dotnet clean` command for all projects.
`make restore` | Restore the dependencies and tools by running the `dotnet restore` command for all projects.
`make migrate` | Migrate the database using the SQL migration scripts located in the `TrappyKeepy.Data/Migrations` directory, using the Flyway application to apply and track the migrations. For more information such as the file naming patterns used by Flyway, see their [SQL-based migrations](https://flywaydb.org/documentation/concepts/migrations#sql-based-migrations) documentation page.
`make dbscaffold` | Reverse-engineer the database context and model classes from the PostgreSQL database into .NET, overwriting existing classes with the current database structure, by running the `dotnet ef dbcontext scaffold` command with proper arguments. This command reads the database connection string from the Microsoft Secrets Manager (see the Secrets Manager section below).
`make format` | Format the source code of  all projects to match the `.editorconfig` file settings by running the `dotnet format` command.
`make build` | Build the project by running `dotnet build` command for the TrappyKeepy.Api project.
`make test` | Execute the unit tests, by running the `dotnet test` command for the TrappyKeepy.Test project. Generates a test coverage report. When run during the GitHub Action CI workflow the test coverage report is uploaded to Codecov.
`make run` | Start the TrappyKeepy application by running the `dotnet run --project TrappyKeepy.Api` command.

### Database  

For development, a Vagrant box is setup to create a fresh PostgreSQL database instance that is ready to go. Read about [installing Vagrant](https://www.vagrantup.com/docs/installation) if needed. Once installed, run `vagrant up` from the root directory of this code repository where the `Vagrantfile` is located, which will create and configure the guest machine using the `Vagrant-setup/bootstrap.sh` shell script. Some helpful details for accessing this database are available in `Vagrant-setup/access.md`.  

### Tests  

Tests are written using xUnit.  

When running `make test` it will run the command `dotnet test --verbosity quiet /p:CollectCoverage=true /p:CoverletOutputFormat=opencover`. This includes the end-to-end tests which require a live development database to be present, and generates a code coverage report. Code coverage is uploaded to Codecov.io manually using their Uploader application.  

When tests run during the GitHub Action CI workflow the end-to-end tests are skipped because there is no PostgreSQL database present.  

### Secret Manager  

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

## Thank you  

Thanks for taking a look at this project.  

