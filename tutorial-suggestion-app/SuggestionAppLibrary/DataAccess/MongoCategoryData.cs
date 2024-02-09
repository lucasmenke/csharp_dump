using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace SuggestionAppLibrary.DataAccess;

public class MongoCategoryData : ICategoryData
{
    private readonly IMongoCollection<CategoryModel> _categories;
    private readonly IMemoryCache _cache;
    private const string CacheName = "CategoryData";

    // using IMemoryCache because the categories doesn't change at all
    public MongoCategoryData(IDbConnection db, IMemoryCache cache)
    {
        _categories = db.CategoryCollection;
        _cache = cache;
    }

    public async Task<List<CategoryModel>> GetAllCategories()
    {
        // asks the cache for the cached data with the key "CategoryData"
        var output = _cache.Get<List<CategoryModel>>(CacheName);
        // if their is no cached data then get it from the db
        if (output is null)
        {
            // get every category
            var results = await _categories.FindAsync(_ => true);
            output = results.ToList();

            // sets & keeps the output in the cache for 1 day
            _cache.Set(CacheName, output, TimeSpan.FromDays(1));
        }
        return output;
    }

    public Task CreateCategory(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
