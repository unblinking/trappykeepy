/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V6
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Create keeper functions for CRUD operations.
 *              - tk.keepers_create
 *              - tk.file_create
 *              - tk.keepers_read_all
 *              - tk.keepers_read_by_id
 *              - tk.keepers_update
 *              - tk.keepers_delete_by_id
 ******************************************************************************/

 /**
 * Function:    tk.keepers_create
 * Created:     2021-11-21
 * Author:      Joshua Gray
 * Description: Function to create a record in the keepers table.
 * Parameters:  filename TEXT - 
 *              description TEXT - 
 *              category TEXT - 
 *              user_posted UUID -
 * Usage:       SELECT * FROM tk.keepers_create('foo.pdf', 'Important file.', 'Comedy', '204208b8-04d8-4c56-a08a-cb4b4f2ec5ea');
 * Returns:     
 */
CREATE OR REPLACE FUNCTION tk.keepers_create (
    filename TEXT,
    user_posted UUID,
    description TEXT DEFAULT NULL,
    category TEXT DEFAULT NULL
)
    RETURNS TABLE (id UUID)
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    RETURN QUERY
    INSERT INTO tk.keepers (filename, description, category, user_posted)
    VALUES ($1, $3, $4, $2)
    RETURNING tk.keepers.id;
END;
$$;
COMMENT ON FUNCTION tk.keepers_create IS 'Function to create a record in the keepers table.';

 /**
 * Function:    tk.filedatas_create
 * Created:     2021-11-21
 * Author:      Joshua Gray
 * Description: Function to create a record in the filedatas table.
 * Parameters:  keeper_id UUID - 
 *              binary_data BYTEA - 
 * Usage:       SELECT * FROM tk.filedatas_create('foo.pdf', 'Important file.', 'Comedy', '204208b8-04d8-4c56-a08a-cb4b4f2ec5ea');
 * Returns:     
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
    INSERT INTO tk.filedatas (keeper_id, binary_data)
    VALUES ($1, $2);

    RETURN keeper_id;
END;
$$;
COMMENT ON FUNCTION tk.filedatas_create IS 'Function to create a record in the filedatas table.';