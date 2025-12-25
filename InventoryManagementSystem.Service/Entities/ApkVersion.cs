using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Service.Entities;

/// <summary>
/// Entity representing an Android APK version stored in MySQL
/// </summary>
[Table("apk_versions")]
public class ApkVersion
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("app_name")]
    public string AppName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [Column("package_name")]
    public string PackageName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    [Column("version_name")]
    public string VersionName { get; set; } = string.Empty;

    [Required]
    [Column("version_code")]
    public int VersionCode { get; set; }

    [Column("release_notes", TypeName = "text")]
    public string? ReleaseNotes { get; set; }

    [Required]
    [Column("file_size")]
    public long FileSize { get; set; }

    [Required]
    [MaxLength(500)]
    [Column("file_path")]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(64)]
    [Column("sha256_hash")]
    public string? Sha256Hash { get; set; }

    [MaxLength(20)]
    [Column("min_android_version")]
    public string? MinAndroidVersion { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("is_latest")]
    public bool IsLatest { get; set; } = false;

    [MaxLength(255)]
    [Column("uploaded_by")]
    public string? UploadedBy { get; set; }
}

