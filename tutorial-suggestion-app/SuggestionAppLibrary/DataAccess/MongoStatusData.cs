using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess;

public class MongoStatusData : IStatusData
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;
    private const string CacheName = "StatusData";

    // using IMemoryCache because the categories doesn't change at all
    public MongoStatusData(IDbConnection db, IMemoryCache cache)
    {
        _statuses = db.StatusCollection;
        _cache = cache;
    }

    public async Task<List<StatusModel>> GetAllStatuses()
    {
        // asks the cache for the cached data with the key "StatusData"
        var output = _cache.Get<List<StatusModel>>(CacheName);
        // if their is no cached data then get it from the db
        if (output is null)
        {
            // get every status
            var results = await _statuses.FindAsync(_ => true);
            output = results.ToList();
            // sets & keeps the output in the cache for 1 day
            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }
        return output;
    }

    public Task CreateStatus(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }
}
