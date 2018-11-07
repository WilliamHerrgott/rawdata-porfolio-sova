using System;
using System.Linq;
using StackOverflowData;
using Xunit;

namespace Tests {
    public class DatabaseFixture : IDisposable {
        protected DataService service { get; private set; }

        protected DatabaseFixture() {
            service = new DataService();

            // initialize data in the test database
            var u1 = service.GetUser("test");
            if (u1 != null)
                service.DeleteUser(u1.Id);
        }

        public void Dispose() {
            var u1 = service.GetUser("test");
            if (u1 != null)
                service.DeleteUser(u1.Id);
        }
    }

    public class WebServiceTests : DatabaseFixture {
        [Fact]
        public void Create_User_Successful() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            Assert.NotNull(user);
            Assert.Equal(user.Email, "test@test.te");
        }

        [Fact]
        public void Create_User_Not_Successful() {
            service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            Assert.Null(user);
        }

        [Fact]
        public void Delete_User_Successful() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var r = service.DeleteUser(user.Id);
            Assert.True(r);
        }

        [Fact]
        public void Modify_User_Email() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.UpdateEmail(user.Id, "test1@test1.te");
            var Nuser = service.GetUser(user.Username);
            Assert.Equal("test1@test1.te", Nuser.Email);
        }

        [Fact]
        public void Modify_User_Location() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.UpdateLocation(user.Id, "Rennes");
            var Nuser = service.GetUser(user.Username);
            Assert.Equal("Rennes", Nuser.Location);
        }

        [Fact]
        public void Modify_User_Password() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.UpdatePassword(user.Id, "pwd", user.Salt);
            var Nuser = service.GetUser(user.Username);
            Assert.Equal("pwd", Nuser.Password);
        }

        [Fact]
        public void Modify_User_Username() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.UpdateUsername(user.Id, "toust");
            var Nuser = service.GetUserById(user.Id);
            Assert.Equal("toust", Nuser.Username);
            service.UpdateUsername(user.Id, "test");
        }

        [Fact]
        public void Search_User_Count() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var resultList = service.Search("python variable", user.Id, 0, 15);
            Assert.Equal(11, resultList.Count);
        }

        [Fact]
        public void Search_User_History_Count() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.Search("python thing", user.Id);
            service.Search("python test", user.Id);
            service.Search("c# boogie doogie", user.Id);
            service.Search("php", user.Id);
            service.Search("roskilde", user.Id);
            var history = service.GetHistory(user.Id);
            Assert.Equal(5, history.Count);
        }

        [Fact]
        public void User_Mark_Post() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var r = service.CreateMark(user.Id, 61545);
            Assert.True(r);
        }

        [Fact]
        public void User_Mark_Multiple_Post_Count() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.CreateMark(user.Id, 61545);
            service.CreateMark(user.Id, 3629183);
            service.CreateMark(user.Id, 64252);
            service.CreateMark(user.Id, 42519);
            service.CreateMark(user.Id, 216574);
            var marked = service.GetMarked(user.Id);
            Assert.Equal(5, marked.Count);
        }

        [Fact]
        public void User_Make_Annotation() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.CreateMark(user.Id, 61545);
            var r = service.MakeOrUpdateAnnotation(user.Id, 61545, "Annotation1");
            Assert.True(r);
        }

        [Fact]
        public void User_Make_Annotation_Not_Successful() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var r = service.MakeOrUpdateAnnotation(user.Id, 61545, "Annotation1");
            Assert.False(r);
        }

        [Fact]
        public void User_Make_Multiple_Annotation_Count() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.CreateMark(user.Id, 61545);
            service.CreateMark(user.Id, 3629183);
            service.CreateMark(user.Id, 64252);
            service.MakeOrUpdateAnnotation(user.Id, 61545, "Annotation1");
            service.MakeOrUpdateAnnotation(user.Id, 3629183, "Annotation1");
            service.MakeOrUpdateAnnotation(user.Id, 64252, "Annotation1");
            var a = service.GetMarked(user.Id).Where(x => x.Annotation != null).ToList();
            Assert.Equal(3, a.Count());
        }

        [Fact]
        public void User_Make_Multiple_Annotation_Content() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.CreateMark(user.Id, 61545);
            service.MakeOrUpdateAnnotation(user.Id, 61545, "Annotation1");
            var a = service.GetMarked(user.Id).Where(x => x.Annotation != null).ToList();
            Assert.Equal("Annotation1", a[0].Annotation);
        }

        [Fact]
        public void User_Delete_Mark() {
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            service.CreateMark(user.Id, 61545);
            service.CreateMark(user.Id, 3629183);
            service.CreateMark(user.Id, 64252);
            service.DeleteMark(user.Id, 61545);
            var m = service.GetMarked(user.Id);
            Assert.Equal(2, m.Count);
        }
    }
}