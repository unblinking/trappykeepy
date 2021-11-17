.PHONY: all
all: clean restore migrate build run

.PHONY: flyway
flyway:
	@# This will download and extract the Flyway application.
	# ----------
	# Downloading and extracting Flyway - https://flywaydb.org/
	wget -qO- https://repo1.maven.org/maven2/org/flywaydb/flyway-commandline/8.0.4/flyway-commandline-8.0.4-linux-x64.tar.gz | tar xvz

.PHONY: clean
clean:
	dotnet clean TrappyKeepy.Api

.PHONY: restore
restore:
	dotnet restore TrappyKeepy.Api

.PHONY: migrate
migrate:
ifeq ($(origin ASPNETCORE_ENVIRONMENT), 'Production')
	# ----------
	# Environment is production
	# Environment variables need to be setup properly in production for this to work
	# Migrating the database
	./flyway-8.0.4/flyway -configFiles=flyway.conf migrate
else
	# ----------
	# Environment is development 
	# Environment variables are hard coded into the development migration command.
	# Migrating the database
	FLYWAY_URL="jdbc:postgresql://localhost:15432/keepydb" DB_USER="dbuser" DB_PASSWORD="dbpass" FLYWAY_LOCATIONS="filesystem:./Migrations" ./flyway-8.0.4/flyway -configFiles=flyway.conf migrate
endif

.PHONY: build
build:
	@# This will create or overwrite the database context class and model classes
	@# based on the current database structure.
	# ----------
	# Scaffolding the database context and model classes
	dotnet ef dbcontext scaffold Name=ConnectionStrings:keepydb --project TrappyKeepy.Api --data-annotations --schema tk --context-dir Data --output-dir Models --force Npgsql.EntityFrameworkCore.PostgreSQL

	@# This will format the code to match the .editorconfig file settings.
	# ----------
	# Applying code formatting
	dotnet format TrappyKeepy.Api

	@# This will build the TrappyKeepy.Api project.
	# ----------
	# Building the TrappyKeepy.Api project
	dotnet build TrappyKeepy.Api

.PHONY: test
test:
	dotnet test TrappyKeepy.Api

.PHONY: run
run:
	dotnet run --project TrappyKeepy.Api
