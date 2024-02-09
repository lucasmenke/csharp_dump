using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public class MongoUserData : IUserData
{
    private readonly IMongoCollection<UserModel> _users;

    public MongoUserData(IDbConnection db)
    {
        _users = db.UserCollection;
    }

    // returns all users
    public async Task<List<UserModel>> GetUsersAsync()
    {
        var result = await _users.FindAsync(_ => true);
        return result.ToList();
    }

    // gets specific user by id
    public async Task<UserModel> GetUser(string id)
    {
        var result = await _users.FindAsync(x => x.Id == id);
        return result.FirstOrDefault();
    }

    // gets specific user by objectId (used for Azure)
    public async Task<UserModel> GetUserFromAuthentification(string objectId)
    {
        var result = await _users.FindAsync(x => x.ObjectIdentifier == objectId);
        return result.FirstOrDefault();
    }

    // creates a new user
    public Task CreateUser(UserModel user)
    {
        return _users.InsertOneAsync(user);
    }

    // updtes user attributes
    public Task UpdateUser(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
        // IsUpsert = true inserts a new user if it doesn't find an existing entry
        return _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}

