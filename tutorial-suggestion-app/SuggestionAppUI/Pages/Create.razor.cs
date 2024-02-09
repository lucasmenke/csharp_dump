using SuggestionAppUI.Models;

namespace SuggestionAppUI.Pages;

public partial class Create
{
    private CreateSuggestionModel suggestion = new();
    private List<CategoryModel> categories;
    private UserModel loggedInUser;
    protected async override Task OnInitializedAsync()
    {
        categories = await categoryData.GetAllCategories();
        loggedInUser = await authProvider.GetUserFromAuth(userData);
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }

    private async Task CreateSuggestion()
    {
        // use the CreateSuggestionModel as a form (ui model) & map the input data to our DataAccess model
        SuggestionModel s = new();
        s.Suggestion = suggestion.Suggestion;
        s.Description = suggestion.Description;
        s.Author = new BasicUserModel(loggedInUser);
        s.Category = categories.Where(x => x.Id == suggestion.CategoryId).FirstOrDefault();
        if (s.Category == null)
        {
            suggestion.CategoryId = "";
            return;
        }

        await suggestionData.CreateSuggestion(s);
        suggestion = new();
        ClosePage();
    }
}