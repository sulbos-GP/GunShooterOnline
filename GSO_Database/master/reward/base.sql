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
(10001, 1000, 0, 0, 0, NULL),
(10002, 0, 2, 0, 0, NULL),
(10003, 0, 0, 5, 0, NULL),
(10004, 2000, 0, 0, 0, NULL),
(10005, 0, 4, 0, 0, NULL),
(10006, 0, 0, 10, 0, NULL),
(10007, 3000, 0, 0, 0, NULL),
(10008, 0, 6, 0, 0, NULL),
(10009, 0, 0, 15, 0, NULL),
(20001, 1000, 1, 0, 500, NULL),
(20002, 0, 0, 0, 1200, 1),
(20003, 500, 0, 0, 500, NULL),
(20004, 0, 0, 0, 2000, 2),
(30001, 500, 0, 0, 500, NULL),
(30002, 0, 0, 0, 500, 3),
(30003, 2000, 0, 0, 0, NULL),
(30004, 0, 0, 0, 2000, NULL),
(40001, 0, 1, 1, 1000, 4),
(40002, 0, 1, 1, 1000, 5);


