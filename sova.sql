/*
 Navicat Premium Data Transfer

 Source Server         : localhost_5432
 Source Server Type    : PostgreSQL
 Source Server Version : 100005
 Source Host           : localhost:5432
 Source Catalog        : sova
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 100005
 File Encoding         : 65001

 Date: 27/09/2018 14:53:08
*/


-- ----------------------------
-- Table structure for SOVA_users
-- ----------------------------
DROP TABLE IF EXISTS "public"."SOVA_users";
CREATE TABLE "public"."SOVA_users" (
  "id" int4 NOT NULL,
  "username" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "password" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "location" varchar(255) COLLATE "pg_catalog"."default" NOT NULL,
  "age" int4 NOT NULL
)
;
-- ----------------------------
-- Table structure for SO_authors
-- ----------------------------
DROP TABLE IF EXISTS "public"."SO_authors";
CREATE TABLE "public"."SO_authors" (
  "id" int4 NOT NULL,
  "name" varchar(255) COLLATE "pg_catalog"."default",
  "created_date" date,
  "location" varchar(255) COLLATE "pg_catalog"."default",
  "age" int4
)
;


-- ----------------------------
-- Table structure for answers
-- ----------------------------
DROP TABLE IF EXISTS "public"."answers";
CREATE TABLE "public"."answers" (
  "id" int4 NOT NULL
)
;

-- ----------------------------
-- Table structure for author_coments
-- ----------------------------
DROP TABLE IF EXISTS "public"."author_coments";
CREATE TABLE "public"."author_coments" (
  "comment_id" int4,
  "user_id" int4
)
;

-- ----------------------------
-- Table structure for author_posts
-- ----------------------------
DROP TABLE IF EXISTS "public"."author_posts";
CREATE TABLE "public"."author_posts" (
  "user_id" int4,
  "post_id" int4
)
;

-- ----------------------------
-- Table structure for commented_on
-- ----------------------------
DROP TABLE IF EXISTS "public"."commented_on";
CREATE TABLE "public"."commented_on" (
  "comment_id" int4,
  "post_id" int4
)
;

-- ----------------------------
-- Table structure for comments
-- ----------------------------
DROP TABLE IF EXISTS "public"."comments";
CREATE TABLE "public"."comments" (
  "id" int4,
  "score" int4,
  "body" varchar(255) COLLATE "pg_catalog"."default",
  "creation_date" date
)
;

-- ----------------------------
-- Table structure for history
-- ----------------------------
DROP TABLE IF EXISTS "public"."history";
CREATE TABLE "public"."history" (
  "id" int4 NOT NULL,
  "search_text" varchar(255) COLLATE "pg_catalog"."default",
  "date" date
)
;

-- ----------------------------
-- Table structure for marks
-- ----------------------------
DROP TABLE IF EXISTS "public"."marks";
CREATE TABLE "public"."marks" (
  "user_id" int4 NOT NULL,
  "post_id" int4 NOT NULL,
  "marked_creationdate" date NOT NULL,
  "text_annotation" varchar(255) COLLATE "pg_catalog"."default"
)
;

-- ----------------------------
-- Table structure for posts
-- ----------------------------
DROP TABLE IF EXISTS "public"."posts";
CREATE TABLE "public"."posts" (
  "id" int4 NOT NULL,
  "creation_date" date,
  "body" text COLLATE "pg_catalog"."default",
  "score" int4
)
;

-- ----------------------------
-- Table structure for questions
-- ----------------------------
DROP TABLE IF EXISTS "public"."questions";
CREATE TABLE "public"."questions" (
  "accepted_answer_id" int4,
  "closed_date" date,
  "title" varchar(255) COLLATE "pg_catalog"."default",
  "tags" varchar(255) COLLATE "pg_catalog"."default",
  "linkpost_id" int4
)
;

-- ----------------------------
-- Table structure for questions_answers
-- ----------------------------
DROP TABLE IF EXISTS "public"."questions_answers";
CREATE TABLE "public"."questions_answers" (
  "question_id" int4 NOT NULL,
  "answer_id" int4 NOT NULL
)
;

-- ----------------------------
-- Table structure for searched
-- ----------------------------
DROP TABLE IF EXISTS "public"."searched";
CREATE TABLE "public"."searched" (
  "history_id" int4,
  "user_id" int4
)
;

-- ----------------------------
-- Primary Key structure for table SOVA_users
-- ----------------------------
ALTER TABLE "public"."SOVA_users" ADD CONSTRAINT "SOVA_users_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table SO_authors
-- ----------------------------
ALTER TABLE "public"."SO_authors" ADD CONSTRAINT "SO_authors_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table answers
-- ----------------------------
ALTER TABLE "public"."answers" ADD CONSTRAINT "answers_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table history
-- ----------------------------
ALTER TABLE "public"."history" ADD CONSTRAINT "history_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table posts
-- ----------------------------
ALTER TABLE "public"."posts" ADD CONSTRAINT "posts_pkey" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table questions_answers
-- ----------------------------
ALTER TABLE "public"."questions_answers" ADD CONSTRAINT "questions_answers_pkey" PRIMARY KEY ("answer_id");
