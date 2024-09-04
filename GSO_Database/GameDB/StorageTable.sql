USE game_database;

DROP TABLE IF EXISTS unit_attributes;
DROP TABLE IF EXISTS storage_unit;
DROP TABLE IF EXISTS storage;

#저장소
CREATE TABLE IF NOT EXISTS storage (
	storage_id			INT 							NOT NULL	AUTO_INCREMENT				COMMENT '저장소 아이디',
	uid					INT 							NULL		DEFAULT NULL				COMMENT '유저 아이디',
    storage_type  		ENUM('backpack', 'cabinet') 	NOT NULL 								COMMENT '저장소 타입(가방/캐비넷)',
    
    PRIMARY KEY (`storage_id`),
    CONSTRAINT FK_storage_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

CREATE TABLE IF NOT EXISTS unit_attributes (
    unit_attributes_id		INT 							NOT NULL	AUTO_INCREMENT				COMMENT '유닛 속성 아이디',
    
    item_id					INT 							NOT NULL 								COMMENT '아이템 아이디',
    durability				INT 							NOT NULL	DEFAULT 0					COMMENT '아이템 내구도',
	unit_storage_id    		INT                 			NULL		DEFAULT NULL        		COMMENT '저장소 아이디 (가방 전용)',
	amount					INT								NOT NULL								COMMENT '아이템 스택 카운터',
            
	PRIMARY KEY (`unit_attributes_id`),
	CONSTRAINT FK_unit_attributes_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_unit_attributes_unit_storage_id_storage_storage_id FOREIGN KEY (`unit_storage_id`) REFERENCES storage (`storage_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

#유저의 컨테이너 슬롯
CREATE TABLE IF NOT EXISTS storage_unit (
    storage_unit_id		INT 							NOT NULL	AUTO_INCREMENT				COMMENT '저장소 슬롯 아이디',
    storage_id    		INT                 			NOT NULL        						COMMENT '저장소 아이디 (가방 ID 또는 캐비넷 ID)',
    grid_x				INT 							NOT NULL 								COMMENT '아이템 x위치',
	grid_y				INT 							NOT NULL 								COMMENT '아이템 y위치',
	rotation			INT 							NOT NULL 								COMMENT '아이템 회전',
	unit_attributes_id  INT								NOT NULL								COMMENT '아이템 속성 아이디',

    PRIMARY KEY (`storage_unit_id`),
    CONSTRAINT FK_storage_unit_storage_id_storage_storage_id FOREIGN KEY (`storage_id`) REFERENCES storage (`storage_id`) ON DELETE CASCADE ON UPDATE RESTRICT,
	CONSTRAINT FK_storage_unit_id_att_id_uatt_att_id FOREIGN KEY (`unit_attributes_id`) REFERENCES unit_attributes(`unit_attributes_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

SELECT * FROM storage;
SELECT * FROM unit_attributes;
SELECT * FROM storage_unit;

#DELETE FROM storage_unit;
#DELETE FROM unit_attributes WHERE unit_attributes_id=47;
#SELECT * FROM storage_unit WHERE storage_id=11