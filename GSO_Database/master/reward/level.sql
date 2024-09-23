USE master_database;

#레벨 보상
CREATE TABLE IF NOT EXISTS master_reward_level
(
    reward_id					INT					NOT NULL				DEFAULT 0				COMMENT '보상 정보',
	level             			INT         		NOT NULL    			DEFAULT 0				COMMENT '레벨',
	name 						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '보상 이름',
	icon						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '보상 아이콘',
        
    PRIMARY KEY (reward_id),
    UNIQUE (level),
	CONSTRAINT FK_master_reward_level_reward_id_master_reward_base_reward_id FOREIGN KEY (`reward_id`) REFERENCES master_reward_base (`reward_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

INSERT INTO master_reward_level (reward_id, level, name, icon) VALUES
(10001, 2, '1000 골드', 'IconS_goldore'),
(10002, 3, '연료 2개', 'IconR_fueltank'),
(10003, 4, '건전지 5개', 'IconS_battery'),
(10004, 5, '2000 골드', 'IconS_goldore'),
(10005, 6, '연료 4개', 'IconR_fueltank'),
(10006, 7, '건전지 10개', 'IconS_battery'),
(10007, 8, '3000 골드', 'IconS_goldore'),
(10008, 9, '연료 6개', 'IconR_fueltank'),
(10009, 10, '건전지 15개', 'IconS_battery');

