using DataAnalysis.Domain.Entities.Identity;
using DataAnalysis.Domain.Entities.Permission;
using DataAnalysis.Domain.Entities.Logging;
using Microsoft.EntityFrameworkCore;
using DataAnalysis.Domain.Common;
using DataAnalysis.Domain.Entities.ExecSummary;
using DataAnalysis.Domain.Entities.CustomSegment;

namespace DataAnalysis.Persistence.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserMfa> UserMfas => Set<UserMfa>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MfaSessionToken> MfaSessionTokens => Set<MfaSessionToken>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<AuthLog> AuthLogs => Set<AuthLog>();
    public DbSet<ExecAiCache> ExecAiCaches => Set<ExecAiCache>();
    public DbSet<CustomSegment> CustomSegments => Set<CustomSegment>();
    public DbSet<CustomSegmentResult> CustomSegmentResults => Set<CustomSegmentResult>();
    public DbSet<ComparisonSegment> ComparisonSegments => Set<ComparisonSegment>();
    public DbSet<ComparisonSegmentResult> ComparisonSegmentResults => Set<ComparisonSegmentResult>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedDate = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedDate = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(x => !x.User.IsDeleted);
        modelBuilder.Entity<UserMfa>().HasQueryFilter(x => !x.User.IsDeleted);
        modelBuilder.Entity<AuditLog>().HasQueryFilter(x => !x.User.IsDeleted);
        modelBuilder.Entity<UserPermission>().HasQueryFilter(x => !x.User.IsDeleted);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MfaSessionToken>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MfaSessionToken>()
            .HasIndex(x => x.Token)
            .IsUnique();

        modelBuilder.Entity<MfaSessionToken>()
            .HasQueryFilter(x => !x.User.IsDeleted);

        modelBuilder.Entity<UserMfa>()
            .HasOne(x => x.User)
            .WithOne(x => x.UserMfa)
            .HasForeignKey<UserMfa>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AuditLog>()
            .HasOne(x => x.User)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<AuthLog>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);     

        modelBuilder.Entity<Permission>()
            .HasOne(x => x.Module)
            .WithMany(x => x.Permissions)
            .HasForeignKey(x => x.ModuleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserPermission>()
            .HasOne(x => x.Permission)
            .WithMany(x => x.UserPermissions)
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExecAiCache>()
            .HasIndex(x => new { x.WeekStart, 
            x.WeekEnd, x.ProductType, x.ModelType })
            .IsUnique();
 
        modelBuilder.Entity<ExecAiCache>()
            .Property(x => x.ProductType)
            .HasMaxLength(10);
 
        modelBuilder.Entity<ExecAiCache>()
            .Property(x => x.ModelType)
            .HasMaxLength(20);
 
        modelBuilder.Entity<ExecAiCache>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);
 
        modelBuilder.Entity<ExecAiCache>()
            .HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CustomSegment>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<CustomSegment>()
            .HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<CustomSegment>()
            .HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<CustomSegment>()
            .Property(x => x.Name)
            .HasMaxLength(100);
 
        modelBuilder.Entity<CustomSegment>()
            .Property(x => x.ProductGroup)
            .HasMaxLength(10);
 
        modelBuilder.Entity<CustomSegment>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<CustomSegmentResult>()
            .HasOne(x => x.Segment)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.SegmentId)
            .OnDelete(DeleteBehavior.Restrict);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CustomSegmentResult>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .HasIndex(x => new { x.SegmentId, x.StartDate, x.EndDate })
            .IsUnique();
 
        modelBuilder.Entity<CustomSegmentResult>()
            .HasQueryFilter(x => !x.IsDeleted);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .Property(x => x.StartShare)
            .HasPrecision(10, 4);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .Property(x => x.EndShare)
            .HasPrecision(10, 4);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .Property(x => x.Change)
            .HasPrecision(10, 4);
 
        modelBuilder.Entity<CustomSegmentResult>()
            .Property(x => x.GrowthMultiple)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ComparisonSegment>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComparisonSegment>()
            .HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComparisonSegment>()
            .HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComparisonSegment>()
            .Property(x => x.Name)
            .HasMaxLength(100);

        modelBuilder.Entity<ComparisonSegment>()
            .Property(x => x.ProductGroup)
            .HasMaxLength(10);

        modelBuilder.Entity<ComparisonSegment>()
            .HasQueryFilter(x => !x.IsDeleted);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasOne(x => x.Comparison)
            .WithMany(x => x.Results)
            .HasForeignKey(x => x.ComparisonSegmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasOne(x => x.DeletedByUser)
            .WithMany()
            .HasForeignKey(x => x.DeletedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .Property(x => x.StartShare)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .Property(x => x.EndShare)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .Property(x => x.Change)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .Property(x => x.GrowthMultiple)
            .HasPrecision(10, 4);

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasIndex(x => new { x.ComparisonSegmentId, x.Side })
            .HasFilter("\"IsDeleted\" = false")
            .IsUnique();

        modelBuilder.Entity<ComparisonSegmentResult>()
            .HasQueryFilter(x => !x.IsDeleted);                                             
    }
}