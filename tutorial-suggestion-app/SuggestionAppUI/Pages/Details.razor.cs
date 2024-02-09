using Microsoft.AspNetCore.Components;

namespace SuggestionAppUI.Pages;

public partial class Details
{
    // Parameter passed by the url {Id}
    [Parameter]
    public string Id { get; set; }

    private UserModel loggedInUser;
    private SuggestionModel suggestion;
    private List<StatusModel> statuses;
    private string settingStatus = "";
    private string urlText = "";
    protected async override Task OnInitializedAsync()
    {
        suggestion = await suggestionData.GetSuggestion(Id);
        loggedInUser = await authProvider.GetUserFromAuth(userData);
        statuses = await statusData.GetAllStatuses();
    }

    private async Task CompleteSetStatus()
    {
        switch (settingStatus)
        {
            case "completed":
                if (string.IsNullOrWhiteSpace(urlText))
                {
                    return;
                }

                suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                suggestion.OwnerNotes = $"Here is our finished resource about it: <a class='color-darkgreen' href='{urlText}' target='_blank'>{urlText}</a>";
                break;
            case "watching":
                suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                suggestion.OwnerNotes = "The topic needs some more traction to be addressed.";
                break;
            case "upcoming":
                suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                suggestion.OwnerNotes = "We have a resource in our pipeline.";
                break;
            case "dismissed":
                suggestion.SuggestionStatus = statuses.Where(s => s.StatusName.ToLower() == settingStatus.ToLower()).First();
                suggestion.OwnerNotes = "Your idea doesn't fit.";
                break;
        }

        settingStatus = null;
        await suggestionData.UpdateSuggestion(suggestion);
    }

    private void ClosePage()
    {
        navManager.NavigateTo("/");
    }

    private string GetUpvoteTopText()
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

    private async Task VoteUp()
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
        }
        else
        {
            // foreced load (true) ensures to go to external page
            navManager.NavigateTo("/MicrosoftIdentity/Account/SignIn", true);
        }
    }

    private string GetUpvoteBottomText()
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

    private string GetVoteClass()
    {
        if (suggestion.UserVotes == null || suggestion.UserVotes.Count == 0)
        {
            return "suggestion-detail-no-votes";
        }
        else if (suggestion.UserVotes.Contains(loggedInUser?.Id))
        {
            return "suggestion-detail-voted";
        }
        else
        {
            return "suggestion-detail-not-voted";
        }
    }

    private string GetStatusClass()
    {
        if (suggestion == null | suggestion.SuggestionStatus == null)
        {
            return "suggstion-detail-status-none";
        }

        string output = suggestion.SuggestionStatus.StatusName switch
        {
            "Completed" => "suggstion-detail-status-completed",
            "Watching" => "suggstion-detail-status-watching",
            "Upcoming" => "suggstion-detail-status-upcoming",
            "Dismissed" => "suggstion-detail-status-dismissed",
            _ => "suggstion-detail-status-completed",
        };
        return output;
    }
}