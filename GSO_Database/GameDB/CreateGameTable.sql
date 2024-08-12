USE game_database;

DROP TABLE IF EXISTS user_metadata;
DROP TABLE IF EXISTS user_skill;

DROP TABLE IF EXISTS user_gear;

DROP TABLE IF EXISTS user_container;
DROP TABLE IF EXISTS user_container_slot;

DROP TABLE IF EXISTS user;

#유저 정보
CREATE TABLE IF NOT EXISTS user (
    uid 				INT 			NOT NULL 	AUTO_INCREMENT 				COMMENT '유저 아이디',
    player_id 			VARCHAR(256) 	NOT NULL 								COMMENT '플레이어 아이디',
    service 			VARCHAR(32) 	NOT NULL								COMMENT '서비스',
	create_dt           DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '생성 일시', 
    recent_login_dt     DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 로그인 일시',
    
	 PRIMARY KEY (uid),
     UNIQUE KEY (nickname)
);

#유저 인증
CREATE TABLE IF NOT EXISTS user_auth (
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
	refresh_token 		VARCHAR(512) 	NULL 		DEFAULT NULL				COMMENT '갱신 토큰',	#TODO NotNull로 교채
	recent_refresh_dt   DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 토큰 갱신 일시'
);
#ALTER TABLE user ADD COLUMN refresh_token VARCHAR(512) NULL DEFAULT NULL AFTER service;

#유저의 프로필
CREATE TABLE IF NOT EXISTS user_profile (
    uid					INT 			NOT NULL								COMMENT '유저 아이디',
    nickname			VARCHAR(10) 	NULL 		DEFAULT NULL 				COMMENT '닉네임',
	recent_update_dt   	DATETIME       	NOT NULL    DEFAULT CURRENT_TIMESTAMP 	COMMENT '최근 프로필 업데이트 일시'
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

#유저의 무장(장비)
CREATE TABLE IF NOT EXISTS user_gear (
    uid				INT 				NOT NULL								COMMENT '유저 아이디',
    main_weapon		INT 				NULL 		DEFAULT NULL				COMMENT '메인 무기',
    sub_weapon		INT 				NULL 		DEFAULT NULL				COMMENT '서브 무기',
	armor			INT 				NULL 		DEFAULT NULL				COMMENT '보호구',
	backpack		INT 				NULL 		DEFAULT NULL				COMMENT '가방',
    pocket_first	INT 				NULL 		DEFAULT NULL				COMMENT '첫번째 주머니',
	pocket_second	INT 				NULL 		DEFAULT NULL				COMMENT '두번째 주머니',
	pocket_third	INT 				NULL 		DEFAULT NULL				COMMENT '세번째 주머니',
    
    PRIMARY KEY (`uid`),
    CONSTRAINT FK_user_gear_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#유저의 컨테이너
CREATE TABLE IF NOT EXISTS user_container (
    container_id		INT 							NOT NULL	AUTO_INCREMENT				COMMENT '컨테이너 아이디',
	container_item_id	INT 							NOT NULL								COMMENT '컨테이너 아이템 아이디',
    uid					INT 							NOT NULL								COMMENT '유저 아이디',
    container_type  	ENUM('backpack', 'cabinet') 	NOT NULL 								COMMENT '아이템 위치 (가방/캐비넷)',

    PRIMARY KEY (`container_id`),
    CONSTRAINT FK_user_container_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_user_container_container_item_id_master_item_base_item_id FOREIGN KEY (`container_item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#유저의 컨테이너 슬롯
CREATE TABLE IF NOT EXISTS user_container_slot (
    container_slot_id	INT 							NOT NULL	AUTO_INCREMENT				COMMENT '컨테이너 슬롯 아이디',
    container_id    	INT                 			NOT NULL        						COMMENT '컨테이너 아이디 (가방 ID 또는 캐비넷 ID)',
	item_id				INT 							NOT NULL 								COMMENT '아이템 아이디',
    grid_x				INT 							NOT NULL 								COMMENT '아이템 x위치',
	grid_y				INT 							NOT NULL 								COMMENT '아이템 y위치',
	rotation			INT 							NOT NULL 								COMMENT '아이템 회전',
	stack_count			INT								NOT NULL								COMMENT '아이템 스택 카운터',

    PRIMARY KEY (`container_slot_id`),
    CONSTRAINT FK_user_container_slot_container_id_user_container_container_id FOREIGN KEY (`container_id`) REFERENCES user_container (`container_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
	CONSTRAINT FK_user_container_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_database.master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#더미 생성 (CreateDummyProcedures 참고)
CALL create_dummy;

SELECT * FROM user;
SELECT * FROM user_metadata;
SELECT * FROM user_skill;
SELECT * FROM user_container;
SELECT * FROM user_container_slot;
