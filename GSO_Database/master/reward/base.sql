USE master_database;

CREATE TABLE IF NOT EXISTS master_reward_base
(
    reward_id             		INT					NOT NULL				DEFAULT 0				COMMENT '보상 아이디', 
    money             			INT         		NOT NULL   				DEFAULT 0 				COMMENT '돈',     
	ticket             			INT         		NOT NULL   				DEFAULT 0 				COMMENT '티켓', 
	gacha             			INT         		NOT NULL   				DEFAULT 0 				COMMENT '가챠',
	experience					INT					NOT NULL				DEFAULT 0				COMMENT '경험치',
	reward_box_id				INT					NULL					DEFAULT NULL			COMMENT '보상 박스 아이디',
    
	PRIMARY KEY (reward_id),
	CONSTRAINT FK_reward_box_id_master_reward_box_id FOREIGN KEY (`reward_box_id`) REFERENCES master_reward_box (`reward_box_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

INSERT INTO master_reward_base (reward_id, money, ticket, gacha, experience, reward_box_id) VALUES
(10001, 300, 0, 0, 0, NULL),
(10002, 0, 2, 0, 0, NULL),
(10003, 0, 0, 4, 0, NULL),
(10004, 600, 0, 0, 0, NULL),
(10005, 0, 4, 0, 0, NULL),
(10006, 0, 0, 8, 0, NULL),
(10007, 900, 0, 0, 0, NULL),
(10008, 0, 6, 0, 0, NULL),
(10009, 0, 0, 12, 0, NULL),
(20001, 120, 0, 0, 0, NULL),
(20002, 80, 0, 0, 0, 1),
(20003, 70, 0, 0, 0, NULL),
(20004, 130, 0, 0, 0, 2),
(30001, 0, 0, 1, 0, NULL),
(30002, 0, 0, 2, 0, 3),
(30003, 0, 0, 1, 0, NULL),
(30004, 0, 0, 2, 0, NULL),
(40001, 0, 1, 0, 0, 4),
(40002, 0, 2, 0, 0, 5);

UPDATE master_reward_base SET money = 300, ticket = 0, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10001;
UPDATE master_reward_base SET money = 0, ticket = 2, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10002;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 4, experience = 0, reward_box_id = NULL WHERE reward_id = 10003;
UPDATE master_reward_base SET money = 600, ticket = 0, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10004;
UPDATE master_reward_base SET money = 0, ticket = 4, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10005;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 8, experience = 0, reward_box_id = NULL WHERE reward_id = 10006;
UPDATE master_reward_base SET money = 900, ticket = 0, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10007;
UPDATE master_reward_base SET money = 0, ticket = 6, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 10008;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 12, experience = 0, reward_box_id = NULL WHERE reward_id = 10009;
UPDATE master_reward_base SET money = 120, ticket = 0, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 20001;
UPDATE master_reward_base SET money = 80, ticket = 0, gacha = 0, experience = 0, reward_box_id = 1 WHERE reward_id = 20002;
UPDATE master_reward_base SET money = 70, ticket = 0, gacha = 0, experience = 0, reward_box_id = NULL WHERE reward_id = 20003;
UPDATE master_reward_base SET money = 130, ticket = 0, gacha = 0, experience = 0, reward_box_id = 2 WHERE reward_id = 20004;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 1, experience = 0, reward_box_id = NULL WHERE reward_id = 30001;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 2, experience = 0, reward_box_id = 3 WHERE reward_id = 30002;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 1, experience = 0, reward_box_id = NULL WHERE reward_id = 30003;
UPDATE master_reward_base SET money = 0, ticket = 0, gacha = 2, experience = 0, reward_box_id = NULL WHERE reward_id = 30004;
UPDATE master_reward_base SET money = 0, ticket = 1, gacha = 0, experience = 0, reward_box_id = 4 WHERE reward_id = 40001;
UPDATE master_reward_base SET money = 0, ticket = 2, gacha = 0, experience = 0, reward_box_id = 5 WHERE reward_id = 40002;


