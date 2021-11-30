# PostgreSQL Development Database  

There is a development database available using Vagrant by running the `vagrant up` command. Once running, the details of the database are listed below:  

## Basics  

Key|Value
--|--
Host|localhost
Port|15432
Database|keepydb
Username|dbuser
Password|dbpass

## Access via the VM  

Admin access to postgres user via VM:

```bash
vagrant ssh
sudo su - postgres
```

psql access to app database user via VM:

```bash
vagrant ssh
sudo su - postgres
PGUSER=dbuser PGPASSWORD=dbpass psql -h localhost keepydb
```

## Access via local commands  

Local command to access the database as dbuser via psql:

```bash
PGUSER=dbuser PGPASSWORD=dbpass psql -h localhost -p 15432 keepydb
```

Local command to access the database as dbowner via psql:

```bash
PGUSER=dbowner PGPASSWORD=dbpass psql -h localhost -p 15432 keepydb
```

## Connection string  

Trappy Keepy application connection string:  

```bash
Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass
```

# User Defined Functions  

In development, the grantee adding the functions is currently `dbowner`.  

To list all functions created by `dbowner`:  

```sql
SELECT routine_name
FROM information_schema.routine_privileges
WHERE grantee = 'dbowner';
```

# Migrations  

To rollback all migrations, connect to the development database using psql as dbowner and run the following:  

```sql
DROP SCHEMA tk CASCADE;
DROP TABLE flyway_schema_history;
```
