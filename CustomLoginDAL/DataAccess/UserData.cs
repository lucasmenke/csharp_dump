using CustomLoginDAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLoginDAL.DataAccess;

public class UserData : IUserData
{
    private readonly DataContext _context;
    private readonly IConfiguration _config;

    public UserData(DataContext _context, IConfiguration config)
    {
        _config = config;
    }

    // create
    public async Task<int> CreateUser(UserModel user)
    {
        _context.Users.Add(user);
        return await _context.SaveChangesAsync();
    }

    // read
    public async Task<UserModel> GetUser(UserDTOModel request)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
    }
}
