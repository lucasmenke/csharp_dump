namespace CustomLoginDAL.Models;

public class RefreshTokenModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public DateTime Expires { get; set; }
}
