﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using StackOverflowData.Functions;

namespace StackOverflowData {
    public interface IDataService {
        GetPostOrCommentResult GetPost(int id);
        GetUserResult GetUser(string username);
        GetUserResult GetUserById(int id);
        List<GetPostOrCommentResult> GetAnswers(int questionId, int page, int pageSize);
        List<GetPostOrCommentResult> GetComments(int postId, int page, int pageSize);
        GetUserResult CreateUser(string email, string username, string location, string passwd, string salt);
        bool DeleteUser(int userId);
        bool UpdateEmail(int id, string email);
        bool UpdateUsername(int id, string username);
        bool UpdatePassword(int id, string password, string salt);
        bool UpdateLocation(int id, string location);
        bool CreateMark(int userId, int postId);
        bool DeleteMark(int userId, int postId);
        bool DeleteMark(int userId);
        bool MakeOrUpdateAnnotation(int userId, int postId, string text);
        bool DeleteAnnotation(int userId, int postId);
        bool IsMarked(int postId, int userId);
        bool DeleteHistory(int userId);

        List<SearchResult> Search(string text, int userId, int page, int pageSize);
        List<SearchResult> SearchExactMatch(string text, int userId, int page, int pageSize);
        List<SearchResult> SearchBestMatch(string text, int userId, int page, int pageSize);
        List<SearchResultWords> SearchRelatedTerm(string text, int userId, bool weighted, int page, int pageSize);
        List<SearchResult> SearchExpandedQuery(string text, int userId, int page, int pageSize);
        List<SearchResultWords> SearchCoWordQuery(string text, int userId, int page, int pageSize);

        List<GetHistoryResult> GetHistory(int userId, int page, int pageSize);
        List<GetMarkedResult> GetMarked(int userId, int page, int pageSize);
        int GetHistoryCount(int userId);
        int GetSearchedCount(string text);
        int GetExactSearchedCount(string text);
        int GetBestSearchedCount(string text);
        int GetNoOfAnswers(int postId);
        int GetNoOfComments(int postId);
        int GetNoOfMarks(int userId);
        GetAuthorResult GetAuthorOfComment(int commentId);
        GetAuthorResult GetAuthorOfPost(int postId);
    }

    [SuppressMessage("ReSharper", "EF1000")]
    public class DataService : IDataService {
        public GetPostOrCommentResult GetPost(int postId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_post({0})", postId)
                    .FirstOrDefault();
                return result;
            }
        }

