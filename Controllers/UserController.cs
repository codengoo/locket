using locket.Helpers;
using locket.Services;
using static locket.DTOs.UserDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace locket.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController(AppConfig appConfig, UserService userService) : ControllerBase
    {
        private readonly AppConfig _appConfig = appConfig;
        private readonly UserService _userService = userService;

        [HttpGet]
        public IActionResult GetUserByID([FromBody] IGetUsersByIDs body)
        {
            string[] users = _userService.GetUsersByIDs(body.Ids);
            return Ok(new ApiResponse<string[]>(users));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetUserByID(string id)
        {
            string? user = _userService.GetUserByID(id);
            return user == null
                ? throw new Exception("No user found")
                : (IActionResult)Ok(new ApiResponse<string>(user));
        }

        [HttpGet]
        [Route("me")]
        public IActionResult GetMe(string id)
        {
            string? token = Request.Cookies["Token"] ?? throw new Exception("Invalid token");



            string? user = JWTHandler.parseToken(_appConfig.Option.Authentication.JWTKey, token);
            return user == null
                ? throw new Exception("No user found")
                : (IActionResult)Ok(new ApiResponse<string>(user));
        }
    }
}
