namespace InventoryManagementSystem.Dto.ApkVersion;

/// <summary>
/// DTO for updating an existing APK version
/// </summary>
public class UpdateApkVersionDto
{
    public string? ReleaseNotes { get; set; }
    
    public string? MinAndroidVersion { get; set; }
    
    public bool? IsLatest { get; set; }
}
