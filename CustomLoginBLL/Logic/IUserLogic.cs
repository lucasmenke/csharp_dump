using CustomLoginDAL.Models;

namespace CustomLoginBLL.Logic
{
    public interface IUserLogic
    {
        Task<AuthResponseDTOModel> RegisterUser(UserDTOModel request);
    }
}