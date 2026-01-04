using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Identity;
using ProgresiumToDo.Application.Abstractions.OAuth;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Infrastructure.Configurations.Auth;
using ProgresiumToDo.Infrastructure.Identity;
using ProgresiumToDo.Infrastructure.Interceptors;
using ProgresiumToDo.Infrastructure.OAuth;
using ProgresiumToDo.Infrastructure.Repositories.Auth;
using ProgresiumToDo.Infrastructure.Repositories.Tasks;
using ProgresiumToDo.Infrastructure.Tasks;

namespace ProgresiumToDo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        
        AddPersistence(services, configuration);
        
        AddIdentity(services);
        
        AddAuthentication(services, configuration);
        
        AddAuthorization(services);
        
        AddRepositories(services);
        
        services.AddTransient<IEmailService, EmailService.EmailService>();

        services.AddTransient<ITaskOrderingService, TaskOrderingService>();
        
        services.AddTransient<ITaskStatusPolicy, TaskStatusPolicy>();
        
        return services;
    }
    
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database") 
                               ?? throw new ArgumentNullException(nameof(configuration), "Database connection string is missing.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new AuditableEntityInterceptor()));
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }
    
    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        
        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };
        });
        
        services.AddTransient<IIdentityService, IdentityService>();
    }
    
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                        throw new ApplicationException("Jwt secret is missing.");
        
        services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Authentication:Issuer"],
                    ValidAudience = configuration["Authentication:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                };
            });
        
        services.AddHttpContextAccessor();
        
        services.AddScoped<IUserContext, UserContext>();

        services.AddTransient<IOAuthService, OAuthService>();
    }
    
    private static void AddAuthorization(IServiceCollection services) {
        services.AddAuthorization();
    }
    
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        services.AddScoped<ITaskOrderRepository, TaskOrderRepository>();
        
        services.AddScoped<ITagRepository, TagRepository>();
    }
}