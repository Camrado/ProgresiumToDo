using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Domain.Abstractions;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Infrastructure.Identity;

namespace ProgresiumToDo.Infrastructure;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public new DbSet<User> Users { get; set; }
    public DbSet<Plan> Plans { get; set; }
    public DbSet<PlanPricing> PlanPricings { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Region> Regions { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<FeatureUsage> FeatureUsages { get; set; }
    public DbSet<PlanFeature> PlanFeatures { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<TaskOrder> TaskOrders { get; set; }
    public DbSet<TaskAttachment> TaskAttachments { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);
        
        return result;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}