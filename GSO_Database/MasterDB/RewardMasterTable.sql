USE master_database;

CREATE TABLE IF NOT EXISTS reward_base
(

);

#레벨 보상
CREATE TABLE IF NOT EXISTS reward_level
(
    reward_id             		INT         NOT NULL    COMMENT '앱 버전', 
	level             			INT         NOT NULL    COMMENT '신규 기능', 
	patch            			INT         NOT NULL    COMMENT '버그 수정'
);
