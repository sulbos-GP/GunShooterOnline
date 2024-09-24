USE game_database;

#유저의 메타데이터
CREATE TABLE IF NOT EXISTS user_metadata (
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
    total_games 		INT 			NOT NULL 	DEFAULT 0					COMMENT '게임',
	kills 				INT 			NOT NULL 	DEFAULT 0					COMMENT '킬',
	deaths 				INT 			NOT NULL 	DEFAULT 0					COMMENT '데스',
    damage 				INT 			NOT NULL 	DEFAULT 0					COMMENT '대미지',
    farming 			INT 			NOT NULL 	DEFAULT 0					COMMENT '파밍',
    escape 				INT 			NOT NULL 	DEFAULT 0					COMMENT '탈출',
	survival_time 		INT 			NOT NULL 	DEFAULT 0					COMMENT '생존',
        
    PRIMARY KEY (`uid`),
    CONSTRAINT FK_user_metadata_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);