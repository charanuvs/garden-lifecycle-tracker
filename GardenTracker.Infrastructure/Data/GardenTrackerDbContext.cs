using GardenTracker.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using GardenTracker.Domain.ValueObjects;

namespace GardenTracker.Infrastructure.Data;

public class GardenTrackerDbContext : IdentityDbContext<ApplicationUser>
{
    public GardenTrackerDbContext(DbContextOptions<GardenTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<CropDefinition> CropDefinitions { get; set; }
    public DbSet<WorkflowStepDefinition> WorkflowStepDefinitions { get; set; }
    public DbSet<CropWorkflowConfiguration> CropWorkflowConfigurations { get; set; }
    public DbSet<UserCrop> UserCrops { get; set; }
    public DbSet<ActiveWorkflowStep> ActiveWorkflowSteps { get; set; }
    public DbSet<WorkflowStepHistory> WorkflowStepHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure value conversions for WorkflowStepParameters
        var parametersConverter = new ValueConverter<WorkflowStepParameters, string>(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<WorkflowStepParameters>(v, (JsonSerializerOptions?)null) ?? new WorkflowStepParameters()
        );

        // CropDefinition
        modelBuilder.Entity<CropDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CropType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.CropType).IsUnique();
        });

        // WorkflowStepDefinition
        modelBuilder.Entity<WorkflowStepDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.StepType).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.StepType).IsUnique();
            entity.Property(e => e.DefaultParameters).HasConversion(parametersConverter);
        });

        // CropWorkflowConfiguration
        modelBuilder.Entity<CropWorkflowConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.CropDefinition)
                .WithMany(c => c.WorkflowConfigurations)
                .HasForeignKey(e => e.CropDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.WorkflowStepDefinition)
                .WithMany(w => w.CropWorkflows)
                .HasForeignKey(e => e.WorkflowStepDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(e => e.ParameterOverrides).HasConversion(parametersConverter);
            
            entity.HasIndex(e => new { e.CropDefinitionId, e.WorkflowStepDefinitionId }).IsUnique();
        });

        // UserCrop
        modelBuilder.Entity<UserCrop>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nickname).IsRequired().HasMaxLength(200);
            
            entity.HasOne(e => e.CropDefinition)
                .WithMany(c => c.UserCrops)
                .HasForeignKey(e => e.CropDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ActiveWorkflowStep
        modelBuilder.Entity<ActiveWorkflowStep>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.UserCrop)
                .WithMany(u => u.ActiveWorkflowSteps)
                .HasForeignKey(e => e.UserCropId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.WorkflowStepDefinition)
                .WithMany()
                .HasForeignKey(e => e.WorkflowStepDefinitionId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.Property(e => e.ResolvedParameters).HasConversion(parametersConverter);
        });

        // WorkflowStepHistory
        modelBuilder.Entity<WorkflowStepHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.ActiveWorkflowStep)
                .WithMany(a => a.History)
                .HasForeignKey(e => e.ActiveWorkflowStepId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity && 
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = DateTime.UtcNow;
            }
            
            entity.ModifiedDate = DateTime.UtcNow;
        }
    }
}
