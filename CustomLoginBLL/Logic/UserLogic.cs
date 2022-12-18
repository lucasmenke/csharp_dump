using CustomLoginDAL.DataAccess;
using CustomLoginDAL.Models;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoginBLL.Logic;

public class UserLogic : IUserLogic
{
    private readonly IUserData _userData;

    public UserLogic(IUserData userData)
    {
        _userData = userData;
    }

    public async Task<AuthResponseDTOModel> RegisterUser(UserDTOModel request)
    {
        // check if username is still available
        var result = await _userData.GetUser(request);
        if (result != null)
        {
            return new AuthResponseDTOModel
            {
                Success = false,
                Message = "Username already exists."
            };
        }

        // methode gets the password as plaintext and return password hash & salt
        PasswordLogic.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

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
                Success = false,
                Message = "Failed to save user in database."
            };
        }

        return new AuthResponseDTOModel
        {
            Success = true,
            Message = "User successfully created."
        };
    }

}
