USE game_database;

DELIMITER $$ 
DROP PROCEDURE IF EXISTS create_dummy $$
CREATE PROCEDURE create_dummy()
BEGIN
	DECLARE counter INT DEFAULT 1;
    DECLARE max_count INT DEFAULT 100;
    DECLARE new_id VARCHAR(50);
    DECLARE new_uid INT;
    DECLARE new_rating DOUBLE;
    DECLARE new_deviation DOUBLE;
	DECLARE new_volatility DOUBLE;
        
    WHILE counter <= max_count DO
        SET new_id = CONCAT('a_', counter);
        
        INSERT INTO user(player_id, nickname, service) VALUES(new_id, new_id, 'Google');
        SET new_uid = LAST_INSERT_ID();
        
        INSERT INTO user_metadata(uid) VALUES(new_uid);
        
        SET new_rating = 1400.0 + (RAND() * 200.0);
        SET new_deviation = 200.0 + (RAND() * 150.0);
		SET new_volatility = 0.05 + (RAND() * 0.02);
        INSERT INTO user_skill(uid, rating, deviation, volatility) VALUES(new_uid, new_rating, new_deviation, new_volatility);
        
        SET counter = counter + 1;
	END WHILE;
END $$
DELIMITER ;