USE game_database;

DELIMITER $$ 
DROP PROCEDURE IF EXISTS create_dummy $$
CREATE PROCEDURE create_dummy()
BEGIN
	DECLARE counter INT DEFAULT 1;
    DECLARE max_count INT DEFAULT 10;
    DECLARE new_id VARCHAR(50);
    DECLARE new_uid INT;
    DECLARE new_rating DOUBLE;
    DECLARE new_deviation DOUBLE;
	DECLARE new_volatility DOUBLE;
    
    DECLARE new_storage_id INT;
	DECLARE new_attributes_id INT;
    
    DELETE FROM user_metadata;
    DELETE FROM user_skill;
    
	DELETE FROM storage;
    
	DELETE FROM user;
        
    WHILE counter <= max_count DO
        SET new_id = CONCAT('a_', counter);
        
        #유저 정보
        INSERT INTO user(player_id, nickname, service) VALUES(new_id, new_id, 'Google');
        SET new_uid = LAST_INSERT_ID();
        
		#유저 메타데이터
        INSERT INTO user_metadata(uid) VALUES(new_uid);
        
        #유저 스킬
        SET new_rating = 1400.0 + (RAND() * 200.0);
        SET new_deviation = 200.0 + (RAND() * 150.0);
		SET new_volatility = 0.05 + (RAND() * 0.02);
        INSERT INTO user_skill(uid, rating, deviation, volatility) VALUES(new_uid, new_rating, new_deviation, new_volatility);
        
		#유저 인벤토리 (임시)
        INSERT INTO storage (uid, storage_type) VALUES (null, 'backpack');
		SET new_storage_id = LAST_INSERT_ID();
                
        INSERT INTO unit_attributes(item_id, unit_storage_id, amount) VALUES(302, new_storage_id, 1);
		SET new_attributes_id = LAST_INSERT_ID();
        
		INSERT INTO gear (uid, part, unit_attributes_id) VALUES(new_uid, 'backpack', new_attributes_id);
        
        SET counter = counter + 1;
	END WHILE;
END $$
DELIMITER ;