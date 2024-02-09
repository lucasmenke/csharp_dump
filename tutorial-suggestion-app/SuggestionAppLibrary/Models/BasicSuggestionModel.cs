namespace SuggestionAppLibrary.Models;

// this model is just a submodel from the UserModel and will not have his own collection
// with this submodel we create duplicate data but that is no problem because we have a
// Single Source of Truth -> SuggestionModel 
public class BasicSuggestionModel
{
    // id for linking it to the UserModel
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Suggestion { get; set; }

    public BasicSuggestionModel()
    {

    }

    // constructor for converting complex SuggestionModel to BasicSuggestionModel
    public BasicSuggestionModel(SuggestionModel suggestion)
    {
        Id = suggestion.Id;
        Suggestion = suggestion.Suggestion;
    }
}

