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
 * Grant access to the application user (dbuser).
 */
GRANT CONNECT ON DATABASE keepydb TO dbuser;
GRANT USAGE ON SCHEMA tk TO dbuser;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA tk TO dbuser;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA tk TO dbuser;
