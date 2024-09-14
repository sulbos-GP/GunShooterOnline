USE master_database;

CREATE TABLE IF NOT EXISTS master_reward_box
(
    reward_box_id             	INT         		NOT NULL    				COMMENT '보상 박스 아이디', 
    box_scale_x					INT         		NOT NULL    				COMMENT '박스 x크기', 
	box_scale_y					INT         		NOT NULL    				COMMENT '박스 y크기', 
	
	PRIMARY KEY (reward_box_id)
);

INSERT INTO master_reward_box (reward_box_id, box_scale_x, box_scale_y) VALUES
(1, 2, 2),
(2, 2, 3),
(3, 1, 0),
(4, 4, 2),
(5, 4, 3);

CREATE TABLE IF NOT EXISTS master_reward_box_item
(
	reward_box_item_id			INT         		NOT NULL    				COMMENT '보상 박스 아이템 아이디', 
    reward_box_id             	INT         		NOT NULL    				COMMENT '보상 박스 아이디', 
	item_code             		VARCHAR(50)        	NOT NULL    				COMMENT '아이템 코드', 
    x							INT         		NOT NULL    				COMMENT '아이템 x 위치', 
	y							INT         		NOT NULL    				COMMENT '아이템 y 위치',
	rotation             		INT         		NOT NULL    				COMMENT '아이템 회전', 
	amount             			INT         		NOT NULL    				COMMENT '아이템 수량', 
        
	PRIMARY KEY (reward_box_item_id),
	CONSTRAINT FK_master_reward_box_item_master_reward_box_id FOREIGN KEY (`reward_box_id`) REFERENCES master_reward_box (`reward_box_id`) ON DELETE CASCADE ON UPDATE RESTRICT,
	CONSTRAINT FK_master_reward_box_item_master_item_base_id FOREIGN KEY (`item_code`) REFERENCES master_item_base (`code`) ON DELETE CASCADE ON UPDATE RESTRICT
);

INSERT INTO master_reward_box_item (reward_box_item_id, reward_box_id, item_code, x, y, rotation, amount) VALUES
(1, 1, 'ITEM_W001', 0, 0, 0, 1),
(2, 2, 'ITEM_D003', 0, 0, 0, 1),
(3, 3, 'ITEM_R002', 0, 0, 0, 2),
(4, 3, 'ITEM_R003', 1, 0, 0, 2),
(5, 4, 'ITEM_W002', 0, 0, 0, 1),
(6, 5, 'ITEM_D001', 0, 0, 0, 1),
(7, 5, 'ITEM_B001', 2, 0, 0, 1);

CREATE TABLE IF NOT EXISTS master_reward_base
(
    reward_id             		INT         		NOT NULL    				COMMENT '보상 아이디', 
    money             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '돈',     
	ticket             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '티켓', 
	gacha             			INT         		NOT NULL   	DEFAULT 0 		COMMENT '가챠',
	experience					INT					NOT NULL	DEFAULT 0		COMMENT '경험치',
	reward_box_id				INT					NULL		DEFAULT NULL	COMMENT '보상 박스 아이디',
    
	PRIMARY KEY (reward_id),
	CONSTRAINT FK_reward_box_id_master_reward_box_id FOREIGN KEY (`reward_box_id`) REFERENCES master_reward_box (`reward_box_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

INSERT INTO master_reward_base (reward_id, money, ticket, gacha, experience, reward_box_id) VALUES
(10001, 1000, 0, 0, 0, NULL),
(10002, 0, 2, 0, 0, NULL),
(10003, 0, 0, 5, 0, NULL),
(10004, 2000, 0, 0, 0, NULL),
(10005, 0, 4, 0, 0, NULL),
(10006, 0, 0, 10, 0, NULL),
(10007, 3000, 0, 0, 0, NULL),
(10008, 0, 6, 0, 0, NULL),
(10009, 0, 0, 15, 0, NULL),
(20001, 1000, 1, 0, 500, NULL),
(20002, 0, 0, 0, 1200, 1),
(20003, 500, 0, 0, 500, NULL),
(20004, 0, 0, 0, 2000, 2),
(30001, 500, 0, 0, 500, NULL),
(30002, 0, 0, 0, 500, 3),
(30003, 2000, 0, 0, 0, NULL),
(30004, 0, 0, 0, 2000, NULL),
(40001, 0, 1, 1, 1000, 4),
(40002, 0, 1, 1, 1000, 5);

#레벨 보상
CREATE TABLE IF NOT EXISTS master_reward_level
(
    reward_id					INT					NOT NULL					COMMENT '보상 정보',
	level             			INT         		NOT NULL    				COMMENT '레벨',
	name 						VARCHAR(50) 		NOT NULL 					COMMENT '보상 이름',
	icon						VARCHAR(50) 		NOT NULL 					COMMENT '보상 아이콘',
        
    PRIMARY KEY (reward_id),
    UNIQUE (level),
	CONSTRAINT FK_master_reward_level_reward_id_master_reward_base_reward_id FOREIGN KEY (`reward_id`) REFERENCES master_reward_base (`reward_id`) ON DELETE CASCADE ON UPDATE RESTRICT
);

INSERT INTO master_reward_level (reward_id, level, name, icon) VALUES
(10001, 2, '1000 골드', 'IconS_GoldOre'),
(10002, 3, '연료 2개', 'IconR_FuelTank'),
(10003, 4, '건전지 5개', 'IconS_battery'),
(10004, 5, '2000 골드', 'IconS_GoldOre'),
(10005, 6, '연료 4개', 'IconR_FuelTank'),
(10006, 7, '건전지 10개', 'IconS_battery'),
(10007, 8, '3000 골드', 'IconS_GoldOre'),
(10008, 9, '연료 6개', 'IconR_FuelTank'),
(10009, 10, '건전지 15개', 'IconS_battery');

