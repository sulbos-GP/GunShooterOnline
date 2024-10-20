USE game_database;

#저장소
CREATE TABLE IF NOT EXISTS storage (
	storage_id			INT 							NOT NULL	AUTO_INCREMENT				COMMENT '저장소 아이디',
	uid					INT 							NULL		DEFAULT NULL				COMMENT '유저 아이디',
    storage_type  		ENUM('backpack', 'cabinet') 	NOT NULL 								COMMENT '저장소 타입(가방/캐비넷)',
    
    PRIMARY KEY (`storage_id`),
    CONSTRAINT FK_storage_uid_user_uid FOREIGN KEY (`uid`) REFERENCES user (`uid`) ON DELETE RESTRICT ON UPDATE RESTRICT
);

DELETE FROM storage;
