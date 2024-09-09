using locket.Helpers;
using locket.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static locket.DTOs.AuthDto;

namespace locket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController(AppConfig appConfig, AuthService authService) : ControllerBase
    {
        private readonly AppConfig _appConfig = appConfig;
        private readonly AuthService _authService = authService;

        [Authorize]
        [HttpGet("private")]
        public IActionResult TestApiAuth()
        {
            return Ok("Ok auth man");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginByUsername([FromBody] ILoginByUsername body)
        {
            Guid? UserID = await _authService.FindUserByUsername(body.Username, body.Password);
            if (UserID == null)
            {
                throw new Exception("Invalid password");
            }
            else
            {
                JWTPayload payload = new()
                {
                    UserID = UserID.ToString()!
                };
                IJWTResponse response = JWTHandler.generateToken(payload, _appConfig.Option.Authentication.JWTKey);
                CookieResponse.AddTokenCookie(Response, response.Token);
                return Ok(new ApiResponse<IJWTResponse>(response));
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInByUsername([FromBody] ISignInByUsername body)
        {
            Guid Uid = await _authService.InsertUserByUsername(body.Username, body.Password);

            IReturnSignInByUser response = new()
            {
                Uid = Uid,
                Username = body.Username
            };
            return Ok(new ApiResponse<IReturnSignInByUser>(response));
        }

        [HttpGet("google")]
        public IActionResult SignInGoogle()
        {
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            };
            return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                throw new Exception("Google authentication failed.");

            string googleID = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
            string username = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email) ?? googleID;
            string? displayName = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);

            Guid? guid = await _authService.FindUserByGoogleID(googleID);
            if (guid == null)
            {
                Guid Uid = await _authService.InsertUserByUsername(username, null, googleID, displayName);

                IReturnSignInByUser response = new()
                {
                    Uid = Uid,
                    Username = username
                };

                return Ok(new ApiResponse<IReturnSignInByUser>(response));
            } else
            {
                JWTPayload payload = new()
                {
                    UserID = guid.ToString()!
                };
                IJWTResponse response = JWTHandler.generateToken(payload, _appConfig.Option.Authentication.JWTKey);
                CookieResponse.AddTokenCookie(Response, response.Token);
                return Ok(new ApiResponse<IJWTResponse>(response));
            }
        }
    }
}
