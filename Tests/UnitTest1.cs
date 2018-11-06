using StackOverflowData;
using Xunit;

namespace Tests {
    public class WebServiceTests {
        [Fact]
        public void Create_User_Successful() {
            var service = new DataService();
            var u1 = service.GetUser("test");
            if (u1 != null)
                service.DeleteUser(u1.Id);
            var user = service.CreateUser("test@test.te", "test", "Roskilde", "testpwd", "salt");
            Assert.NotNull(user);
        }
    }
}