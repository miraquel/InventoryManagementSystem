using InventoryManagementSystem.Dto.Common;

namespace InventoryManagementSystem.Service.Interface;

public interface ILocationService
{
    Task<ServiceResponse> GetInventLocationListAsync(string inventSiteId);
    Task<ServiceResponse> GetWMSLocationAsync(string wmsLocationId, string inventLocationId);
    Task<ServiceResponse> GetWMSLocationPagedListAsync(int pageNumber, int pageSize, string inventLocationId, string? wmsLocationId = null);
}

