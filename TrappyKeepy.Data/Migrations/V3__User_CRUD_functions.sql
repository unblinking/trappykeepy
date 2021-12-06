/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V3
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Create user functions for CRUD operations.
 *              - tk.users_create
 *              - tk.users_read_all
 *              - tk.users_read_by_id
 *              - tk.users_update
 *              - tk.users_delete_by_id
 ******************************************************************************/

/**
 * Function:    tk.users_create
 * Created:     2021-11-21
 * Author:      Joshua Gray
 * Description: Function to create a record in the users table.
 * Parameters:  name VARCHAR(50) - Unique user display name.
 *              password TEXT - Plain text user password that will be salted/hashed.
 *              email TEXT - 
 *              role TEXT - 
 * Usage:       SELECT * FROM tk.users_create('foo', 'passwordfoo', 'foo@example.com', 'basic');
 * Returns:     The record that was created.
 */
CREATE OR REPLACE FUNCTION tk.users_create (
    name VARCHAR( 50 ),
    password TEXT,
    email TEXT,
    role TEXT
)
    RETURNS SETOF tk.users
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    saltedhash TEXT;
BEGIN
    SELECT crypt($2, gen_salt('bf', 8))
    INTO saltedhash;

    RETURN QUERY
    INSERT
    INTO tk.users (name, password, email, role)
    VALUES ($1, saltedhash, $3, $4)
    RETURNING *;
END;
$$;
COMMENT ON FUNCTION tk.users_create IS 'Function to create a record in the users table.';

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
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.users;
END;
$$;
COMMENT ON FUNCTION tk.users_read_all IS 'Function to return all records from the users table.';

/**
 * Function:    tk.users_read_by_id
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to return a record from the users table by id.
 * Parameters:  id_value UUID - The id of the user record.
 * Usage:       SELECT * FROM tk.users_read_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.users table.
 */
CREATE OR REPLACE FUNCTION tk.users_read_by_id (
    id_value UUID
)
    RETURNS SETOF tk.users
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.users
    WHERE tk.users.id = $1;
END;
$$;
COMMENT ON FUNCTION tk.users_read_by_id IS 'Function to return a record from the users table by id.';

/**
 * Function:    tk.users_update
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to update a record in the users table. The id cannot be changed. The password can only be changed via tk.users_update_password(). The date_created cannot be changed.
 * Parameters:  id UUID - Primary key id for the record to be updated.
 *              name VARCHAR(50)
 *              email TEXT
 *              date_activated TIMESTAMPTZ
 *              date_last_login TIMESTAMPTZ
 * Usage:       SELECT * FROM tk.users_update('00000000-0000-0000-0000-000000000000', 'foo', 'foo@example.com', '0', '2021-10-10T13:10:10', '2021-10-10T13:10:10');
 * Returns:     True if the user was updated, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.users_update (
    id UUID,
    name VARCHAR( 50 ) DEFAULT NULL,
    email TEXT DEFAULT NULL,
    role TEXT DEFAULT NULL,
    date_activated TIMESTAMPTZ DEFAULT NULL,
    date_last_login TIMESTAMPTZ DEFAULT NULL
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    UPDATE tk.users
    SET name = COALESCE($2, tk.users.name),
        email = COALESCE($3, tk.users.email),
        role = COALESCE($4, tk.users.role),
        date_activated = COALESCE($5, tk.users.date_activated),
        date_last_login = COALESCE($6, tk.users.date_last_login)
    WHERE tk.users.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.users_update IS 'Function to update a record in the users table. ';

/**
 * Function:    tk.users_delete_by_id
 * Created:     2021-11-23
 * Author:      Joshua Gray
 * Description: Function to delete a record from the users table by id.
 * Parameters:  id UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.users_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the user was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.users_delete_by_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.users
    WHERE tk.users.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.users_delete_by_id IS 'Function to delete a record from the users table by id.';
