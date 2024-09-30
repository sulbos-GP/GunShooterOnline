USE game_database;

#유저 정보
CREATE TABLE IF NOT EXISTS user (
    uid 				INT 			NOT NULL 	AUTO_INCREMENT 				COMMENT '유저 아이디',
    player_id 			VARCHAR(256) 	NOT NULL 								COMMENT '플레이어 아이디',
    nickname			VARCHAR(10) 	NULL 		DEFAULT NULL 				COMMENT '닉네임',
	experience 			INT 			NOT NULL	DEFAULT 100					COMMENT '경험치',
	money 				INT 			NOT NULL	DEFAULT 0					COMMENT '돈',
	ticket 				INT 			NOT NULL	DEFAULT 0					COMMENT '티켓',
	gacha	 			INT 			NOT NULL	DEFAULT 0					COMMENT '가챠',
    service 			VARCHAR(32) 	NOT NULL								COMMENT '서비스',
    refresh_token 		VARCHAR(512) 	NULL 		DEFAULT NULL				COMMENT '갱신 토큰',	#TODO NotNull로 교채
	create_dt           DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '생성 일시',
    recent_login_dt     DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 로그인 일시',
    recent_ticket_dt 	DATETIME 		NOT NULL 	DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 티켓 사용 일시',
    
	 PRIMARY KEY (uid),
     UNIQUE KEY (nickname)
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