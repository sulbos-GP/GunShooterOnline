USE master_database;

#아이템 - 사용
CREATE TABLE IF NOT EXISTS master_item_use (
    item_id						INT 				NOT NULL				DEFAULT 0				COMMENT '아이템 아이디',
    energy						INT					NOT NULL				DEFAULT 0				COMMENT '회복 아이템의 회복량',
	active_time					DOUBLE				NOT NULL				DEFAULT 0.0				COMMENT '효과발동 시간',
	duration					DOUBLE				NOT NULL				DEFAULT 0.0				COMMENT '회복 아이템의 지속시간',
	effect						ENUM('immediate', 'buff')	NOT NULL		DEFAULT 'immediate'		COMMENT '해당 아이템의 효과 타입',
	cool_time					DOUBLE				NOT NULL				DEFAULT 0.0				COMMENT '재사용 대기시간',
    
    PRIMARY KEY (item_id),
	CONSTRAINT FK_master_item_use_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);