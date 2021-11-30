/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V8
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Create the group type, and the groups table.
 *              Create the membership type, and the memberships table.
 ******************************************************************************/

/**
 * Type:        tk.group_type
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Type for an individual group record.
 * Attributes:  id UUID - 
 *              name TEXT - 
 *              description TEXT - 
 *              date_created TIMESTAMPTZ - 
 */
CREATE TYPE tk.group_type AS (
    id UUID,
    name TEXT,
    description TEXT,
    date_created TIMESTAMPTZ
);
COMMENT ON TYPE tk.group_type IS 'Type for an individual group record.';

/**
 * Table:       tk.groups
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Table to store group records.
 * Columns:     id - Primary key with default using the gen_random_uuid() function.
 *              name - Unique, and not null.
 *              description - 
 *              date_created - Not null.
 */
CREATE TABLE IF NOT EXISTS tk.groups OF tk.group_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    name WITH OPTIONS UNIQUE NOT NULL,
    date_created WITH OPTIONS NOT NULL DEFAULT CURRENT_TIMESTAMP
);
COMMENT ON TABLE tk.groups IS 'Table to store group records.';
COMMENT ON COLUMN tk.groups.id IS 'UUID primary key.';
COMMENT ON COLUMN tk.groups.name IS 'Unique group name.';
COMMENT ON COLUMN tk.groups.description IS 'Description of the group.';
COMMENT ON COLUMN tk.groups.date_created IS 'Datetime the group was created in the database.';

/**
 * Type:        tk.membership_type
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Type for an individual group membership record.
 * Attributes:  group_id UUID - 
 *              user_id UUID -
 */
CREATE TYPE tk.membership_type AS (
    group_id UUID,
    user_id UUID
);
COMMENT ON TYPE tk.membership_type IS 'Type for an individual membership record.';

/**
 * Table:       tk.memberships
 * Created:     2021-11-28
 * Author:      Joshua Gray
 * Description: Table to store group membership records.
 * Columns:     group_id - Primary key using the id from the associated tk.groups record.
 *              user_id - Not null.
 */
CREATE TABLE IF NOT EXISTS tk.memberships OF tk.membership_type (
    group_id WITH OPTIONS NOT NULL,
    user_id WITH OPTIONS NOT NULL,
    CONSTRAINT fk_group_of_memberships FOREIGN KEY (group_id) REFERENCES tk.groups (id) ON DELETE NO ACTION,
    CONSTRAINT fk_user_of_memberships FOREIGN KEY (user_id) REFERENCES tk.users (id) ON DELETE NO ACTION
);
CREATE INDEX group_membership_index ON tk.memberships (group_id);
CREATE INDEX user_membership_index ON tk.memberships (user_id);
COMMENT ON TABLE tk.memberships IS 'Table to store group membership records.';
COMMENT ON COLUMN tk.memberships.group_id IS 'UUID primary key, and foreign key to the tk.groups table.';
COMMENT ON COLUMN tk.memberships.user_id IS 'UUID, and foreign key to the tk.users table.';
