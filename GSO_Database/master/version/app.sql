USE master_database;

#앱 버전
CREATE TABLE IF NOT EXISTS master_version_app
(
    major            			INT         		NOT NULL    			DEFAULT 0				COMMENT '앱 버전', 
	minor            			INT         		NOT NULL    			DEFAULT 0				COMMENT '신규 기능', 
	patch            			INT         		NOT NULL    			DEFAULT 0				COMMENT '버그 수정',

    UNIQUE(major, minor, patch)
);

DELETE FROM master_version_app;
INSERT INTO master_version_app (major, minor, patch) VALUES (1, 0, 0);