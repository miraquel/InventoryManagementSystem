using InventoryManagementSystem.Dto.ApkVersion;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

/// <summary>
/// API Controller for Android APK version management
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ApkController : ControllerBase
{
    private readonly IApkVersionService _apkVersionService;
    private readonly ILogger<ApkController> _logger;

    public ApkController(IApkVersionService apkVersionService, ILogger<ApkController> logger)
    {
        _apkVersionService = apkVersionService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all APK versions with optional filtering and pagination
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PaginatedResult<ApkVersionDto>>> GetAll([FromQuery] ApkVersionQueryDto query)
    {
        var (versions, totalCount) = await _apkVersionService.GetAllAsync(query);
        
        return Ok(new PaginatedResult<ApkVersionDto>
        {
            Items = versions,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
        });
    }

    /// <summary>
    /// Gets a specific APK version by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApkVersionDto>> GetById(int id)
    {
        var version = await _apkVersionService.GetByIdAsync(id);
        if (version == null)
        {
            return NotFound(new { message = $"APK version with ID {id} not found" });
        }
        return Ok(version);
    }

    /// <summary>
    /// Gets the latest version for a specific package
    /// </summary>
    [HttpGet("latest/{packageName}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApkVersionDto>> GetLatest(string packageName)
    {
        var version = await _apkVersionService.GetLatestVersionAsync(packageName);
        if (version == null)
        {
            return NotFound(new { message = $"No versions found for package {packageName}" });
        }
        return Ok(version);
    }

    /// <summary>
    /// Gets all unique app package names
    /// </summary>
    [HttpGet("apps")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<string>>> GetApps()
    {
        var apps = await _apkVersionService.GetDistinctAppsAsync();
        return Ok(apps);
    }

    /// <summary>
    /// Uploads a new APK version
    /// </summary>
    [HttpPost]
    [RequestSizeLimit(500 * 1024 * 1024)] // 500 MB limit
    [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
    public async Task<ActionResult<ApkVersionDto>> Upload([FromForm] UploadApkVersionDto uploadDto)
    {
        try
        {
            var uploadedBy = User.Identity?.Name ?? User.FindFirst("preferred_username")?.Value ?? "anonymous";
            var version = await _apkVersionService.UploadAsync(uploadDto, uploadedBy);
            
            _logger.LogInformation("APK uploaded: {PackageName} v{VersionCode} by {User}",
                uploadDto.PackageName, uploadDto.VersionCode, uploadedBy);
            
            return CreatedAtAction(nameof(GetById), new { id = version.Id }, version);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading APK");
            return StatusCode(500, new { message = "An error occurred while uploading the APK" });
        }
    }

    /// <summary>
    /// Updates an existing APK version metadata
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApkVersionDto>> Update(int id, [FromBody] UpdateApkVersionDto updateDto)
    {
        var version = await _apkVersionService.UpdateAsync(id, updateDto);
        if (version == null)
        {
            return NotFound(new { message = $"APK version with ID {id} not found" });
        }
        return Ok(version);
    }

    /// <summary>
    /// Deletes an APK version
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _apkVersionService.DeleteAsync(id);
        if (!result)
        {
            return NotFound(new { message = $"APK version with ID {id} not found" });
        }
        return NoContent();
    }

    /// <summary>
    /// Downloads the APK file for a specific version
    /// </summary>
    [HttpGet("{id:int}/download")]
    [AllowAnonymous]
    public async Task<IActionResult> Download(int id)
    {
        var version = await _apkVersionService.GetByIdAsync(id);
        if (version == null)
        {
            return NotFound(new { message = $"APK version with ID {id} not found" });
        }

        var filePath = await _apkVersionService.GetApkFilePathAsync(id);
        if (filePath == null || !System.IO.File.Exists(filePath))
        {
            return NotFound(new { message = "APK file not found" });
        }

        var fileName = $"{version.PackageName}_{version.VersionName}.apk";
        const string mimeType = "application/vnd.android.package-archive";

        return PhysicalFile(filePath, mimeType, fileName);
    }

    /// <summary>
    /// Gets release notes for a specific version (HTML format)
    /// </summary>
    [HttpGet("{id:int}/release-notes")]
    [AllowAnonymous]
    public async Task<IActionResult> GetReleaseNotes(int id)
    {
        var version = await _apkVersionService.GetByIdAsync(id);
        if (version == null)
        {
            return NotFound(new { message = $"APK version with ID {id} not found" });
        }

        var html = $$"""
                     <!DOCTYPE html>
                     <html>
                     <head>
                         <meta charset="UTF-8">
                         <title>{{version.AppName}} v{{version.VersionName}} Release Notes</title>
                         <style>
                             body { font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif; padding: 20px; max-width: 800px; margin: 0 auto; }
                             h1 { color: #333; }
                             .version-info { color: #666; margin-bottom: 20px; }
                             .release-notes { line-height: 1.6; }
                         </style>
                     </head>
                     <body>
                         <h1>{{version.AppName}}</h1>
                         <div class="version-info">
                             <strong>Version:</strong> {{version.VersionName}} (Build {{version.VersionCode}})<br>
                             <strong>Released:</strong> {{version.CreatedAt:MMMM dd, yyyy}}
                         </div>
                         <div class="release-notes">
                             <h2>Release Notes</h2>
                             <p>{{version.ReleaseNotes ?? "No release notes available."}}</p>
                         </div>
                     </body>
                     </html>
                     """;

        return Content(html, "text/html");
    }

    /// <summary>
    /// Generates Sparkle Appcast XML for a specific package
    /// </summary>
    [HttpGet("appcast/{packageName}")]
    [AllowAnonymous]
    [Produces("application/xml")]
    public async Task<IActionResult> GetAppcast(string packageName)
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var xml = await _apkVersionService.GenerateAppcastXmlAsync(packageName, baseUrl);
            return Content(xml, "application/xml");
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}

/// <summary>
/// Generic paginated result wrapper
/// </summary>
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

