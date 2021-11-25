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

To see the source of a function in the database:  

```psql
\sf tk.get_table_types
```

# Seed data  

Seed a couple of user records:  

```sql
INSERT INTO tk.users (name, password, email, date_created) VALUES ('foo', 'passwordfoo', 'foo@example.com', '2021-10-10 10:10:10-10');
INSERT INTO tk.users (name, password, email, date_created) VALUES ('bar', 'passwordbar', 'bar@example.com', '2021-10-10 10:10:10-10');
```

# Migrations  

## V1  

To rollback all migrations:  

```sql
DROP FUNCTION tk.filebyteas_create(uuid, bytea);
DROP FUNCTION tk.keepers_create(text, text, text, uuid);
DROP TABLE tk.filebyteas;
DROP TYPE tk.filebytea_type;
DROP TABLE tk.keepers;
DROP TYPE tk.keeper_type;
DROP FUNCTION tk.users_authenticate(text, text);
DROP FUNCTION tk.users_count_by_column_value_text(text, text);
DROP FUNCTION tk.users_delete_by_id(uuid);
DROP FUNCTION tk.users_update_password(uuid, text);
DROP FUNCTION tk.users_update(uuid, varchar(50), text, timestamptz, timestamptz);
DROP FUNCTION tk.users_read_by_id(uuid);
DROP FUNCTION tk.users_read_all();
DROP FUNCTION tk.users_create(varchar(50), text, text, timestamptz);
DROP TABLE tk.users;
DROP TYPE tk.user_type;
DROP FUNCTION tk.get_table_types(text);
DROP SCHEMA tk;
DROP TABLE flyway_schema_history;

```
