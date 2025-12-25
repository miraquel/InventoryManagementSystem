namespace InventoryManagementSystem.Service.Interface;

/// <summary>
/// Service for managing Keycloak groups and their attributes via Admin API
/// </summary>
public interface IKeycloakAdminService
{
    /// <summary>
    /// Adds warehouse access to a group by updating its 'warehouses' attribute
    /// </summary>
    Task<bool> AddWarehouseToGroupAsync(string groupId, string warehouseId);
    
    /// <summary>
    /// Removes warehouse access from a group by updating its 'warehouses' attribute
    /// </summary>
    Task<bool> RemoveWarehouseFromGroupAsync(string groupId, string warehouseId);
    
    /// <summary>
    /// Sets the complete list of warehouses for a group (replaces existing)
    /// </summary>
    Task<bool> SetGroupWarehousesAsync(string groupId, List<string> warehouseIds);
    
    /// <summary>
    /// Gets the warehouse IDs assigned to a group
    /// </summary>
    Task<List<string>> GetGroupWarehousesAsync(string groupId);
    
    /// <summary>
    /// Gets all groups that have warehouse attributes
    /// </summary>
    Task<Dictionary<string, List<string>>> GetAllWarehouseGroupsAsync();
    
    /// <summary>
    /// Finds a group by name
    /// </summary>
    Task<string?> GetGroupIdByNameAsync(string groupName);
}

