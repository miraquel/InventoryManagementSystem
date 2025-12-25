namespace InventoryManagementSystem.Dto.ApkVersion;

/// <summary>
/// DTO for APK version information
/// </summary>
public class ApkVersionDto
{
    public int Id { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public string VersionName { get; set; } = string.Empty;
    public int VersionCode { get; set; }
    public string? ReleaseNotes { get; set; }
    public long FileSize { get; set; }
    public string? FileSizeFormatted => FormatFileSize(FileSize);
    public string? DownloadUrl { get; set; }
    public string? Sha256Hash { get; set; }
    public string? MinAndroidVersion { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsLatest { get; set; }
    public string? UploadedBy { get; set; }

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}

