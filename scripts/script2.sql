-- ----------------------------
-- Function for creating a new user
-- ----------------------------
DROP FUNCTION IF EXISTS create_user;
CREATE FUNCTION create_user(this_email varchar(50), this_username varchar(50), this_password varchar(50), this_location varchar(50))
RETURNS void AS $$ 
	BEGIN
		INSERT INTO "SOVA_users"(email, username, password, location) VALUES (this_email, this_username, this_password, this_location);
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting a user.
-- It calls a trigger to delete 
-- all the records of marking and
-- history of that user.
-- ----------------------------

DROP TRIGGER IF EXISTS user_deletion ON "SOVA_users";
DROP FUNCTION IF EXISTS delete_user;
CREATE FUNCTION delete_user(this_user_id integer)
RETURNS void AS $$ 
	BEGIN
		DELETE FROM "SOVA_users"
		WHERE id = this_user_id;
	END; $$
LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS users_mark_and_history_deletion;
CREATE FUNCTION users_mark_and_history_deletion()
RETURNS TRIGGER AS $$
	BEGIN
			DELETE FROM marks m
			WHERE m.user_id = old.id;
			
			DELETE FROM searched s
			WHERE s.user_id = old.id;
			
			RETURN NULL;
	END; $$
LANGUAGE plpgsql;

CREATE TRIGGER user_deletion
AFTER DELETE ON "SOVA_users"
FOR EACH ROW EXECUTE PROCEDURE users_mark_and_history_deletion();

-- ----------------------------
-- Function for updating the email of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_email;
CREATE FUNCTION update_email(this_user_id integer, var_email varchar(50))
RETURNS VOID AS $$
BEGIN
	UPDATE "SOVA_users" s
	SET email = var_email
	WHERE s.id = this_user_id;					
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for updating the username of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_username;
CREATE FUNCTION update_username(this_user_id integer, var_username varchar(50))
RETURNS VOID AS $$
BEGIN
	UPDATE "SOVA_users" s
	SET username = var_username
	WHERE s.id = this_user_id;						
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for updating the password of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_password;
CREATE FUNCTION update_password(this_user_id integer, var_password varchar(20))
RETURNS VOID AS $$
BEGIN
	UPDATE "SOVA_users" s
	SET password = var_password
	WHERE s.id = this_user_id;
				
END; $$
LANGUAGE plpgsql;
	
-- ----------------------------
-- Function for updating the location of a user 
-- ----------------------------

DROP FUNCTION IF EXISTS update_location;
CREATE FUNCTION update_location(this_user_id INTEGER, var_location VARCHAR(20))
RETURNS VOID AS $$
BEGIN
	UPDATE "SOVA_users" s
	SET location = var_location
	WHERE s.id = this_user_id;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting body, score and creation_date
-- of a post by specified by post id
-- ----------------------------

DROP FUNCTION IF EXISTS get_post;
CREATE FUNCTION get_post(this_post_id integer)
RETURNS table (body integer, score integer, creation_date timestamp) AS $$
BEGIN		
	RETURN QUERY
	SELECT p.body, p.score, p.creation_date
	FROM posts p
	WHERE p.id = this_post_id;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting all answers to a question
-- ----------------------------

DROP FUNCTION IF EXISTS get_answers;
CREATE FUNCTION get_answers(this_question_id integer)
RETURNS table (body text, score integer, creation_date timestamp) AS $$
BEGIN		
	RETURN QUERY
	SELECT p.body, p.score, p.creation_date
	FROM posts p, questions_answers qa
	WHERE this_question_id = qa.question_id 
	AND qa.answer_id = p.id
	ORDER BY creation_date desc;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting all the comments to a post 
-- ----------------------------

DROP FUNCTION IF EXISTS get_comments;
CREATE FUNCTION get_comments(this_post_id integer)
RETURNS table (body text, score integer, creation_date timestamp) AS $$
BEGIN		
	RETURN QUERY
	SELECT c.body, c.score, c.creation_date
	FROM comments c, commented_on r
	WHERE r.post_id = this_post_id
	AND r.comment_id = c.id
	ORDER BY creation_date desc;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for making a new marking
-- ----------------------------

DROP FUNCTION IF EXISTS mark;
CREATE FUNCTION mark(this_user_id integer, this_post_id integer)
RETURNS void AS $$
BEGIN
	IF (this_user_id IN (SELECT id FROM "SOVA_users")) THEN
		INSERT INTO marks VALUES (this_user_id, this_post_id, date_trunc('second', LOCALTIMESTAMP), '');
	ELSE 
		RAISE NOTICE 'User id is unknown';
	END IF;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting a marked post
-- ----------------------------

