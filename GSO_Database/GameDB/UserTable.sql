USE game_database;

#DROP TABLE IF EXISTS user_metadata;
#DROP TABLE IF EXISTS user_skill;
#DROP TABLE IF EXISTS user;

#유저 정보
CREATE TABLE IF NOT EXISTS user (
    uid 				INT 			NOT NULL 	AUTO_INCREMENT 				COMMENT '유저 아이디',
    player_id 			VARCHAR(256) 	NOT NULL 								COMMENT '플레이어 아이디',
    nickname			VARCHAR(10) 	NULL 		DEFAULT NULL 				COMMENT '닉네임',
	experience 			INT 			NOT NULL	DEFAULT 0					COMMENT '경험치',
	money 				INT 			NOT NULL	DEFAULT 0					COMMENT '돈',
	ticket 				INT 			NOT NULL	DEFAULT 0					COMMENT '티켓',
	gacha	 			INT 			NOT NULL	DEFAULT 0					COMMENT '가챠',
    service 			VARCHAR(32) 	NOT NULL								COMMENT '서비스',
    refresh_token 		VARCHAR(512) 	NULL 		DEFAULT NULL				COMMENT '갱신 토큰',	#TODO NotNull로 교채
	create_dt           DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '생성 일시', 
    recent_login_dt     DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 로그인 일시',
    
	 PRIMARY KEY (uid),
     UNIQUE KEY (nickname)
);
#ALTER TABLE user ADD COLUMN gacha INT NOT NULL DEFAULT 0 AFTER ticket;

CREATE TABLE IF NOT EXISTS user_level_reward (
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
    reward_id			INT 			NOT NULL								COMMENT '보상 아이디',
    received			BOOLEAN  		NOT NULL	DEFAULT FALSE				COMMENT '수령 확인',
    received_dt 		DATETIME  		NULL		DEFAULT NULL				COMMENT '수령 확인 날짜',
    
	PRIMARY KEY (`uid`),
    CONSTRAINT FK_user_reward_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_user_level_reward_master_reward_level_id FOREIGN KEY (`reward_id`) REFERENCES master_database.master_reward_level(`reward_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

#유저 인증
#CREATE TABLE IF NOT EXISTS user_auth (
#    uid					INT 			NOT NULL								COMMENT '유저 아이디',
#	refresh_token 		VARCHAR(512) 	NULL 		DEFAULT NULL				COMMENT '갱신 토큰',	#TODO NotNull로 교채
#	recent_refresh_dt   DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 토큰 갱신 일시'
#);
#ALTER TABLE user ADD COLUMN refresh_token VARCHAR(512) NULL DEFAULT NULL AFTER service;

#유저의 프로필
#CREATE TABLE IF NOT EXISTS user_profile (
#    uid					INT 			NOT NULL								COMMENT '유저 아이디',
#    nickname			VARCHAR(10) 	NULL 		DEFAULT NULL 				COMMENT '닉네임',
#	recent_update_dt   	DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 프로필 업데이트 일시'
#);

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
#CALL create_dummy;

SELECT * FROM user;
SELECT * FROM user_metadata;
SELECT * FROM user_skill;

UPDATE User SET nickname=null WHERE uid=11