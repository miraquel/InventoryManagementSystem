using InventoryManagementSystem.Common.Logging;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service;

public partial class LocationService : ILocationService
{
    private readonly GMKInventoryManagementService _inventoryManagementService;
    private readonly ICallContextFactory _callContextFactory;
    private readonly ILogger<LocationService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public LocationService(
        GMKInventoryManagementService inventoryManagementService, 
        ICallContextFactory callContextFactory,
        ILogger<LocationService> logger)
    {
        _inventoryManagementService = inventoryManagementService;
        _callContextFactory = callContextFactory;
        _logger = logger;
    }

    public async Task<ServiceResponse> GetInventLocationListAsync(string inventSiteId)
    {
        _logger.LogRetrievingEntity("inventory locations", "InventSiteId", inventSiteId);
        
        var request = new GMKInventoryManagementServiceGetInventLocationListRequest
        {
            CallContext = _callContextFactory.Create(),
            parm = new GMKInventLocationDataContract
            {
                InventSiteId = inventSiteId
            }
        };
        
        var response = await _inventoryManagementService.getInventLocationListAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("inventory locations", "InventSiteId", inventSiteId);
            return ServiceResponse.Failure("Failed to retrieve inventory locations. No response from service.");
        }
        
        _logger.LogEntitiesListRetrievedSuccessfully("Inventory locations", response.response.Length);
        return ServiceResponse<List<InventLocationDto>>.Success(
            _mapper.MapToInventLocationDtoList(response.response), "Inventory locations retrieved successfully.");
    }

    public async Task<ServiceResponse> GetWMSLocationAsync(string wmsLocationId, string inventLocationId)
    {
        _logger.LogRetrievingWMSLocation(wmsLocationId, inventLocationId);
        
        var request = new GMKInventoryManagementServiceGetWMSLocationRequest
        {
            CallContext = _callContextFactory.Create(),
            parm = new GMKWMSLocationDataContract
            {
                WMSLocationId = wmsLocationId,
                InventLocationId = inventLocationId
            }
        };
        
        var response = await _inventoryManagementService.getWMSLocationAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveWMSLocation(wmsLocationId, inventLocationId);
            return ServiceResponse.Failure("Failed to retrieve WMS location. No response from service.");
        }

        if (string.IsNullOrEmpty(response.response.WMSLocationId))
        {
            _logger.LogWMSLocationNotFoundWmsLocationIdInInventlocationidInventlocationid(wmsLocationId, inventLocationId);
            return ServiceResponse.Failure("WMS location not found.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("WMS location", "WMSLocationId", wmsLocationId);
        return ServiceResponse<WMSLocationDto>.Success(
            _mapper.MapToDto(response.response), "WMS location retrieved successfully.");
    }

    public async Task<ServiceResponse> GetWMSLocationPagedListAsync(int pageNumber, int pageSize, string inventLocationId, string? wmsLocationId = null)
    {
        _logger.LogRetrievingWMSLocationsPaged(inventLocationId, pageNumber, pageSize, wmsLocationId);
        
        var request = new GMKInventoryManagementServiceGetWMSLocationPagedListRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber = pageNumber,
            pageSize = pageSize,
            parm = new GMKWMSLocationDataContract
            {
                InventLocationId = inventLocationId,
                WMSLocationId = wmsLocationId
            }
        };
        
        var response = await _inventoryManagementService.getWMSLocationPagedListAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("WMS locations", "InventLocationId", inventLocationId);
            return ServiceResponse.Failure("Failed to retrieve WMS locations. No response from service.");
        }
        
        _logger.LogWMSLocationsRetrievedSuccessfully(inventLocationId, response.response.TotalCount);
        return ServiceResponse<PagedListDto<WMSLocationDto>>.Success(
            _mapper.MapToDto(response.response), "WMS locations retrieved successfully.");
    }
}

