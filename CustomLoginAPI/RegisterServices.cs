using CustomLoginBLL.Logic;
using CustomLoginDAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace CustomLoginAPI;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
        builder.Services.AddScoped<IUserLogic, UserLogic>();
        builder.Services.AddScoped<IUserData, UserData>();
    }
}