using CustomLoginDAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CustomLoginDAL.DataAccess;

public class UserData : IUserData
{
    private readonly DataContext _context;

    public UserData(DataContext context)
    {
        _context = context;
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

    // update
    public async Task<int> UpdateUser(UserModel user)
    {
        return await _context.SaveChangesAsync();
    }

    // delete
}
