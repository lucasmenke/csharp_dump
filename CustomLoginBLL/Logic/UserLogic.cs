using CustomLoginDAL.DataAccess;
using CustomLoginDAL.Models;

namespace CustomLoginBLL.Logic;

public class UserLogic : IUserLogic
{
    private readonly IUserData _userData;
    private readonly IPasswordLogic _password;
    private readonly ITokenLogic _token;

    public UserLogic(IUserData userData, IPasswordLogic password, ITokenLogic token)
    {
        _userData = userData;
        _password = password;
        _token = token;
    }

    public async Task<AuthResponseDTOModel> RegisterUser(UserDTOModel request)
    {
        // check if username is still available
        var result = await _userData.GetUser(request);
        if (result != null)
        {
            return new AuthResponseDTOModel
            {
                Message = "Username already exists."
            };
        }

        // methode gets the password as plaintext and return password hash & salt
        _password.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

        var user = new UserModel
        {
            Username = request.Username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            Role = "User"
        };

        // save user in database
        int response = await _userData.CreateUser(user);
        if (response == 0)
        {
            return new AuthResponseDTOModel
            {
                Message = "Failed to save user in database."
            };
        }

        return new AuthResponseDTOModel
        {
            Success = true,
            Message = "User successfully created."
        };
    }

    public async Task<AuthResponseDTOModel> LoginUser(UserDTOModel request)
    {
        // check if user is registered
        var user = await _userData.GetUser(request);
        if (user == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "User not found."
            };
        }

        if (!_password.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return new AuthResponseDTOModel 
            { 
                Message = "Wrong password." 
            };
        }

        string token = _token.CreateToken(user);
        var refreshToken = _token.CreateRefreshToken();
        var updatedUser = _token.SetRefreshToken(refreshToken, user);

        int response = await _userData.UpdateUser(updatedUser);
        if (response == 0)
        {
            return new AuthResponseDTOModel
            {
                Message = "Failed to update user."
            };
        }

        return new AuthResponseDTOModel
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken.Token,
            TokenExpires = refreshToken.Expires,
            Message = "Succesfully logged in user."
        };
    }
}
