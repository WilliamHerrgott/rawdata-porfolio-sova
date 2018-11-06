using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Tests
{
    public class ApiTests
    {
        [Fact]
        public void Works()
        {
            var client = new HttpClient(); // no HttpServer

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost:5000/StackOverflow/answers/9387810/"),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = client.SendAsync(request).Result)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}