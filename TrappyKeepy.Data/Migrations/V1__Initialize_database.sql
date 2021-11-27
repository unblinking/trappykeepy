/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V1
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Initialize the TrappyKeepy database, creating the schema, and a
 *              function to return table types.
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
 * Function:    tk.get_table_types
 * Created:     2021-11-20
 * Author:      Joshua Gray
 * Description: Function to return table column types.
 * Parameters:  table_name TEXT - The name of the table without the schema.
 * Usage:       SELECT * FROM tk.get_table_types('users');
 * Returns:     column_name, data_type
 */
CREATE OR REPLACE FUNCTION tk.get_table_types (table_name TEXT)
    RETURNS TABLE (column_name VARCHAR ( 255 ), data_type VARCHAR ( 255 ))
    LANGUAGE PLPGSQL
    AS
$$
BEGIN
    CREATE TEMP TABLE IF NOT EXISTS users_information_schema_columns(
        column_name VARCHAR ( 255 ),
        data_type VARCHAR ( 255 )
    ) ON COMMIT DROP;

    INSERT INTO users_information_schema_columns ( column_name, data_type )
    SELECT isc.column_name, isc.data_type
    FROM information_schema.columns as isc
    WHERE isc.table_name = $1;

    RETURN QUERY
    SELECT * FROM users_information_schema_columns;
END;
$$;
COMMENT ON FUNCTION tk.get_table_types IS 'Function to return table column types.';
