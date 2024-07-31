USE master_database;


DROP TABLE IF EXISTS master_item_base;

DROP TABLE IF EXISTS version;

#버전
CREATE TABLE IF NOT EXISTS version
(
    app_version            INT         NOT NULL    COMMENT '앱 버전', 
    master_data_version    INT         NOT NULL    COMMENT '마스터 데이터 버전'
);

#아이템 정보
CREATE TABLE IF NOT EXISTS master_item_base (
    item_id						INT 			NOT NULL	COMMENT '아이템 아이디',
    code						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 코드',
	name 						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 이름',
	weight 						double 			NOT NULL 	COMMENT '아이템 무게',
	type						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 타입',
	#description 				INT 			NOT NULL 	COMMENT '아이템 설명',
	scale_x 					INT 			NOT NULL 	COMMENT '아이템 가로 크기',
	scale_y 					INT				NOT NULL 	COMMENT '아이템 세로 크기',
    purchase_price 				INT 			NOT NULL 	COMMENT '아이템 구매 가격',
	inquiry_time 				DOUBLE 			NOT NULL 	COMMENT '아이템 조회 시간',
    sell_price 					INT 			NOT NULL 	COMMENT '아이템 판매 가격',
    stack_count					INT				NOT NULL	COMMENT '스택 카운터',
	#loot_xp 					INT 			NOT NULL 	COMMENT '획득 경험치',
	#exam_xp 					INT 			NOT NULL 	COMMENT '조사 경험치',
	prefab						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 프리펩 경로',
    icon						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 아이콘 경로',
        
    PRIMARY KEY (item_id),
	UNIQUE KEY (code)
);