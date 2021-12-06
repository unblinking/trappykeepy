/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V9999
 * Created:     2021-12-02
 * Author:      Joshua Gray
 * Description: Grant safe access to the API database user (dbuser).
 ******************************************************************************/

GRANT CONNECT ON DATABASE keepydb TO dbuser;
GRANT USAGE ON SCHEMA tk TO dbuser;
GRANT EXECUTE ON ALL FUNCTIONS IN SCHEMA tk TO dbuser;
GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA tk TO dbuser;
