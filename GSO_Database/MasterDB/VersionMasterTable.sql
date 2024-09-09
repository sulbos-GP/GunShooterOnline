USE master_database;

DROP TABLE IF EXISTS app_version;
DROP TABLE IF EXISTS data_version;

#앱 버전
CREATE TABLE IF NOT EXISTS app_version
(
    major            			INT         NOT NULL    COMMENT '앱 버전', 
	minor            			INT         NOT NULL    COMMENT '신규 기능', 
	patch            			INT         NOT NULL    COMMENT '버그 수정',
    
    UNIQUE(major, minor, patch)
);
INSERT INTO app_version (major, minor, patch) VALUES (1, 0, 0);

#데이터 버전
CREATE TABLE IF NOT EXISTS data_version
(
    major            			INT         NOT NULL    COMMENT '데이터 버전', 
	minor            			INT         NOT NULL    COMMENT '신규 데이터', 
	patch            			INT         NOT NULL    COMMENT '데이터 수정',
    
	UNIQUE(major, minor, patch)
);
INSERT INTO data_version (major, minor, patch) VALUES (1, 0, 0);