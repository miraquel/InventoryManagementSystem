using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Dto.ApkVersion;

/// <summary>
/// DTO for uploading a new APK version
/// </summary>
public class UploadApkVersionDto
{
    [Required]
    public string AppName { get; set; } = string.Empty;
    
    [Required]
    public string PackageName { get; set; } = string.Empty;
    
    [Required]
    public string VersionName { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Version code must be a positive integer")]
    public int VersionCode { get; set; }
    
    public string? ReleaseNotes { get; set; }
    
    public string? MinAndroidVersion { get; set; }
    
    [Required]
    public IFormFile ApkFile { get; set; } = null!;
}
