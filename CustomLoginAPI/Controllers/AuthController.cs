using CustomLoginBLL.Logic;
using CustomLoginDAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        
        [HttpPost]
        public async Task<ActionResult<AuthResponseDTOModel>> RegisterUser(UserDTOModel request)
        {
           return await _userLogic.RegisterUser(request);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDTOModel>> Login(UserDTOModel request)
        {
            return await _userLogic.LoginUser(request);
        }

        [HttpPost("change-username"), Authorize]
        public async Task<ActionResult<AuthResponseDTOModel>> ChangeUsername(string newUsername)
        {
            return await _userLogic.ChangeUsername(newUsername);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDTOModel>> RefreshToken()
        {
            return await _userLogic.RefreshToken();
        }
    }
}
