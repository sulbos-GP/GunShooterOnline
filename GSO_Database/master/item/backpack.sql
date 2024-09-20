USE master_database;

#아이템 - 가방
CREATE TABLE IF NOT EXISTS master_item_backpack (
    item_id						INT 				NOT NULL				DEFAULT 0				COMMENT '아이템 아이디',
    total_scale_x				INT					NOT NULL				DEFAULT 0				COMMENT '가방 x크기',
	total_scale_y				INT					NOT NULL				DEFAULT 0				COMMENT '가방 y크기',
	total_weight				DOUBLE				NOT NULL				DEFAULT 0.0				COMMENT '가방 무게',
        
    PRIMARY KEY (item_id),
	CONSTRAINT FK_master_item_backpack_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);