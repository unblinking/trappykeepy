/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V12
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Create permit functions for CRUD operations.
 *              - tk.permits_create
 *              - tk.permits_read_all
 *              - tk.permits_read_by_keeper_id
 *              - tk.permits_read_by_user_id
 *              - tk.permits_read_by_group_id
 *              - tk.permits_delete_by_id
 *              - tk.permits_delete_by_keeper_id
 *              - tk.permits_delete_by_user_id
 *              - tk.permits_delete_by_group_id
 ******************************************************************************/

/**
 * Function:    tk.permits_create
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to create a record in the permit table.
 * Parameters:  keeper_id UUID - 
 *              user_id UUID - 
 *              group_id UUID - 
 * Usage:       SELECT * FROM tk.permits_create('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', null);
 *              SELECT * FROM tk.permits_create('00000000-0000-0000-0000-000000000000', null, '00000000-0000-0000-0000-000000000000');
 * Returns:     The record that was created.
 */
CREATE OR REPLACE FUNCTION tk.permits_create (
    keeper_id UUID,
    user_id UUID,
    group_id UUID
)
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    INSERT
    INTO tk.permits (keeper_id, user_id, group_id)
    VALUES ($1, $2, $3)
    RETURNING *;
END;
$$;
COMMENT ON FUNCTION tk.permits_create IS 'Function to create a record in the permits table.';

/**
 * Function:    tk.permits_read_all
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to return all records from the permits table.
 * Parameters:  None
 * Usage:       SELECT * FROM tk.permits_read_all();
 * Returns:     All columns for all records from the tk.permits table.
 */
CREATE OR REPLACE FUNCTION tk.permits_read_all ()
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.permits;
END;
$$;
COMMENT ON FUNCTION tk.permits_read_all IS 'Function to return all records from the permits table.';

/**
 * Function:    tk.permits_read_by_id
 * Created:     2021-12-02
 * Author:      Joshua Gray
 * Description: Function to return a record from the permits table by id.
 * Parameters:  id_value UUID - The id of the permit record.
 * Usage:       SELECT * FROM tk.permits_read_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.permits table.
 */
CREATE OR REPLACE FUNCTION tk.permits_read_by_id (
    id_value UUID
)
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.permits
    WHERE tk.permits.id = $1;
END;
$$;
COMMENT ON FUNCTION tk.permits_read_by_id IS 'Function to return a record from the permits table by id.';

/**
 * Function:    tk.permits_read_by_keeper_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to return all record from the permits table by keeper_id.
 * Parameters:  id_value UUID - The id of the keeper record for the permits.
 * Usage:       SELECT * FROM tk.permits_read_by_keeper_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.permits table for the specified keeper_id.
 */
CREATE OR REPLACE FUNCTION tk.permits_read_by_keeper_id (
    id_value UUID
)
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.permits
    WHERE tk.permits.keeper_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.permits_read_by_keeper_id IS 'Function to return all record from the permits table by keeper_id.';

/**
 * Function:    tk.permits_read_by_user_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to return all record from the permits table by user_id.
 * Parameters:  id_value UUID - The id of the user record for the permits.
 * Usage:       SELECT * FROM tk.permits_read_by_user_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.permits table for the specified user_id.
 */
CREATE OR REPLACE FUNCTION tk.permits_read_by_user_id (
    id_value UUID
)
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.permits
    WHERE tk.permits.user_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.permits_read_by_user_id IS 'Function to return all record from the permits table by user_id.';

/**
 * Function:    tk.permits_read_by_group_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to return all record from the permits table by group_id.
 * Parameters:  id_value UUID - The id of the group record for the permits.
 * Usage:       SELECT * FROM tk.permits_read_by_group_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.permits table for the specified group_id.
 */
CREATE OR REPLACE FUNCTION tk.permits_read_by_group_id (
    id_value UUID
)
    RETURNS SETOF tk.permits
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.permits
    WHERE tk.permits.group_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.permits_read_by_group_id IS 'Function to return all record from the permits table by group_id.';

/**
 * Function:    tk.permits_delete_by_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to delete a record from the permits table by id.
 * Parameters:  id UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.permits_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the user was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.permits_delete_by_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.permits
    WHERE tk.permits.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.permits_delete_by_id IS 'Function to delete a record from the permits table by id.';

/**
 * Function:    tk.permits_delete_by_keeper_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to delete a record from the permits table by keeper_id.
 * Parameters:  id UUID - keeper_id for the record to be deleted.
 * Usage:       SELECT * FROM tk.permits_delete_by_keeper_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the user was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.permits_delete_by_keeper_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.permits
    WHERE tk.permits.keeper_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.permits_delete_by_keeper_id IS 'Function to delete a record from the permits table by keeper_id.';

/**
 * Function:    tk.permits_delete_by_user_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to delete a record from the permits table by user_id.
 * Parameters:  id UUID - user_id for the record to be deleted.
 * Usage:       SELECT * FROM tk.permits_delete_by_user_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the user was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.permits_delete_by_user_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.permits
    WHERE tk.permits.user_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.permits_delete_by_user_id IS 'Function to delete a record from the permits table by user_id.';

/**
 * Function:    tk.permits_delete_by_group_id
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Function to delete a record from the permits table by group_id.
 * Parameters:  id UUID - group_id for the record to be deleted.
 * Usage:       SELECT * FROM tk.permits_delete_by_group_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the user was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.permits_delete_by_group_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.permits
    WHERE tk.permits.group_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.permits_delete_by_group_id IS 'Function to delete a record from the permits table by group_id.';