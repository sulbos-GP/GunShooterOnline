USE game_database;

DROP TABLE user_register_quest;

#유저에 등록된 퀘스트
CREATE TABLE IF NOT EXISTS user_register_quest (
    register_quest_id	INT 			NOT NULL	AUTO_INCREMENT				COMMENT '등록된 퀘스트 아이디',
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
    quest_id			INT 			NOT NULL								COMMENT '퀘스트 아이디',
    progress			INT 			NOT NULL	DEFAULT 0					COMMENT '퀘스트 진행도',
    completed           BOOLEAN  		NOT NULL	DEFAULT FALSE				COMMENT '퀘스트 완료 여부',
	register_dt 		DATETIME  		NOT NULL	DEFAULT CURRENT_TIMESTAMP	COMMENT '퀘스트 수령 날짜',
    
	PRIMARY KEY (`register_quest_id`),
    UNIQUE(uid, quest_id),
    CONSTRAINT FK_user_register_quest_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_user_register_quest_master_quest_base_id FOREIGN KEY (`quest_id`) REFERENCES master_database.master_quest_base(`quest_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);