        public GetUserResult GetUser(string username) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetUserResult
                    .FromSql("select * from get_user({0})", username).FirstOrDefault();
                return result;
            }
        }

        public GetUserResult GetUserById(int id) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetUserResult
                    .FromSql("select * from get_user_by_id({0})", id).FirstOrDefault();
                return result;
            }
        }

        public List<GetPostOrCommentResult> GetAnswers(int questionId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_answers({0})", questionId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public int GetNoOfAnswers(int questionId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_answers({0})", questionId)
                    .Count();
                return result;
            }
        }

        public List<GetPostOrCommentResult> GetComments(int postId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_comments({0})", postId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public int GetNoOfComments(int postId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_comments({0})", postId)
                    .Count();

                return result;
            }
        }

        public GetUserResult CreateUser(string email, string username, string password, string location, string salt) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetUserResult
                    .FromSql("SELECT * FROM create_user({0},{1},{2},{3},{4})",
                        email, username, password, location, salt).FirstOrDefault();

                db.SaveChanges();
                return result;
            }
        }

        public bool DeleteUser(int userId) {
            using (var db = new StackOverflowContext()) {
                var deleted = db.BooleanResult
                    .FromSql("SELECT * FROM delete_user({0}) AS successful", userId)
                    .First().Successful;

                db.SaveChanges();
                return deleted;
            }
        }

        public bool UpdateEmail(int id, string email) {
            using (var db = new StackOverflowContext()) {
                var updated = db.BooleanResult
                    .FromSql("SELECT * FROM update_email({0},{1}) AS successful", id, email)
                    .First().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdateUsername(int id, string username) {
            using (var db = new StackOverflowContext()) {
                var updated = db.BooleanResult
                    .FromSql("SELECT * FROM update_username({0},{1}) AS successful", id, username)
                    .First().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdatePassword(int id, string password, string salt) {
            using (var db = new StackOverflowContext()) {
                var updated = db.BooleanResult
                    .FromSql("SELECT * FROM update_password({0},{1},{2}) AS successful", id, password, salt)
                    .First().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdateLocation(int id, string location) {
            using (var db = new StackOverflowContext()) {
                var updated = db.BooleanResult
                    .FromSql("SELECT * FROM update_location({0},{1}) AS successful", id, location)
                    .First().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool CreateMark(int userId, int postId) {
            using (var db = new StackOverflowContext()) {
                var marked = db.BooleanResult
                    .FromSql("SELECT * FROM mark({0},{1}) AS successful", userId, postId)
                    .First().Successful;
                db.SaveChanges();
                return marked;
            }
        }

        public bool DeleteMark(int userId, int postId) {
            using (var db = new StackOverflowContext()) {
                var deleted = db.BooleanResult
                    .FromSql("SELECT * FROM delete_mark({0},{1}) AS successful", userId, postId)
                    .First().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool DeleteMark(int userId) {
            using (var db = new StackOverflowContext()) {
                var deleted = db.BooleanResult
                    .FromSql("SELECT * FROM delete_mark({0}) AS successful", userId)
                    .First().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool MakeOrUpdateAnnotation(int userId, int postId, string text) {
            using (var db = new StackOverflowContext()) {
                var successful = db.BooleanResult
                    .FromSql("SELECT * FROM make_annotation({0},{1},{2}) AS successful", userId, postId, text)
                    .First().Successful;
                db.SaveChanges();
                return successful;
            }
        }


        public bool DeleteAnnotation(int userId, int postId) {
            using (var db = new StackOverflowContext()) {
                var deleted = db.BooleanResult
                    .FromSql("SELECT * FROM delete_annotation({0},{1}) AS successful", userId, postId)
                    .First().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool IsMarked(int postId, int userId) {
            using (var db = new StackOverflowContext()) {
                var marked = db.BooleanResult
                    .FromSql("SELECT * FROM is_marked({0},{1}) AS successful", postId, userId)
                    .First().Successful;
                db.SaveChanges();
                return marked;
            }
        }

        public bool DeleteHistory(int userId) {
            using (var db = new StackOverflowContext()) {
                var deleted = db.BooleanResult
                    .FromSql("SELECT * FROM delete_history({0}) AS successful", userId)
                    .First().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public List<SearchResult> Search(string text, int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResults.FromSql("SELECT * FROM search_sova({0},{1})", text, userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<SearchResult> SearchExactMatch(string text, int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResults.FromSql("SELECT * FROM exact_match({0})", text.Split(' '))
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<SearchResult> SearchBestMatch(string text, int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResults
                    .FromSql("SELECT * FROM dynamic_search_with_history({0}, {1})", text, userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<SearchResultWords> SearchRelatedTerm(string text, int userId, bool weighted, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResultWords
                    .FromSql("SELECT * FROM dynamic_keyword_list" + (weighted ? "_weighted" : "") + "({0}, {1})", text, userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<SearchResultWords> SearchCoWordQuery(string text, int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResultWords
                    .FromSql("SELECT * FROM get_co_occurrent_words({0}) order by grade desc", text)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<SearchResult> SearchExpandedQuery(string text, int userId, int page = 0, int pageSize = 10) {
            text = text.Split(' ').Join("', '");
            using (var db = new StackOverflowContext()) {
                var result = db.SearchResults
                    .FromSql("SELECT * FROM expanded_query_search{0}", text)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public int GetSearchedCount(string text) {
            using (var db = new StackOverflowContext()) {
                var count = db.SearchResults.FromSql("SELECT * FROM search_posts({0})", text)
                    .Count();
                return count;
            }
        }

        public int GetBestSearchedCount(string text) {
            using (var db = new StackOverflowContext()) {
                var count = db.SearchResults.FromSql("SELECT * FROM dynamic_search_without_history({0})", text)
                    .Count();
                return count;
            }
        }

        public int GetExactSearchedCount(string text) {
            using (var db = new StackOverflowContext()) {
                var count = db.SearchResults.FromSql("SELECT * FROM exact_match({0})", text)
                    .Count();
                return count;
            }
        }

        public List<GetHistoryResult> GetHistory(int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetHistoryResult.FromSql("SELECT * FROM get_history({0})", userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public int GetHistoryCount(int userId) {
            using (var db = new StackOverflowContext()) {
                var count = db.GetHistoryResult.FromSql("SELECT * FROM get_history({0})", userId)
                    .Count();
                return count;
            }
        }

        public List<GetMarkedResult> GetMarked(int userId, int page = 0, int pageSize = 10) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetMarkedResult.FromSql("SELECT * FROM get_marked({0})", userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public int GetNoOfMarks(int userId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetMarkedResult.FromSql("SELECT * FROM get_marked({0})", userId)
                    .Count();
                return result;
            }
        }

        public GetAuthorResult GetAuthorOfPost(int postId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetAuthorResult.FromSql("SELECT * FROM get_author_of_post({0})", postId)
                    .FirstOrDefault();
                return result;
            }
        }

        public GetAuthorResult GetAuthorOfComment(int commentId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetAuthorResult.FromSql("SELECT * FROM get_author_of_comment({0})", commentId)
                    .FirstOrDefault();
                return result;
            }
        }
    }
}