using InventoryManagementSystem.Dto.Common;

namespace InventoryManagementSystem.Service.Interface;

public interface IInventTableService
{
    Task<ServiceResponse> GetInventTableAsync(string itemId);
    Task<ServiceResponse> GetInventTablePagedListAsync(int pageNumber, int pageSize, string? searchQuery = null);
    Task<ServiceResponse> GetOnHandInventLocationListAsync(string? searchQuery, string inventSiteId);
    Task<ServiceResponse> GetOnHandWMSLocationPagedListAsync(int pageNumber, int pageSize, string? searchQuery, string inventLocationId);
    Task<ServiceResponse> GetOnHandPagedListAsync(int pageNumber, int pageSize, string? searchQuery = null, string? inventLocationId = null, string? wmsLocationId = null, string? storageDimensionGroup = null, string? trackingDimensionGroup = null);
    Task<ServiceResponse> GetOnHandAsync(string itemId, string inventBatchId, string inventSiteId, string inventLocationId, string wmsLocationId);
    Task<ServiceResponse> GetInventBatchAsync(string itemId, string inventBatchId);
    Task<ServiceResponse> GetInventBatchPagedListAsync(int pageNumber, int pageSize, string itemId, string? searchQuery = null);
}

