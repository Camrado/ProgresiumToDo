using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgresiumToDo.Application.Abstractions.Behaviors.Contracts;
using ProgresiumToDo.Domain.Auth;
using ProgresiumToDo.Domain.Billing;
using ProgresiumToDo.Domain.FeatureUsage;
using ProgresiumToDo.Domain.Projects;
using ProgresiumToDo.Domain.Tags;
using ProgresiumToDo.Domain.Tasks;
using ProgresiumToDo.Domain.Waitlist;
using ProgresiumToDo.Infrastructure.Persistence;
using ProgresiumToDo.Infrastructure.Services.Auth.Identity;

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
    public DbSet<WaitlistEntry> WaitlistEntries { get; set; }

    public async Task<IApplicationTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var efTransaction = await Database.BeginTransactionAsync(cancellationToken);
        return new EfTransactionWrapper(efTransaction);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}