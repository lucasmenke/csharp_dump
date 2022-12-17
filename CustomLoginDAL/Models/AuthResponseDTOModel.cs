namespace CustomLoginDAL.Models;

public class AuthResponseDTOModel
{
    // will be used to transfer a response to the user
    // if the logging was successfull or not
    // + the refresh token
    public bool Success { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpires { get; set; }
}
