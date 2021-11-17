/**
 * tk schema.
 * The namespace for the TrappyKeepy stuff in the database.
 */
CREATE SCHEMA IF NOT EXISTS tk;

/**
 * Users table.
 * id: Going with UUID. Very low probability that a UUID will be duplicated.
 *   I know, sort order issues, but I won't sort on it.
 * name: Limiting to 50 characters for display purposes mostly.
 * password: Going with salted/hashed passwords using pgcrypto.
 */
CREATE TABLE IF NOT EXISTS tk.users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR (50) UNIQUE NOT NULL,
    password TEXT NOT NULL,
    email TEXT UNIQUE NOT NULL,
    date_created TIMESTAMPTZ NOT NULL,
    date_activated TIMESTAMPTZ,
    date_last_login TIMESTAMPTZ
);
COMMENT ON TABLE tk.users IS 'Individual user records including login credentials.';
COMMENT ON COLUMN tk.users.id IS 'UUID/GUID primary key of the user record.';
COMMENT ON COLUMN tk.users.name IS 'Unique user display name.';
COMMENT ON COLUMN tk.users.password IS 'Encrypted user password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.';
COMMENT ON COLUMN tk.users.email IS 'Unique email address for the user.';
COMMENT ON COLUMN tk.users.date_created IS 'The datetime when the user record was created in the database.';
COMMENT ON COLUMN tk.users.date_activated IS 'The datetime when the user record was activated for login.';
COMMENT ON COLUMN tk.users.date_last_login IS 'The datetime when the user last logged into the system successfully.';
