USE master_database;

CREATE TABLE IF NOT EXISTS master_item_weapon (
    item_id						INT 				NOT NULL				DEFAULT 0				COMMENT '아이템 아이디',
	attack_range				INT 				NOT NULL				DEFAULT 0				COMMENT '공격 범위',
	damage						INT 				NOT NULL				DEFAULT 0				COMMENT '공격 데미지',
	distance					INT 				NOT NULL				DEFAULT 0				COMMENT '공격 거리',
	reload_round				INT 				NOT NULL				DEFAULT 0				COMMENT '재장전 수',
	attack_speed				DOUBLE 				NOT NULL				DEFAULT 0.0				COMMENT '공격 속도',
	reload_time					INT 				NOT NULL				DEFAULT 0				COMMENT '재장전 시간',
   	bullet						VARCHAR(20) 		NOT NULL				DEFAULT ''				COMMENT '사용 탄환',
    
    PRIMARY KEY (item_id),
	CONSTRAINT FK_master_item_weapon_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);