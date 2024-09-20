USE master_database;

CREATE TABLE IF NOT EXISTS master_reward_box
(
    reward_box_id             	INT         		NOT NULL    			DEFAULT 0				COMMENT '보상 박스 아이디', 
    box_scale_x					INT         		NOT NULL    			DEFAULT 0				COMMENT '박스 x크기', 
	box_scale_y					INT         		NOT NULL    			DEFAULT 0				COMMENT '박스 y크기', 
	
	PRIMARY KEY (reward_box_id)
);

INSERT INTO master_reward_box (reward_box_id, box_scale_x, box_scale_y) VALUES
(1, 2, 2),
(2, 2, 3),
(3, 1, 0),
(4, 4, 2),
(5, 4, 3);