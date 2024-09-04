USE game_database;

#자신의 UID 검색 후 넣기
SET @uid = 11;

#아이템 생성 (Main, Sub, Armor, Backpack, Pocket)
#메인
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (102, 0, 1);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'main_weapon', @last_unit_attributes);

#서브
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (101, 0, 1);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'sub_weapon', @last_unit_attributes);

#방어구
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (201, 100, 1);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'armor', @last_unit_attributes);

#가방
INSERT INTO storage (storage_type) VALUES ('backpack');
SET @last_storage_id = LAST_INSERT_ID();

INSERT INTO unit_attributes (item_id, durability, unit_storage_id, amount) VALUES (302, 0, @last_storage_id, 1);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'backpack', @last_unit_attributes);

#포켓 1
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (402, 0, 5);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'pocket_first', @last_unit_attributes);

#포켓 2
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (502, 0, 10);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'pocket_second', @last_unit_attributes);

#포켓 3
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (602, 0, 15);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO gear (uid, part, unit_attributes_id) VALUES (@uid, 'pocket_third', @last_unit_attributes);

#인벤토리에 들어있는 아이템들
#아이템 1
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (402, 0, 5);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO storage_unit (storage_id, grid_x, grid_y, rotation, unit_attributes_id) VALUES (@last_storage_id, 0, 0, 0 , @last_unit_attributes);

#아이템 2
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (502, 0, 10);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO storage_unit (storage_id, grid_x, grid_y, rotation, unit_attributes_id) VALUES (@last_storage_id, 0, 2, 0 , @last_unit_attributes);

#아이템 3
INSERT INTO unit_attributes (item_id, durability, amount) VALUES (602, 0, 15);
SET @last_unit_attributes = LAST_INSERT_ID();
INSERT INTO storage_unit (storage_id, grid_x, grid_y, rotation, unit_attributes_id) VALUES (@last_storage_id, 2, 2, 0 , @last_unit_attributes);

