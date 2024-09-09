USE master_database;

DROP TABLE IF EXISTS master_item_base;

#아이템 정보
CREATE TABLE IF NOT EXISTS master_item_base (
    item_id						INT 			NOT NULL	COMMENT '아이템 아이디',
    code						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 코드',
	name 						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 이름',
	weight 						DOUBLE 			NOT NULL 	COMMENT '아이템 무게',
	type						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 타입',
	description 				INT 			NOT NULL 	COMMENT '아이템 설명',
	scale_x 					INT 			NOT NULL 	COMMENT '아이템 가로 크기',
	scale_y 					INT				NOT NULL 	COMMENT '아이템 세로 크기',
    purchase_price 				INT 			NOT NULL 	COMMENT '아이템 구매 가격',
	inquiry_time 				DOUBLE 			NOT NULL 	COMMENT '아이템 조회 시간',
    sell_price 					INT 			NOT NULL 	COMMENT '아이템 판매 가격',
    amount						INT				NOT NULL	COMMENT '수량',
	#loot_xp 					INT 			NOT NULL 	COMMENT '획득 경험치',
	#exam_xp 					INT 			NOT NULL 	COMMENT '조사 경험치',
    icon						VARCHAR(50) 	NOT NULL 	COMMENT '아이템 아이콘 경로',
        
    PRIMARY KEY (item_id),
	UNIQUE KEY (code)
);


#아이템 - 가방
CREATE TABLE IF NOT EXISTS master_item_backpack (
    item_id						INT 			NOT NULL	COMMENT '아이템 아이디',
    total_scale_x				INT				NOT NULL	COMMENT '가방 x크기',
	total_scale_y				INT				NOT NULL	COMMENT '가방 y크기',
	total_weight				INT				NOT NULL	COMMENT '가방 무게',
        
    PRIMARY KEY (item_id),
	CONSTRAINT FK_master_item_backpack_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

#아이템 - 사용
CREATE TABLE IF NOT EXISTS master_item_use (
    item_id						INT 			NOT NULL	COMMENT '아이템 아이디',
    power				INT				NOT NULL	COMMENT '가방 x크기',
	active_time				INT				NOT NULL	COMMENT '가방 y크기',
	duration				INT				NOT NULL	COMMENT '가방 무게',
	effect				INT				NOT NULL	COMMENT '가방 무게',
	cool_time				INT				NOT NULL	COMMENT '가방 무게',
    
    PRIMARY KEY (item_id),
	CONSTRAINT FK_master_item_backpack_item_id_master_item_base_item_id FOREIGN KEY (`item_id`) REFERENCES master_item_base(`item_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
);