----------------------------------------------------------------
-- Creating a user
----------------------------------------------------------------
SELECT create_user('apecini@ruc.dk', 'ap', '1234', 'Albania', '1');
SELECT create_user('blmeec@ruc.dk', 'blm', '5678', 'France', '2');
SELECT create_user('vdsa@ruc.dk', 'vsilva', 'asd123', 'Portugal', '3');
SELECT create_user('wihe@ruc.dk', 'william', '987654321', 'France', '4');

SElECT * FROM "SOVA_users";

----------------------------------------------------------------
-- Deleting a user
----------------------------------------------------------------

SELECT delete_user(2);

SELECT * FROM "SOVA_users";

----------------------------------------------------------------
-- Storing a marked post
----------------------------------------------------------------

SELECT * FROM marks;

SELECT mark(1, 712);
SELECT mark(1, 713);
SELECT mark(1, 709);
SELECT mark(1, 718);
SELECT mark(1, 798);
SELECT mark(2,713);
SELECT mark(2,712);
SELECT mark(2,798);
SELECT mark(3,709);
SELECT mark(3,713);
SELECT mark(4,718);

SELECT * FROM marks;


----------------------------------------------------------------
-- Storing an annotation made by the user on a marked post
----------------------------------------------------------------

SELECT make_annotation(1, 712, 'Nice solution');
SELECT make_annotation(1, 713, 'Similar. Might use in the future');
SELECT make_annotation(2, 713, 'Should remember this');
SELECT make_annotation(3,709, 'This is what I wanted');
SELECT make_annotation(4,718, 'I must use this code');

SELECT * FROM marks;

----------------------------------------------------------------
-- Visualizing all the marked posts by a given user
----------------------------------------------------------------

SELECT *
FROM marks
where user_id = 2;

----------------------------------------------------------------
-- Deleting all marks from a given user
----------------------------------------------------------------

SELECT * FROM marks;

SELECT delete_mark(4);

SELECT * FROM marks;

----------------------------------------------------------------
-- Deleting a marked post from a given user and post
----------------------------------------------------------------

SELECT * FROM delete_mark(1,712) AS successful;

SELECT * FROM marks;

----------------------------------------------------------------
-- Changing the annotation on a post by a given user
----------------------------------------------------------------

SELECT update_annotation(1, 712, 'Afterall this was not the solution');

SELECT * FROM marks;

----------------------------------------------------------------
-- Deleting an annotation made on a given marked post by a given user
----------------------------------------------------------------

SELECT delete_annotation(1,713);

SELECT * FROM marks;

-----------------------------------
-- Getting a subset of marked posts 
-----------------------------------

SELECT get_marked(1);

SELECT * FROM marks;

----------------------------------------------------------------
-- Getting posts for a given search text
----------------------------------------------------------------


SELECT search_sova('php', 1);

SELECT search_sova('Python', 1);
SELECT search_sova('c#', 1);
SELECT search_sova('sql', 1);
SELECT search_sova('polymorphism', 1);

-- grabbing the first returned post_id in order to test the function

SELECT * 
FROM POSTS 
WHERE ID = 48211;

SELECT * 
FROM history;

----------------------------------------------------------------
-- Getting history of searched text
-- You can pass as a parameter the limit and offset 
----------------------------------------------------------------

SELECT * from get_history(1);

----------------------------------------------------------------
-- Deleting history for a given user
-- It will trigger a procedure to delete in history table as well
----------------------------------------------------------------

SELECT delete_history(1);

SELECT * 
FROM history;


----------------------------------------------------------------
-- Retrieving a given post 
----------------------------------------------------------------

SELECT get_post(798);

SELECT *
FROM POSTS
WHERE ID = 798;

----------------------------------------------------------------
-- Retrieving answers for a given question 
----------------------------------------------------------------

SELECT get_answers(5131954);

SELECT  P.BODY
FROM POSTS P, ANSWERS A
WHERE P.ID = A.ID AND  A.PARENT_ID = 5131954;

----------------------------------------------------------------
-- Retrieving comments for a given post
----------------------------------------------------------------

SELECT get_comments(13649012);

SELECT C.BODY
FROM COMMENTS C, POSTS P
WHERE C.POST_ID = P.ID AND P.ID = 13649012;

----------------------------------------------------------------
-- Update user's enail, displayname, password or location
----------------------------------------------------------------

SELECT * FROM "SOVA_users";
SELECT update_email(1,'anothername@ruc.dk');

SELECT * FROM "SOVA_users";
SELECT update_username(2, 'User Name updated');

SELECT * FROM "SOVA_users";
SELECT update_password(4, '1995', 'oui');

SELECT * FROM "SOVA_users";
SELECT update_location(5, 'Roskilde');

SELECT * FROM "SOVA_users";

select * from get_author_of_post(3987810); 

----------------------------------------------------------------
-- Update user's enail, displayname, password or location
----------------------------------------------------------------

SELECT get_user('vsilva');

SELECT ID
FROM "SOVA_users"
WHERE USERNAME = 'vsilva' AND PASSWORD = '1995';


----------------------------------------------------------------
-- Deleting a user
-- It should delete history, marks, annotations for given user
----------------------------------------------------------------
SELECT search_sova('Python', 1);
SELECT * FROM search_posts('c++');

SELECT *
FROM "SOVA_users"
WHERE ID = 1;

SELECT *
FROM MARKS
WHERE USER_ID = 1;

SELECT *
FROM HISTORY
WHERE USER_ID = 1;

SELECT delete_user(1);

SELECT *
FROM "SOVA_users"
WHERE ID = 1;

SELECT *
FROM MARKS
WHERE USER_ID = 1;

SELECT *
FROM HISTORY
WHERE ID = 3;