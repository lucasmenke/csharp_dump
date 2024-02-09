using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace CustomLoginAPI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        builder.Services.AddScoped<IUserLogic, UserLogic>();
        builder.Services.AddScoped<IPasswordLogic, PasswordLogic>();
        builder.Services.AddScoped<ITokenLogic, TokenLogic>();
        builder.Services.AddScoped<IUserData, UserData>();
        // needed to get the HttpContext in the TokenLogic.cs
        builder.Services.AddHttpContextAccessor();
        // customize swagger to create the possibility to give swagger an authorization token
        // so endpoints that require authorization (change-username) can be tested
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                // in swagger we have to enter the value like this: Bearer {token}
                // just {token} won't work
                Description = "Standard Authorization header using the Bearer scheme(\"Bearer {token} \")",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            });

            options.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        // needed for endpoints that require authorization (change-username)
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Key").Value)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
    }
}