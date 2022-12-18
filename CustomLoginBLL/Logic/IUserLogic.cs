using CustomLoginDAL.Models;

namespace CustomLoginBLL.Logic
{
    public interface IUserLogic
    {
        Task<AuthResponseDTOModel> LoginUser(UserDTOModel request);
        Task<AuthResponseDTOModel> RegisterUser(UserDTOModel request);
    }
}