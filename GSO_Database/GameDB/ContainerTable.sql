USE game_database;

DROP TABLE IF EXISTS gear;

DROP TABLE IF EXISTS storage;
DROP TABLE IF EXISTS storage_unit;

#유저의 무장(장비)
CREATE TABLE IF NOT EXISTS gear (
	uid			INT 				NOT NULL								COMMENT '유저 아이디',
	item_id		INT 				NOT NULL								COMMENT '아이템 아이디',
	gear_type	ENUM('main_weapon', 'sub_weapon', 'armor', 'backpack', 'pocket_first', 'pocket_second', 'pocket_third') 	NOT NULL	COMMENT '슬롯 타입',
    
	PRIMARY KEY (`uid`),
	CONSTRAINT FK_gear_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_gear_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#저장소
CREATE TABLE IF NOT EXISTS storage (
	storage_id			INT 							NOT NULL	AUTO_INCREMENT				COMMENT '저장소 아이디',
	storage_item_id		INT 							NOT NULL								COMMENT '저장소 아이템 아이디',
	uid					INT 							NULL		DEFAULT NULL				COMMENT '유저 아이디',
    storage_type  		ENUM('backpack', 'cabinet') 	NOT NULL 								COMMENT '저장소 타입(가방/캐비넷)',
    
    PRIMARY KEY (`storage_id`),
    CONSTRAINT FK_storage_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_storage_storage_item_id_master_item_base_item_id FOREIGN KEY (`storage_item_id`) REFERENCES master_database.master_item_base (`item_id`)
);

#유저의 컨테이너 슬롯
CREATE TABLE IF NOT EXISTS storage_unit (
    storage_unit_id		INT 							NOT NULL	AUTO_INCREMENT				COMMENT '저장소 슬롯 아이디',
    storage_id    		INT                 			NOT NULL        						COMMENT '저장소 아이디 (가방 ID 또는 캐비넷 ID)',
	item_id				INT 							NOT NULL 								COMMENT '아이템 아이디',
    grid_x				INT 							NOT NULL 								COMMENT '아이템 x위치',
	grid_y				INT 							NOT NULL 								COMMENT '아이템 y위치',
	rotation			INT 							NOT NULL 								COMMENT '아이템 회전',
	stack_count			INT								NOT NULL								COMMENT '아이템 스택 카운터',

    PRIMARY KEY (`storage_unit_id`),
    CONSTRAINT FK_storage_unit_storage_id_storage_storage_id FOREIGN KEY (`storage_id`) REFERENCES storage (`storage_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_storage_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);


SELECT * FROM gear;
SELECT * FROM storage;
SELECT * FROM storage_unit;