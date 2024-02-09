using Microsoft.Extensions.Caching.Memory;
using System.Runtime.CompilerServices;

namespace SuggestionAppLibrary.DataAccess;

public class MongoSuggestionData : ISuggestionData
{
    private readonly IDbConnection _db;
    private readonly IUserData _userData;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<SuggestionModel> _suggestions;
    private const string CacheName = "SuggestionData";

    // caching the suggestions for a short time span to minimize db requests a little bit
    public MongoSuggestionData(IDbConnection db, IUserData userData, IMemoryCache cache)
    {
        _db = db;
        _userData = userData;
        _cache = cache;
        _suggestions = db.SuggestionCollection;
    }

    // gets all suggestions that are not archived
    public async Task<List<SuggestionModel>> GetAllSuggestions()
    {
        var output = _cache.Get<List<SuggestionModel>>(CacheName);
        if (output is null)
        {
            var result = await _suggestions.FindAsync(x => x.Archived == false);
            output = result.ToList();

            _cache.Set(CacheName, output, TimeSpan.FromMinutes(1));
        }
        return output;
    }

    // gets all the suggestion a user has made (archived & not archived once)
    public async Task<List<SuggestionModel>> GetUsersSuggestions(string userId)
    {
        var output = _cache.Get<List<SuggestionModel>>(userId);
        if (output == null)
        {
            var results = await _suggestions.FindAsync(s => s.Author.Id == userId);
            output = results.ToList();

            _cache.Set(userId, output, TimeSpan.FromMinutes(1));
        }

        return output;
    }

    // gets all approved suggestions that are not archived
    public async Task<List<SuggestionModel>> GetAllApprovedSuggestions()
    {
        // reuse of the cache
        var output = await GetAllSuggestions();
        return output.Where(x => x.ApprovedForRelease).ToList();
    }

    // gets one specific suggestion
    public async Task<SuggestionModel> GetSuggestion(string id)
    {
        // don't use cache because it`s a small request 
        var result = await _suggestions.FindAsync(x => x.Id == id);
        return result.FirstOrDefault();
    }

    // gets all suggestions that wait for approval & are not archived
    public async Task<List<SuggestionModel>> GetAllSuggestionsWaitingForApproval()
    {
        // reuse of the cache
        var output = await GetAllSuggestions();
        return output.Where(x => x.ApprovedForRelease == false && x.Rejected == false).ToList();
    }

    // updates an existing suggestion
    public async Task UpdateSuggestion(SuggestionModel suggestion)
    {
        await _suggestions.ReplaceOneAsync(x => x.Id == suggestion.Id, suggestion);
        // works for a small active group of people good but should be updated when the side grows
        _cache.Remove(CacheName);
    }

    // allows to upvote for a suggestion
    public async Task UpvoteSuggestion(string suggestionId, string userId)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();
        // creates a transaction which ensures that we write to different collection the write complety succeed or completly fails
        // start transaction
        session.StartTransaction();

        try
        {
            // 1. Part: add or remove votes of a suggestion

            // we need to use a new database instance
            // can't use the client db instance because the session needs it for the transaction
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            // no use of FirstOrDefault() because we want to have an exception if we don't find a suggestion
            // it doesn`t make sense to upvote on a default value
            var suggestion = (await suggestionsInTransaction.FindAsync(x => x.Id == suggestionId)).First();

            // UserVotes is a HashSet which dosen`t allow duplicate values
            // if the Add() fails it means, that the user already upvoted on this suggestion
            // that means the user wants to remove the given upvote
            bool isUpvote = suggestion.UserVotes.Add(userId);
            if (isUpvote == false)
            {
                suggestion.UserVotes.Remove(userId);
            }

            // updates the suggestion with the new version of it
            // needs the session to properly abort it when an error occurs
            await suggestionsInTransaction.ReplaceOneAsync(session, x => x.Id == suggestionId, suggestion);

            // 2.Part: add or remove the suggestions a user voted on into user model

            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUser(userId);

            if (isUpvote)
            {
                // fill the list of suggestions a user voated on with the new suggestion the user voted on
                user.VotedOnSuggestions.Add(new BasicSuggestionModel(suggestion));
            }
            else
            {
                // remove the suggestion from the list of suggestions a user voated
                var suggestionToRemove = user.VotedOnSuggestions.Where(x => x.Id == suggestionId).First();
                user.VotedOnSuggestions.Remove(suggestionToRemove);
            }
            await usersInTransaction.ReplaceOneAsync(session, x => x.Id == userId, user);

            // commit transaction
            await session.CommitTransactionAsync();

            _cache.Remove(CacheName);
        }
        catch (Exception ex)
        {
            // in case of an exception the session will be aborted -> not db data changed
            await session.AbortTransactionAsync();
            throw;
        }
    }

    // creates a new suggestion
    public async Task CreateSuggestion(SuggestionModel suggestion)
    {
        var client = _db.Client;

        using var session = await client.StartSessionAsync();

        session.StartTransaction();

        try
        {
            // 1.Part: create new suggestion
            var db = client.GetDatabase(_db.DbName);
            var suggestionsInTransaction = db.GetCollection<SuggestionModel>(_db.SuggestionCollectionName);
            await suggestionsInTransaction.InsertOneAsync(session, suggestion);

            // 2.Part: link the new suggestion to the user account
            var usersInTransaction = db.GetCollection<UserModel>(_db.UserCollectionName);
            var user = await _userData.GetUser(suggestion.Author.Id);
            user.AuthoredSuggestions.Add(new BasicSuggestionModel(suggestion));
            await usersInTransaction.ReplaceOneAsync(session, x => x.Id == user.Id, user);

            await session.CommitTransactionAsync();

            // no removal of the cache because the user can't see the suggestion until it gets approved by an admin
        }
        catch (Exception ex)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
