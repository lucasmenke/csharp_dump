using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

// connection to db will be opened once because the class will be used as a singleton -> better performance
// when the connection should be openend more often the singleton can be changed
// to a scoped class, where in the dependency injection every user gets a version of it
public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    // is private readonly because the db should just be used internaly
    private readonly IMongoDatabase _db;
    private string _connectionId = "MongoDB";

    // privat setter means that these attributes can be set directly like "CategoryCollectionName"
    // or in the constructor
    public string DbName { get; private set; }
    public string CategoryCollectionName { get; private set; } = "categories";
    public string StatusCollectionName { get; private set; } = "statuses";
    public string SuggestionCollectionName { get; private set; } = "suggestions";
    public string UserCollectionName { get; private set; } = "users";

    // public because external classes need to connect to the collection
    public MongoClient Client { get; private set; }
    public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }
    public IMongoCollection<StatusModel> StatusCollection { get; private set; }
    public IMongoCollection<SuggestionModel> SuggestionCollection { get; private set; }
    public IMongoCollection<UserModel> UserCollection { get; private set; }


    public DbConnection(IConfiguration config)
    {
        _config = config;
        Client = new MongoClient(_config.GetConnectionString(_connectionId));
        DbName = _config["DatabaseName"];
        _db = Client.GetDatabase(DbName);

        CategoryCollection = _db.GetCollection<CategoryModel>(CategoryCollectionName);
        StatusCollection = _db.GetCollection<StatusModel>(StatusCollectionName);
        SuggestionCollection = _db.GetCollection<SuggestionModel>(SuggestionCollectionName);
        UserCollection = _db.GetCollection<UserModel>(UserCollectionName);
    }
}