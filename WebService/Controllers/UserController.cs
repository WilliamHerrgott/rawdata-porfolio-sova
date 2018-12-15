using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserRegModel model) {
            int.TryParse(_config["security:pwdsize"], out var size);
            var salt = PasswordService.GenerateSalt(size);
            var passwd = PasswordService.HashPassword(model.Password, salt, size);
            var user = _dataService.CreateUser(model.Email, model.Username, passwd, model.Location, salt);

            if (user == null) return BadRequest();

            var displayUser = Mapper.Map<GetUserModel>(user);
            displayUser.Marks = Url.RouteUrl(nameof(MarkController.GetMarked));
            displayUser.History = Url.RouteUrl(nameof(HistoryController.GetAllHistoryOfUser));
            return Created("", displayUser);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetUser() {
            int.TryParse(HttpContext.User.Identity.Name, out var userId);
            var user = _dataService.GetUserById(userId);
            if (user == null) return NotFound();

            var displayUser = Mapper.Map<GetUserModel>(user);
            displayUser.Marks = Url.RouteUrl(nameof(MarkController.GetMarked));
            displayUser.History = Url.RouteUrl(nameof(HistoryController.GetAllHistoryOfUser));
            return Ok(displayUser);
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] UserLoginModel model) {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password)) return BadRequest();

            int.TryParse(_config["security:pwdsize"], out var size);
            if (size == 0) return BadRequest();

            var user = _dataService.GetUser(model.Username);
            if (user == null) return Unauthorized();

            var pwd = PasswordService.HashPassword(model.Password, user.Salt, size);
            if (pwd != user.Password) return Unauthorized();

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

        [Authorize]
        [HttpDelete]
        public IActionResult DeleteUser() {
            int.TryParse(HttpContext.User.Identity.Name, out var id);
            var user = _dataService.DeleteUser(id);

            if (user == false) return BadRequest();

            return Ok();
        }

        [Authorize]
        [HttpPut("update/email/{newEmail}")]
        public IActionResult UpdateEmail(string newEmail) {
            int.TryParse(HttpContext.User.Identity.Name, out var id);
            var updated = _dataService.UpdateEmail(id, newEmail);

            if (updated == false) return BadRequest();

            return Ok();
        }

        [HttpPut("update/username/{newUsername}")]
        public IActionResult UpdateUsername(string newUsername) {
            int.TryParse(HttpContext.User.Identity.Name, out var id);
            var updated = _dataService.UpdateUsername(id, newUsername);

            if (updated == false) return BadRequest();

            return Ok();
        }

        [Authorize]
        [HttpPut("update/password")]
        public IActionResult UpdatePassword([FromBody] UserUpdatePasswordModel model) {
            int.TryParse(HttpContext.User.Identity.Name, out var id);
            int.TryParse(_config["security:pwdsize"], out var size);
            var salt = PasswordService.GenerateSalt(size);
            var user = _dataService.GetUserById(id);
            var pwd = PasswordService.HashPassword(model.Password, salt, size);
            var updated = _dataService.UpdatePassword(id, pwd, salt);

            if (updated == false) return BadRequest();

            return Ok();
        }

        [Authorize]
        [HttpPut("update/location/{newLocation}")]
        public IActionResult UpdateLocation(string newLocation) {
            int.TryParse(HttpContext.User.Identity.Name, out var id);
            var updated = _dataService.UpdateLocation(id, newLocation);

            if (updated == false) return BadRequest();

            return Ok();
        }
    }
}