using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service;

/// <summary>
/// Service for managing Keycloak groups and attributes via Admin API
/// </summary>
public partial class KeycloakAdminService : IKeycloakAdminService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<KeycloakAdminService> _logger;
    private readonly string _keycloakUrl;
    private readonly string _realm;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public KeycloakAdminService(HttpClient httpClient, IConfiguration configuration, ILogger<KeycloakAdminService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        _keycloakUrl = configuration["Keycloak:Authority"] ?? configuration["Keycloak:BaseUrl"] ?? "http://localhost:8080";
        _realm = configuration["Keycloak:Realm"] ?? "InventoryManagement";
        _clientId = configuration["Keycloak:ClientId"] ?? "";
        _clientSecret = configuration["Keycloak:ClientSecret"] ?? "";
        
        LogKeycloakadminserviceInitializedUrlUrlRealmRealmClientidClientid(_keycloakUrl, _realm, _clientId);
    }

    /// <summary>
    /// Gets an admin access token using client credentials
    /// </summary>
    private async Task<string?> GetAdminTokenAsync()
    {
        var tokenUrl = $"{_keycloakUrl}/realms/{_realm}/protocol/openid-connect/token";
        
        LogRequestingAdminTokenFromTokenurl(tokenUrl);
        
        var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret
            })
        };

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            LogFailedToGetAdminTokenStatusStatusResponseResponse(response.StatusCode, responseContent);
            return null;
        }

        var tokenResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        LogSuccessfullyObtainedAdminToken();
        return tokenResponse.GetProperty("access_token").GetString();
    }

    /// <inheritdoc />
    public async Task<string?> GetGroupIdByNameAsync(string groupName)
    {
        var token = await GetAdminTokenAsync();
        if (token == null)
        {
            LogFailedToObtainAdminTokenForGetgroupidbynameasync();
            return null;
        }

        var url = $"{_keycloakUrl}/admin/realms/{_realm}/groups?search={Uri.EscapeDataString(groupName)}";
        
        LogSearchingForGroupGroupnameAtUrl(groupName, url);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            LogFailedToGetGroupByNameStatusStatusResponseResponse(response.StatusCode, content);
            
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                LogForbiddenServiceAccountNeedsQueryGroupsRoleInRealmManagementClientGoToKeycloak(_clientId);
            }
            
            return null;
        }

        var groups = JsonSerializer.Deserialize<JsonElement[]>(content);
        
        // Find exact match
        var group = groups?.FirstOrDefault(g => 
            g.GetProperty("name").GetString()?.Equals(groupName, StringComparison.OrdinalIgnoreCase) == true);
        
        if (group == null)
        {
            LogGroupGroupnameNotFound(groupName);
        }
        
        return group?.GetProperty("id").GetString();
    }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <inheritdoc />
    public async Task<List<string>> GetGroupWarehousesAsync(string groupId)
    {
        var token = await GetAdminTokenAsync();
        if (token == null)
        {
            LogFailedToObtainAdminTokenForGetgroupwarehousesasync();
            return [];
        }

        var url = $"{_keycloakUrl}/admin/realms/{_realm}/groups/{groupId}";
        
        LogGettingWarehousesForGroupGroupid(groupId);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            LogFailedToGetGroupWarehousesStatusStatusResponseResponse(response.StatusCode, content);
            
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                LogForbiddenServiceAccountNeedsQueryGroupsAndViewUsersRolesGoToKeycloakClients(_clientId);
            }
            
            return [];
        }

        var group = JsonSerializer.Deserialize<JsonElement>(content);
        
        if (!group.TryGetProperty("attributes", out var attributes) || !attributes.TryGetProperty("warehouses", out var warehousesElement)) 
            return [];

        var warehouses = new List<string>();
        switch (warehousesElement.ValueKind)
        {
            case JsonValueKind.Array:
            {
                foreach (var element in warehousesElement.EnumerateArray())
                {
                    var value = element.GetString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        // Handle comma-separated values in a single element
                        warehouses.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w.Trim()));
                    }
                }

                break;
            }
            case JsonValueKind.String:
            {
                var value = warehousesElement.GetString();
                if (!string.IsNullOrEmpty(value))
                {
                    warehouses.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(w => w.Trim()));
                }

                break;
            }
            case JsonValueKind.Undefined:
            case JsonValueKind.Object:
            case JsonValueKind.Number:
            case JsonValueKind.True:
            case JsonValueKind.False:
            case JsonValueKind.Null:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return warehouses.Distinct().ToList();
    }

    /// <inheritdoc />
    public async Task<bool> SetGroupWarehousesAsync(string groupId, List<string> warehouseIds)
    {
        LogSettingWarehousesForGroupGroupidWarehouses(groupId, string.Join(",", warehouseIds));
        
        var token = await GetAdminTokenAsync();
        if (token == null)
        {
            LogFailedToObtainAdminTokenForSetgroupwarehousesasync();
            return false;
        }

        // First, get the current group data
        var getUrl = $"{_keycloakUrl}/admin/realms/{_realm}/groups/{groupId}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var getResponse = await _httpClient.GetAsync(getUrl);
        var getContent = await getResponse.Content.ReadAsStringAsync();
        
        if (!getResponse.IsSuccessStatusCode)
        {
            LogFailedToGetGroupDataStatusStatusResponseResponse(getResponse.StatusCode, getContent);
            
            if (getResponse.StatusCode == HttpStatusCode.Forbidden)
            {
                LogForbiddenServiceAccountNeedsQueryGroupsAndManageUsersRolesGoToKeycloakClients(_clientId);
            }
            
            return false;
        }

        var groupData = JsonSerializer.Deserialize<Dictionary<string, object>>(getContent);
        
        if (groupData == null) return false;

        // Update the attributes
        if (!groupData.ContainsKey("attributes"))
        {
            groupData["attributes"] = new Dictionary<string, object>();
        }

        var attributes = groupData["attributes"] as Dictionary<string, object> 
            ?? JsonSerializer.Deserialize<Dictionary<string, object>>(groupData["attributes"].ToString() ?? "{}");
        
        if (attributes == null) return false;

        // Set the warehouses attribute (as comma-separated string or array)
        attributes["warehouses"] = new[] { string.Join(",", warehouseIds) };
        groupData["attributes"] = attributes;

        // Update the group
        var updateUrl = $"{_keycloakUrl}/admin/realms/{_realm}/groups/{groupId}";
        var jsonContent = JsonSerializer.Serialize(groupData);
        
        LogUpdatingGroupAttributesWithJsonJson(jsonContent);
        
        var updateResponse = await _httpClient.PutAsync(
            updateUrl, 
            new StringContent(jsonContent, Encoding.UTF8, "application/json")
        );
        
        var updateContent = await updateResponse.Content.ReadAsStringAsync();

        if (!updateResponse.IsSuccessStatusCode)
        {
            LogFailedToUpdateGroupWarehousesStatusStatusResponseResponse(updateResponse.StatusCode, updateContent);
            
            if (updateResponse.StatusCode == HttpStatusCode.Forbidden)
            {
                LogForbiddenServiceAccountNeedsManageUsersRoleToModifyGroupAttributesGoToKeycloak(_clientId);
            }
            
            return false;
        }
        
        LogSuccessfullyUpdatedWarehousesForGroupGroupid(groupId);
        return true;
    }

    /// <inheritdoc />
    public async Task<bool> AddWarehouseToGroupAsync(string groupId, string warehouseId)
    {
        var currentWarehouses = await GetGroupWarehousesAsync(groupId);
        
        if (currentWarehouses.Contains(warehouseId, StringComparer.OrdinalIgnoreCase))
        {
            return true; // Already exists
        }

        currentWarehouses.Add(warehouseId);
        return await SetGroupWarehousesAsync(groupId, currentWarehouses);
    }

    /// <inheritdoc />
    public async Task<bool> RemoveWarehouseFromGroupAsync(string groupId, string warehouseId)
    {
        var currentWarehouses = await GetGroupWarehousesAsync(groupId);
        
        currentWarehouses.RemoveAll(w => w.Equals(warehouseId, StringComparison.OrdinalIgnoreCase));
        
        return await SetGroupWarehousesAsync(groupId, currentWarehouses);
    }

    /// <inheritdoc />
    public async Task<Dictionary<string, List<string>>> GetAllWarehouseGroupsAsync()
    {
        LogGettingAllWarehouseGroupsFromKeycloak();
        
        var token = await GetAdminTokenAsync();
        if (token == null)
        {
            LogFailedToObtainAdminTokenForGetallwarehousegroupsasync();
            return new Dictionary<string, List<string>>();
        }

        var url = $"{_keycloakUrl}/admin/realms/{_realm}/groups";
        
        LogFetchingAllGroupsFromUrl(url);
        
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            LogFailedToGetAllWarehouseGroupsStatusStatusResponseResponse(response.StatusCode, content);
            
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                LogForbiddenServiceAccountLacksPermissionsToQueryGroupsRequiredRolesQueryGroups();
                LogFixGoToKeycloakAdminConsoleClientsClientidServiceAccountRolesTab(_clientId);
                LogThenSelectClientRolesDropdownRealmManagementAddTheseRolesFromAvailableRoles();
                LogQueryGroupsToReadGroupInformation();
                LogViewUsersToViewUserData();
                LogManageUsersIfYouNeedToModifyGroupAttributesLater();
            }
            
            return new Dictionary<string, List<string>>();
        }
        var groups = JsonSerializer.Deserialize<JsonElement[]>(content);
        
        var result = new Dictionary<string, List<string>>();
        
        if (groups == null) return result;

        foreach (var group in groups)
        {
            var groupName = group.GetProperty("name").GetString();
            var groupId = group.GetProperty("id").GetString();
            
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(groupId)) 
                continue;

            if (group.TryGetProperty("attributes", out var attributes) &&
                attributes.TryGetProperty("warehouses", out var warehousesElement))
            {
                var warehouses = new List<string>();
                
                if (warehousesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var element in warehousesElement.EnumerateArray())
                    {
                        var value = element.GetString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            warehouses.AddRange(value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(w => w.Trim()));
                        }
                    }
                }

                if (warehouses.Any())
                {
                    result[groupName] = warehouses;
                }
            }
        }

        return result;
    }

    [LoggerMessage(LogLevel.Information, "KeycloakAdminService initialized: URL={url}, Realm={realm}, ClientId={clientId}")]
    partial void LogKeycloakadminserviceInitializedUrlUrlRealmRealmClientidClientid(string url, string realm, string clientId);

    [LoggerMessage(LogLevel.Debug, "Requesting admin token from: {tokenUrl}")]
    partial void LogRequestingAdminTokenFromTokenurl(string tokenUrl);

    [LoggerMessage(LogLevel.Error, "Failed to get admin token. Status: {status}, Response: {response}")]
    partial void LogFailedToGetAdminTokenStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Debug, "Successfully obtained admin token")]
    partial void LogSuccessfullyObtainedAdminToken();

    [LoggerMessage(LogLevel.Error, "Failed to obtain admin token for GetGroupIdByNameAsync")]
    partial void LogFailedToObtainAdminTokenForGetgroupidbynameasync();

    [LoggerMessage(LogLevel.Debug, "Searching for group: {groupName} at {url}")]
    partial void LogSearchingForGroupGroupnameAtUrl(string groupName, string url);

    [LoggerMessage(LogLevel.Error, "Failed to get group by name. Status: {status}, Response: {response}")]
    partial void LogFailedToGetGroupByNameStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Error, "FORBIDDEN: Service account needs 'query-groups' role in realm-management client. " +
                                   "Go to Keycloak: Clients → {clientId} → Service Account Roles → Add 'realm-management' → 'query-groups'")]
    partial void LogForbiddenServiceAccountNeedsQueryGroupsRoleInRealmManagementClientGoToKeycloak(string clientId);

    [LoggerMessage(LogLevel.Warning, "Group '{groupName}' not found")]
    partial void LogGroupGroupnameNotFound(string groupName);

    [LoggerMessage(LogLevel.Error, "Failed to obtain admin token for GetGroupWarehousesAsync")]
    partial void LogFailedToObtainAdminTokenForGetgroupwarehousesasync();

    [LoggerMessage(LogLevel.Debug, "Getting warehouses for group: {groupId}")]
    partial void LogGettingWarehousesForGroupGroupid(string groupId);

    [LoggerMessage(LogLevel.Error, "Failed to get group warehouses. Status: {status}, Response: {response}")]
    partial void LogFailedToGetGroupWarehousesStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Error, "FORBIDDEN: Service account needs 'query-groups' and 'view-users' roles. " +
                                   "Go to Keycloak: Clients → {clientId} → Service Account Roles → Add 'realm-management' → 'query-groups', 'view-users'")]
    partial void LogForbiddenServiceAccountNeedsQueryGroupsAndViewUsersRolesGoToKeycloakClients(string clientId);

    [LoggerMessage(LogLevel.Information, "Setting warehouses for group {groupId}: {warehouses}")]
    partial void LogSettingWarehousesForGroupGroupidWarehouses(string groupId, string warehouses);

    [LoggerMessage(LogLevel.Error, "Failed to obtain admin token for SetGroupWarehousesAsync")]
    partial void LogFailedToObtainAdminTokenForSetgroupwarehousesasync();

    [LoggerMessage(LogLevel.Error, "Failed to get group data. Status: {status}, Response: {response}")]
    partial void LogFailedToGetGroupDataStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Error, "FORBIDDEN: Service account needs 'query-groups' and 'manage-users' roles. " +
                                   "Go to Keycloak: Clients → {clientId} → Service Account Roles → Add 'realm-management' → 'query-groups', 'manage-users'")]
    partial void LogForbiddenServiceAccountNeedsQueryGroupsAndManageUsersRolesGoToKeycloakClients(string clientId);

    [LoggerMessage(LogLevel.Debug, "Updating group attributes with JSON: {json}")]
    partial void LogUpdatingGroupAttributesWithJsonJson(string json);

    [LoggerMessage(LogLevel.Error, "Failed to update group warehouses. Status: {status}, Response: {response}")]
    partial void LogFailedToUpdateGroupWarehousesStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Error, "FORBIDDEN: Service account needs 'manage-users' role to modify group attributes. " +
                                   "Go to Keycloak: Clients → {clientId} → Service Account Roles → Client Roles: realm-management → Add 'manage-users'")]
    partial void LogForbiddenServiceAccountNeedsManageUsersRoleToModifyGroupAttributesGoToKeycloak(string clientId);

    [LoggerMessage(LogLevel.Information, "Successfully updated warehouses for group {groupId}")]
    partial void LogSuccessfullyUpdatedWarehousesForGroupGroupid(string groupId);

    [LoggerMessage(LogLevel.Debug, "Getting all warehouse groups from Keycloak")]
    partial void LogGettingAllWarehouseGroupsFromKeycloak();

    [LoggerMessage(LogLevel.Error, "Failed to obtain admin token for GetAllWarehouseGroupsAsync")]
    partial void LogFailedToObtainAdminTokenForGetallwarehousegroupsasync();

    [LoggerMessage(LogLevel.Debug, "Fetching all groups from: {url}")]
    partial void LogFetchingAllGroupsFromUrl(string url);

    [LoggerMessage(LogLevel.Error, "Failed to get all warehouse groups. Status: {status}, Response: {response}")]
    partial void LogFailedToGetAllWarehouseGroupsStatusStatusResponseResponse(HttpStatusCode status, string response);

    [LoggerMessage(LogLevel.Error, "FORBIDDEN (403): Service account lacks permissions to query groups. " +
                                   "Required roles: 'query-groups' and 'view-users' in realm-management client.")]
    partial void LogForbiddenServiceAccountLacksPermissionsToQueryGroupsRequiredRolesQueryGroups();

    [LoggerMessage(LogLevel.Error, "Fix: Go to Keycloak Admin Console → Clients → {clientId} → Service Account Roles tab")]
    partial void LogFixGoToKeycloakAdminConsoleClientsClientidServiceAccountRolesTab(string clientId);

    [LoggerMessage(LogLevel.Error, "Then: Select Client Roles dropdown → 'realm-management' → Add these roles from Available Roles:")]
    partial void LogThenSelectClientRolesDropdownRealmManagementAddTheseRolesFromAvailableRoles();

    [LoggerMessage(LogLevel.Error, "  1. query-groups (to read group information)")]
    partial void LogQueryGroupsToReadGroupInformation();

    [LoggerMessage(LogLevel.Error, "  2. view-users (to view user data)")]
    partial void LogViewUsersToViewUserData();

    [LoggerMessage(LogLevel.Error, "  3. manage-users (if you need to modify group attributes later)")]
    partial void LogManageUsersIfYouNeedToModifyGroupAttributesLater();
}

