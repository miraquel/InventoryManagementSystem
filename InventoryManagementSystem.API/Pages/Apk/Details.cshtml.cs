using InventoryManagementSystem.Dto.ApkVersion;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagementSystem.API.Pages.Apk;

public class DetailsModel : PageModel
{
    private readonly IApkVersionService _apkVersionService;

    public DetailsModel(IApkVersionService apkVersionService)
    {
        _apkVersionService = apkVersionService;
    }

    public ApkVersionDto? Version { get; set; }
    public IEnumerable<ApkVersionDto> OtherVersions { get; set; } = [];
    public string BaseUrl { get; set; } = string.Empty;

    public async Task OnGetAsync(int id)
    {
        BaseUrl = $"{Request.Scheme}://{Request.Host}";
        Version = await _apkVersionService.GetByIdAsync(id);

        if (Version != null)
        {
            var query = new ApkVersionQueryDto
            {
                PackageName = Version.PackageName,
                PageSize = 10
            };
            var (versions, _) = await _apkVersionService.GetAllAsync(query);
            OtherVersions = versions.Where(v => v.Id != id);
        }
    }
}

