using dotenv.net;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.API.Extensions.ExceptionHandling;
using ProgresiumToDo.API.Filters;
using ProgresiumToDo.API.Extensions.RateLimiting;
using ProgresiumToDo.Application;
using ProgresiumToDo.Infrastructure;
using Scalar.AspNetCore;

DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Progresium ToDo API";
        document.Info.Version = "v1";
        document.Info.Description = "API documentation for the Progresium ToDo application.";
        
        return Task.CompletedTask;
    });
});

builder.Services.AddExceptionHandling();

builder.Services.AddMemoryCache();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCustomRateLimitings(builder.Configuration);

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

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = [new HangfireDashboardAuthorizationFilter(app.Environment)]
});

// Remove this block in future in production deployments
if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();

// Temporarily enable Swagger in all environments
// if (app.Environment.IsDevelopment())
// {
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Progresium ToDo API")
            .WithTheme(ScalarTheme.Moon)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
// }

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Frontend");

app.UseExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseRateLimiter();

app.Run();