using CustomLoginBLL.Logic;
using CustomLoginDAL.Models;
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
    }
}
