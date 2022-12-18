using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomLoginAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserLogic _userLogic;
        
        public AuthController(IUserLogic userLogic)
        {
            _userLogic = userLogic;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDTOModel>> RegisterUser(UserDTOModel request)
        {
           return await _userLogic.RegisterUser(request);
        }

        [HttpPost("delete")]
        public async Task<ActionResult<AuthResponseDTOModel>> DeleteUser(UserDTOModel request)
        {
            return await _userLogic.DeleteUser(request);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTOModel>> LoginUser(UserDTOModel request)
        {
            return await _userLogic.LoginUser(request);
        }

        [HttpPost("change-username"), Authorize]
        public async Task<ActionResult<AuthResponseDTOModel>> ChangeUsername(string newUsername)
        {
            return await _userLogic.ChangeUsername(newUsername);
        }

        [HttpPost("change-password"), Authorize]
        public async Task<ActionResult<AuthResponseDTOModel>> ChangePassword(string newPassword)
        {
            return await _userLogic.ChangePassword(newPassword);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDTOModel>> RefreshToken()
        {
            return await _userLogic.RefreshToken();
        }
    }
}
