using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace InventoryManagementSystem.Service;

public interface ICallContextFactory
{
    CallContext Create();
}

public class CallContextFactory : ICallContextFactory
{
    private readonly string _company;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _defaultUser;

    public CallContextFactory(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _company = configuration["DynamicsAXIntegration:Company"] ?? "GMK";
        _defaultUser = configuration["DynamicsAXIntegration:DefaultUser"] ?? "axservices";
        _httpContextAccessor = httpContextAccessor;
    }

    public CallContext Create()
    {
        var username = GetUsernameFromClaims();
        
        return new CallContext
        {
            Company = _company
        };
    }

    private string GetUsernameFromClaims()
    {
        var user = _httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true) return _defaultUser;
        
        // Try to get preferred_username (Keycloak standard claim)
        var preferredUsername = user.FindFirst("preferred_username")?.Value;
        if (!string.IsNullOrEmpty(preferredUsername))
            return preferredUsername;
            
        // Try to get username claim
        var username = user.FindFirst("username")?.Value;
        if (!string.IsNullOrEmpty(username))
            return username;
            
        // Try to get name claim
        var name = user.FindFirst("name")?.Value;
        if (!string.IsNullOrEmpty(name))
            return name;
            
        // Try to get sub (subject) claim as fallback
        var sub = user.FindFirst("sub")?.Value;
        return !string.IsNullOrEmpty(sub) ? sub :
            // Return default user if no claims found or user is not authenticated
            _defaultUser;
    }
}

