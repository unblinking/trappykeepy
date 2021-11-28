/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V7
 * Created:     2021-11-25
 * Author:      Joshua Gray
 * Description: Create additional keeper functions.
 *              - tk.keepers_count_by_column_value_text
 ******************************************************************************/

/**
 * Function:    tk.keepers_count_by_column_value_text
 * Created:     2021-11-25
 * Author:      Joshua Gray
 * Description: Function to return the count of keeper records that match a given column/value.
 * Parameters:  column_name TEXT - The name of the column to match on.
 *              column_value TEXT - The value of the column to match on.
 * Usage:       SELECT * FROM tk.keepers_count_by_column_value_text('filename', 'foo');
 * Returns:     An integer count of the number of matching records found.
 */
CREATE OR REPLACE FUNCTION tk.keepers_count_by_column_value_text (
    column_name TEXT,
    column_value TEXT
)
    RETURNS integer
    LANGUAGE PLPGSQL
    AS
$$
DECLARE
    row_count integer;
    query text := 'SELECT COUNT(*) FROM tk.keepers';
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
COMMENT ON FUNCTION tk.keepers_count_by_column_value_text IS 'Function to count records from the keepers table by the specified column/value.';