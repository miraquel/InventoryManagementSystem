using Microsoft.AspNetCore.Authorization;

namespace InventoryManagementSystem.API.Authorization;

/// <summary>
/// Custom requirement for client-specific roles from Keycloak
/// </summary>
public class ClientRoleRequirement : IAuthorizationRequirement
{
    public string ClientId { get; }
    public string[] Roles { get; }

    public ClientRoleRequirement(string clientId, params string[] roles)
    {
        ClientId = clientId;
        Roles = roles;
    }
}

/// <summary>
/// Handler that checks if user has required client-specific role from Keycloak resource_access claim
/// </summary>
public partial class ClientRoleHandler : AuthorizationHandler<ClientRoleRequirement>
{
    private readonly ILogger<ClientRoleHandler> _logger;

    public ClientRoleHandler(ILogger<ClientRoleHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        ClientRoleRequirement requirement)
    {
        // First, try to get roles from the "roles" claim (your current setup)
        // This handles the flat roles array in your token
        var rolesClaims = context.User.FindAll("roles").Select(c => c.Value).ToList();
        
        LogCheckingAuthorizationForClientRequirementClientId(requirement.ClientId);
        LogRequiredRolesJoin(string.Join(", ", requirement.Roles));
        LogUserRolesFromRolesClaimJoin(string.Join(", ", rolesClaims));
        
        // Check if user has any of the required roles in the flat roles array
        var hasRequiredRole = requirement.Roles.Any(requiredRole => 
            rolesClaims.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));
        
        if (hasRequiredRole)
        {
            LogAuthorizationSucceededUserHasRequiredRoleInRolesClaim();
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Fallback: Try to get roles from resource_access claim (proper Keycloak client roles)
        // This handles client-specific roles when properly configured
        var resourceAccessClaim = context.User.FindFirst("resource_access")?.Value;
        
        if (!string.IsNullOrEmpty(resourceAccessClaim))
        {
            try
            {
                // Parse the resource_access JSON to get client-specific roles
                var resourceAccess = System.Text.Json.JsonDocument.Parse(resourceAccessClaim);
                
                if (resourceAccess.RootElement.TryGetProperty(requirement.ClientId, out var clientElement))
                {
                    if (clientElement.TryGetProperty("roles", out var rolesElement))
                    {
                        var clientRoles = rolesElement.EnumerateArray()
                            .Select(r => r.GetString())
                            .Where(r => r != null)
                            .ToList();

                        LogUserRolesFromResourceAccessRequirementClientIdJoin(requirement.ClientId, string.Join(", ", clientRoles));

                        var hasClientRole = requirement.Roles.Any(requiredRole => 
                            clientRoles.Contains(requiredRole, StringComparer.OrdinalIgnoreCase));

                        if (hasClientRole)
                        {
                            LogAuthorizationSucceededUserHasRequiredRoleInClientRoles();
                            context.Succeed(requirement);
                            return Task.CompletedTask;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogFailedToParseResourceAccessClaimExMessage(ex.Message);
            }
        }

        LogAuthorizationFailedUserDoesNotHaveRequiredRole();
        return Task.CompletedTask;
    }

    [LoggerMessage(LogLevel.Information, "Checking authorization for client: {requirementClientId}")]
    partial void LogCheckingAuthorizationForClientRequirementClientId(string requirementClientId);

    [LoggerMessage(LogLevel.Information, "Required roles: {join}")]
    partial void LogRequiredRolesJoin(string join);

    [LoggerMessage(LogLevel.Information, "User roles from 'roles' claim: {join}")]
    partial void LogUserRolesFromRolesClaimJoin(string join);

    [LoggerMessage(LogLevel.Information, "Authorization succeeded: User has required role in roles claim")]
    partial void LogAuthorizationSucceededUserHasRequiredRoleInRolesClaim();

    [LoggerMessage(LogLevel.Information, "User roles from resource_access.{requirementClientId}: {join}")]
    partial void LogUserRolesFromResourceAccessRequirementClientIdJoin(string requirementClientId, string join);

    [LoggerMessage(LogLevel.Information, "Authorization succeeded: User has required role in client roles")]
    partial void LogAuthorizationSucceededUserHasRequiredRoleInClientRoles();

    [LoggerMessage(LogLevel.Warning, "Failed to parse resource_access claim: {exMessage}")]
    partial void LogFailedToParseResourceAccessClaimExMessage(string exMessage);

    [LoggerMessage(LogLevel.Warning, "Authorization failed: User does not have required role")]
    partial void LogAuthorizationFailedUserDoesNotHaveRequiredRole();
}

