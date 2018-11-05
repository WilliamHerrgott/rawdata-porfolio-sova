-- ----------------------------
-- Function for creating a new user
-- It returns the id of the newly created
-- user or -1 if the insertion fails
-- ----------------------------
DROP FUNCTION IF EXISTS create_user;
CREATE FUNCTION create_user(this_email varchar(50), this_username varchar(50), this_password varchar(50), this_location varchar(50))
RETURNS TABLE (id integer, email text, username text, password text, location text) AS $$ 
	BEGIN
		IF(this_email NOT IN(SELECT email FROM "SOVA_users") and this_username NOT IN(SELECT username FROM "SOVA_users")) THEN
		INSERT INTO "SOVA_users"(email, username, password, location) VALUES (this_email, this_username, this_password, this_location);
		RETURN QUERY
			SELECT *
			FROM "SOVA_users" u
			WHERE u.username = this_username and u.password = this_password;
		END IF;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting a user.
-- It calls a trigger to delete 
-- all the records of marking and
-- history of that user.
-- ----------------------------

DROP FUNCTION IF EXISTS delete_user;
CREATE FUNCTION delete_user(this_user_id integer)
RETURNS BOOLEAN AS $$ 
	BEGIN
		IF(this_user_id  IN(SELECT ID FROM "SOVA_users")) THEN
			DELETE FROM marks m
			WHERE m.user_id = this_user_id;
			
			DELETE FROM history h
			WHERE h.user_id = this_user_id;
		
			DELETE FROM "SOVA_users"
			WHERE id = this_user_id;
			IF(this_user_id  IN(SELECT id FROM "SOVA_users")) THEN 
					RETURN FALSE;
			ELSIF(this_user_id  IN(SELECT user_id FROM history)) THEN 
					RETURN FALSE;			
			ELSIF(this_user_id  IN(SELECT user_id FROM marks)) THEN 
					RETURN FALSE;		
			ELSE RETURN TRUE;
			END IF;
		ELSE
			RETURN FALSE;
		END IF;
		END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for updating the email of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_email;
CREATE FUNCTION update_email(this_user_id integer, var_email varchar(50))
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id  IN(SELECT ID FROM "SOVA_users")) THEN 
		UPDATE "SOVA_users" s
		SET email = var_email
		WHERE s.id = this_user_id;
		RETURN TRUE;
	ELSE
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for login a user 
-- ----------------------------
DROP FUNCTION IF EXISTS get_user;
CREATE FUNCTION get_user(login varchar, password_var varchar)
RETURNS TABLE (id integer, email text, username text, password text, location text) AS $$
BEGIN	RETURN QUERY
	SELECT *
	FROM "SOVA_users" u
	WHERE u.username = login and u.password = password_var;
	END; $$			
LANGUAGE plpgsql;

-- ----------------------------
-- Function for updating the username of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_username;
CREATE FUNCTION update_username(this_user_id integer, var_username varchar(50))
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id  IN(SELECT ID FROM "SOVA_users")) THEN 
		UPDATE "SOVA_users" s
		SET username = var_username
		WHERE s.id = this_user_id;
		RETURN TRUE;
	ELSE 
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for updating the password of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_password;
CREATE FUNCTION update_password(this_user_id integer, var_password varchar(20))
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id  IN(SELECT ID FROM "SOVA_users")) THEN 
		UPDATE "SOVA_users" s
		SET password = var_password
		WHERE s.id = this_user_id;
		RETURN TRUE;
	ELSE 	
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;
	
-- ----------------------------
-- Function for updating the location of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_location;
CREATE FUNCTION update_location(this_user_id INTEGER, var_location VARCHAR(20))
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id  IN(SELECT ID FROM "SOVA_users")) THEN 
		UPDATE "SOVA_users" s
		SET location = var_location
		WHERE s.id = this_user_id;
		RETURN TRUE;
	ELSE
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting body, score and creation_date
-- of a post by specified by post id
-- ----------------------------

