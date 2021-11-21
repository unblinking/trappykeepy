/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V1
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Initialize the TrappyKeepy database, creating the schema, the 
 *              type for the users table, the users table, a function to query
 *              the users table, and a function to return table types.
 ******************************************************************************/

/**
 * Schema:      tk
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: The namespace for TrappyKeepy types, tables, and functions.
 */
CREATE SCHEMA IF NOT EXISTS tk;
COMMENT ON SCHEMA tk IS 'The namespace for TrappyKeepy types, tables, and functions.';

/**
 * Type:        tk.user_type
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Type for an individual user record including login credentials.
 * Attributes:  id UUID - Very low probability that a UUID will be duplicated.
 *              name VARCHAR(50) - 50 char limit for display purposes.
 *              password TEXT - Salted/hashed passwords using pgcrypto.
 *              email TEXT - 
 *              date_created - 
 *              date_activated - 
 *              date_last_login - 
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
COMMENT ON TYPE tk.user_type IS 'Type for a user record.';

/**
 * Table:       tk.users
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Table to store user records.
 * Columns:     id - Primary key with default using the gen_random_uuid() function.
 *              name - Unique, and not null.
 *              password - Not null.
 *              email - Unique, and not null.
 *              date_created - Not null.
 *              date_activated - 
 *              date_last_login - 
 */
CREATE TABLE IF NOT EXISTS tk.users OF tk.user_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    name WITH OPTIONS UNIQUE NOT NULL,
    password WITH OPTIONS NOT NULL,
    email WITH OPTIONS UNIQUE NOT NULL,
    date_created WITH OPTIONS NOT NULL
);
COMMENT ON TABLE tk.users IS 'Individual user records including login credentials.';
COMMENT ON COLUMN tk.users.id IS 'UUID primary key of the user record.';
COMMENT ON COLUMN tk.users.name IS 'Unique user display name.';
COMMENT ON COLUMN tk.users.password IS 'Encrypted user password using the pgcrypto crypt function, and gen_salt with the blowfish algorithm and iteration count of 8.';
COMMENT ON COLUMN tk.users.email IS 'Unique email address for the user.';
COMMENT ON COLUMN tk.users.date_created IS 'The datetime when the user record was created in the database.';
COMMENT ON COLUMN tk.users.date_activated IS 'The datetime when the user record was activated for login.';
COMMENT ON COLUMN tk.users.date_last_login IS 'The datetime when the user last logged into the system successfully.';


/**
 * Function:    tk.users_read_all
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Function to return all records from the users table.
 * Parameters:  None
 * Usage:       SELECT * FROM tk.users_read_all();
 * Returns:     All columns for all records from the tk.users table.
 */
CREATE OR REPLACE FUNCTION tk.users_read_all ()
RETURNS SETOF tk.users
AS 'SELECT * FROM tk.users;'
LANGUAGE SQL;
COMMENT ON FUNCTION tk.users_read_all IS 'Function to return all records from the users table.';

/**
 * Function:    tk.get_table_types
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Function to return table column types.
 * Parameters:  table_name - The name of the table without the schema.
 * Usage:       SELECT * FROM tk.get_table_types('users');
 * Returns:     column_name, data_type
 */
CREATE OR REPLACE FUNCTION tk.get_table_types (table_name TEXT)
RETURNS TABLE (column_name VARCHAR ( 255 ), data_type VARCHAR ( 255 ))
AS $$
DECLARE
    BEGIN
        CREATE TEMP TABLE IF NOT EXISTS users_information_schema_columns(
            column_name VARCHAR ( 255 ),
            data_type VARCHAR ( 255 )
        ) ON COMMIT DROP;
        INSERT INTO users_information_schema_columns ( column_name, data_type )
        SELECT isc.column_name, isc.data_type
        FROM information_schema.columns as isc
        WHERE isc.table_name = $1;
        RETURN QUERY SELECT * FROM users_information_schema_columns;
    END;
$$ LANGUAGE PLPGSQL;
COMMENT ON FUNCTION tk.get_table_types IS 'Function to return table column types.';
