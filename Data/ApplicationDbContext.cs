using Microsoft.EntityFrameworkCore;
using RiseUpAPI.Models;

namespace RiseUpAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Opportunity> Opportunities { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Audience> Audiences { get; set; }
    public DbSet<Region> Regions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<User>()
            .HasIndex(u => u.CPF)
            .IsUnique();

        builder.Entity<Organization>()
            .HasMany(o => o.Opportunities)
            .WithOne(o => o.Organization)
            .HasForeignKey(o => o.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Opportunity>()
            .HasMany(o => o.Activities)
            .WithOne(a => a.Opportunity)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Opportunity>()
            .HasOne(o => o.Audience)
            .WithOne(a => a.Opportunity)
            .HasForeignKey<Audience>(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Audience>()
            .HasMany(a => a.Regions)
            .WithOne(r => r.Audience)
            .HasForeignKey(r => r.AudienceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
} 