DROP FUNCTION IF EXISTS get_post;
CREATE FUNCTION get_post(this_post_id integer)
RETURNS table (id integer, body text, score integer, creation_date timestamp, author_id integer) AS $$
BEGIN		
	RETURN QUERY
		SELECT p.id, p.body, p.score, p.creation_date, p.author_id
		FROM posts p
		WHERE p.id = this_post_id;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting all answers to a question
-- ----------------------------

DROP FUNCTION IF EXISTS get_answers;
CREATE FUNCTION get_answers(this_question_id integer)
RETURNS table (id integer, body text, score integer, creation_date timestamp, author_id integer) AS $$
BEGIN		
	RETURN QUERY
		SELECT p.id, p.body, p.score, p.creation_date, p.author_id
		FROM posts p, answers a
		WHERE this_question_id = a.parent_id 
		AND a.id = p.id
		ORDER BY creation_date desc;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting all the comments to a post 
-- ----------------------------

DROP FUNCTION IF EXISTS get_comments;
CREATE FUNCTION get_comments(this_post_id integer)
RETURNS table (id integer, body text, score integer, creation_date timestamp, author_id integer) AS $$
BEGIN	
	RETURN QUERY
		SELECT c.id, c.body, c.score, c.creation_date, c.author_id
		FROM comments c
		WHERE c.post_id = this_post_id
		ORDER BY creation_date desc;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for making a new marking
-- ----------------------------

DROP FUNCTION IF EXISTS mark;
CREATE FUNCTION mark(this_user_id integer, this_post_id integer)
RETURNS BOOLEAN AS $$
BEGIN
	IF (this_user_id IN (SELECT id FROM "SOVA_users")) THEN
		INSERT INTO marks VALUES (this_user_id, this_post_id, date_trunc('second', LOCALTIMESTAMP), null, null);
		RETURN TRUE;
	ELSE 
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting a marked post
-- ----------------------------

DROP FUNCTION IF EXISTS delete_mark;
CREATE FUNCTION delete_mark(this_user_id integer, this_post_id integer DEFAULT NULL)
RETURNS BOOLEAN AS $$
BEGIN
		IF this_post_id IS NULL AND this_user_id IN(SELECT USER_ID FROM marks) THEN
			DELETE FROM marks m
			WHERE m.user_id = this_user_id;
			RETURN TRUE;
		ELSIF this_post_id IS NOT NULL AND this_user_id IN(SELECT USER_ID FROM marks) THEN
			DELETE FROM marks
			WHERE user_id = this_user_id
			AND post_id = this_post_id;
			RETURN TRUE;
		ELSE 
			RETURN FALSE;
		END IF;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for making a new annotation
-- ----------------------------

DROP FUNCTION IF EXISTS make_annotation;
CREATE FUNCTION make_annotation(this_user_id integer, this_post_id integer, new_text text)
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id IN (SELECT ID FROM "SOVA_users") AND this_user_id IN (SELECT USER_ID FROM MARKS)) THEN
		UPDATE marks m 
		SET text_annotation = new_text, annotation_creationdate = date_trunc('second', LOCALTIMESTAMP)
		WHERE m.user_id = this_user_id 
		AND m.post_id = this_post_id;
		RETURN TRUE;
	ELSE
		RETURN FALSE;
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting an annotation
-- ----------------------------

DROP FUNCTION IF EXISTS delete_annotation;
CREATE FUNCTION delete_annotation(this_user_id integer, this_post_id integer)
RETURNS BOOLEAN AS $$
BEGIN
	IF(this_user_id IN (SELECT ID FROM "SOVA_users") AND this_user_id IN (SELECT USER_ID FROM MARKS)) THEN
		UPDATE marks
		SET text_annotation = NULL, annotation_creationdate = NULL
		WHERE user_id = this_user_id
		AND post_id = this_post_id;
		RETURN TRUE;
	ELSE
		RETURN FALSE;
	END IF;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting posts marked by a user 
