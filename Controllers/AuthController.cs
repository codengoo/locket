using locket.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static locket.DTOs.AuthDto;

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

        [HttpPost("signin")]
        [Route("signin")]
        public IActionResult SignInByUsername([FromBody] ISignInByUsername body)
        {
            throw new Exception("Do an hai");
            //return Ok();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginByUsername()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Option.Authentication.JWTKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.WriteToken(token);

            return Ok(new { token = jwt });
        }

        [HttpGet("google")]
        public IActionResult SignInGoogle()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(authenticationProperties, "Google");
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest("Google authentication failed.");

            var userId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            //var jwtToken = GenerateJwtToken(userId);

            return Ok(new { Token = userId });
        }
    }
}
