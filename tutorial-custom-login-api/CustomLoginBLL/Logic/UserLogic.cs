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

    public async Task<AuthResponseDTOModel> DeleteUser(UserDTOModel request)
    {
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

        var result = await _userData.DeleteUser(user);
        if (result == 0)
        {
            return new AuthResponseDTOModel
            {
                Message = "Failed to delete user."
            };
        }

        return new AuthResponseDTOModel
        {
            Success = true,
            Message = "User successfully deleted."
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

    public async Task<AuthResponseDTOModel> ChangeUsername(string newUsername)
    {
        // check if user is authorized
        var refreshToken = _token.GetRefreshToken();
        if (refreshToken == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "No Refresh token provided."
            };
        }
        var user = await _userData.GetUserByrefreshToken(refreshToken);
        if (user == null)
        {
            return new AuthResponseDTOModel 
            { 
                Message = "Invalid Refresh Token." 
            };
        }
        else if (user.TokenExpires < DateTime.Now)
        {
            return new AuthResponseDTOModel 
            { 
                Message = "Token already expired." 
            };
        }

        // change username
        user.Username = newUsername;
        var response = await _userData.UpdateUser(user);
        if (response == 0)
        {
            return new AuthResponseDTOModel 
            { 
                Message = "Failed to change the username." 
            };
        }

        return new AuthResponseDTOModel 
        { 
            Success = true,
            Message = "Username succesfully changed." 
        };
    }

    public async Task<AuthResponseDTOModel> ChangePassword(string newPassword)
    {
        // check if user is authorized
        var refreshToken = _token.GetRefreshToken();
        if (refreshToken == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "No Refresh token provided."
            };
        }
        var user = await _userData.GetUserByrefreshToken(refreshToken);
        if (user == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "Invalid Refresh Token."
            };
        }
        else if (user.TokenExpires < DateTime.Now)
        {
            return new AuthResponseDTOModel
            {
                Message = "Token already expired."
            };
        }

        // change password
        _password.CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
        user.PasswordHash = passwordHash;
        user.PasswordSalt = passwordSalt;

        var response = await _userData.UpdateUser(user);
        if (response == 0)
        {
            return new AuthResponseDTOModel
            {
                Message = "Failed to change the password."
            };
        }

        return new AuthResponseDTOModel
        {
            Success = true,
            Message = "Password succesfully changed."
        };
    }

    public async Task<AuthResponseDTOModel> RefreshToken()
    {
        // every request contains the old token when cookies don't get deleted
        var refreshToken = _token.GetRefreshToken();
        if (refreshToken == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "No Refresh token provided."
            };
        }
        var user = await _userData.GetUserByrefreshToken(refreshToken);
        if (user == null)
        {
            return new AuthResponseDTOModel
            {
                Message = "Invalid Refresh Token."
            };
        }
        else if (user.TokenExpires < DateTime.Now)
        {
            return new AuthResponseDTOModel
            {
                Message = "Token already expired."
            };
        }

        string token = _token.CreateToken(user);
        var newRefreshToken = _token.CreateRefreshToken();
        var updatedUser = _token.SetRefreshToken(newRefreshToken, user);

        return new AuthResponseDTOModel
        {
            Success = true,
            Token = token,
            RefreshToken = newRefreshToken.Token,
            TokenExpires = newRefreshToken.Expires,
            Message = "New Refresh Token created."
        };
    }
}
