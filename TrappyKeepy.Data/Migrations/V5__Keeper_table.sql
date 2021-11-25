/*******************************************************************************
 * Migration:   TrappyKeepy
 * Version:     V5
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Create the keeper type, and the keepers table.
 ******************************************************************************/

/**
 * Type:        tk.keeper_type
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Type for an individual document record worth keeping.
 * Attributes:  id UUID - 
 *              filename TEXT - 
 *              description TEXT - 
 *              category TEXT - 
 *              date_posted TIMESTAMPTZ - 
 *              user_posted UUID -
 */
CREATE TYPE tk.keeper_type AS (
    id UUID,
    filename TEXT,
    description TEXT,
    category TEXT,
    date_posted TIMESTAMPTZ,
    user_posted UUID
);
COMMENT ON TYPE tk.keeper_type IS 'Type for an individual keeper/document metadata record.';

/**
 * Table:       tk.keepers
 * Created:     2021-11-24
 * Author:      Joshua Gray
 * Description: Table to store document records worth keeping.
 * Columns:     id - Primary key with default using the gen_random_uuid() function.
 *              filename - Unique, and not null.
 *              description - 
 *              category - 
 *              date_posted - Not null.
 *              user_posted - 
 */
CREATE TABLE IF NOT EXISTS tk.keepers OF tk.keeper_type (
    id WITH OPTIONS PRIMARY KEY DEFAULT gen_random_uuid(),
    filename WITH OPTIONS UNIQUE NOT NULL,
    date_posted WITH OPTIONS NOT NULL,
    user_posted WITH OPTIONS NOT NULL,
    CONSTRAINT fk_user_posted_keeper FOREIGN KEY (user_posted) REFERENCES tk.users (id) ON DELETE NO ACTION
);
COMMENT ON TABLE tk.keepers IS 'Table to store keeper/document metadata records.';
COMMENT ON COLUMN tk.keepers.id IS 'UUID primary key.';
COMMENT ON COLUMN tk.keepers.filename IS 'Unique document filename.';
COMMENT ON COLUMN tk.keepers.description IS 'Description of the document.';
COMMENT ON COLUMN tk.keepers.category IS 'Category of the document.';
COMMENT ON COLUMN tk.keepers.date_posted IS 'Datetime the document was created in the database.';
COMMENT ON COLUMN tk.keepers.user_posted IS 'User id associated with creating the document in the database.';

/**
 *
 */
CREATE TYPE tk.filebytea_type AS (
    keeper_id UUID,
    filebytea bytea
);
COMMENT ON TYPE tk.filebytea_type IS 'Type for an individual keeper/document file/bytea/blob record.';

/**
 *
 */
CREATE TABLE IF NOT EXISTS tk.filebyteas OF tk.filebytea_type (
    keeper_id WITH OPTIONS PRIMARY KEY,
    filebytea WITH OPTIONS NOT NULL,
    CONSTRAINT fk_keeper_of_filebytea FOREIGN KEY (keeper_id) REFERENCES tk.keepers (id) ON DELETE NO ACTION
);
COMMENT ON TABLE tk.filebyteas IS 'Table to store keeper/document file/bytea/blob records.';
COMMENT ON COLUMN tk.filebyteas.keeper_id IS 'UUID primary key, and foreign key to the tk.keepers table.';
COMMENT ON COLUMN tk.filebyteas.filebytea IS 'Bytea blob of the actual keeper/document uploaded.';
