using static Locket.UserLocket.DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Locket.UserLocket.Helpers;
using Locket.UserLocket.Services;

namespace Locket.UserLocket.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(AppConfig appConfig, UserService userService) : ControllerBase
    {
        private readonly AppConfig _appConfig = appConfig;
        private readonly UserService _userService = userService;

        [HttpGet("ids")]
        public IActionResult GetUserByID([FromBody] IGetUsersByIDs body)
        {
            string[] users = _userService.GetUsersByIDs(body.Ids);
            return Ok(new ApiResponse<string[]>(users));
        }

        [HttpGet("{id}/name")]
        public IActionResult GetUserByID(string id)
        {
            string? user = _userService.GetUserByID(id);
            return user == null
                ? throw new Exception("No user found")
                : (IActionResult)Ok(new ApiResponse<string>(user));
        }

        [HttpGet("me")]
        public IActionResult GetMe()
        {
            string? token = Request.Cookies["Token"] ?? throw new Exception("Invalid token");
            IJWTPayload user = JWTHandler.parseToken(_appConfig.Option.Authentication.JWTKey, token);

            return user == null
                ? throw new Exception("No user found")
                : (IActionResult)Ok(new ApiResponse<IJWTPayload>(user));
        }
    }
}
