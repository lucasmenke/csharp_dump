namespace CustomLoginDAL.Models;

public class UserDTOModel
{
    // will be used for registering & logging in the user
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
