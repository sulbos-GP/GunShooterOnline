USE game_database;

CREATE TABLE IF NOT EXISTS unit_attributes (
    unit_attributes_id		INT 							NOT NULL	AUTO_INCREMENT				COMMENT '유닛 속성 아이디',
    
    item_id					INT 							NOT NULL 								COMMENT '아이템 아이디',
    durability				INT 							NOT NULL	DEFAULT 0					COMMENT '아이템 내구도',
    loaded_ammo				INT 							NOT NULL	DEFAULT 0					COMMENT '장전된 총알 개수',
	unit_storage_id    		INT                 			NULL		DEFAULT NULL        		COMMENT '저장소 아이디 (가방 전용)',
	amount					INT								NOT NULL								COMMENT '아이템 스택 카운터',
            
	PRIMARY KEY (`unit_attributes_id`),
	CONSTRAINT FK_unit_attributes_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_unit_attributes_unit_storage_id_storage_storage_id FOREIGN KEY (`unit_storage_id`) REFERENCES storage (`storage_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);