USE master_database;

#아이템 정보
CREATE TABLE IF NOT EXISTS master_item_base (
    item_id						INT 				NOT NULL				DEFAULT 0				COMMENT '아이템 아이디',
    code						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '아이템 코드',
	name 						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '아이템 이름',
	weight 						DOUBLE 				NOT NULL 				DEFAULT 0				COMMENT '아이템 무게',
	type						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '아이템 타입',
	description 				INT 				NOT NULL 				DEFAULT 0				COMMENT '아이템 설명',
	scale_x 					INT 				NOT NULL 				DEFAULT 0				COMMENT '아이템 가로 크기',
	scale_y 					INT					NOT NULL 				DEFAULT 0				COMMENT '아이템 세로 크기',
    purchase_price 				INT 				NOT NULL 				DEFAULT 0				COMMENT '아이템 구매 가격',
	inquiry_time 				DOUBLE 				NOT NULL 				DEFAULT 0				COMMENT '아이템 조회 시간',
    sell_price 					INT 				NOT NULL 				DEFAULT 0				COMMENT '아이템 판매 가격',
    amount						INT					NOT NULL				DEFAULT 0				COMMENT '수량',
    icon						VARCHAR(50) 		NOT NULL 				DEFAULT ''				COMMENT '아이템 아이콘 경로',

    PRIMARY KEY (item_id),
	UNIQUE KEY (code)
);

#loot_xp 					INT 			NOT NULL 	COMMENT '획득 경험치',
#exam_xp 					INT 			NOT NULL 	COMMENT '조사 경험치',