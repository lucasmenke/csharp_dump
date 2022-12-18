using CustomLoginDAL.Models;

namespace CustomLoginDAL.DataAccess
{
    public interface IUserData
    {
        Task<int> CreateUser(UserModel user);
        Task<UserModel> GetUser(UserDTOModel request);
        Task<int> UpdateUser(UserModel user);
    }
}