-- ----------------------------

DROP FUNCTION IF EXISTS get_marked;
CREATE FUNCTION get_marked(this_user_id integer)
RETURNS table (user_id integer, post_id integer, marked_creationdate timestamp, annotation_creationdate timestamp, text_annotation text) AS $$
BEGIN		
	RETURN QUERY
	SELECT *
	FROM marks m
	WHERE m.user_id = this_user_id
	ORDER BY marked_creationdate desc;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting the searches of a user,
-- 10 at a time starting from the most recent one
-- ----------------------------

DROP FUNCTION IF EXISTS get_history;
CREATE FUNCTION get_history(id_user integer)
	RETURNS TABLE(id integer, search_text varchar(255), date timestamp, user_id integer) 
	AS $$
		BEGIN
		IF id_user IN (SELECT "SOVA_users".id FROM "SOVA_users") THEN
			RETURN QUERY
				SELECT h.id, h.search_text, h.date, h.user_id
				FROM history h
				WHERE h.user_id = id_user 
				ORDER BY date desc;
		END IF;
	END;	$$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting the id of a post which contains a given phrase
-- typed by the user and adding this search to his history.
-- ----------------------------

DROP FUNCTION IF EXISTS search_sova;
CREATE FUNCTION search_sova(this_searched_text varchar(50), this_user_id integer)
RETURNS TABLE (id integer, body text) AS $$
BEGIN
	RETURN QUERY
		SELECT p.id, p.body
		FROM posts p
			JOIN questions q ON p.id = q.id
		WHERE to_tsvector(q.title || '. ' || p.body || replace(q.tags, '::', ' ')) @@ plainto_tsquery(this_searched_text);
		

		INSERT INTO history (search_text, date, user_id)
		VALUES(this_searched_text, date_trunc('second', LOCALTIMESTAMP), this_user_id);
		
	
END; $$
LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS search_posts;
CREATE FUNCTION search_posts(this_searched_text varchar(50))
RETURNS TABLE (id integer, body text) AS $$
BEGIN
	RETURN QUERY
		SELECT p.id, p.body
		FROM posts p
			JOIN questions q ON p.id = q.id
		WHERE to_tsvector(q.title || '. ' || p.body || replace(q.tags, '::', ' ')) @@ plainto_tsquery(this_searched_text);

END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting history of a user 
-- from search relation. It calls a trigger 
-- that also deletes history content from 
-- history table.
-- ----------------------------


DROP FUNCTION IF EXISTS delete_history;
CREATE FUNCTION delete_history(user_id_to integer)
RETURNS BOOLEAN AS $$
	BEGIN
		IF(user_id_to IN (SELECT ID FROM "SOVA_users") AND user_id_to IN (SELECT USER_ID FROM history)) THEN
			DELETE FROM history
			WHERE history.user_id = user_id_to;
			RETURN TRUE;
		ELSE 
			RETURN FALSE;
		END IF;
	END; $$
LANGUAGE plpgsql;


DROP FUNCTION IF EXISTS get_author_of_post;
CREATE FUNCTION get_author_of_post(this_post_id integer)
RETURNS TABLE (id integer, name varchar(255), created_date timestamp, location varchar(255), age integer) AS $$
	BEGIN
	RETURN QUERY
		SELECT a.id, a.name, a.created_date, a.location, a.age
		FROM "SO_authors" a, posts p
		WHERE a.id = p.author_id
		AND p.id = this_post_id;
	END; $$
LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS get_author_of_comment;
CREATE FUNCTION get_author_of_comment(this_comment_id integer)
RETURNS TABLE (id integer, name varchar(255), created_date timestamp, location varchar(255), age integer) AS $$
	BEGIN
	RETURN QUERY
		SELECT a.id, a.name, a.created_date, a.location, a.age
		FROM "SO_authors" a, comments c
		WHERE a.id = c.author_id
		AND c.id = this_comment_id;
	END; $$
LANGUAGE plpgsql;
adsff