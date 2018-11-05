using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackOverflowData;
using WebService.Models;
using WebService.Services;

namespace WebService.Controllers {
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller {
        private readonly IDataService _dataService;
        private readonly IConfiguration _config;

        public UserController(IDataService dataService, IConfiguration config) {
            _dataService = dataService;
            _config = config;
        }

        [HttpGet("{login}")]
        public IActionResult GetUser(string username) {
            var user = _dataService.GetUser(username);

            if (user.Id == -1) {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserRegModel model) {
            int.TryParse(_config["security:pwdsize"], out var size);
            var salt = PasswordService.GenerateSalt(size);
            var passwd = PasswordService.HashPassword(model.Password, salt, size);
            var user = _dataService.CreateUser(model.Email, model.Username, model.Location, passwd, salt);

            if (user.Id == -1) {
                return BadRequest();
            }

            return Created("", user);
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] UserLoginModel model) {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) {
                return BadRequest();
            }

            int.TryParse(_config["security:pwdsize"], out var size);
            if (size == 0) {
                return BadRequest();
            }

            var user = _dataService.GetUser(model.Username);
            var pwd = PasswordService.HashPassword(model.Password, user.Salt, size);

            if (user == null || pwd != user.Password) {
                return BadRequest();                
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["security:key"]);

            var tokenDescription = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                }),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescription));

            var resp = new {
                user.Username,
                token
            };
            
            return Ok(resp);
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