DROP FUNCTION IF EXISTS delete_mark;
CREATE FUNCTION delete_mark(this_user_id integer, this_post_id integer DEFAULT NULL)
RETURNS void AS $$
BEGIN
		IF this_post_id IS NULL THEN
			DELETE FROM marks m
			WHERE m.user_id = this_user_id;
		ELSE
			DELETE FROM marks
			WHERE user_id = this_user_id
			AND post_id = this_post_id;
		END IF;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for making a new annotation
-- ----------------------------

DROP FUNCTION IF EXISTS make_annotation;
CREATE FUNCTION make_annotation(this_user_id integer, this_post_id integer, this_text text)
RETURNS void AS $$
BEGIN
	IF (this_user_id IN (SELECT id FROM "SOVA_users")) THEN
		INSERT INTO marks VALUES (this_user_id, this_post_id, date_trunc('second', LOCALTIMESTAMP), this_text);
	ELSE 
		RAISE EXCEPTION 'User id is unknown';
	END IF;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for making changes to an annotation
-- ----------------------------

DROP FUNCTION IF EXISTS update_annotation;
CREATE FUNCTION update_annotation(this_user_id integer, this_post_id integer, new_text text)
RETURNS void AS $$
BEGIN
	UPDATE marks m 
	SET text_annotation = new_text
	WHERE m.user_id = this_user_id 
	AND m.post_id = this_post_id;
	RETURN;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting an annotation
-- ----------------------------

DROP FUNCTION IF EXISTS delete_annotation;
CREATE FUNCTION delete_annotation(this_user_id integer, this_post_id integer)
RETURNS void AS $$
BEGIN
		UPDATE marks
		SET text_annotation = ''
		WHERE user_id = this_user_id
		AND post_id = this_post_id;
	END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting posts marked by a user, 10 at a time 
-- ----------------------------

DROP FUNCTION IF EXISTS get_marked;
CREATE FUNCTION get_marked(this_user_id integer, this_offset integer)
RETURNS table (post_id integer, text_annotation text, creation_date timestamp) AS $$
BEGIN		
	RETURN QUERY
	SELECT m.post_id, m.text_annotation, m.creation_date
	FROM marks m
	WHERE m.user_id = this_post_id
	ORDER BY creation_date desc 
	LIMIT 10 OFFSET this_offset;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting the searches of a user,
-- 10 at a time starting from the most recent one
-- ----------------------------

DROP FUNCTION IF EXISTS get_history;
CREATE FUNCTION get_history(id_user integer, this_offset integer)
	RETURNS TABLE(search_text varchar(255),  date timestamp) 
	AS $$
		BEGIN
		RETURN QUERY
			SELECT h.search_text, h.date
			FROM history h, searched s
			WHERE s.user_id = id_user 
			AND s.history_id = s.id
			ORDER BY creation_date desc 
			LIMIT 10 OFFSET this_offset;
	END;	$$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for getting the id of a post which contains a given phrase
-- typed by the user and adding this search to his history.
-- ----------------------------

DROP FUNCTION IF EXISTS search_sova;
CREATE FUNCTION search_sova(this_searched_text varchar(50), this_user_id integer)
RETURNS TABLE (id integer) AS $$
BEGIN
	RETURN QUERY
		SELECT q.id
		FROM questions q
			JOIN posts p ON p.id = q.id
		WHERE to_tsvector(q.title || '. ' || p.body || replace(q.tags, '::', ' ')) @@ plainto_tsquery(this_searched_text);
		
	WITH x AS (
		INSERT INTO history (search_text, date)
		VALUES(this_searched_text, date_trunc('second', LOCALTIMESTAMP))
		RETURNING history.id
	)
	INSERT INTO searched (history_id, user_id)
	SELECT X.id, this_user_id
	FROM X;
END; $$
LANGUAGE plpgsql;

-- ----------------------------
-- Function for deleting history of a user 
-- from search relation. It calls a trigger 
-- that also deletes history content from 
-- history table.
-- ----------------------------

DROP TRIGGER IF EXISTS delete_history_content ON searched;
DROP FUNCTION IF EXISTS delete_history;
CREATE FUNCTION delete_history(user_id_to integer)
RETURNS void AS $$
	BEGIN
		DELETE FROM searched 
		WHERE searched.user_id = user_id_to;
		RETURN;
	END; $$
LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS delete_history_content;
CREATE FUNCTION delete_history_content()
RETURNS TRIGGER AS $$
	BEGIN
		DELETE FROM history s 
		WHERE s.id = old.history_id;
		RETURN NULL;
	END; $$
LANGUAGE plpgsql;

CREATE TRIGGER delete_history_content
   AFTER DELETE ON searched FOR EACH ROW
   EXECUTE PROCEDURE delete_history_content();