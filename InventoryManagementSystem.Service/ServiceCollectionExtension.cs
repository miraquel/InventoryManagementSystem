using System.ServiceModel;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystem.Service;

public static class ServiceCollectionExtension
{
    public static void AddInventoryManagementService(this IServiceCollection services)
    {
        services.AddScoped<GMKInventoryManagementService, GMKInventoryManagementServiceClient>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var urlConfig = config["DynamicsAXIntegration:Url"] ?? throw new InvalidOperationException("DynamicsAXIntegration:Url is not exists in configuration");
            var endpointIdentityConfig = config["DynamicsAXIntegration:EndpointIdentity"] ?? throw new InvalidOperationException("DynamicsAXIntegration:EndpointIdentity is not exists in configuration");
            var binding = new NetTcpBinding
            {
                ReceiveTimeout = TimeSpan.FromMinutes(5),
                SendTimeout = TimeSpan.FromMinutes(5),
                MaxReceivedMessageSize = 2000000,
                Security =
                {
                    Mode = SecurityMode.Transport,
                    Transport =
                    {
                        ClientCredentialType = TcpClientCredentialType.Windows
                    }
                },
            };

            var endpointIdentity = new UpnEndpointIdentity(endpointIdentityConfig);

            var uri = new Uri(urlConfig);
            var endpointAddress = new EndpointAddress(uri, endpointIdentity); // You can see "UpnEndpointIdentity" referenced here.
            var client = new GMKInventoryManagementServiceClient(binding, endpointAddress);
            // use client.ClientCredentials.Windows.ClientCredential to set the credentials
            client.ClientCredentials.Windows.ClientCredential.UserName = config["DynamicsAXIntegration:UserName"] ?? throw new InvalidOperationException("DynamicsAXIntegration:UserName is not exists in configuration");
            client.ClientCredentials.Windows.ClientCredential.Password = config["DynamicsAXIntegration:Password"] ?? throw new InvalidOperationException("DynamicsAXIntegration:Password is not exists in configuration");
            client.ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
            
            return client;
        });
        
        services.AddScoped<ICountingService, CountingService>();
    }
}