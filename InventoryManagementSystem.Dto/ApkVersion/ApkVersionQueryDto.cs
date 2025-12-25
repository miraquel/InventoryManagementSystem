namespace InventoryManagementSystem.Dto.ApkVersion;

/// <summary>
/// Query parameters for listing APK versions
/// </summary>
public class ApkVersionQueryDto
{
    public string? AppName { get; set; }
    public string? PackageName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool LatestOnly { get; set; } = false;
}
