USE game_database;

CREATE TABLE IF NOT EXISTS user_level_reward (
    reward_level_id		INT 			NOT NULL	AUTO_INCREMENT				COMMENT '레벨 보상 아이디',
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
    reward_id			INT 			NOT NULL								COMMENT '보상 아이디',
    received			BOOLEAN  		NOT NULL	DEFAULT FALSE				COMMENT '수령 확인',
    received_dt 		DATETIME  		NULL		DEFAULT NULL				COMMENT '수령 확인 날짜',
    
	PRIMARY KEY (`reward_level_id`),
    UNIQUE(uid, reward_id),
    CONSTRAINT FK_user_reward_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_user_level_reward_master_reward_level_id FOREIGN KEY (`reward_id`) REFERENCES master_database.master_reward_level(`reward_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);