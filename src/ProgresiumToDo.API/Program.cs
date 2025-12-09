using dotenv.net;
using Microsoft.OpenApi.Models;
using ProgresiumToDo.API.Extensions;
using ProgresiumToDo.Application;
using ProgresiumToDo.Infrastructure;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Progresium ToDo API",
        Version = "v1",
        Description = "API documentation for the Progresium ToDo application."
    });
});

builder.Services.AddExceptionHandling();

builder.Services.AddMemoryCache();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Progresium ToDo API v1");
    });
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();