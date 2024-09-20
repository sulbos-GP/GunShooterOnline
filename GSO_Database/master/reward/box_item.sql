USE master_database;

CREATE TABLE IF NOT EXISTS master_reward_box_item
(
	reward_box_item_id			INT         		NOT NULL				DEFAULT 0				COMMENT '보상 박스 아이템 아이디', 
    reward_box_id             	INT         		NOT NULL    			DEFAULT 0				COMMENT '보상 박스 아이디', 
	item_code             		VARCHAR(50)        	NOT NULL    			DEFAULT ''				COMMENT '아이템 코드', 
    x							INT         		NOT NULL    			DEFAULT 0				COMMENT '아이템 x 위치', 
	y							INT         		NOT NULL    			DEFAULT 0				COMMENT '아이템 y 위치',
	rotation             		INT         		NOT NULL    			DEFAULT 0				COMMENT '아이템 회전', 
	amount             			INT         		NOT NULL    			DEFAULT 0				COMMENT '아이템 수량', 
        
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
