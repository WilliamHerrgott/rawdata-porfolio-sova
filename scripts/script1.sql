/*
 Date: 27/09/2018 17:06:24
*/


-- ----------------------------
-- Table structure for SOVA_users
-- ----------------------------
DROP TABLE IF EXISTS "SOVA_users";
CREATE TABLE "SOVA_users" (
  id SERIAL,
	email varchar(50) NOT NULL, 
  username varchar(50) NOT NULL,
  password varchar(50) NOT NULL,
  location varchar(50) NOT NULL
)
;


-- ----------------------------
-- Table structure for SO_authors
-- ----------------------------
DROP TABLE IF EXISTS "SO_authors";
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
DROP TABLE IF EXISTS answers;
CREATE TABLE answers(
  id integer NOT NULL
)
;

-- ----------------------------
-- Table structure for author_coments
-- ----------------------------
DROP TABLE IF EXISTS author_comments;
CREATE TABLE author_comments (
  comment_id integer,
  author_id integer
)
;


-- ----------------------------
-- Table structure for author_posts
-- ----------------------------
DROP TABLE IF EXISTS author_posts;
CREATE TABLE author_posts (
	post_id integer,
	author_id integer
)
;


-- ----------------------------
-- Table structure for commented_on
-- ----------------------------
DROP TABLE IF EXISTS commented_on;
CREATE TABLE commented_on (
  comment_id integer,
  post_id integer
)
;


-- ----------------------------
-- Table structure for comments
-- ----------------------------
DROP TABLE IF EXISTS comments;
CREATE TABLE comments (
  id integer NOT NULL,
  score integer,
  body text, 
  creation_date timestamp
)
;


-- ----------------------------
-- Table structure for history
-- ----------------------------
DROP TABLE IF EXISTS history;
CREATE TABLE history (
  id SERIAL,
  search_text varchar(255),
  date timestamp
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
  text_annotation text
)
;


-- ----------------------------
-- Table structure for posts
-- ----------------------------
DROP TABLE IF EXISTS posts;
CREATE TABLE posts (
  id integer NOT NULL,
  creation_date timestamp,
  body text,
  score integer
)
;


-- ----------------------------
-- Table structure for questions
-- ----------------------------
DROP TABLE IF EXISTS questions;
CREATE TABLE questions (
  id integer NOT NULL,
	accepted_answer_id integer,
  closed_date timestamp,
  title varchar(255),
  tags varchar(255)

)
;


-- ----------------------------
-- Table structure for questions_answers
-- ----------------------------
DROP TABLE IF EXISTS questions_answers;
CREATE TABLE questions_answers (
  answer_id integer NOT NULL,
	question_id integer NOT NULL
  
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
-- Table structure for linked
-- ----------------------------
DROP TABLE IF EXISTS searched;
CREATE TABLE searched (
  history_id integer,
  user_id integer
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
-- Primary Key structure for table questions_answers
-- ----------------------------
ALTER TABLE questions_answers ADD PRIMARY KEY (answer_id);

-- ----------------------------
-- Primary Key structure for table searched
-- ----------------------------
ALTER TABLE searched ADD PRIMARY KEY (history_id);

-- ----------------------------
-- Primary Key structure for table marks
-- ----------------------------
ALTER TABLE marks ADD PRIMARY KEY (user_id, post_id);

-- ----------------------------
-- Primary Key structure for table commented_on
-- ----------------------------
ALTER TABLE commented_on ADD PRIMARY KEY (comment_id);

-- ----------------------------
-- Primary Key structure for table author_posts
-- ----------------------------
ALTER TABLE author_posts ADD PRIMARY KEY (post_id);

-- ----------------------------
-- Primary Key structure for table author_comments
-- ----------------------------
ALTER TABLE author_comments ADD PRIMARY KEY (comment_id);

-- ----------------------------
-- Primary Key structure for table linked
-- ----------------------------
ALTER TABLE linked ADD PRIMARY KEY (question_id, linkpost_id);

INSERT INTO posts
SELECT DISTINCT id, creationdate, body, score
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
SELECT id
FROM posts_universal
WHERE posttypeid = 2;

INSERT INTO questions_answers
SELECT id, parentid
FROM posts_universal
WHERE posttypeid = 2;

INSERT INTO comments 
SELECT commentid, commentscore, commenttext, commentcreatedate
FROM comments_universal;

INSERT INTO commented_on
SELECT DISTINCT commentid, postid
FROM comments_universal;

INSERT INTO author_comments
SELECT DISTINCT commentid, authorid
FROM comments_universal;

INSERT INTO author_posts
SELECT DISTINCT id, ownerid
FROM posts_universal;

INSERT INTO "SO_authors"
SELECT DISTINCT ownerid, ownerdisplayname, ownercreationdate, ownerlocation, ownerage
FROM posts_universal;

INSERT INTO "SO_authors"
SELECT DISTINCT authorid, authordisplayname, authorcreationdate, authorlocation, authorage
FROM comments_universal 
WHERE authorid NOT IN (SELECT DISTINCT ownerid
											 FROM posts_universal);
											 
