USE game_database;

SET @uid = 12;

#INSERT INTO user_level_reward (uid, reward_id) VALUES (@uid, 10001);

SELECT * FROM user_level_reward WHERE uid=@uid;