using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using StackOverflowData;
using Xunit;

namespace Tests {
    public class User {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Location { get; set; }
        public string Token { get; set; }
    }

    public class ApiTests : DatabaseFixture {
        private static readonly DataService S = new DataService();
        private const string BaseApi = "https://localhost:5001/api/";

        private static readonly HttpClient Client = new HttpClient(new HttpClientHandler {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        });

        [Fact]
        public void Register() {
            var user = new User {
                Username = "test",
                Email = "test@test.te",
                Password = "passwd",
                Location = "Rennes"
            };

            var request = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "users/"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = Client.SendAsync(request).Result) {
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                var u = GetObjectFromResponse<User>(response);
                Assert.Equal("test", u.Username);
                DeleteUser(u);
            }
        }

        [Fact]
        public void Login() {
            var user = new {
                Username = "test",
                Password = "passwd",
                Email = "test@test.te",
                Location = "Rennes"
            };
            var request1 = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "users/"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };
            var response1 = Client.SendAsync(request1).Result;

            var request = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "users/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = Client.SendAsync(request).Result) {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var u = GetObjectFromResponse<User>(response);
                Assert.Equal("test", u.Username);
                DeleteUser(u);
            }
        }

        [Fact]
        public void UpdateUser() {
            var user = new {
                Username = "test",
                Password = "passwd",
                Email = "test@test.te",
                Location = "Rennes"
            };
            var request = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "users/"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };
            var _ = Client.SendAsync(request).Result;

            var request1 = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "users/login"),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json")
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response1 = Client.SendAsync(request1).Result) {
                Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
                var u = GetObjectFromResponse<User>(response1);
                Assert.Equal("test", u.Username);
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", u.Token);
                var request2 = new HttpRequestMessage {
                    RequestUri = new Uri(BaseApi + "users/update/location/Roskilde"),
                    Method = HttpMethod.Put,
                    Content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"),
                };
                using (var response2 = Client.SendAsync(request2).Result) {
                    Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
                    var u1 = service.GetUser("test");
                    Assert.Equal("Roskilde", u1.Location);
                }

                DeleteUser(u);
            }
        }

        [Fact]
        public void SeeAnswer() {
            var request = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "StackOverflow/answers/9387810/"),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = Client.SendAsync(request).Result) {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public void SeeAnswer_Bad_Request() {
            var request = new HttpRequestMessage {
                RequestUri = new Uri(BaseApi + "StackOverflow/answers/9389989887810/"),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = Client.SendAsync(request).Result) {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        private static T GetObjectFromResponse<T>(HttpResponseMessage response) {
            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
        }

        private static void DeleteUser(User u) {
            S.DeleteUser(S.GetUser(u.Username).Id);
        }
    }
}