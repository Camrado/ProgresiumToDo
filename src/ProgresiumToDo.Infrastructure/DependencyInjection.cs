using System.Net.Http.Headers;
using System.Text;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProgresiumToDo.Application.Abstractions.Auth.Entitlement;
using ProgresiumToDo.Application.Abstractions.Auth.Identity;
using ProgresiumToDo.Application.Abstractions.Auth.OAuth;
using ProgresiumToDo.Application.Abstractions.Auth.Onboarding;
using ProgresiumToDo.Application.Abstractions.Auth.Tokens;
using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;
using ProgresiumToDo.Application.Abstractions.Billing;
using ProgresiumToDo.Application.Abstractions.BackgroundJobs;
using ProgresiumToDo.Application.Abstractions.EmailService;
using ProgresiumToDo.Application.Abstractions.Tags;
using ProgresiumToDo.Application.Abstractions.Tasks;
using ProgresiumToDo.Application.Auth.Repositories;
using ProgresiumToDo.Application.Billing.Repositories;
using ProgresiumToDo.Application.Projects.Repositories;
using ProgresiumToDo.Application.Tags.Repositories;
using ProgresiumToDo.Application.Tasks.Repositories;
using ProgresiumToDo.Application.Users.Repositories;
using ProgresiumToDo.Application.Waitlist.Repositories;
using ProgresiumToDo.Infrastructure.Interceptors;
using ProgresiumToDo.Infrastructure.Repositories.Auth;
using ProgresiumToDo.Infrastructure.Repositories.Billing;
using ProgresiumToDo.Infrastructure.Repositories.Projects;
using ProgresiumToDo.Infrastructure.Repositories.Tags;
using ProgresiumToDo.Infrastructure.Repositories.Tasks;
using ProgresiumToDo.Infrastructure.Repositories.Waitlist;
using ProgresiumToDo.Infrastructure.Services.Auth.Authentication;
using ProgresiumToDo.Infrastructure.Services.Auth.Entitlement;
using ProgresiumToDo.Infrastructure.Services.Auth.Identity;
using ProgresiumToDo.Infrastructure.Services.Auth.OAuth;
using ProgresiumToDo.Infrastructure.Services.Auth.Onboarding;
using ProgresiumToDo.Infrastructure.Services.Auth.Tokens;
using ProgresiumToDo.Infrastructure.Services.Billing;
using ProgresiumToDo.Infrastructure.Services.BackgroundJobs;
using ProgresiumToDo.Infrastructure.Services.Email;
using ProgresiumToDo.Infrastructure.Services.Tags;
using ProgresiumToDo.Infrastructure.Services.Tasks;

namespace ProgresiumToDo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        
        AddPersistence(services);
        
        AddIdentity(services);
        
        AddAuthentication(services, configuration);
        
        AddAuthorization(services);

        AddRepositories(services);

        AddEmailService(services, configuration);

        AddHangfire(services);
        
        services.AddTransient<IUserOnboardingService, UserOnboardingService>();
        
        services.AddTransient<ISubscriptionService, SubscriptionService>();
        
        services.AddTransient<ITaskOrderingService, TaskOrderingService>();
        
        services.AddTransient<ITaskStatusPolicy, TaskStatusPolicy>();

        services.AddTransient<IEntitlementService, EntitlementService>();

        services.AddTransient<ITagService, TagService>();
        
        return services;
    }
    
    private static void AddPersistence(IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                               throw new ApplicationException("Database connection string secret is missing.");

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
                options.Password.RequireNonAlphanumeric = false;
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

        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
    }
    
    private static void AddAuthorization(IServiceCollection services) {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.EmailVerified, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(CustomClaims.EmailVerified, true.ToString());
            });
        });
        
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();
    }
    
    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IProjectRepository, ProjectRepository>();

        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        services.AddScoped<ITaskOrderRepository, TaskOrderRepository>();
        
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<IPlanRepository, PlanRepository>();
        
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

        services.AddScoped<IPlanPricingRepository, PlanPricingRepository>();
        
        services.AddScoped<IPlanFeatureRepository, PlanFeatureRepository>();
        
        services.AddScoped<IFeatureUsageRepository, FeatureUsageRepository>();

        services.AddScoped<IWaitlistEntryRepository, WaitlistEntryRepository>();

        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
    }
    
    private static void AddEmailService(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailtrapSettings>(configuration.GetSection("Mailtrap"));
        
        var settings = configuration.GetSection("Mailtrap").Get<MailtrapSettings>() ??
                       throw new InvalidOperationException("Mailtrap settings are not configured properly.");
        var apiToken = Environment.GetEnvironmentVariable("MAILTRAP_API_KEY") ?? 
                        throw new InvalidOperationException("Mailtrap API token is not set in environment variables.");
        
        services.AddHttpClient<IEmailService, MailtrapEmailService>(client =>
        {
            client.BaseAddress = new Uri(settings.ApiUrl);
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiToken);
        });
    }

    private static void AddHangfire(IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ??
                               throw new ApplicationException("Database connection string secret is missing.");
        
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(connectionString))
            .UseFilter(new ResultCheckJobFilter()));
        
        services.AddHangfireServer(options =>
        {
            options.WorkerCount = Environment.ProcessorCount;
        });
        
        services.AddTransient<IBackgroundJobService, HangfireBackgroundJobService>();
    }
}