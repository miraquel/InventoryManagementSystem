using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly IInventTableService _inventTableService;

    public ItemsController(IInventTableService inventTableService)
    {
        _inventTableService = inventTableService;
    }

    // GET: api/Items/{itemId}
    [HttpGet("{itemId}")]
    [Authorize(Policy = Policies.ViewItems)]
    public async Task<IActionResult> GetItem(string itemId)
    {
        var response = await _inventTableService.GetInventTableAsync(itemId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items
    [HttpGet]
    [Authorize(Policy = Policies.ViewItems)]
    public async Task<IActionResult> GetItemsPaged([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? searchQuery = null)
    {
        var response = await _inventTableService.GetInventTablePagedListAsync(pageNumber, pageSize, searchQuery);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/OnHand/{itemId}
    [HttpGet("OnHand/{itemId}")]
    [Authorize(Policy = Policies.ViewOnHand)]
    public async Task<IActionResult> GetOnHand(
        string itemId, 
        [FromQuery] string inventBatchId,
        [FromQuery] string inventSiteId,
        [FromQuery] string inventLocationId,
        [FromQuery] string wmsLocationId)
    {
        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(inventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _inventTableService.GetOnHandAsync(itemId, inventBatchId, inventSiteId, inventLocationId, wmsLocationId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/OnHand/InventLocations/{inventSiteId}
    [HttpGet("OnHand/InventLocations/{inventSiteId}")]
    [Authorize(Policy = Policies.ViewOnHand)]
    public async Task<IActionResult> GetOnHandInventLocationList(string inventSiteId, [FromQuery] string? searchQuery = null)
    {
        var response = await _inventTableService.GetOnHandInventLocationListAsync(searchQuery, inventSiteId);
        
        // Filter results based on user's allowed inventLocations
        // Note: Consider filtering at service level for better performance
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/OnHand/WMSLocations/{inventLocationId}
    [HttpGet("OnHand/WMSLocations/{inventLocationId}")]
    [Authorize(Policy = Policies.ViewOnHand)]
    public async Task<IActionResult> GetOnHandWMSLocationPagedList(string inventLocationId, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? searchQuery = null)
    {
        // // Check inventLocation access from user groups
        // if (!User.HasInventLocationAccess(inventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _inventTableService.GetOnHandWMSLocationPagedListAsync(pageNumber, pageSize, searchQuery, inventLocationId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/onhand
    [HttpGet("OnHand")]
    [Authorize(Policy = Policies.ViewOnHand)]
    public async Task<IActionResult> GetOnHandPagedList(
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] string? searchQuery = null, 
        [FromQuery] string? inventLocationId = null, 
        [FromQuery] string? wmsLocationId = null, 
        [FromQuery] string? storageDimensionGroup = null, 
        [FromQuery] string? trackingDimensionGroup = null)
    {
        // // Check inventLocation access from user groups if specified
        // if (!string.IsNullOrEmpty(inventLocationId) && !User.HasInventLocationAccess(inventLocationId))
        // {
        //     return Forbid();
        // }
        
        var response = await _inventTableService.GetOnHandPagedListAsync(
            pageNumber, 
            pageSize, 
            searchQuery, 
            inventLocationId, 
            wmsLocationId, 
            storageDimensionGroup, 
            trackingDimensionGroup);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/{itemId}/Batches/{inventBatchId}
    [HttpGet("{itemId}/Batches/{inventBatchId}")]
    [Authorize(Policy = Policies.ViewItems)]
    public async Task<IActionResult> GetInventBatch(string itemId, string inventBatchId)
    {
        var response = await _inventTableService.GetInventBatchAsync(itemId, inventBatchId);
        return StatusCode(response.StatusCode, response);
    }

    // GET: api/Items/{itemId}/Batches
    [HttpGet("{itemId}/Batches")]
    [Authorize(Policy = Policies.ViewItems)]
    public async Task<IActionResult> GetInventBatchPagedList(
        string itemId,
        [FromQuery] int pageNumber, 
        [FromQuery] int pageSize, 
        [FromQuery] string? searchQuery = null)
    {
        var response = await _inventTableService.GetInventBatchPagedListAsync(pageNumber, pageSize, itemId, searchQuery);
        return StatusCode(response.StatusCode, response);
    }
}