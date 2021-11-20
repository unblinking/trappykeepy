/**
 * tk schema.
 * The namespace for TrappyKeepy.
 */
CREATE SCHEMA IF NOT EXISTS tk;

/**
 * user type.
 * id: Very low probability that a UUID will be duplicated.
 * name: Limiting to 50 characters for display purposes mostly.
 * password: Salted/hashed passwords using pgcrypto.
 */
CREATE TYPE tk.user_type AS (
    id UUID,
    name VARCHAR ( 50 ),
    password TEXT,
    email TEXT,
    date_created TIMESTAMPTZ,
    date_activated TIMESTAMPTZ,
    date_last_login TIMESTAMPTZ
);

/**
 * users table.
 * id: Primary key with default using the gen_random_uuid() function.
 * name: Unique, and not null.
 * password: Not null.
 * email: Unique, and not null.
 * date_created: Not null.
 */
CREATE TABLE IF NOT EXISTS tk.users OF tk.user_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    name WITH OPTIONS UNIQUE NOT NULL,
    password WITH OPTIONS NOT NULL,
    email WITH OPTIONS UNIQUE NOT NULL,
    date_created WITH OPTIONS NOT NULL
);
COMMENT ON TABLE tk.users IS 'Individual user records including login credentials.';
COMMENT ON COLUMN tk.users.id IS 'UUID/GUID primary key of the user record.';
COMMENT ON COLUMN tk.users.name IS 'Unique user display name.';
COMMENT ON COLUMN tk.users.password IS 'Encrypted user password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.';
COMMENT ON COLUMN tk.users.email IS 'Unique email address for the user.';
COMMENT ON COLUMN tk.users.date_created IS 'The datetime when the user record was created in the database.';
COMMENT ON COLUMN tk.users.date_activated IS 'The datetime when the user record was activated for login.';
COMMENT ON COLUMN tk.users.date_last_login IS 'The datetime when the user last logged into the system successfully.';

/**
 * users function to return all records from the users table.
 */
CREATE OR REPLACE FUNCTION tk.users_ReadAll ()
RETURNS SETOF tk.users
AS 'SELECT * FROM tk.users;'
LANGUAGE SQL;