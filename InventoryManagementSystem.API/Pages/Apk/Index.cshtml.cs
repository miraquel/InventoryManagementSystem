using InventoryManagementSystem.Dto.ApkVersion;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace InventoryManagementSystem.API.Pages.Apk;

public class IndexModel : PageModel
{
    private readonly IApkVersionService _apkVersionService;

    public IndexModel(IApkVersionService apkVersionService)
    {
        _apkVersionService = apkVersionService;
    }

    public IEnumerable<ApkVersionDto> Versions { get; set; } = [];
    public IEnumerable<string> Apps { get; set; } = [];
    public int TotalCount { get; set; }
    public new int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages { get; set; }
    public string? PackageName { get; set; }
    public bool LatestOnly { get; set; }

    public async Task OnGetAsync(int page = 1, string? packageName = null, bool latestOnly = false)
    {
        Page = page;
        PackageName = packageName;
        LatestOnly = latestOnly;

        Apps = await _apkVersionService.GetDistinctAppsAsync();

        var query = new ApkVersionQueryDto
        {
            Page = page,
            PageSize = PageSize,
            PackageName = packageName,
            LatestOnly = latestOnly
        };

        var (versions, totalCount) = await _apkVersionService.GetAllAsync(query);
        Versions = versions;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling((double)totalCount / PageSize);
    }
}

