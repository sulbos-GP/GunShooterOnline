USE game_database;

SET SQL_SAFE_UPDATES = 0;

DELETE FROM gear;
DELETE FROM storage;
DELETE FROM storage_unit;
DELETE FROM unit_attributes;

CALL create_dummy_item;