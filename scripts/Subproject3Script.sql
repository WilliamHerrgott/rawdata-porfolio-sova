--Data Preparation
DROP TABLE IF EXISTS wi;
CREATE TABLE wi AS
SELECT id, lower(word) word, count(word) AS occurence
FROM words
WHERE word ~* '^[a-z][a-z0-9_]+$'
AND tablename = 'posts'
AND (what='title' OR what='body')
AND word NOT IN (SELECT * FROM stopwords)
GROUP BY id,word
ORDER BY occurence DESC;

--Every word from a question as well as every word in the answers to this
--question, are entered in the index.
DROP TABLE IF EXISTS temp;
CREATE TABLE temp AS
SELECT w.id, w.word, w.occurence
FROM wi w, questions q
WHERE w.id = q.id;

INSERT INTO temp
SELECT a.parent_id, w.word, w.occurence
FROM wi w, answers a
WHERE w.id = a.id;

DROP TABLE IF EXISTS wiq;
CREATE TABLE wiq AS
SELECT DISTINCT id, word, sum(occurence) AS occurence
FROM temp
GROUP BY id, word;

DROP TABLE IF EXISTS temp;

CREATE INDEX wordidx on wiq(word);

select * from wiq;

--B.1
--Exact-match querying and Boolean indexing

DROP FUNCTION IF EXISTS exact_match(text[]);
CREATE FUNCTION exact_match(VARIADIC w text[])
RETURNS TABLE (id integer, creation_date timestamp, body text, score integer, author_id integer) AS $$
DECLARE
	w_elem text;
BEGIN
	DROP TABLE IF EXISTS exact_match_table;
	CREATE TEMP TABLE exact_match_table (
		postid integer
	);
	FOREACH w_elem IN ARRAY w
	LOOP
		INSERT INTO exact_match_table 
		SELECT DISTINCT w.id
		FROM words w 
		WHERE word = w_elem
		AND tablename = 'posts';
	END LOOP;
	
	RETURN QUERY
	SELECT DISTINCT p.id, p.creation_date, p.body, p.score, p.author_id
	FROM posts p, exact_match_table t
	WHERE p.id = t.postid;
END $$
LANGUAGE plpgsql;

SELECT * FROM exact_match('table', 'width', 'layout');

--B.2
--Best-match querying
DROP FUNCTION IF EXISTS bestmatch(text[]);
CREATE FUNCTION bestmatch(VARIADIC w text[])
RETURNS TABLE(id integer, rank numeric, body text) AS $$
DECLARE
	w_elem text;
BEGIN
	DROP TABLE IF EXISTS best_match_table;
	CREATE TEMP TABLE best_match_table (
		postid integer, 
		score bigint 
);
	FOREACH w_elem IN ARRAY w
	LOOP
		INSERT INTO best_match_table 
		SELECT DISTINCT w.id, 1 score
		FROM words w 
		WHERE word = w_elem;
	END LOOP;
	
	RETURN QUERY
	SELECT p.id, sum(t.score) rank, p.body
	FROM posts p, best_match_table t
	WHERE p.id = t.postid
	GROUP BY p.id
	ORDER BY rank DESC;
END $$
LANGUAGE plpgsql;

SELECT * FROM bestmatch('table', 'width', 'layout');

--This function takes a string as a parameter and generates a dynamic SQL- expression that executes
--the function bestmatchweighted.

DROP FUNCTION IF EXISTS dynamic_search_without_history;
CREATE FUNCTION dynamic_search_without_history(searched_text text)
RETURNS TABLE (id integer, rank real, body text) AS $$
DECLARE
	q text :='';
	tokens text[] := regexp_split_to_array(searched_text, ' ');
	a_elem text;
