using CustomLoginDAL.Models;

namespace CustomLoginBLL.Logic
{
    public interface IUserLogic
    {
        Task<AuthResponseDTOModel> LoginUser(UserDTOModel request);
        Task<AuthResponseDTOModel> RegisterUser(UserDTOModel request);
        Task<AuthResponseDTOModel> ChangeUsername(string newUsername);
        Task<AuthResponseDTOModel> RefreshToken();
        Task<AuthResponseDTOModel> ChangePassword(string newPassword);
    }
}