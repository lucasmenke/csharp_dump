namespace SuggestionAppUI.Pages;

public partial class Index
{
    private UserModel loggedInUser;
    private List<SuggestionModel> suggestions;
    private List<CategoryModel> categories;
    private List<StatusModel> statuses;
    private SuggestionModel archivingSuggestion;
    private string selectedCategory = "All";
    private string selectedStatus = "All";
    private string searchText = "";
    private bool isSortedByNew = true;
    private bool showCategories = false;
    private bool showStatuses = false;
    // page gets renderd twice -> methode gets called twice
    // 1. prerendered on the server Pages\_Host.cshtml -> <component type="typeof(App)" render-mode="ServerPrerendered" />
    // 2. rerendered on the client side
    // prerendering takes care of not showing an empty page & client side rendering makes page usable
    protected override async Task OnInitializedAsync()
    {
        categories = await categoryData.GetAllCategories();
        statuses = await statusData.GetAllStatuses();
        await LoadAndVerifyUser();
    }

    // navigates to create or login page
    private void LoadCreatePage()
    {
        if (loggedInUser != null)
        {
            navManager.NavigateTo("/Create");
        }
        else
        {
            navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }

    // creates or updates user to mongodb database
    private async Task LoadAndVerifyUser()
    {
        var authState = await authProvider.GetAuthenticationStateAsync();
        string objectId = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("objectidentifier"))?.Value;
        // check if user is logged in
        if (string.IsNullOrWhiteSpace(objectId) == false)
        {
            loggedInUser = await userData.GetUserFromAuthentification(objectId) ?? new();
            string firstName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value;
            string lastName = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("surname"))?.Value;
            string displayName = authState.User.Claims.FirstOrDefault(c => c.Type.Equals("name"))?.Value;
            string email = authState.User.Claims.FirstOrDefault(c => c.Type.Contains("email"))?.Value;
            bool userInfosChanged = false;
            if (objectId.Equals(loggedInUser.ObjectIdentifier) == false)
            {
                userInfosChanged = true;
                loggedInUser.ObjectIdentifier = objectId;
            }

            if (firstName.Equals(loggedInUser.FirstName) == false)
            {
                userInfosChanged = true;
                loggedInUser.FirstName = firstName;
            }

            if (lastName.Equals(loggedInUser.LastName) == false)
            {
                userInfosChanged = true;
                loggedInUser.LastName = lastName;
            }

            if (displayName.Equals(loggedInUser.DisplayName) == false)
            {
                userInfosChanged = true;
                loggedInUser.DisplayName = displayName;
            }

            if (email.Equals(loggedInUser.EmailAddress) == false)
            {
                userInfosChanged = true;
                loggedInUser.EmailAddress = email;
            }

            if (userInfosChanged)
            {
                // user doesnt exist in mongo and needs to be added
                if (string.IsNullOrWhiteSpace(loggedInUser.Id))
                {
                    await userData.CreateUser(loggedInUser);
                }
                else
                {
                    await userData.UpdateUser(loggedInUser);
                }
            }
        }
    }

    // page gets renderd twice -> methode gets called twice
    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        // only true after the first render of the page
        if (firstRender)
        {
            // gets information from the session storage of the client which is only available after the page has been renderd
            await LoadFilterState();
            await FilterSuggestions();
            // triggers a new render -> endless loop if methode is called outside the if statement
            StateHasChanged();
        }
    }

    // loads session data (filters)
    private async Task LoadFilterState()
    {
        var stringResults = await sessionStorage.GetAsync<string>(nameof(selectedCategory));
        selectedCategory = stringResults.Success ? stringResults.Value : "All";
        stringResults = await sessionStorage.GetAsync<string>(nameof(selectedStatus));
        selectedStatus = stringResults.Success ? stringResults.Value : "All";
        stringResults = await sessionStorage.GetAsync<string>(nameof(searchText));
        searchText = stringResults.Success ? stringResults.Value : "";
        var boolResults = await sessionStorage.GetAsync<bool>(nameof(isSortedByNew));
        isSortedByNew = stringResults.Success ? boolResults.Value : true;
    }

    // sets session data (filters)
    private async Task SaveFilterState()
    {
        // key value pairs
        await sessionStorage.SetAsync(nameof(selectedCategory), selectedCategory);
        await sessionStorage.SetAsync(nameof(selectedStatus), selectedStatus);
        await sessionStorage.SetAsync(nameof(searchText), searchText);
        await sessionStorage.SetAsync(nameof(isSortedByNew), isSortedByNew);
    }

    // filters all approved suggestions based on the loaded filters
    private async Task FilterSuggestions()
    {
        var output = await suggestionData.GetAllApprovedSuggestions();
        if (selectedCategory != "All")
        {
            output = output.Where(x => x.Category?.CategoryName == selectedCategory).ToList();
        }

        if (selectedStatus != "All")
        {
            output = output.Where(x => x.SuggestionStatus?.StatusName == selectedStatus).ToList();
        }

        if (string.IsNullOrWhiteSpace(searchText) == false)
        {
            output = output.Where(x => x.Suggestion.Contains(searchText, StringComparison.InvariantCultureIgnoreCase) || x.Description.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }

        if (isSortedByNew)
        {
            output = output.OrderByDescending(x => x.DateCreated).ToList();
        }
        else
        {
            output = output.OrderByDescending(x => x.UserVotes.Count).ThenByDescending(x => x.DateCreated).ToList();
        }

        suggestions = output;
        await SaveFilterState();
    }

    // toggles the isSortedByNew variable (new filter)
    private async Task OrderByNew(bool isNew)
    {
        isSortedByNew = isNew;
        await FilterSuggestions();
    }

    // filters the suggestions when search text is getting entered in the search bar
    private async Task OnSearchInput(string searchInput)
    {
        searchText = searchInput;
        await FilterSuggestions();
    }

    // filters the suggestions when a categorty is selected
    private async Task OnCategoryClick(string category = "All")
    {
        selectedCategory = category;
        showCategories = false;
        await FilterSuggestions();
    }

    // filters the suggestions when a status is selected
    private async Task OnStatusClick(string status = "All")
    {
        selectedStatus = status;
        showStatuses = false;
        await FilterSuggestions();
    }

    private async Task VoteUp(SuggestionModel suggestion)
    {
        if (loggedInUser != null)
        {
            if (suggestion.Author.Id == loggedInUser.Id)
            {
                // Can't vote on own suggestion
                return;
            }

            // false means that the user already voted on the object & a hashset only contains unique values
            if (suggestion.UserVotes.Add(loggedInUser.Id) == false)
            {
                // that means the user wants to remove their given vote
                suggestion.UserVotes.Remove(loggedInUser.Id);
            }

            await suggestionData.UpvoteSuggestion(suggestion.Id, loggedInUser.Id);
            // update popularity ranking
            if (isSortedByNew == false)
            {
                suggestions = suggestions.OrderByDescending(s => s.UserVotes.Count).ThenByDescending(s => s.DateCreated).ToList();
            }
        }
        else
        {
            // foreced load (true) ensures to go to external page
            navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }

    // returns the upvote text next to the suggestion
    private string GetUpvoteTopText(SuggestionModel suggestion)
    {
        if (suggestion.UserVotes?.Count > 0)
        {
            return suggestion.UserVotes.Count.ToString("00");
        }
        else
        {
            if (suggestion.Author.Id == loggedInUser?.Id)
            {
                return "Awaiting";
            }
            else
            {
                return "Click To";
            }
        }
    }

    // returns the upvote text next to the suggestion
    private string GetUpvoteBottomText(SuggestionModel suggestion)
    {
        if (suggestion.UserVotes?.Count > 1)
        {
            return "Upvotes";
        }
        else
        {
            return "Upvote";
        }
    }

    // opens the details page when clicked onto a suggestion
    private void OpenDetails(SuggestionModel suggestion)
    {
        navManager.NavigateTo($"/Details/{suggestion.Id}");
    }

    private async Task ArchiveSuggestion()
    {
        archivingSuggestion.Archived = true;
        await suggestionData.UpdateSuggestion(archivingSuggestion);
        suggestions.Remove(archivingSuggestion);
        archivingSuggestion = null;
    }

    // return css class
    private string SortedByNewClass(bool isNew)
    {
        if (isNew == isSortedByNew)
        {
            return "sort-selected";
        }
        else
        {
            return "";
        }
    }

    // return css class
    private string GetVoteClass(SuggestionModel suggestion)
    {
        if (suggestion.UserVotes == null || suggestion.UserVotes.Count == 0)
        {
            return "suggestion-entry-no-votes";
        }
        else if (suggestion.UserVotes.Contains(loggedInUser?.Id))
        {
            return "suggestion-entry-voted";
        }
        else
        {
            return "suggestion-entry-not-voted";
        }
    }

    // return css class
    private string GetSuggestionStatusClass(SuggestionModel suggestion)
    {
        if (suggestion == null | suggestion.SuggestionStatus == null)
        {
            return "suggstion-entry-status-none";
        }

        string output = suggestion.SuggestionStatus.StatusName switch
        {
            "Completed" => "suggstion-entry-status-completed",
            "Watching" => "suggstion-entry-status-watching",
            "Upcoming" => "suggstion-entry-status-upcoming",
            "Dismissed" => "suggstion-entry-status-dismissed",
            _ => "suggstion-entry-status-completed",
        };
        return output;
    }

    // return css class
    private string GetSelectedCategory(string category = "All")
    {
        if (category == selectedCategory)
        {
            return "selected-category";
        }
        else
        {
            return "";
        }
    }

    // return css class
    private string GetSelectedStatus(string status = "All")
    {
        if (status == selectedStatus)
        {
            return "selected-status";
        }
        else
        {
            return "";
        }
    }
}