BEGIN
	q:= 'SELECT * FROM bestmatchweighted(''';
	FOREACH a_elem IN ARRAY tokens
	LOOP
		q := q || a_elem || ''',''';
	END LOOP;
	q := substring(q from 0 for (char_length(q)-2));
	q := q ||''');';
	RAISE NOTICE '%', q;
	RETURN QUERY EXECUTE q;
END$$
LANGUAGE plpgsql;

-- SELECT * FROM dynamic_search_without_history('initialize constructor');

DROP FUNCTION IF EXISTS dynamic_search;

DROP FUNCTION IF EXISTS dynamic_search_with_history;
CREATE FUNCTION dynamic_search_with_history(searched_text text, this_user_id integer)
RETURNS TABLE (id integer, rank real, body text) AS $$
DECLARE
	q text :='';
	tokens text[] := regexp_split_to_array(searched_text, ' ');
	a_elem text;
BEGIN
	q:= 'SELECT * FROM bestmatchweighted(''';
	FOREACH a_elem IN ARRAY tokens
	LOOP
		q := q || a_elem || ''',''';
	END LOOP;
	q := substring(q from 0 for (char_length(q)-2));
	q := q ||''');';
	RAISE NOTICE '%', q;
	RETURN QUERY EXECUTE q;
	
	IF ( this_user_id IN (SELECT u.id FROM "SOVA_users" u) ) THEN
		INSERT INTO history (search_text, date, user_id)
		VALUES(searched_text, date_trunc('second', LOCALTIMESTAMP), this_user_id);
	END IF;
END$$
LANGUAGE plpgsql;

-- SELECT * FROM dynamic_search_with_history('initialize constructor', 3);

--B.3
--Weighted indexing

--Creating a table to store the number of terms for each post
DROP TABLE IF EXISTS number_of_terms;
CREATE TABLE number_of_terms AS
SELECT id, count(*) AS n_d
FROM wiq
GROUP BY id;

select * from number_of_terms where id = 19;

--Creating a table to store the number of 
--posts that contain a word
DROP TABLE IF EXISTS no_of_docs_per_term;
CREATE TABLE no_of_docs_per_term AS
SELECT word, count(distinct id) AS n_t
FROM wiq
GROUP BY word;

select * from no_of_docs_per_term where word = 'solved';

--Adding weights to wiq
ALTER TABLE wiq 
ADD COLUMN weight real;

CREATE INDEX wordt_index ON no_of_docs_per_term(word);
CREATE INDEX idd_index ON number_of_terms(id);

UPDATE wiq
SET weight = (log(100000/(SELECT n_t from no_of_docs_per_term t WHERE wiq.word = t.word)) * log(1 + 
							(wiq.occurence/(SELECT n_d FROM number_of_terms d WHERE wiq.id = d.id))));
							
UPDATE wiq
SET weight = weight / (WITH max_weight AS (SELECT max(weight) FROM wiq) SELECT max FROM max_weight);

select * from wiq;

--B.4
--Ranked weighted querying
DROP FUNCTION IF EXISTS bestmatchweighted(text[]);
CREATE FUNCTION bestmatchweighted(VARIADIC w text[])
RETURNS TABLE(id integer, rank real, body text) AS $$
DECLARE
	w_elem text;
BEGIN
	DROP TABLE IF EXISTS best_match_table;
	CREATE TEMP TABLE best_match_table (
		postid integer, 
		score real 
);
	FOREACH w_elem IN ARRAY w
	LOOP
		INSERT INTO best_match_table 
		SELECT DISTINCT w.id, w.weight
		FROM wiq w 
		WHERE word = w_elem;
	END LOOP;
	
	RETURN QUERY
	SELECT DISTINCT p.id, sum(t.score) rank, p.body
	FROM posts p, best_match_table t
	WHERE p.id = t.postid
	GROUP BY p.id
	ORDER BY rank DESC;
END $$
LANGUAGE plpgsql;

SELECT * FROM bestmatchweighted('table', 'mysql', 'select');

--B.5
--Word-to-words querying

DROP FUNCTION IF EXISTS keyword_list(text[]);
CREATE FUNCTION keyword_list(VARIADIC w text[])
RETURNS TABLE(weight numeric, word text) AS $$
DECLARE
	w_elem text;
BEGIN
	DROP TABLE IF EXISTS keyword_list_table;
	CREATE TEMP TABLE keyword_list_table (
		postid integer
);
	FOREACH w_elem IN ARRAY w
	LOOP
		INSERT INTO keyword_list_table 
		SELECT DISTINCT w.id
		FROM wiq w 
		WHERE w.word = w_elem;
	END LOOP;
	
	RETURN QUERY
	SELECT sum(w.occurence) as tot_occurence, w.word
	FROM wiq w, keyword_list_table t
	WHERE w.id = t.postid
	AND w.word NOT IN (SELECT * FROM stopwords)
	GROUP BY w.word
	ORDER BY tot_occurence DESC;
END $$
LANGUAGE plpgsql;

select * from keyword_list('sql') where weight > 150;

DROP FUNCTION IF EXISTS dynamic_keyword_list;
CREATE FUNCTION dynamic_keyword_list(searched_text text, this_user_id integer)
RETURNS TABLE (weight numeric, word text) AS $$
DECLARE
	q text :='';
	tokens text[] := regexp_split_to_array(searched_text, ' ');
	a_elem text;
BEGIN
	q:= 'SELECT * FROM keyword_list(''';
	FOREACH a_elem IN ARRAY tokens
	LOOP
		q := q || a_elem || ''',''';
	END LOOP;
	q := substring(q from 0 for (char_length(q)-2));
	q := q ||''');';
	RAISE NOTICE '%', q;
	RETURN QUERY EXECUTE q;
	
	IF ( this_user_id IN (SELECT u.id FROM "SOVA_users" u) ) THEN
		INSERT INTO history (search_text, date, user_id)
		VALUES(searched_text, date_trunc('second', LOCALTIMESTAMP), this_user_id);
	END IF;

END$$
LANGUAGE plpgsql;

-- SELECT * FROM dynamic_keyword_list('initialize constructor');

--Word-to-words querying with weights

DROP FUNCTION IF EXISTS keyword_list_weighted(text[]);
CREATE FUNCTION keyword_list_weighted(VARIADIC w text[])
RETURNS TABLE(weight real, word text) AS $$
DECLARE
	w_elem text;
BEGIN
	DROP TABLE IF EXISTS keyword_list_table;
	CREATE TEMP TABLE keyword_list_table (
		postid integer
);
	FOREACH w_elem IN ARRAY w
	LOOP
		INSERT INTO keyword_list_table 
		SELECT DISTINCT w.id
		FROM wiq w 
		WHERE w.word = w_elem;
	END LOOP;
	
	RETURN QUERY
	SELECT sum(w.weight) as tot_weight, w.word
	FROM wiq w, keyword_list_table t
	WHERE w.id = t.postid
	AND w.word NOT IN (SELECT * FROM stopwords)
	GROUP BY w.word
	ORDER BY tot_weight DESC;
END $$
LANGUAGE plpgsql;

select * from keyword_list_weighted('sql');

DROP FUNCTION IF EXISTS dynamic_keyword_list_weighted;
CREATE FUNCTION dynamic_keyword_list_weighted(searched_text text, this_user_id integer)
RETURNS TABLE (weight real, word text) AS $$
DECLARE
	q text :='';
	tokens text[] := regexp_split_to_array(searched_text, ' ');
	a_elem text;
BEGIN
	q:= 'SELECT * FROM keyword_list_weighted(''';
	FOREACH a_elem IN ARRAY tokens
	LOOP
		q := q || a_elem || ''',''';
	END LOOP;
	q := substring(q from 0 for (char_length(q)-2));
	q := q ||''');';
	RAISE NOTICE '%', q;
	RETURN QUERY EXECUTE q;

	IF ( this_user_id IN (SELECT u.id FROM "SOVA_users" u) ) THEN
		INSERT INTO history (search_text, date, user_id)
		VALUES(searched_text, date_trunc('second', LOCALTIMESTAMP), this_user_id);
	END IF;
END$$
LANGUAGE plpgsql;

-- SELECT * FROM dynamic_keyword_list_weighted('initialize constructor', 11);

-- B.7
-- Co-occurrence term network
DROP TABLE IF EXISTS co_occurrence;
CREATE TABLE co_occurrence AS 
SELECT w1.word AS first,w2.word AS second,count(*) grade 
FROM wiq w1,wiq w2
WHERE w1.id=w2.id AND w1.word<w2.word
AND w1.weight>0.02 AND w2.weight>0.02
AND w1.word NOT IN (SELECT * FROM stopwords)
AND w2.word NOT IN (SELECT * FROM stopwords)
AND w1.word IN (SELECT d1.word FROM no_of_docs_per_term d1 WHERE n_t > 40)
AND w2.word IN (SELECT d2.word FROM no_of_docs_per_term d2 WHERE n_t > 40)
GROUP BY w1.word,w2.word 
ORDER BY count(*) DESC;

DROP FUNCTION IF EXISTS get_co_occurrent_words(text);
CREATE FUNCTION get_co_occurrent_words(to_this_word text)
RETURNS TABLE (word text, grade bigint) AS
$$
BEGIN	
	RETURN QUERY
	SELECT c.second, c.grade
	FROM co_occurrence c
	WHERE c.first = to_this_word;
	
	RETURN QUERY
	SELECT c.first, c.grade
	FROM co_occurrence c
	WHERE c.second = to_this_word;
END $$
LANGUAGE plpgsql;

SELECT * FROM get_co_occurrent_words('language');

--B.8. Force network visualization
drop function if exists generate_force_graph_input;
create function generate_force_graph_input(in w varchar(100), n real) 
returns table (line text) as $$
declare
	l text;
begin
line := '{"nodes":[';
return next;
line := '';
return next;
for l in (select '{"id":"'||lower(second)||'"},' from co_occurrence where first=w and grade>=n)
loop
	line:=l;
	return next;
end loop;

line := '{"id":"'||w||'"},';
return next;
line :=  '],';
return next;
line :=  '"links":[';
return next;

for l in (select '{"source":"'||lower(first)||'", "target":"'||lower(second)||'", "value":'||grade/2||'},'  
from (
select * from co_occurrence where first = w and grade>=n
union
select * from co_occurrence 
where first in (select second from co_occurrence where first=w and grade>=n)
and second in (select second from co_occurrence where first=w and grade>=n)) t)
loop
	line:=l;
	return next;
end loop;

line := ']}';
return next;
return;
end;
$$
language 'plpgsql';
select generate_force_graph_input('sql', 2);

--OPTIONAL
--Query expansion
DROP FUNCTION IF EXISTS expanded_query_search(text[]);
CREATE FUNCTION expanded_query_search(VARIADIC w text[])
RETURNS TABLE(id integer, creation_date timestamp, body text, score integer, author_id integer, rank real) AS $$
DECLARE
	q text :='';
	w_elem text;
	a_elem text;
	related_words text[];
BEGIN
	q:= 'SELECT * FROM bestmatchweighted(''';
	FOREACH w_elem IN ARRAY w
	LOOP
		related_words = ARRAY(SELECT DISTINCT UNNEST(related_words || ARRAY(SELECT word FROM keyword_list_weighted(w_elem) WHERE word != w_elem LIMIT 5)));
		related_words = ARRAY(SELECT DISTINCT UNNEST(related_words || ARRAY(SELECT word FROM get_co_occurrent_words(w_elem) WHERE grade >= 5)));
		q := q || w_elem || ''',''';
	END LOOP;
	FOREACH a_elem in ARRAY related_words
	LOOP
			q := q || a_elem || ''',''';
	END LOOP;
	q := substring(q from 0 for (char_length(q)-2));
	q := q ||''');';
	RAISE NOTICE '%', q;
	RETURN QUERY EXECUTE q;
END$$
LANGUAGE plpgsql;

-- SELECT * FROM expanded_query_search('programming', 'language');
-- SELECT * FROM expanded_query_search('c#', 'linq');
-- SELECT * FROM expanded_query_search('sql');
-- SELECT * FROM bestmatchweighted('sql');
