using System.Linq;
using Microsoft.EntityFrameworkCore;
using StackOverflowData.Functions;

namespace StackOverflowData {
    public class DataService {
        /***********************************
         *  StackOverflowContext Functions  *
         ***********************************/

        public IQueryable<GetPostOrCommentResult> GetPost(int id) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_post({0})", id);
                return result;
            }
        }

        public IQueryable<GetPostOrCommentResult> GetAnswers(int questionId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_answers({0})", questionId);
                return result;
            }
        }

        public IQueryable<GetPostOrCommentResult> GetComments(int postId) {
            using (var db = new StackOverflowContext()) {
                var result = db.GetPostResults.FromSql("select * from get_comments({0})", postId);
                return result;
            }
        }

        /**************************
        *  SOVAContext Functions  *
        **************************/

        public IQueryable<GetUserResult> GetUser(string username, string password) {
            using (var db = new SOVAContext()) {
                var result = db.GetUserResult.FromSql("select * from get_user({0},{1})", username,
                    password);
                return result;
            }
        }

        public int CreateUser(string email, string username, string password, string location) {
            using (var db = new SOVAContext()) {
                var userId = db.Database.ExecuteSqlCommand("EXEC create_user({0},{1},{2},{3})",
                    email, username, password, location);
                return userId;
            }
        }

        public bool DeleteUser(int userId) {
            using (var db = new SOVAContext()) {
                var success = db.Database.ExecuteSqlCommand("EXEC delete_user({0})", userId);
                return success;
            }
        }

        public void UpdateEmail(int id, string email) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC update_email({0},{1})", id, email);
            }
        }

        public void UpdateUsername(int id, string username) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC update_username({0},{1})", id, username);
            }
        }

        public void UpdatePassword(int id, string password) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC update_password({0},{1})", id, password);
            }
        }

        public void UpdateLocation(int id, string location) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC update_location({0},{1})", id, location);
            }
        }

        public void Mark(int userId, int postId) {
            using (var db = new SOVAContext()) {
                db.Database.ExecuteSqlCommand("EXEC mark({0},{1})", userId, postId);
            }
        }

        public void DeleteMark(int userId, string postId = null) {
            using (var db = new SOVAContext()) {
                int id;
                db.Database.ExecuteSqlCommand("EXEC delete_mark({0},{1})", userId, int.TryParse(postId, out id));
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