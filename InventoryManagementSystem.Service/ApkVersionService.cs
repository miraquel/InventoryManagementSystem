using System.Security.Cryptography;
using System.Text;
using System.Xml;
using InventoryManagementSystem.Dto.ApkVersion;
using InventoryManagementSystem.Service.Data;
using InventoryManagementSystem.Service.Entities;
using InventoryManagementSystem.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service;

/// <summary>
/// Service for managing Android APK versions
/// </summary>
public class ApkVersionService : IApkVersionService
{
    private readonly ApkDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ApkVersionService> _logger;
    private readonly string _apkStoragePath;

    public ApkVersionService(
        ApkDbContext dbContext,
        IConfiguration configuration,
        ILogger<ApkVersionService> logger)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _logger = logger;
        _apkStoragePath = configuration["ApkStorage:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "ApkFiles");
        
        // Ensure storage directory exists
        if (!Directory.Exists(_apkStoragePath))
        {
            Directory.CreateDirectory(_apkStoragePath);
        }
    }

    public async Task<(IEnumerable<ApkVersionDto> Versions, int TotalCount)> GetAllAsync(ApkVersionQueryDto query)
    {
        var queryable = _dbContext.ApkVersions.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.AppName))
        {
            queryable = queryable.Where(v => v.AppName.Contains(query.AppName));
        }

        if (!string.IsNullOrWhiteSpace(query.PackageName))
        {
            queryable = queryable.Where(v => v.PackageName == query.PackageName);
        }

        if (query.LatestOnly)
        {
            queryable = queryable.Where(v => v.IsLatest);
        }

        var totalCount = await queryable.CountAsync();

        var versions = await queryable
            .OrderByDescending(v => v.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(v => MapToDto(v, null))
            .ToListAsync();

        return (versions, totalCount);
    }

    public async Task<ApkVersionDto?> GetByIdAsync(int id)
    {
        var version = await _dbContext.ApkVersions.FindAsync(id);
        return version == null ? null : MapToDto(version, null);
    }

    public async Task<ApkVersionDto?> GetLatestVersionAsync(string packageName)
    {
        var version = await _dbContext.ApkVersions
            .Where(v => v.PackageName == packageName && v.IsLatest)
            .FirstOrDefaultAsync();

        if (version == null)
        {
            // If no version marked as latest, get the one with highest version code
            version = await _dbContext.ApkVersions
                .Where(v => v.PackageName == packageName)
                .OrderByDescending(v => v.VersionCode)
                .FirstOrDefaultAsync();
        }

        return version == null ? null : MapToDto(version, null);
    }

    public async Task<IEnumerable<string>> GetDistinctAppsAsync()
    {
        return await _dbContext.ApkVersions
            .Select(v => v.PackageName)
            .Distinct()
            .OrderBy(p => p)
            .ToListAsync();
    }

    public async Task<ApkVersionDto> UploadAsync(UploadApkVersionDto uploadDto, string? uploadedBy)
    {
        // Check if version already exists
        var existingVersion = await _dbContext.ApkVersions
            .FirstOrDefaultAsync(v => v.PackageName == uploadDto.PackageName && v.VersionCode == uploadDto.VersionCode);

        if (existingVersion != null)
        {
            throw new InvalidOperationException($"Version {uploadDto.VersionCode} already exists for package {uploadDto.PackageName}");
        }

        // Create directory for package if it doesn't exist
        var packageDir = Path.Combine(_apkStoragePath, SanitizeFileName(uploadDto.PackageName));
        if (!Directory.Exists(packageDir))
        {
            Directory.CreateDirectory(packageDir);
        }

        // Generate unique filename
        var fileName = $"{SanitizeFileName(uploadDto.PackageName)}_{uploadDto.VersionCode}_{DateTime.UtcNow:yyyyMMddHHmmss}.apk";
        var filePath = Path.Combine(packageDir, fileName);

        // Save the file
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await uploadDto.ApkFile.CopyToAsync(stream);
        }

        // Calculate SHA256 hash
        var sha256Hash = await CalculateSha256Async(filePath);

        // Set all other versions of this package as not latest
        var previousVersions = await _dbContext.ApkVersions
            .Where(v => v.PackageName == uploadDto.PackageName && v.IsLatest)
            .ToListAsync();

        foreach (var v in previousVersions)
        {
            v.IsLatest = false;
            v.UpdatedAt = DateTime.UtcNow;
        }

        // Create the new version entity
        var apkVersion = new ApkVersion
        {
            AppName = uploadDto.AppName,
            PackageName = uploadDto.PackageName,
            VersionName = uploadDto.VersionName,
            VersionCode = uploadDto.VersionCode,
            ReleaseNotes = uploadDto.ReleaseNotes,
            FileSize = uploadDto.ApkFile.Length,
            FilePath = filePath,
            Sha256Hash = sha256Hash,
            MinAndroidVersion = uploadDto.MinAndroidVersion,
            CreatedAt = DateTime.UtcNow,
            IsLatest = true,
            UploadedBy = uploadedBy
        };

        _dbContext.ApkVersions.Add(apkVersion);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Uploaded new APK version: {PackageName} v{VersionCode} ({VersionName})",
            uploadDto.PackageName, uploadDto.VersionCode, uploadDto.VersionName);

        return MapToDto(apkVersion, null);
    }

    public async Task<ApkVersionDto?> UpdateAsync(int id, UpdateApkVersionDto updateDto)
    {
        var version = await _dbContext.ApkVersions.FindAsync(id);
        if (version == null)
        {
            return null;
        }

        if (updateDto.ReleaseNotes != null)
        {
            version.ReleaseNotes = updateDto.ReleaseNotes;
        }

        if (updateDto.MinAndroidVersion != null)
        {
            version.MinAndroidVersion = updateDto.MinAndroidVersion;
        }

        if (updateDto.IsLatest.HasValue && updateDto.IsLatest.Value)
        {
            // Set all other versions of this package as not latest
            var otherVersions = await _dbContext.ApkVersions
                .Where(v => v.PackageName == version.PackageName && v.Id != id && v.IsLatest)
                .ToListAsync();

            foreach (var v in otherVersions)
            {
                v.IsLatest = false;
                v.UpdatedAt = DateTime.UtcNow;
            }

            version.IsLatest = true;
        }
        else if (updateDto.IsLatest.HasValue && !updateDto.IsLatest.Value)
        {
            version.IsLatest = false;
        }

        version.UpdatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return MapToDto(version, null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var version = await _dbContext.ApkVersions.FindAsync(id);
        if (version == null)
        {
            return false;
        }

        // Delete the file if it exists
        if (File.Exists(version.FilePath))
        {
            try
            {
                File.Delete(version.FilePath);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete APK file: {FilePath}", version.FilePath);
            }
        }

        _dbContext.ApkVersions.Remove(version);
        await _dbContext.SaveChangesAsync();

        // If this was the latest version, mark the previous version as latest
        if (version.IsLatest)
        {
            var newLatest = await _dbContext.ApkVersions
                .Where(v => v.PackageName == version.PackageName)
                .OrderByDescending(v => v.VersionCode)
                .FirstOrDefaultAsync();

            if (newLatest != null)
            {
                newLatest.IsLatest = true;
                newLatest.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        }

        _logger.LogInformation("Deleted APK version: {PackageName} v{VersionCode}", version.PackageName, version.VersionCode);

        return true;
    }

    public async Task<string?> GetApkFilePathAsync(int id)
    {
        var version = await _dbContext.ApkVersions.FindAsync(id);
        if (version == null || !File.Exists(version.FilePath))
        {
            return null;
        }
        return version.FilePath;
    }

    public async Task<string> GenerateAppcastXmlAsync(string packageName, string? baseUrl = null)
    {
        baseUrl ??= _configuration["ApkStorage:BaseUrl"] ?? "https://localhost";

        var versions = await _dbContext.ApkVersions
            .Where(v => v.PackageName == packageName)
            .OrderByDescending(v => v.VersionCode)
            .ToListAsync();

        if (!versions.Any())
        {
            throw new InvalidOperationException($"No versions found for package: {packageName}");
        }

        var latestVersion = versions.FirstOrDefault(v => v.IsLatest) ?? versions.First();

        var settings = new XmlWriterSettings
        {
            Indent = true,
            Encoding = Encoding.UTF8,
            Async = true
        };

        using var stringWriter = new StringWriter();
        await using var xmlWriter = XmlWriter.Create(stringWriter, settings);

        await xmlWriter.WriteStartDocumentAsync();

        // Start RSS element
        await xmlWriter.WriteStartElementAsync(null, "rss", null);
        await xmlWriter.WriteAttributeStringAsync(null, "version", null, "2.0");
        await xmlWriter.WriteAttributeStringAsync("xmlns", "sparkle", null, "http://www.andymatuschak.org/xml-namespaces/sparkle");
        await xmlWriter.WriteAttributeStringAsync("xmlns", "android", null, "http://schemas.android.com/apk/res/android");

        // Channel element
        await xmlWriter.WriteStartElementAsync(null, "channel", null);

        await xmlWriter.WriteElementStringAsync(null, "title", null, latestVersion.AppName);
        await xmlWriter.WriteElementStringAsync(null, "link", null, $"{baseUrl}/api/apk/appcast/{packageName}");
        await xmlWriter.WriteElementStringAsync(null, "description", null, $"Updates for {latestVersion.AppName}");
        await xmlWriter.WriteElementStringAsync(null, "language", null, "en");

        // Generate items for each version
        foreach (var version in versions)
        {
            await xmlWriter.WriteStartElementAsync(null, "item", null);

            await xmlWriter.WriteElementStringAsync(null, "title", null, $"Version {version.VersionName}");
            await xmlWriter.WriteElementStringAsync(null, "pubDate", null, version.CreatedAt.ToString("r"));

            // Sparkle-specific elements
            await xmlWriter.WriteStartElementAsync("sparkle", "releaseNotesLink", null);
            await xmlWriter.WriteStringAsync($"{baseUrl}/api/apk/{version.Id}/release-notes");
            await xmlWriter.WriteEndElementAsync();

            // Enclosure element with download information
            await xmlWriter.WriteStartElementAsync(null, "enclosure", null);
            await xmlWriter.WriteAttributeStringAsync(null, "url", null, $"{baseUrl}/api/apk/{version.Id}/download");
            await xmlWriter.WriteAttributeStringAsync(null, "type", null, "application/vnd.android.package-archive");
            await xmlWriter.WriteAttributeStringAsync(null, "length", null, version.FileSize.ToString());

            // Sparkle version attributes
            await xmlWriter.WriteAttributeStringAsync("sparkle", "version", null, version.VersionCode.ToString());
            await xmlWriter.WriteAttributeStringAsync("sparkle", "shortVersionString", null, version.VersionName);

            // Android-specific attributes
            await xmlWriter.WriteAttributeStringAsync("android", "versionCode", null, version.VersionCode.ToString());
            await xmlWriter.WriteAttributeStringAsync("android", "versionName", null, version.VersionName);

            if (!string.IsNullOrEmpty(version.MinAndroidVersion))
            {
                await xmlWriter.WriteAttributeStringAsync("android", "minSdkVersion", null, version.MinAndroidVersion);
            }

            if (!string.IsNullOrEmpty(version.Sha256Hash))
            {
                await xmlWriter.WriteAttributeStringAsync("sparkle", "dsaSignature", null, version.Sha256Hash);
            }

            await xmlWriter.WriteEndElementAsync(); // enclosure

            // Release notes as description
            if (!string.IsNullOrEmpty(version.ReleaseNotes))
            {
                await xmlWriter.WriteElementStringAsync(null, "description", null, version.ReleaseNotes);
            }

            await xmlWriter.WriteEndElementAsync(); // item
        }

        await xmlWriter.WriteEndElementAsync(); // channel
        await xmlWriter.WriteEndElementAsync(); // rss
        await xmlWriter.WriteEndDocumentAsync();

        await xmlWriter.FlushAsync();
        return stringWriter.ToString();
    }

    private static ApkVersionDto MapToDto(ApkVersion entity, string? downloadUrl)
    {
        return new ApkVersionDto
        {
            Id = entity.Id,
            AppName = entity.AppName,
            PackageName = entity.PackageName,
            VersionName = entity.VersionName,
            VersionCode = entity.VersionCode,
            ReleaseNotes = entity.ReleaseNotes,
            FileSize = entity.FileSize,
            DownloadUrl = downloadUrl,
            Sha256Hash = entity.Sha256Hash,
            MinAndroidVersion = entity.MinAndroidVersion,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            IsLatest = entity.IsLatest,
            UploadedBy = entity.UploadedBy
        };
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        return string.Join("_", fileName.Split(invalidChars, StringSplitOptions.RemoveEmptyEntries));
    }

    private static async Task<string> CalculateSha256Async(string filePath)
    {
        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream);
        return Convert.ToHexStringLower(hash);
    }
}

