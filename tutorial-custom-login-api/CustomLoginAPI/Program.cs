var builder = WebApplication.CreateBuilder(args);

// outsourced dependency injection
builder.ConfigureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// middleware needed for authentication purpose
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();