/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V4
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Create additional user functions.
 *              - tk.users_authenticate
 *              - tk.users_update_password
 *              - tk.users_count_by_column_value_text
 ******************************************************************************/

/**
 * Function:    tk.users_authenticate
 * Created:     2021-11-23
 * Author:      Joshua Gray
 * Description: Function to authenticate a user by email and password.
 * Parameters:  email TEXT - 
 *              password TEXT -
 * Usage:       SELECT * FROM tk.users_authenticate('foo@trappykeepy.com', 'passwordfoo');
 * Returns:     The user record if found.
 */
CREATE OR REPLACE FUNCTION tk.users_authenticate (
    email TEXT,
    password TEXT
)
    RETURNS SETOF tk.users
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.users
    WHERE tk.users.email = $1
        AND tk.users.password = crypt($2, tk.users.password);
END;
$$;
COMMENT ON FUNCTION tk.users_authenticate IS 'Function to authenticate a user by email and password.';

/**
 * Function:    tk.users_update_password
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to update a record in the users table with a new password.
 * Parameters:  id UUID - Primary key id for the record to be updated.
 *              password TEXT - The new plain text password to be salted/hashed and saved.
 * Usage:       SELECT * FROM tk.users_update_password('00000000-0000-0000-0000-000000000000', 'passwordfoo');
 * Returns:     True if the user password was updated, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.users_update_password (
    id UUID,
    password TEXT
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    saltedhash TEXT;
BEGIN
    SELECT crypt($2, gen_salt('bf', 8)) INTO saltedhash;

    UPDATE tk.users
    SET password = saltedhash
    WHERE tk.users.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.users_update_password IS 'Function to update a record in the users table with a new password.';

/**
 * Function:    tk.users_count_by_column_value_text
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to return the count of user records that match a given column/value.
 * Parameters:  column_name TEXT - The name of the column to match on.
 *              column_value TEXT - The value of the column to match on.
 * Usage:       SELECT * FROM tk.users_count_by_column_value_text('name', 'foo');
 * Returns:     An integer count of the number of matching records found.
 */
CREATE OR REPLACE FUNCTION tk.users_count_by_column_value_text (
    column_name TEXT,
    column_value TEXT
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
    query text := 'SELECT COUNT(*) FROM tk.users';
BEGIN
    IF column_name IS NOT NULL THEN
        query := query || ' WHERE ' || quote_ident(column_name) || ' = $1';
    END IF;
    EXECUTE query
    USING column_value
    INTO row_count;
    RETURN row_count;
END;
$$;
COMMENT ON FUNCTION tk.users_count_by_column_value_text IS 'Function to count records from the users table by the specified column/value.';
