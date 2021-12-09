/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V6
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Create keeper and filedata functions for CRUD operations.
 *              - tk.keepers_create
 *              - tk.filedatas_create
 *              - tk.keepers_read_all
 *              - tk.keepers_read_by_id
 *              - tk.filedatas_read_by_keeper_id
 *              - tk.keepers_update
 *              - tk.keepers_delete_by_id
 *              - tk.filedatas_delete_by_keeper_id
 ******************************************************************************/

 /**
 * Function:    tk.keepers_create
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Function to create a record in the keepers table.
 * Parameters:  filename TEXT - 
 *              content_type TEXT - 
 *              description TEXT - 
 *              category TEXT - 
 *              user_posted UUID -
 * Usage:       SELECT * FROM tk.keepers_create('foo.pdf', 'application/pdf', 'Important file.', 'Comedy', '00000000-0000-0000-0000-000000000000');
 * Returns:     The record that was created.
 */
CREATE OR REPLACE FUNCTION tk.keepers_create (
    filename TEXT,
    content_type TEXT,
    user_posted UUID,
    description TEXT DEFAULT NULL,
    category TEXT DEFAULT NULL
)
    RETURNS SETOF tk.keepers
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    INSERT
    INTO tk.keepers (filename, content_type, description, category, user_posted)
    VALUES ($1, $2, $4, $5, $3)
    RETURNING *;
END;
$$;
COMMENT ON FUNCTION tk.keepers_create IS 'Function to create a record in the keepers table.';

 /**
 * Function:    tk.filedatas_create
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Function to create a record in the filedatas table.
 * Parameters:  keeper_id UUID - 
 *              binary_data BYTEA - 
 * Usage:       SELECT * FROM tk.filedatas_create('foo.pdf', 'Important file.', 'Comedy', '00000000-0000-0000-0000-000000000000');
 * Returns:     The keeper_id of the record that was created.
 */
CREATE OR REPLACE FUNCTION tk.filedatas_create (
    keeper_id UUID,
    binary_data BYTEA
)
    RETURNS UUID
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    INSERT
    INTO tk.filedatas (keeper_id, binary_data)
    VALUES ($1, $2);

    RETURN keeper_id;
END;
$$;
COMMENT ON FUNCTION tk.filedatas_create IS 'Function to create a record in the filedatas table.';

/**
 * Function:    tk.keepers_read_all
 * Created:     2021-11-26
 * Author:      Joshua Gray
 * Description: Function to return all records from the keepers table.
 * Parameters:  None
 * Usage:       SELECT * FROM tk.keepers_read_all();
 * Returns:     All columns for all records from the tk.keepers table.
 */
CREATE OR REPLACE FUNCTION tk.keepers_read_all ()
    RETURNS SETOF tk.keepers
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.keepers;
END;
$$;
COMMENT ON FUNCTION tk.keepers_read_all IS 'Function to return all records from the keepers table.';

/**
 * Function:    tk.keepers_read_all_permitted
 * Created:     2021-12-05
 * Author:      Joshua Gray
 * Description: Function to return all records from the keepers table, that the requesting user is permitted to read.
 * Parameters:  id_user UUID - 
 * Usage:       SELECT * FROM tk.keepers_read_all_permitted('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for all records from the tk.keepers table.
 */
CREATE OR REPLACE FUNCTION tk.keepers_read_all_permitted (
    id_user UUID
)
    RETURNS SETOF tk.keepers
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT tk.keepers.*
    FROM tk.keepers
    INNER JOIN tk.permits ON tk.keepers.id = tk.permits.keeper_id
    WHERE tk.permits.user_id = $1
    OR tk.permits.group_id IN (
        SELECT group_id
        FROM tk.memberships
        WHERE user_id = $1
    );
END;
$$;
COMMENT ON FUNCTION tk.keepers_read_all_permitted IS 'Function to return all records from the keepers table, that the requesting user is permitted to read.';

/**
 * Function:    tk.keepers_read_by_id
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to return a record from the keepers table by id.
 * Parameters:  id_value UUID - The id of the keeper record.
 * Usage:       SELECT * FROM tk.keepers_read_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.keepers table.
 */
CREATE OR REPLACE FUNCTION tk.keepers_read_by_id (
    id_value UUID
)
    RETURNS SETOF tk.keepers
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.keepers
    WHERE tk.keepers.id = $1;
END;
$$;
COMMENT ON FUNCTION tk.keepers_read_by_id IS 'Function to return a record from the keepers table by id.';

