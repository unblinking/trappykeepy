/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V2
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Create the user type, and the users table.
 ******************************************************************************/

/**
 * Type:        tk.user_type
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Type for an individual user record including login credentials.
 * Attributes:  id UUID - Very low probability that a UUID will be duplicated.
 *              name VARCHAR(50) - 50 char limit for display purposes.
 *              password TEXT - Salted/hashed passwords using pgcrypto.
 *              email TEXT - 
 *              role TEXT -
 *              date_created TIMESTAMPTZ - 
 *              date_activated TIMESTAMPTZ - 
 *              date_last_login TIMESTAMPTZ - 
 */
CREATE TYPE tk.user_type AS (
    id UUID,
    name VARCHAR ( 50 ),
    password TEXT,
    email TEXT,
    role TEXT,
    date_created TIMESTAMPTZ,
    date_activated TIMESTAMPTZ,
    date_last_login TIMESTAMPTZ
);
COMMENT ON TYPE tk.user_type IS 'Type for an individual user record including login credentials.';

/**
 * Table:       tk.users
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Table to store user records.
 * Columns:     id - Primary key with default using the gen_random_uuid() function.
 *              name - Unique, and not null.
 *              password - Not null.
 *              email - Unique, and not null.
 *              role - Not null.
 *              date_created - Not null.
 *              date_activated - 
 *              date_last_login - 
 */
CREATE TABLE IF NOT EXISTS tk.users OF tk.user_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    name WITH OPTIONS UNIQUE NOT NULL,
    password WITH OPTIONS NOT NULL,
    email WITH OPTIONS UNIQUE NOT NULL,
    role WITH OPTIONS NOT NULL CHECK (role IN ('basic', 'manager', 'admin')) DEFAULT 'basic',
    date_created WITH OPTIONS NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX user_role_index ON tk.users (role);
COMMENT ON TABLE tk.users IS 'Table to store user records.';
COMMENT ON COLUMN tk.users.id IS 'UUID primary key.';
COMMENT ON COLUMN tk.users.name IS 'Unique display name.';
COMMENT ON COLUMN tk.users.password IS 'Salted/Hashed password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.';
COMMENT ON COLUMN tk.users.email IS 'Unique email address.';
COMMENT ON COLUMN tk.users.role IS 'Security level role.';
COMMENT ON COLUMN tk.users.date_created IS 'Datetime the user was created in the database.';
COMMENT ON COLUMN tk.users.date_activated IS 'Datetime the user was activated for login.';
COMMENT ON COLUMN tk.users.date_last_login IS 'Datetime the user last logged into the system successfully.';
