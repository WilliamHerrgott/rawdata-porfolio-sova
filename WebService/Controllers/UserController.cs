using Microsoft.AspNetCore.Mvc;
using StackOverflowData;

namespace WebService.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller {
        private readonly IDataService _dataService;

        public UserController(IDataService dataService) {
            _dataService = dataService;
        }

        [HttpGet("{username}/{password}")]
        public IActionResult GetUser(string username, string password) {
            var userId = _dataService.GetUser(username, password);

            if (userId == -1) {
                return NotFound();
            }

            return Ok(userId);
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

        [HttpPut("{userId}")]
        public IActionResult UpdateEmail(int userId, string email) {
            var updated = _dataService.UpdateEmail(userId, email);

            if (updated == false) {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateUsername(int userId, string username) {
            var updated = _dataService.UpdateUsername(userId, username);

            if (updated == false) {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdatePassword(int userId, string password) {
            var updated = _dataService.UpdatePassword(userId, password);

            if (updated == false) {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateLocation(int userId, string location) {
            var updated = _dataService.UpdateLocation(userId, location);

            if (updated == false) {
                return NotFound();
            }

            return Ok();
        }
    }
}