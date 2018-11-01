/*
 Date: 27/09/2018 17:06:24
*/


-- ----------------------------
-- Table structure for SOVA_users
-- ----------------------------
DROP TABLE IF EXISTS "SOVA_users" CASCADE;
CREATE TABLE "SOVA_users" (
  id SERIAL,
	email varchar(50) NOT NULL, 
  username varchar(50) NOT NULL,
  password varchar(50) NOT NULL,
  location varchar(50)
)
;


-- ----------------------------
-- Table structure for SO_authors
-- ----------------------------
DROP TABLE IF EXISTS "SO_authors" CASCADE;
CREATE TABLE "SO_authors" (
  id integer NOT NULL,
  name varchar(255),
  created_date timestamp,
  location varchar(255),
  age integer
)
;


-- ----------------------------
-- Table structure for answers
-- ----------------------------
DROP TABLE IF EXISTS answers CASCADE;
CREATE TABLE answers(
  id integer NOT NULL,
	parent_id integer
)
;

-- ----------------------------
-- Table structure for comments
-- ----------------------------
DROP TABLE IF EXISTS comments CASCADE;
CREATE TABLE comments (
  id integer NOT NULL,
  score integer,
  body text, 
  creation_date timestamp,
	author_id integer,
	post_id integer
)
;


-- ----------------------------
-- Table structure for history
-- ----------------------------
DROP TABLE IF EXISTS history CASCADE;
CREATE TABLE history (
  id SERIAL,
  search_text varchar(255),
  date timestamp,
	user_id integer
)
;


-- ----------------------------
-- Table structure for marks
-- ----------------------------
DROP TABLE IF EXISTS marks;
CREATE TABLE marks (
  user_id integer NOT NULL,
  post_id integer NOT NULL,
  marked_creationdate timestamp NOT NULL,
	annotation_creationdate timestamp,
  text_annotation text
)
;


-- ----------------------------
-- Table structure for posts
-- ----------------------------
DROP TABLE IF EXISTS posts CASCADE;
CREATE TABLE posts (
  id integer NOT NULL,
  creation_date timestamp,
  body text,
  score integer,
	author_id integer
)
;


-- ----------------------------
-- Table structure for questions
-- ----------------------------
DROP TABLE IF EXISTS questions CASCADE;
CREATE TABLE questions (
  id integer NOT NULL,
	accepted_answer_id integer,
  closed_date timestamp,
  title varchar(255),
  tags varchar(255)
)
;

-- ----------------------------
-- Table structure for linked
-- ----------------------------
DROP TABLE IF EXISTS linked;
CREATE TABLE linked (
  question_id integer,
  linkpost_id integer
)
;

-- ----------------------------
-- Primary Key structure for table SOVA_users
-- ----------------------------
ALTER TABLE "SOVA_users" ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table SO_authors
-- ----------------------------
ALTER TABLE "SO_authors" ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table answers
-- ----------------------------
ALTER TABLE answers ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table comments
-- ----------------------------
ALTER TABLE comments ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table history
-- ----------------------------
ALTER TABLE history ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table posts
-- ----------------------------
ALTER TABLE posts ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table questions
-- ----------------------------
ALTER TABLE questions ADD PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table marks
-- ----------------------------
ALTER TABLE marks ADD PRIMARY KEY (user_id, post_id);

-- ----------------------------
-- Primary Key structure for table linked
-- ----------------------------
ALTER TABLE linked ADD PRIMARY KEY (question_id, linkpost_id);

-- ----------------------------
-- Foreign Key structure for table answers
-- ----------------------------
ALTER TABLE answers
ADD FOREIGN KEY (id) REFERENCES posts(id),
ADD FOREIGN KEY (parent_id) REFERENCES questions(id);

-- ----------------------------
-- Foreign Key structure for table comments
-- ----------------------------
ALTER TABLE comments
ADD FOREIGN KEY (author_id) REFERENCES "SO_authors"(id),
ADD FOREIGN KEY (post_id) REFERENCES posts(id);

-- ----------------------------
-- Foreign Key structure for table history
-- ----------------------------
ALTER TABLE history
ADD FOREIGN KEY (user_id) REFERENCES "SOVA_users"(id) ON DELETE NO ACTION;

-- ----------------------------
-- Foreign Key structure for table questions
-- ----------------------------
ALTER TABLE questions
ADD FOREIGN KEY (id) REFERENCES posts(id);

-- ----------------------------
-- Foreign Key structure for table posts
-- ----------------------------
ALTER TABLE posts
ADD FOREIGN KEY (author_id) REFERENCES "SO_authors"(id);

INSERT INTO "SO_authors"
SELECT DISTINCT ownerid, ownerdisplayname, ownercreationdate, ownerlocation, ownerage
FROM posts_universal;

INSERT INTO "SO_authors"
SELECT DISTINCT authorid, authordisplayname, authorcreationdate, authorlocation, authorage
FROM comments_universal 
WHERE authorid NOT IN (SELECT DISTINCT ownerid
											 FROM posts_universal);

INSERT INTO posts
SELECT DISTINCT id, creationdate, body, score, ownerid
FROM posts_universal;

INSERT INTO questions
SELECT DISTINCT id, acceptedanswerid, closeddate, title, tags
FROM posts_universal
WHERE posttypeid = 1;

INSERT INTO linked
SELECT id, linkpostid
FROM posts_universal
WHERE posttypeid = 1
AND linkpostid IS NOT NULL;

INSERT INTO answers
SELECT id, parentid
FROM posts_universal
WHERE posttypeid = 2;

INSERT INTO comments 
SELECT commentid, commentscore, commenttext, commentcreatedate, authorid, postid
FROM comments_universal;


DROP TABLE IF EXISTS posts_universal;
DROP TABLE IF EXISTS comments_universal; 
											 
