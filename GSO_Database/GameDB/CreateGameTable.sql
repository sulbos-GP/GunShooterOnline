USE game_database;

DROP TABLE IF EXISTS user_metadata;
DROP TABLE IF EXISTS user_skill;

DROP TABLE IF EXISTS user;

#유저의 로그인 정보
CREATE TABLE IF NOT EXISTS user (
    uid 				INT 			NOT NULL 	AUTO_INCREMENT 				COMMENT '유저 아이디',
    player_id 			VARCHAR(256) 	NOT NULL 								COMMENT '플레이어 아이디',
    nickname			VARCHAR(10) 	NULL 		DEFAULT NULL 				COMMENT '닉네임',
    service 			VARCHAR(32) 	NOT NULL								COMMENT '서비스',
	create_dt           DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '생성 일시', 
    recent_login_dt     DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 로그인 일시',
    
	 PRIMARY KEY (uid),
     UNIQUE KEY (nickname)
);

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

#유저의 스킬(e.g MMR)
CREATE TABLE IF NOT EXISTS user_skill (
    uid				INT 				NOT NULL								COMMENT '유저 아이디',
    rating			DOUBLE 				NOT NULL 	DEFAULT 1500.0				COMMENT '레이팅',
    deviation		DOUBLE 				NOT NULL 	DEFAULT 350.0				COMMENT '표준 편차',
	volatility		DOUBLE 				NOT NULL 	DEFAULT 0.06				COMMENT '변동성',
    
    PRIMARY KEY (`uid`),
    CONSTRAINT FK_user_skill_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#더미 생성 (CreateDummyProcedures 참고)

SELECT * FROM user;
SELECT * FROM user_metadata;
SELECT * FROM user_skill;
