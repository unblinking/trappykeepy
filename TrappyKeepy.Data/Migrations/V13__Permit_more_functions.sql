/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V13
 * Created:     2021-12-02
 * Author:      Joshua Gray
 * Description: Create additional permit functions.
 *              - tk.permit_match_count
 ******************************************************************************/

 /**
 * Function:    tk.permit_match_count
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return the count of permit records that match the specified keeper_id with either a group_id or user_id.
 *              See if an exact matching permit already exists.
 * Parameters:  id_keeper UUID - The id of the keeper.
 *              id_user UUID - The id of the user.
 *              id_group UUID - The id of the group.
 * Usage:       SELECT * FROM tk.permit_match_count('00000000-0000-0000-0000-000000000000', '00000000-0000-0000-0000-000000000000', null);
 *              SELECT * FROM tk.permit_match_count('00000000-0000-0000-0000-000000000000', null, '00000000-0000-0000-0000-000000000000');
 * Returns:     An integer count of the number of matching records found (should only find one exact match).
 */
CREATE OR REPLACE FUNCTION tk.permit_match_count (
    id_keeper UUID,
    id_user UUID,
    id_group UUID
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
    query text := 'SELECT COUNT(*) FROM tk.permits WHERE tk.permits.keeper_id = $1';
BEGIN
    IF id_user IS NOT NULL THEN
        query := query || ' AND tk.permits.user_id = $2';
    END IF;
    IF id_group IS NOT NULL THEN
        query := query || ' AND tk.permits.group_id = $3';
    END IF;
    EXECUTE query
    USING id_keeper, id_user, id_group
    INTO row_count;
    RETURN row_count;
END;
$$;
COMMENT ON FUNCTION tk.permit_match_count IS 'Function to return the count of permit records that match the specified keeper_id with either a group_id or user_id.';

/**
 * Function:    tk.permits_count_by_column_value_uuid
 * Created:     2021-12-02
 * Author:      Joshua Gray
 * Description: Function to return the count of permit records that match a given column/value.
 * Parameters:  column_name TEXT - The name of the column to match on.
 *              column_value UUID - The value of the column to match on.
 * Usage:       SELECT * FROM tk.permits_count_by_column_value_uuid('keeper_id', '00000000-0000-0000-0000-000000000000');
 *              SELECT * FROM tk.permits_count_by_column_value_uuid('group_id', '00000000-0000-0000-0000-000000000000');
 *              SELECT * FROM tk.permits_count_by_column_value_uuid('user_id', '00000000-0000-0000-0000-000000000000');
 * Returns:     An integer count of the number of matching records found.
 */
CREATE OR REPLACE FUNCTION tk.permits_count_by_column_value_uuid (
    column_name TEXT,
    column_value uuid
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
    query text := 'SELECT COUNT(*) FROM tk.permits';
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
COMMENT ON FUNCTION tk.permits_count_by_column_value_uuid IS 'Function to count records from the permits table by the specified column/value.';