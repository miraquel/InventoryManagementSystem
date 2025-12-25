using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers;

/// <summary>
/// Controller for managing warehouse group attributes in Keycloak
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseGroupsController : ControllerBase
{
    private readonly IKeycloakAdminService _keycloakAdminService;
    private readonly ILogger<WarehouseGroupsController> _logger;

    public WarehouseGroupsController(IKeycloakAdminService keycloakAdminService, ILogger<WarehouseGroupsController> logger)
    {
        _keycloakAdminService = keycloakAdminService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all groups with their warehouse assignments
    /// </summary>
    [HttpGet]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetAllWarehouseGroups()
    {
        var groups = await _keycloakAdminService.GetAllWarehouseGroupsAsync();
        return Ok(groups);
    }

    /// <summary>
    /// Gets warehouse IDs assigned to a specific group
    /// </summary>
    [HttpGet("{groupId}/warehouses")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult<List<string>>> GetGroupWarehouses(string groupId)
    {
        var warehouses = await _keycloakAdminService.GetGroupWarehousesAsync(groupId);
        return Ok(warehouses);
    }

    /// <summary>
    /// Gets warehouse IDs assigned to a group by group name
    /// </summary>
    [HttpGet("by-name/{groupName}/warehouses")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult<List<string>>> GetGroupWarehousesByName(string groupName)
    {
        var groupId = await _keycloakAdminService.GetGroupIdByNameAsync(groupName);
        if (groupId == null)
        {
            return NotFound(new { message = $"Group '{groupName}' not found" });
        }

        var warehouses = await _keycloakAdminService.GetGroupWarehousesAsync(groupId);
        return Ok(warehouses);
    }

    /// <summary>
    /// Adds warehouse access to a group
    /// </summary>
    [HttpPost("{groupId}/warehouses/{warehouseId}")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> AddWarehouseToGroup(string groupId, string warehouseId)
    {
        _logger.LogInformation("Adding warehouse {WarehouseId} to group {GroupId}", warehouseId, groupId);
        
        var success = await _keycloakAdminService.AddWarehouseToGroupAsync(groupId, warehouseId);
        
        if (!success)
        {
            _logger.LogError("Failed to add warehouse {WarehouseId} to group {GroupId}. Check logs for Keycloak API errors.", 
                warehouseId, groupId);
            return StatusCode(500, new { 
                message = "Failed to add warehouse to group", 
                hint = "Check API logs for detailed error. Service account may need 'query-groups' and 'manage-users' roles in Keycloak."
            });
        }

        return Ok(new { message = $"Warehouse '{warehouseId}' added to group" });
    }

    /// <summary>
    /// Adds warehouse access to a group by group name
    /// </summary>
    [HttpPost("by-name/{groupName}/warehouses/{warehouseId}")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> AddWarehouseToGroupByName(string groupName, string warehouseId)
    {
        var groupId = await _keycloakAdminService.GetGroupIdByNameAsync(groupName);
        if (groupId == null)
        {
            return NotFound(new { message = $"Group '{groupName}' not found" });
        }

        var success = await _keycloakAdminService.AddWarehouseToGroupAsync(groupId, warehouseId);
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to add warehouse to group" });
        }

        return Ok(new { message = $"Warehouse '{warehouseId}' added to group '{groupName}'" });
    }

    /// <summary>
    /// Removes warehouse access from a group
    /// </summary>
    [HttpDelete("{groupId}/warehouses/{warehouseId}")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> RemoveWarehouseFromGroup(string groupId, string warehouseId)
    {
        var success = await _keycloakAdminService.RemoveWarehouseFromGroupAsync(groupId, warehouseId);
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to remove warehouse from group" });
        }

        return Ok(new { message = $"Warehouse '{warehouseId}' removed from group" });
    }

    /// <summary>
    /// Removes warehouse access from a group by group name
    /// </summary>
    [HttpDelete("by-name/{groupName}/warehouses/{warehouseId}")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> RemoveWarehouseFromGroupByName(string groupName, string warehouseId)
    {
        var groupId = await _keycloakAdminService.GetGroupIdByNameAsync(groupName);
        if (groupId == null)
        {
            return NotFound(new { message = $"Group '{groupName}' not found" });
        }

        var success = await _keycloakAdminService.RemoveWarehouseFromGroupAsync(groupId, warehouseId);
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to remove warehouse from group" });
        }

        return Ok(new { message = $"Warehouse '{warehouseId}' removed from group '{groupName}'" });
    }

    /// <summary>
    /// Sets the complete list of warehouses for a group (replaces existing)
    /// </summary>
    [HttpPut("{groupId}/warehouses")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> SetGroupWarehouses(string groupId, [FromBody] SetWarehousesRequest request)
    {
        var success = await _keycloakAdminService.SetGroupWarehousesAsync(groupId, request.WarehouseIds);
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to update group warehouses" });
        }

        return Ok(new { message = "Group warehouses updated successfully" });
    }

    /// <summary>
    /// Sets the complete list of warehouses for a group by group name
    /// </summary>
    [HttpPut("by-name/{groupName}/warehouses")]
    [Authorize(Policy = Policies.RequireAdminRole)]
    public async Task<ActionResult> SetGroupWarehousesByName(string groupName, [FromBody] SetWarehousesRequest request)
    {
        var groupId = await _keycloakAdminService.GetGroupIdByNameAsync(groupName);
        if (groupId == null)
        {
            return NotFound(new { message = $"Group '{groupName}' not found" });
        }

        var success = await _keycloakAdminService.SetGroupWarehousesAsync(groupId, request.WarehouseIds);
        
        if (!success)
        {
            return BadRequest(new { message = "Failed to update group warehouses" });
        }

        return Ok(new { message = $"Warehouses for group '{groupName}' updated successfully" });
    }
}

/// <summary>
/// Request model for setting warehouse IDs
/// </summary>
public class SetWarehousesRequest
{
    /// <summary>
    /// List of warehouse IDs to assign to the group. Use "*" for all warehouses.
    /// </summary>
    public List<string> WarehouseIds { get; set; } = [];
}