/**
 * Function:    tk.keepers_read_by_id_permitted
 * Created:     2021-12-05
 * Author:      Joshua Gray
 * Description: Function to return a record from the keepers table by id, if the requesting user is permitted to read it.
 * Parameters:  id_value UUID - The id of the keeper record.
 * Usage:       SELECT * FROM tk.keepers_read_by_id_permitted('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.keepers table.
 */
CREATE OR REPLACE FUNCTION tk.keepers_read_by_id_permitted (
    id_keeper UUID,
    id_user UUID
)
    RETURNS SETOF tk.keepers
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT tk.keepers.*
    FROM tk.keepers
    INNER JOIN tk.permits ON tk.keepers.id = tk.permits.keeper_id
    WHERE tk.keepers.id = $1
    AND (
        tk.permits.user_id = $2
        OR tk.permits.group_id IN (
            SELECT group_id
            FROM tk.memberships
            WHERE user_id = $2
        )
    );
END;
$$;
COMMENT ON FUNCTION tk.keepers_read_by_id_permitted IS 'Function to return a record from the keepers table by id, if the requesting user is permitted to read it.';

/**
 * Function:    tk.filedatas_read_by_keeper_id
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to return a record from the filedatas table by keeper_id.
 * Parameters:  id_value UUID - The keeper_id of the filedata record.
 * Usage:       SELECT * FROM tk.filedatas_read_by_keeper_id('00000000-0000-0000-0000-000000000000');
 * Returns:     All columns for a record from the tk.filedatas table.
 */
CREATE OR REPLACE FUNCTION tk.filedatas_read_by_keeper_id (
    id_value UUID
)
    RETURNS SETOF tk.filedatas
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    SELECT *
    FROM tk.filedatas
    WHERE tk.filedatas.keeper_id = $1;
END;
$$;
COMMENT ON FUNCTION tk.filedatas_read_by_keeper_id IS 'Function to return a record from the filedatas table by id.';

/**
 * Function:    tk.keepers_update
 * Created:     2021-11-22
 * Author:      Joshua Gray
 * Description: Function to update a record in the keepers table. The id cannot be changed. The content_type cannot be changed because that is set based on the original binary data that is stored in the filedatas table. The date posted cannot be changed. The user_posted cannot be changed.
 * Parameters:  id UUID - Primary key id for the record to be updated.
 *              filename TEXT - 
 *              description TEXT - 
 *              category TEXT - 
 * Usage:       SELECT * FROM tk.keepers_update('00000000-0000-0000-0000-000000000000', 'foo.pdf', 'application/pdf', 'Simple PDF file.', 'Drama');
 * Returns:     True if the keeper was updated, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.keepers_update (
    id UUID,
    filename TEXT DEFAULT NULL,
    description TEXT DEFAULT NULL,
    category TEXT DEFAULT NULL
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    UPDATE tk.keepers
    SET filename = COALESCE($2, tk.keepers.filename),
        description = COALESCE($3, tk.keepers.description),
        category = COALESCE($4, tk.keepers.category)
    WHERE tk.keepers.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.keepers_update IS 'Function to update a record in the keepers table. ';

/**
 * Function:    tk.keepers_delete_by_id
 * Created:     2021-11-23
 * Author:      Joshua Gray
 * Description: Function to delete a record from the keepers table by id.
 * Parameters:  id UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.keepers_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the keeper was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.keepers_delete_by_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.keepers
    WHERE tk.keepers.id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.keepers_delete_by_id IS 'Function to delete a record from the keepers table by id.';

/**
 * Function:    tk.filedatas_delete_by_keeper_id
 * Created:     2021-11-23
 * Author:      Joshua Gray
 * Description: Function to delete a record from the filedatas table by id.
 * Parameters:  id UUID - Primary key id for the record to be deleted.
 * Usage:       SELECT * FROM tk.filedatas_delete_by_id('00000000-0000-0000-0000-000000000000');
 * Returns:     True if the filedata was deleted, and false if not.
 */
CREATE OR REPLACE FUNCTION tk.filedatas_delete_by_keeper_id (
    id_value UUID
)
    RETURNS BOOLEAN
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    DELETE
    FROM tk.filedatas
    WHERE tk.filedatas.keeper_id = $1;
    RETURN FOUND;
END;
$$;
COMMENT ON FUNCTION tk.filedatas_delete_by_keeper_id IS 'Function to delete a record from the filedatas table by id.';