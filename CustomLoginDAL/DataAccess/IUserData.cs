namespace CustomLoginDAL.DataAccess;

public interface IUserData
{
    Task<int> CreateUser(UserModel user);
    Task<UserModel> GetUser(UserDTOModel request);
    Task<int> UpdateUser(UserModel user);
    Task<UserModel> GetUserByrefreshToken(string refreshToken);
    Task<int> DeleteUser(UserModel user);
}