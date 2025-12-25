using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    // GET: api/Locations/InventLocations/{inventSiteId}
    [HttpGet("InventLocations/{inventSiteId}")]
    [Authorize(Policy = Policies.ViewLocations)]
    public async Task<IActionResult> GetInventLocations(string inventSiteId)
    {
        var response = await _locationService.GetInventLocationListAsync(inventSiteId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Locations/WMSLocations/{wmsLocationId}/{inventLocationId}
    [HttpGet("WMSLocations/{inventLocationId}/{wmsLocationId}")]
    [Authorize(Policy = Policies.ViewLocations)]
    public async Task<IActionResult> GetWMSLocation(string wmsLocationId, string inventLocationId)
    {
        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(inventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _locationService.GetWMSLocationAsync(wmsLocationId, inventLocationId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Locations/WMSLocations/{inventLocationId}
    [HttpGet("WMSLocations/{inventLocationId}")]
    [Authorize(Policy = Policies.ViewLocations)]
    public async Task<IActionResult> GetWMSLocationsPaged([FromQuery] int pageNumber, [FromQuery] int pageSize, string inventLocationId, [FromQuery] string? wmsLocationId = null)
    {
        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(inventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _locationService.GetWMSLocationPagedListAsync(pageNumber, pageSize, inventLocationId, wmsLocationId);
        return StatusCode(response.StatusCode, response);
    }
}

