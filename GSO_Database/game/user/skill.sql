USE game_database;

#유저의 스킬(e.g MMR)
CREATE TABLE IF NOT EXISTS user_skill (
    uid				INT 				NOT NULL	DEFAULT 0					COMMENT '유저 아이디',
    rating			DOUBLE 				NOT NULL 	DEFAULT 1500.0				COMMENT '레이팅',
    deviation		DOUBLE 				NOT NULL 	DEFAULT 350.0				COMMENT '표준 편차',
	volatility		DOUBLE 				NOT NULL 	DEFAULT 0.06				COMMENT '변동성',
    
    PRIMARY KEY (`uid`),
    CONSTRAINT FK_user_skill_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

UPDATE user_skill SET rating=1500 WHERE rating!=1500;
UPDATE user_skill SET deviation=350 WHERE deviation!=350;
UPDATE user_skill SET volatility=0.06 WHERE volatility!=0.06;