using System.Linq;
using Microsoft.EntityFrameworkCore;
using StackOverflowData.Functions;

namespace StackOverflowData {
    class DataService {
        public List<GetPostOrCommentResult> GetPost(int id)
        {
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_post({0})", id).ToList();
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

        public List<GetPostOrCommentResult> GetAnswers(int questionId)
        { 
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_answers({0})", questionId).ToList();
                return result;
            }
        }

        public List<GetPostOrCommentResult> GetComments(int postId)
        {
            using (var db = new StackOverflowContext())
            {
                var result = db.GetPostResults.FromSql("select * from get_comments({0})", postId).ToList();
                return result;
            }
        }

        public int CreateUser(string email, string username, string password, string location)
        {
            using (var db = new SOVAContext())
            {
                return db.GetUserResult.FromSql("SELECT create_user({0},{1},{2},{3})", 
                    email, username, password, location).FirstOrDefault().Id;
            }
        }

        public bool DeleteUser(int userId)
        {
            using (var db = new SOVAContext())
            {
                var deleted = db.BooleanResult.FromSql("SELECT delete_user({0})", userId)
                    .FirstOrDefault().Successful;
                return deleted;
            }
        }

        public bool UpdateEmail(int id, string email)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_email({0},{1})", id, email)
                    .FirstOrDefault().Successful;
                return updated;
            }
        }

        public bool UpdateUsername(int id, string username)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_username({0},{1})", id, username)
                    .FirstOrDefault().Successful;
                return updated;
            }
        }

        public bool UpdatePassword(int id, string password)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_password({0},{1})", id, password)
                    .FirstOrDefault().Successful;
                return updated;
            }
        }

        public bool UpdateLocation(int id, string location)
        {
            using (var db = new SOVAContext())
            {
                var updated = db.BooleanResult.FromSql("SELECT update_location({0},{1})", id, location)
                    .FirstOrDefault().Successful;
                return updated;
            }
        }

        public bool Mark(int userId, int postId)
        {
            using (var db = new SOVAContext())
            {
                var marked = db.BooleanResult.FromSql("SELECT mark({0},{1})", userId, postId)
                    .FirstOrDefault().Successful;
                return marked;
            }
        }

        public bool DeleteMark(int userId, string postId = null)
        {
            using (var db = new SOVAContext())
            {
                var deleted = db.BooleanResult.FromSql("SELECT delete_mark({0},{1})", userId, int.TryParse(postId, out int id))
                    .FirstOrDefault().Successful;
                return deleted;
            }
        }

        public void MakeAnnotation(int userId, int postId, string text) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC make_annotation({0},{1},{2})", userId, postId, text);
            }
        }

        public void UpdateAnnotation(int userId, int postId, string newText) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC update_annotation({0},{1},{2})", userId, postId, newText);
            }
        }

        public void DeleteAnnotation(int userId, int postId) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC delete_annotation({0},{1})", userId, postId);
            }
        }

        public void DeleteHistory(int userId) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC delete_history({0})", userId);
            }
        }
    }
}