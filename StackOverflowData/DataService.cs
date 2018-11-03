using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StackOverflowData.Functions;

namespace StackOverflowData {
    public interface IDataService
    {
        GetPostOrCommentResult GetPost(int id);
        int GetUser(string username, string password);
        List<GetPostOrCommentResult> GetAnswers(int questionId, int page, int pageSize);
        List<GetPostOrCommentResult> GetComments(int postId, int page, int pageSize);
        int CreateUser(string email, string username, string password, string location);
        bool DeleteUser(int userId);
        bool UpdateEmail(int id, string email);
        bool UpdateUsername(int id, string username);
        bool UpdatePassword(int id, string password);
        bool UpdateLocation(int id, string location);
        bool CreateMark(int userId, int postId);
        bool DeleteMark(int userId, int postId);
        bool DeleteMark(int userId);
        bool MakeOrUpdateAnnotation(int userId, int postId, string text);
        bool DeleteAnnotation(int userId, int postId);
        bool DeleteHistory(int userId);
        List<SearchResult> Search(string text, int userId, int page, int pageSize);
        List<GetHistoryResult> GetHistory(int userId, int page, int pageSize);
        List<GetMarkedResult> GetMarked(int userId, int page, int pageSize);
    }

    public class DataService : IDataService{
        public GetPostOrCommentResult GetPost(int id)
        {
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_post({0})", id)
                    .FirstOrDefault();
                return result;
            }
        }

        public int GetUser(string username, string password)
        {
            using (var db = new SOVAContext())
            {
                var result = db.GetUserResult.FromSql("select * from get_user({0},{1})", username,
                    password).FirstOrDefault().Id;
                return result;
            }
        }

        public List<GetPostOrCommentResult> GetAnswers(int questionId, int page, int pageSize)
        { 
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_answers({0})", questionId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public List<GetPostOrCommentResult> GetComments(int postId, int page, int pageSize)
        {
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_comments({0})", postId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public int CreateUser(string email, string username, string password, string location)
        {
            using (var db = new SOVAContext())
            {
                int result =  db.GetUserResult.FromSql("SELECT create_user({0},{1},{2},{3})", 
                    email, username, password, location).FirstOrDefault().Id;

                db.SaveChanges();
                return result;
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var db = new SOVAContext())
            {
                var deleted = db.BooleanResult.FromSql("SELECT delete_user({0})", userId)
                    .FirstOrDefault().Successful;

                db.SaveChanges();
                return deleted;
            }
        }

        public bool UpdateEmail(int id, string email)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_email({0},{1})", id, email)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdateUsername(int id, string username)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_username({0},{1})", id, username)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdatePassword(int id, string password)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_password({0},{1})", id, password)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool UpdateLocation(int id, string location)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_location({0},{1})", id, location)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return updated;
            }
        }

        public bool CreateMark(int userId, int postId)
        {
            using (var db = new SOVAContext())
            {
                var marked = db.BooleanResult.FromSql("SELECT mark({0},{1})", userId, postId)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return marked;
            }
        }

        public bool DeleteMark(int userId, int postId)
        {
            using (var db = new SOVAContext())
            {
                var deleted = db.BooleanResult.FromSql("SELECT delete_mark({0},{1})", userId, postId)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool DeleteMark(int userId)
        {
            using (var db = new SOVAContext())
            {
                var deleted = db.BooleanResult.FromSql("SELECT delete_mark({0})", userId)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool MakeOrUpdateAnnotation(int userId, int postId, string text) {
            using (var db = new SOVAContext()) {
                var successful = db.BooleanResult.FromSql("SELECT make_annotation({0},{1},{2})", userId, postId, text)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return successful;
            }
        }


        public bool DeleteAnnotation(int userId, int postId) {
            using (var db = new SOVAContext()) {
                var deleted = db.BooleanResult.FromSql("SELECT delete_annotation({0},{1})", userId, postId)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public bool DeleteHistory(int userId) {
            using (var db = new SOVAContext()) {
                var deleted = db.BooleanResult.FromSql("SELECT delete_history({0})", userId)
                    .FirstOrDefault().Successful;
                db.SaveChanges();
                return deleted;
            }
        }

        public List<SearchResult> Search(string text, int userId, int page, int pageSize)
        {
            using (var db = new StackOverflowContext()){
                var result = db.SearchResults.FromSql("SELECT search_sova({0},{1})", text, userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                db.SaveChanges();
                return result;
            }
        }

        public List<GetHistoryResult> GetHistory(int userId, int page, int pageSize) {
            using (var db = new SOVAContext()) {
                var result = db.GetHistoryResult.FromSql("SELECT get_history({0})", userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }

        public List<GetMarkedResult> GetMarked(int userId, int page, int pageSize)
        {
            using (var db = new SOVAContext())
            {
                var result = db.GetMarkedResult.FromSql("SELECT get_marked({0})", userId)
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
                return result;
            }
        }
    }
}