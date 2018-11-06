using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Tests
{
    public class ApiTests
    {
        private const string BaseApi = "http://localhost:5000/api/StackOverflow/";


        [Fact]
        public void SeeAnswer()
        {
            var client = new HttpClient(); // no HttpServer

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(BaseApi + "answers/9387810/"),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = client.SendAsync(request).Result)
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//                dynamic stuff = JObject.Parse(response);

//                Assert.Equal(188, stuff.NumberOfItems);

            }
        }
        
        [Fact]
        public void SeeAnswer_Bad_Request()
        {
            var client = new HttpClient(); // no HttpServer

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(BaseApi + "answers/9389989887810/"),
                Method = HttpMethod.Get
            };

            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (var response = client.SendAsync(request).Result)
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }
    }
    
    
}