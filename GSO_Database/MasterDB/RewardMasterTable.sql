USE master_database;

CREATE TABLE IF NOT EXISTS master_reward_box
(
    reward_box_id             	INT         		NOT NULL    				COMMENT '보상 박스 아이디', 
    
	PRIMARY KEY (reward_box_id)
);

CREATE TABLE IF NOT EXISTS master_reward_base
(
    reward_id             		INT         		NOT NULL    				COMMENT '보상 아이디', 
    money             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '돈',     
	ticket             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '티켓', 
	gacha             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '가챠',
	reward_box_id				INT					NULL		DEFAULT NULL	COMMENT '보상 박스 아이디',
    
	PRIMARY KEY (reward_id),
	CONSTRAINT FK_reward_box_id_master_reward_box_id FOREIGN KEY (`reward_box_id`) REFERENCES master_reward_box (`reward_box_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

#레벨 보상
CREATE TABLE IF NOT EXISTS master_reward_level
(
    reward_id					INT					NOT NULL					COMMENT '보상 정보',
	level             			INT         		NOT NULL    				COMMENT '레벨',
    experience					INT					NOT NULL					COMMENT '경험치',
	name 						VARCHAR(50) 		NOT NULL 					COMMENT '보상 이름',
	icon						VARCHAR(50) 		NOT NULL 					COMMENT '보상 아이콘',
        
    PRIMARY KEY (reward_id),
    UNIQUE (level),
	CONSTRAINT FK_master_reward_level_reward_id_master_reward_base_reward_id FOREIGN KEY (`reward_id`) REFERENCES master_reward_base (`reward_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

DELETE FROM master_reward_level;
DELETE FROM master_reward_base;

INSERT INTO master_reward_base(reward_id, money) 	VALUES 	(10001, 1000);
INSERT INTO master_reward_base(reward_id, ticket) 	VALUES 	(10002, 2);
INSERT INTO master_reward_base(reward_id, gacha) 	VALUES 	(10003, 5);

INSERT INTO master_reward_level(reward_id, level, experience, name, icon) 	VALUES 	(10001, 1, 200, '1000골드', 'IconR_pill');
INSERT INTO master_reward_level(reward_id, level, experience, name, icon) 	VALUES 	(10002, 2, 300, '티켓 2장', 'IconR_FuelTank');
INSERT INTO master_reward_level(reward_id, level, experience, name, icon) 	VALUES 	(10003, 3, 400, '뽑기 5장', 'IconS_battery');

