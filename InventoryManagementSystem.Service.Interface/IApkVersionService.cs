using InventoryManagementSystem.Dto.ApkVersion;

namespace InventoryManagementSystem.Service.Interface;

/// <summary>
/// Interface for APK version management service
/// </summary>
public interface IApkVersionService
{
    /// <summary>
    /// Gets all APK versions with optional filtering
    /// </summary>
    Task<(IEnumerable<ApkVersionDto> Versions, int TotalCount)> GetAllAsync(ApkVersionQueryDto query);
    
    /// <summary>
    /// Gets a specific APK version by ID
    /// </summary>
    Task<ApkVersionDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Gets the latest version for a specific package
    /// </summary>
    Task<ApkVersionDto?> GetLatestVersionAsync(string packageName);
    
    /// <summary>
    /// Gets all unique apps (distinct package names)
    /// </summary>
    Task<IEnumerable<string>> GetDistinctAppsAsync();
    
    /// <summary>
    /// Uploads a new APK version
    /// </summary>
    Task<ApkVersionDto> UploadAsync(UploadApkVersionDto uploadDto, string? uploadedBy);
    
    /// <summary>
    /// Updates an existing APK version metadata
    /// </summary>
    Task<ApkVersionDto?> UpdateAsync(int id, UpdateApkVersionDto updateDto);
    
    /// <summary>
    /// Deletes an APK version
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Gets the file path for downloading an APK
    /// </summary>
    Task<string?> GetApkFilePathAsync(int id);
    
    /// <summary>
    /// Generates Sparkle Appcast XML for Android updates
    /// </summary>
    Task<string> GenerateAppcastXmlAsync(string packageName, string? baseUrl = null);
}

