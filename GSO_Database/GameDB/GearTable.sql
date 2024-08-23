USE game_database;

DROP TABLE IF EXISTS gear;

#유저의 무장(장비)
CREATE TABLE IF NOT EXISTS gear (
	gear_id		INT 				NOT NULL AUTO_INCREMENT					COMMENT '장비 아이디',
	uid			INT 				NOT NULL								COMMENT '유저 아이디',
	part		ENUM('main_weapon', 'sub_weapon', 'armor', 'backpack', 'pocket_first', 'pocket_second', 'pocket_third') 	NOT NULL	COMMENT '슬롯 타입',
    
    PRIMARY KEY (`gear_id`),
    UNIQUE (`uid`, `part`),
	CONSTRAINT FK_gear_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);