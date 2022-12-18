using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CustomLoginBLL.Logic;

public class TokenLogic : ITokenLogic
{
    private readonly IConfiguration _config;
    private readonly IHttpContextAccessor _httpContext;

    public TokenLogic(IConfiguration config, IHttpContextAccessor httpContext)
    {
        _config = config;
        _httpContext = httpContext;
    }

    public string CreateToken(UserModel user)
    {
        // claims are informatins of the user that will be stored in the web token
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config.GetSection("AppSettings:Key").Value));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            // short expiration time to enhance the security
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public RefreshTokenModel CreateRefreshToken()
    {
        var refreshToken = new RefreshTokenModel
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.Now.AddDays(7),
            Created = DateTime.Now
        };

        return refreshToken;
    }

    // sets Cookie with refresh token in the response
    public UserModel SetRefreshToken(RefreshTokenModel refreshToken, UserModel user)
    {
        var cookieOptions = new CookieOptions
        {
            // only accesible via requests and not via JS
            HttpOnly = true,
            Expires = refreshToken.Expires,
        };

        _httpContext?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);

        user.RefreshToken = refreshToken.Token;
        user.TokenCreated = refreshToken.Created;
        user.TokenExpires = refreshToken.Expires;

        return user;
    }

    public string GetRefreshToken()
    {
        return _httpContext?.HttpContext?.Request.Cookies["refreshToken"];
    }
}
