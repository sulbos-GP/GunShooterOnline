USE game_database;

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

DELETE FROM storage_unit;