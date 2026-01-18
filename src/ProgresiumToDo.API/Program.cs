using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
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

// var origins = (Environment.GetEnvironmentVariable("CORS_ORIGINS") ??
//                throw new ApplicationException("CORS origins secret is missing."))
//     .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        // policy
        //     .WithOrigins(origins)
        //     .AllowAnyHeader()
        //     .AllowAnyMethod()
        //     .AllowCredentials();
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Remove this block in future in production deployments
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Temporarily enable Swagger in all environments
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Progresium ToDo API v1");
    });
// }

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Frontend");

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();