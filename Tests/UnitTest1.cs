
using System;
using StackOverflowData;
using WebService.Controllers;
using Xunit;

namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        protected DataService service { get; private set; }

        protected DatabaseFixture()
        {
            service = new DataService();

            // initialize data in the test database
            var u1 = service.GetUser("test");
            if (u1 != null)
                service.DeleteUser(u1.Id);
        }

        public void Dispose()
        {
            var u1 = service.GetUser("test");
            if (u1 != null)
                service.DeleteUser(u1.Id);        }
    }   
    public class WebServiceTests: DatabaseFixture
    {
             
        
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
        public void Search_User_Count() {
            service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            Assert.Null(user);
            var resultList = service.Search("python variable", user.Id);
        }
    }
    
}