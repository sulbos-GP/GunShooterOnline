USE master_database;

DROP TABLE master_quest_base;

#퀘스트 정보
CREATE TABLE IF NOT EXISTS master_quest_base (
    quest_id					INT 				NOT NULL				DEFAULT 0				COMMENT '퀘스트 아이디',
    type						VARCHAR(5) 			NOT NULL 				DEFAULT ''				COMMENT '퀘스트 타입',
	category 					VARCHAR(5) 			NOT NULL 				DEFAULT ''				COMMENT '퀘스트 카테고리',
	title 						VARCHAR(20) 		NOT NULL 				DEFAULT ''				COMMENT '퀘스트 제목',
	target						INT					NOT NULL 				DEFAULT 0				COMMENT '퀘스트 목표',
	tag 						VARCHAR(10) 		NOT NULL 				DEFAULT ''				COMMENT '퀘스트 태그',
    reward_id 					INT 				NOT NULL  				DEFAULT 0				COMMENT '보상 아이디',
	next_quest_id 				INT					NULL 					DEFAULT 0				COMMENT '연계 퀘스트',
    start_condition_id			INT 				NULL 					DEFAULT 0				COMMENT '선행 조건',

    PRIMARY KEY (quest_id),
	CONSTRAINT FK_master_quest_base_master_reward_base_id FOREIGN KEY (`reward_id`) REFERENCES master_reward_base(`reward_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

INSERT INTO master_quest_base (
    quest_id, type, category, title, target, tag, reward_id, next_quest_id, start_condition_id
) VALUES
(20001, 'day', '전투', '플레이어 적 처치', 	1, 'PLAYER', 20001, NULL,  NULL),
(20002, 'day', '전투', '특수지역 적 처치', 	2, 'NPC202', 20002, NULL, NULL),
(20003, 'day', '전투', '좀비 처치', 		3, 'NPC201', 20003, NULL, NULL),
(20004, 'day', '전투', 'NPC 적 처치', 		4, 'NPC203', 20004, NULL, NULL),
(20005, 'day', '보급', '건전지 회수', 		2, 'S002', 20005, NULL, NULL),
(20006, 'day', '보급', '의약품 가방 회수', 	1, 'B001', 20006, NULL, NULL),
(20007, 'day', '보급', '통나무 회수', 		8, 'S006', 20007, NULL, NULL),
(20008, 'day', '보급', '밧줄 한묶음 회수', 	10, 'S003', 20008, NULL, NULL),
(20009, 'day', '플레이', '전장 플레이', 		3, 'PLAY_IN', 20009, NULL, NULL),
(20010, 'day', '플레이', '전장 탈출', 		2, 'PLAY_OUT', 20010, NULL, NULL);
