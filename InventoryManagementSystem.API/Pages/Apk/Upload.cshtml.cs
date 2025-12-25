using InventoryManagementSystem.Dto.ApkVersion;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagementSystem.API.Pages.Apk;

public class UploadModel : PageModel
{
    private readonly IApkVersionService _apkVersionService;
    private readonly ILogger<UploadModel> _logger;

    public UploadModel(IApkVersionService apkVersionService, ILogger<UploadModel> logger)
    {
        _apkVersionService = apkVersionService;
        _logger = logger;
    }

    [BindProperty]
    public UploadApkVersionDto? Input { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please fill in all required fields.";
            return Page();
        }

        if (Input == null || Input.ApkFile == null || Input.ApkFile.Length == 0)
        {
            ErrorMessage = "Please select an APK file to upload.";
            return Page();
        }

        if (!Input.ApkFile.FileName.EndsWith(".apk", StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "Only APK files are allowed.";
            return Page();
        }

        try
        {
            var uploadedBy = User.Identity?.Name ?? User.FindFirst("preferred_username")?.Value ?? "web_upload";
            var result = await _apkVersionService.UploadAsync(Input, uploadedBy);

            _logger.LogInformation("APK uploaded via web: {PackageName} v{VersionCode}", 
                Input.PackageName, Input.VersionCode);

            return RedirectToPage("./Details", new { id = result.Id });
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading APK via web");
            ErrorMessage = "An error occurred while uploading the APK. Please try again.";
            return Page();
        }
    }
}

