namespace CustomLoginBLL.Logic;

public interface ITokenLogic
{
    RefreshTokenModel CreateRefreshToken();
    string CreateToken(UserModel user);
    UserModel SetRefreshToken(RefreshTokenModel refreshToken, UserModel user);
    string GetRefreshToken();
}