/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V11
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Create the permit type, and the permit table.
 ******************************************************************************/

/**
 * Type:        tk.permit_type
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Type for an individual permit record.
 * Attributes:  id UUID - Very low probability that a UUID will be duplicated.
 *              keeper_id UUID - 
 *              user_id UUID - 
 *              group_id UUID - 
 */
CREATE TYPE tk.permit_type AS (
    id UUID,
    keeper_id UUID,
    user_id UUID,
    group_id UUID
);
COMMENT ON TYPE tk.permit_type IS 'Type for an individual permit record.';

/**
 * Table:       tk.permit
 * Created:     2021-12-01
 * Author:      Joshua Gray
 * Description: Table to store permit records.
 * Columns:     id - Primary key with default using the gen_random_uuid() function.
 *              keeper_id - Not null.
 *              user_id - 
 *              group_id - 
 */
CREATE TABLE IF NOT EXISTS tk.permits OF tk.permit_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    keeper_id WITH OPTIONS NOT NULL
);
COMMENT ON TABLE tk.permits IS 'Table to store permit records.';
COMMENT ON COLUMN tk.permits.id IS 'UUID primary key.';
COMMENT ON COLUMN tk.permits.keeper_id IS 'UUID from the tk.keepers table.';
COMMENT ON COLUMN tk.permits.user_id IS 'UUID from the tk.users table.';
COMMENT ON COLUMN tk.permits.group_id IS 'UUID from the tk.groups table.';
