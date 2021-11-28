# PostgreSQL Development Database  

Your PostgreSQL database has been setup and can be accessed on your local machine on the forwarded port (default: 15432).  

Key|Value
--|--
Host|localhost
Port|15432
Database|keepydb
Username|dbuser
Password|dbpass

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

Env variable for application development:

```bash
DATABASE_URL=postgresql://dbuser:dbpass@localhost:15432/keepydb
```

Local command to access the database via psql:

```bash
PGUSER=dbuser PGPASSWORD=dbpass psql -h localhost -p 15432 keepydb
```

Connection string:

```
Host=localhost;Database=keepydb;Port=15432;Username=dbuser;Password=dbpass
```

# User Defined Functions  

In development, the grantee adding the functions is currently `dbuser`.  

To list all functions created by `dbuser`:  

```sql
SELECT routine_name
FROM information_schema.routine_privileges
WHERE grantee = 'dbuser';
```

# Migrations  

## V1  

To rollback all migrations:  

```sql
DROP SCHEMA tk CASCADE;
DROP TABLE flyway_schema_history;
```
