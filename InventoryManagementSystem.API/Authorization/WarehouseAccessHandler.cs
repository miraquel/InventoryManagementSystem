using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagementSystem.API.Authorization;

/// <summary>
/// Requirement that checks if user has access to a specific warehouse
/// </summary>
public class WarehouseAccessRequirement : IAuthorizationRequirement
{
}

/// <summary>
/// Handler that validates warehouse access from user's groups.
/// 
/// Keycloak Setup:
/// 1. Create groups in Keycloak: /Warehouses/WH001, /Warehouses/WH002, /Warehouses/ALL
/// 2. Create a Client Scope named "groups" with a mapper:
///    - Mapper Type: Group Membership
///    - Name: groups
///    - Token Claim Name: groups
///    - Full group path: OFF (to get just the group name)
///    - Add to ID token: ON
///    - Add to access token: ON
/// 3. Add the "groups" scope to your client as a Default scope
/// 4. Assign users to the appropriate warehouse groups
/// </summary>
public class WarehouseAccessHandler : AuthorizationHandler<WarehouseAccessRequirement>
{
    private const string WarehouseGroupPrefix = "IMS_Warehouse_";
    private const string AllWarehousesGroup = "IMS_Warehouse_ALL";
    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WarehouseAccessHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        WarehouseAccessRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        // Try to get warehouse from route values or query string
        var warehouseId = httpContext.Request.RouteValues["warehouseId"]?.ToString()
            ?? httpContext.Request.RouteValues["inventLocationId"]?.ToString()
            ?? httpContext.Request.Query["warehouseId"].FirstOrDefault()
            ?? httpContext.Request.Query["inventLocationId"].FirstOrDefault();

        // Also check for inventLocationId in the request body for POST/PUT requests
        // This is handled separately at the controller level

        if (string.IsNullOrEmpty(warehouseId))
        {
            // If no warehouse is specified, succeed (let the endpoint handle it)
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Get allowed warehouses (inventLocationIds) from the user's groups
        var allowedWarehouses = GetAllowedWarehouses(context.User);

        // If user has no warehouse groups assigned, deny access
        if (allowedWarehouses.Count == 0)
        {
            return Task.CompletedTask; // Fail - no warehouse access
        }

        // Check if user has access to the requested warehouse/inventLocationId
        if (allowedWarehouses.Contains(warehouseId, StringComparer.OrdinalIgnoreCase) 
            || allowedWarehouses.Contains("*")) // "*" means access to all warehouses
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Extracts the allowed warehouses from the user's group claims.
    /// Expected group format: "Warehouse_WH001", "Warehouse_CENTRAL", "Warehouse_ALL"
    /// </summary>
    public static List<string> GetAllowedWarehouses(ClaimsPrincipal user)
    {
        var allowedWarehouses = new List<string>();

        // Get all group claims (Keycloak sends groups as "groups" claim)
        var groups = user.FindAll("groups").Select(c => c.Value).ToList();
        
        // If groups claim is a JSON array (some Keycloak configurations)
        var groupsClaim = user.FindFirst("groups")?.Value;
        if (!string.IsNullOrEmpty(groupsClaim) && groupsClaim.StartsWith('['))
        {
            try
            {
                var jsonArray = System.Text.Json.JsonDocument.Parse(groupsClaim);
                foreach (var element in jsonArray.RootElement.EnumerateArray())
                {
                    var groupName = element.GetString();
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        groups.Add(groupName);
                    }
                }
            }
            catch
            {
                // Ignore JSON parsing errors
            }
        }

        foreach (var group in groups.Distinct())
        {
            // Check for "Warehouse_ALL" - grants access to all warehouses
            if (group.Equals(AllWarehousesGroup, StringComparison.OrdinalIgnoreCase) 
                || group.EndsWith("/" + AllWarehousesGroup, StringComparison.OrdinalIgnoreCase))
            {
                allowedWarehouses.Add("*");
                continue;
            }

            // Extract warehouse ID from group name (e.g., "Warehouse_WH001" -> "WH001")
            // Handle both flat groups and hierarchical paths (e.g., "/Warehouses/Warehouse_WH001")
            var groupName = group.Contains('/') ? group.Split('/').Last() : group;

            if (!groupName.StartsWith(WarehouseGroupPrefix, StringComparison.OrdinalIgnoreCase)) continue;
            
            var warehouseId = groupName[WarehouseGroupPrefix.Length..];
            if (!string.IsNullOrEmpty(warehouseId))
            {
                allowedWarehouses.Add(warehouseId);
            }
        }

        return allowedWarehouses.Distinct().ToList();
    }
}

/// <summary>
/// Extension methods for warehouse access
/// </summary>
public static class WarehouseAccessExtensions
{
    extension(ClaimsPrincipal user)
    {
        /// <summary>
        /// Gets the allowed warehouses (inventLocationIds) for the current user based on their groups
        /// </summary>
        public List<string> GetAllowedWarehouses()
        {
            return WarehouseAccessHandler.GetAllowedWarehouses(user);
        }
        
        /// <summary>
        /// Gets the allowed inventLocationIds for the current user based on their groups
        /// </summary>
        public List<string> GetAllowedInventLocations()
        {
            return WarehouseAccessHandler.GetAllowedWarehouses(user);
        }

        /// <summary>
        /// Checks if the user has access to a specific warehouse based on their groups[0] = {string} "RM" View
        /// </summary>
        public bool HasWarehouseAccess(string warehouseId)
        {
            var allowedWarehouses = user.GetAllowedWarehouses();
            return allowedWarehouses.Contains(warehouseId, StringComparer.OrdinalIgnoreCase)
                   || allowedWarehouses.Contains("*");
        }
        
        /// <summary>
        /// Checks if the user has access to a specific inventLocationId based on their groups
        /// </summary>
        public bool HasInventLocationAccess(string inventLocationId)
        {
            var allowedLocations = user.GetAllowedInventLocations();
            return allowedLocations.Count == 0 // No restrictions if no groups assigned
                   || allowedLocations.Contains(inventLocationId, StringComparer.OrdinalIgnoreCase)
                   || allowedLocations.Contains("*");
        }

        /// <summary>
        /// Gets the user's warehouse groups
        /// </summary>
        public List<string> GetWarehouseGroups()
        {
            return user.FindAll("groups")
                .Select(c => c.Value)
                .Where(g => g.Contains("IMS_Warehouse_", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}

