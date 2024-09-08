using locket.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace locket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppConfig _appConfig;

        public AuthController(AppConfig appConfig) { _appConfig = appConfig; }

        [HttpGet]
        public IActionResult TestApi()
        {
            return Ok("Ok man");
        }

        [HttpGet]
        [Authorize]
        [Route("private")]
        public IActionResult TestApiAuth()
        {
            return Ok("Ok auth man");
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginByUsername()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Option.JWTKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }
    }
}
