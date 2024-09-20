USE master_database;

#데이터 버전
CREATE TABLE IF NOT EXISTS master_version_data
(
    major            			INT         		NOT NULL    			DEFAULT 0				COMMENT '데이터 버전', 
	minor            			INT         		NOT NULL    			DEFAULT 0				COMMENT '신규 데이터', 
	patch            			INT         		NOT NULL    			DEFAULT 0				COMMENT '데이터 수정',

	UNIQUE(major, minor, patch)
);