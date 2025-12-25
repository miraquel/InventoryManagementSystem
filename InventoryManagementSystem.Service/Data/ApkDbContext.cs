using InventoryManagementSystem.Service.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Service.Data;

/// <summary>
/// Database context for APK version management using MySQL
/// </summary>
public class ApkDbContext : DbContext
{
    public ApkDbContext(DbContextOptions<ApkDbContext> options) : base(options)
    {
    }

    public DbSet<ApkVersion> ApkVersions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApkVersion>(entity =>
        {
            // Create composite unique index for package_name + version_code
            entity.HasIndex(e => new { e.PackageName, e.VersionCode })
                .IsUnique()
                .HasDatabaseName("idx_package_version_unique");

            // Index for app_name for filtering
            entity.HasIndex(e => e.AppName)
                .HasDatabaseName("idx_app_name");

            // Index for package_name for filtering
            entity.HasIndex(e => e.PackageName)
                .HasDatabaseName("idx_package_name");

            // Index for is_latest for quick filtering of latest versions
            entity.HasIndex(e => new { e.PackageName, e.IsLatest })
                .HasDatabaseName("idx_package_latest");

            // Index for created_at for sorting
            entity.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("idx_created_at");
        });
    }
}

