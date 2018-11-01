using Microsoft.AspNetCore.Mvc;
using StackOverflowData;

namespace WebService.Controllers {
    [Route("api/SOVA")]
    [ApiController]
    public class SOVAController : Controller {
        private readonly DataService _dataService;

        public SOVAController(DataService dataService) {
            _dataService = dataService;
        }

        [HttpGet]
        public IActionResult GetUser(string username, string password) {
            var user = _dataService.GetUser(username, password);

            if (user == null) {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser(string email, string username, string passwd, string location) {
            var user = _dataService.CreateUser(email, username, passwd, location);

            if (user == -1) {
                return BadRequest();
            }

            return Created("", user);
        }

        [HttpDelete("{userId}")]
        public IActionResult DeleteUser(int userId) {
            var user = _dataService.DeleteUser(userId);

            if (user == false) {
                return NotFound();
            }

            return Ok();
        }
    }
}