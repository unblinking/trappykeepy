/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V9
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Create group and membership functions for CRUD operations.
 *              - tk.groups_create
 *              - tk.memberships_create
 *              - tk.groups_read_all
 *              - tk.memberships_read_all
 *              - tk.groups_read_by_id
 *              - tk.memberships_read_by_group_id
 *              - tk.memberships_read_by_user_id
 *              - tk.groups_update
 *              - tk.groups_delete_by_id
 *              - tk.memberships_delete_by_id
 *              - tk.memberships_delete_by_group_id
 *              - tk.memberships_delete_by_user_id
 ******************************************************************************/

 /**
 * Function:    tk.groups_create
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to create a record in the groups table.
 * Parameters:  name TEXT - 
 *              description TEXT - 
 * Usage:       SELECT * FROM tk.groups_create('foo', 'Group of foo');
 * Returns:     
 */
CREATE OR REPLACE FUNCTION tk.groups_create (
    name TEXT,
    description TEXT DEFAULT NULL
)
    RETURNS SETOF tk.groups
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    INSERT
    INTO tk.groups (name, description)
    VALUES ($1, $2)
    RETURNING *;
END;
$$;
COMMENT ON FUNCTION tk.groups_create IS 'Function to create a record in the groups table.';

 /**
 * Function:    tk.memberships_create
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to create a record in the memberships table.
 * Parameters:  group_id UUID - 
 *              user_id UUID - 
 * Usage:       SELECT * FROM tk.memberships_create('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000');
 * Returns:     
 */
CREATE OR REPLACE FUNCTION tk.memberships_create (
    group_id UUID,
    user_id UUID
)
    RETURNS SETOF tk.memberships
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    INSERT
    INTO tk.memberships (group_id, user_id)
    VALUES ($1, $2)
    RETURNING *;
END;
$$;
COMMENT ON FUNCTION tk.memberships_create IS 'Function to create a record in the memberships table.';

/**
 * Function:    tk.groups_read_all
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return all records from the groups table.
 * Parameters:  None
 * Usage:       SELECT * FROM tk.groups_read_all();
 * Returns:     All columns for all records from the tk.groups table.
 */
CREATE OR REPLACE FUNCTION tk.groups_read_all ()
    RETURNS SETOF tk.groups
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.groups;
END;
$$;
COMMENT ON FUNCTION tk.groups_read_all IS 'Function to return all records from the groups table.';

/**
 * Function:    tk.memberships_read_all
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return all records from the memberships table.
 * Parameters:  None
 * Usage:       SELECT * FROM tk.memberships_read_all();
 * Returns:     All columns for all records from the tk.memberships table.
 */
CREATE OR REPLACE FUNCTION tk.memberships_read_all ()
    RETURNS SETOF tk.memberships
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.memberships;
END;
$$;
COMMENT ON FUNCTION tk.memberships_read_all IS 'Function to return all records from the memberships table.';


/**
 * Function:    tk.groups_read_by_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return a record from the groups table by id.
 * Parameters:  id_value UUID - The id of the group record.
 * Usage:       SELECT * FROM tk.groups_read_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.groups table.
 */
CREATE OR REPLACE FUNCTION tk.groups_read_by_id (
    id_value UUID
)
    RETURNS SETOF tk.groups
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.groups
    WHERE tk.groups.id = $1;
END;
$$;
COMMENT ON FUNCTION tk.groups_read_by_id IS 'Function to return a record from the groups table by id.';

/**
 * Function:    tk.memberships_read_by_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return a record from the memberships table by id.
 * Parameters:  id_value UUID - The id of the membership record.
 * Usage:       SELECT * FROM tk.memberships_read_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.memberships table.
 */
CREATE OR REPLACE FUNCTION tk.memberships_read_by_id (
    id_value UUID
)
    RETURNS SETOF tk.memberships
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.memberships
    WHERE tk.memberships.id = $1;
END;
$$;
COMMENT ON FUNCTION tk.memberships_read_by_id IS 'Function to return a record from the memberships table by id.';

/**
 * Function:    tk.memberships_read_by_group_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return records from the memberships table by group_id.
 * Parameters:  id_value UUID - The group_id of the membership records.
 * Usage:       SELECT * FROM tk.memberships_read_by_group_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.filedatas table that match a group_id.
 */
CREATE OR REPLACE FUNCTION tk.memberships_read_by_group_id (
    id_value UUID
)
    RETURNS SETOF tk.memberships
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.memberships
    WHERE tk.memberships.group_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.memberships_read_by_group_id IS 'Function to return records from the memberships table by group_id.';

/**
 * Function:    tk.memberships_read_by_user_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return records from the memberships table by user_id.
 * Parameters:  id_value UUID - The user_id of the membership records.
 * Usage:       SELECT * FROM tk.memberships_read_by_user_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.filedatas table that match a user_id.
 */
CREATE OR REPLACE FUNCTION tk.memberships_read_by_user_id (
    id_value UUID
)
    RETURNS SETOF tk.memberships
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.memberships
    WHERE tk.memberships.user_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.memberships_read_by_user_id IS 'Function to return records from the memberships table by user_id.';

/**
 * Function:    tk.groups_update
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to update a record in the groups table. The id cannot
                be changed. The date created cannot be changed.
 * Parameters:  id UUID - Primary key id for the record to be updated.
 *              name TEXT - 
 *              description TEXT - 
 * Usage:       SELECT * FROM tk.groups_update('00000000-0000-0000-0000-000000000000', 'foo', 'Group of foo');
 * Returns:     True if the group was updated, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.groups_update (
    id UUID,
    name TEXT,
    description TEXT DEFAULT NULL
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    UPDATE tk.groups
    SET name = COALESCE($2, tk.groups.filename),
        description = COALESCE($3, tk.groups.description)
    WHERE tk.groups.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.groups_update IS 'Function to update a record in the groups table. ';

/**
 * Function:    tk.groups_delete_by_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to delete a record from the groups table by id.
 * Parameters:  id UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.groups_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the group was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.groups_delete_by_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.groups
    WHERE tk.groups.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.groups_delete_by_id IS 'Function to delete a record from the groups table by id.';

/**
 * Function:    tk.memberships_delete_by_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to delete a record from the memberships table by id.
 * Parameters:  id_value UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.memberships_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the membership was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.memberships_delete_by_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.memberships
    WHERE tk.memberships.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.memberships_delete_by_id IS 'Function to delete a record from the memberships table by id.';

/**
 * Function:    tk.memberships_delete_by_group_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to delete all records from the memberships table by group_id.
 * Parameters:  id UUID - group_id for the records to be deleted.
 * Usage:       SELECT * FROM tk.memberships_delete_by_group_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the memberships were deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.memberships_delete_by_group_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.memberships
    WHERE tk.memberships.group_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.memberships_delete_by_group_id IS 'Function to delete all records from the memberships table by group_id.';

/**
 * Function:    tk.memberships_delete_by_user_id
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to delete all records from the memberships table by user_id.
 * Parameters:  id UUID - user_id for the records to be deleted.
 * Usage:       SELECT * FROM tk.memberships_delete_by_user_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the memberships were deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.memberships_delete_by_user_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.memberships
    WHERE tk.memberships.user_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.memberships_delete_by_user_id IS 'Function to delete all records from the memberships table by user_id.';