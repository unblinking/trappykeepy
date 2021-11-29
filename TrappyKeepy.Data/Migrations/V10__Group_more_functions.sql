/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V10
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Create additional group functions.
 *              - tk.groups_count_by_column_value_text
 *              - tk.memberships_count_by_group_and_user
 ******************************************************************************/

/**
 * Function:    tk.groups_count_by_column_value_text
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return the count of group records that match a given column/value.
 * Parameters:  column_name TEXT - The name of the column to match on.
 *              column_value TEXT - The value of the column to match on.
 * Usage:       SELECT * FROM tk.groups_count_by_column_value_text('name', 'foo');
 * Returns:     An integer count of the number of matching records found.
 */
CREATE OR REPLACE FUNCTION tk.groups_count_by_column_value_text (
    column_name TEXT,
    column_value TEXT
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
    query text := 'SELECT COUNT(*) FROM tk.groups';
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
COMMENT ON FUNCTION tk.groups_count_by_column_value_text IS 'Function to count records from the groups table by the specified column/value.';

/**
 * Function:    tk.memberships_count_by_group_and_user
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Function to return the count of membership records that match a given group/user.
 *              See if a user is already in a group.
 * Parameters:  id_group UUID - The id of the group
 *              id_user UUID - The id of the user
 * Usage:       SELECT * FROM tk.memberships_count_by_group_and_user('a1e84bb3-3429-4bfc-95c8-e184fceaa036', 'a1e84bb3-3429-4bfc-95c8-e184fceaa036');
 * Returns:     An integer count of the number of matching records found.
 */
CREATE OR REPLACE FUNCTION tk.memberships_count_by_group_and_user (
    id_group UUID,
    id_user UUID
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
BEGIN
    SELECT COUNT(*)
    FROM tk.memberships
    WHERE tk.memberships.group_id = $1
        AND tk.memberships.user_id = $2
    INTO row_count;
    RETURN row_count;
END;
$$;
COMMENT ON FUNCTION tk.memberships_count_by_group_and_user IS 'Function to return the count of membership records that match a given group/user.';