using InventoryManagementSystem.API.Authorization;
using InventoryManagementSystem.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Serilog.Events;

// Configure Serilog with file sink (one file per day, 7-day retention)
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7, // One week retention
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB per file
        rollOnFileSizeLimit: true,
        shared: true,
        flushToDiskInterval: TimeSpan.FromSeconds(1))
    .CreateLogger();

try
{
    // Enable PII logging for development (shows actual token values in errors)
    IdentityModelEventSource.ShowPII = true;

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    // Add services to the container.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddInventoryManagementService();
    
    // Add APK Database (MySQL) support
    builder.Services.AddApkDatabase(builder.Configuration);

    // Register custom authorization handler for client-specific roles
    builder.Services.AddSingleton<IAuthorizationHandler, ClientRoleHandler>();
    builder.Services.AddScoped<IAuthorizationHandler, WarehouseAccessHandler>();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Keycloak:Issuer"];
            options.RequireHttpsMetadata = false; // Set to true in production
            options.Audience = "account";
            options.MapInboundClaims = false; // Keep original claim names from Keycloak

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Keycloak:Issuer"],

                ValidateAudience = true,
                ValidAudiences = ["account"],

                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5),

                NameClaimType = "preferred_username",
                RoleClaimType = "roles"
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers.Authorization.ToString();
                    Console.WriteLine(
                        $"OnMessageReceived - Authorization header received: {(!string.IsNullOrEmpty(authHeader) ? "Yes" : "No")}");
                    if (!string.IsNullOrEmpty(authHeader))
                    {
                        Console.WriteLine($"Authorization header length: {authHeader.Length}");
                    }

                    // Don't manually extract token - let the middleware do it automatically
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                    Console.WriteLine($"Exception type: {context.Exception.GetType().Name}");
                    if (context.Exception.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {context.Exception.InnerException.Message}");
                    }

                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var username = context.Principal?.Identity?.Name;
                    var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}");
                    Console.WriteLine($"Token validated successfully for user: {username}");
                    Console.WriteLine($"Claims: {string.Join(", ", claims ?? [])}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"OnChallenge - Error: {context.Error}");
                    Console.WriteLine($"OnChallenge - ErrorDescription: {context.ErrorDescription}");
                    Console.WriteLine($"OnChallenge - AuthenticateFailure: {context.AuthenticateFailure?.Message}");
                    return Task.CompletedTask;
                }
            };
        });

    // Configure authorization policies
    // Get client ID from configuration for client-specific role checking
    var clientId = builder.Configuration["Keycloak:ClientId"] ?? "inventory_management_system";

    builder.Services.AddAuthorizationBuilder()
        .AddPolicy(Policies.ViewCountingJournals, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.CreateCountingJournals, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.ManageCountingJournals, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.ViewCountingJournalLines, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.CreateCountingJournalLines, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.EditCountingJournalLines, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.DeleteCountingJournalLines, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.LockUnlockJournals, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.ViewItems, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.ViewOnHand, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.ViewLocations, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin, Roles.Warehouse)))
        .AddPolicy(Policies.WarehouseAccess, policy =>
            policy.Requirements.Add(new WarehouseAccessRequirement()))
        .AddPolicy(Policies.RequireAdminRole, policy =>
            policy.Requirements.Add(new ClientRoleRequirement(clientId, Roles.Admin)));

    builder.Services.AddControllers();
    // Add Razor Pages for APK version management UI
    builder.Services.AddRazorPages();
    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    
    // Enable static files for APK pages
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapRazorPages();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}