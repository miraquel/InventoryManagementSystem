using InventoryManagementSystem.Common.Logging;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using InventoryManagementSystem.Service.Interface;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service;

public partial class InventTableService : IInventTableService
{
    private readonly GMKInventoryManagementService _inventoryManagementService;
    private readonly ICallContextFactory _callContextFactory;
    private readonly ILogger<InventTableService> _logger;
    private readonly MapperlyMapper _mapper = new();

    public InventTableService(
        GMKInventoryManagementService inventoryManagementService, 
        ICallContextFactory callContextFactory,
        ILogger<InventTableService> logger)
    {
        _inventoryManagementService = inventoryManagementService;
        _callContextFactory = callContextFactory;
        _logger = logger;
    }

    public async Task<ServiceResponse> GetInventTableAsync(string itemId)
    {
        _logger.LogRetrievingEntity("inventory item", "ItemId", itemId);
        
        var request = new GMKInventoryManagementServiceGetInventTableRequest
        {
            CallContext = _callContextFactory.Create(),
            itemId = itemId
        };
        
        var response = await _inventoryManagementService.getInventTableAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("inventory item", "ItemId", itemId);
            return ServiceResponse.Failure("Failed to retrieve inventory item. No response from service.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Inventory item", "ItemId", itemId);
        return ServiceResponse<InventTableDto>.Success(
            _mapper.MapToDto(response.response), "Inventory item retrieved successfully.");
    }

    public async Task<ServiceResponse> GetInventTablePagedListAsync(int pageNumber, int pageSize, string? searchQuery = null)
    {
        _logger.LogRetrievingEntitiesPagedWithSearch("inventory items", pageNumber, pageSize, searchQuery);
        
        var request = new GMKInventoryManagementServiceGetInventTablePagedListRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchQuery = searchQuery
        };
        
        var response = await _inventoryManagementService.getInventTablePagedListAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponse("inventory items");
            return ServiceResponse.Failure("Failed to retrieve inventory items. No response from service.");
        }
        
        _logger.LogEntitiesRetrievedSuccessfully("Inventory items", response.response.TotalCount);
        return ServiceResponse<PagedListDto<InventTableDto>>.Success(
            _mapper.MapToDto(response.response), "Inventory items retrieved successfully.");
    }

    public async Task<ServiceResponse> GetOnHandInventLocationListAsync(string? searchQuery, string inventSiteId)
    {
        _logger.LogRetrievingOnHandInventoryLocations(inventSiteId, searchQuery);
        
        var request = new GMKInventoryManagementServiceGetOnHandInventLocationListRequest
        {
            CallContext = _callContextFactory.Create(),
            searchQuery = searchQuery ?? string.Empty,
            inventSiteId = inventSiteId
        };
        
        var response = await _inventoryManagementService.getOnHandInventLocationListAsync(request);

        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("on-hand inventory locations", "InventSiteId", inventSiteId);
            return ServiceResponse.Failure("Failed to retrieve on-hand inventory locations. No response from service.");
        }

        _logger.LogOnHandInventoryLocationsRetrievedSuccessfully(inventSiteId, response.response.Length);
        return ServiceResponse<List<InventLocationDto>>.Success(
            _mapper.MapToInventLocationDtoList(response.response),
            "On-hand inventory locations retrieved successfully.");
    }

    public async Task<ServiceResponse> GetOnHandWMSLocationPagedListAsync(int pageNumber, int pageSize, string? searchQuery, string inventLocationId)
    {
        _logger.LogRetrievingOnHandWMSLocations(inventLocationId, pageNumber, pageSize, searchQuery);
        
        var request = new GMKInventoryManagementServiceGetOnHandWMSLocationPagedListRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchQuery = searchQuery ?? string.Empty,
            inventLocationId = inventLocationId
        };
        var response = await _inventoryManagementService.getOnHandWMSLocationPagedListAsync(request);

        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("on-hand WMS locations", "InventLocationId", inventLocationId);
            return ServiceResponse.Failure("Failed to retrieve on-hand WMS locations. No response from service.");
        }

        _logger.LogOnHandWMSLocationsRetrievedSuccessfully(inventLocationId, response.response.TotalCount);
        return ServiceResponse<PagedListDto<WMSLocationDto>>.Success(
            _mapper.MapToDto(response.response),
            "On-hand WMS locations retrieved successfully.");
    }

    public async Task<ServiceResponse> GetOnHandPagedListAsync(int pageNumber, int pageSize, string? searchQuery = null, string? inventLocationId = null, string? wmsLocationId = null, string? storageDimensionGroup = null, string? trackingDimensionGroup = null)
    {
        _logger.LogRetrievingOnHandInventory(pageNumber, pageSize, searchQuery, inventLocationId, wmsLocationId);
        
        var request = new GMKInventoryManagementServiceGetOnHandPagedListRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber = pageNumber,
            pageSize = pageSize,
            searchQuery = searchQuery ?? string.Empty,
            inventLocationId = inventLocationId ?? string.Empty,
            wmsLocationId = wmsLocationId ?? string.Empty,
            storageDimensionGroup = storageDimensionGroup ?? string.Empty,
            trackingDimensionGroup = trackingDimensionGroup ?? string.Empty
        };
        var response = await _inventoryManagementService.getOnHandPagedListAsync(request);

        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponse("on-hand inventory");
            return ServiceResponse.Failure("Failed to retrieve on-hand inventory. No response from service.");
        }

        _logger.LogEntitiesRetrievedSuccessfully("On-hand inventory", response.response.TotalCount);
        return ServiceResponse<PagedListDto<OnHandDto>>.Success(
            _mapper.MapToDto(response.response),
            "On-hand inventory retrieved successfully.");
    }

    public async Task<ServiceResponse> GetOnHandAsync(string itemId, string inventBatchId, string inventSiteId, string inventLocationId, string wmsLocationId)
    {
        _logger.LogRetrievingOnHandInfo(itemId, inventBatchId, inventSiteId, inventLocationId, wmsLocationId);
        
        var request = new GMKInventoryManagementServiceGetOnHandRequest
        {
            CallContext = _callContextFactory.Create(),
            itemId = itemId,
            inventBatchId = inventBatchId,
            inventSiteId = inventSiteId,
            inventLocationId = inventLocationId,
            wmsLocationId = wmsLocationId
        };
        
        var response = await _inventoryManagementService.getOnHandAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("on-hand information", "ItemId", itemId);
            return ServiceResponse.Failure("Failed to retrieve on-hand information. No response from service.");
        }
        
        _logger.LogOnHandInfoRetrievedSuccessfully(itemId, response.response.AvailPhysical);
        return ServiceResponse<OnHandDto>.Success(
            _mapper.MapToDto(response.response), "On-hand information retrieved successfully.");
    }

    public async Task<ServiceResponse> GetInventBatchAsync(string itemId, string inventBatchId)
    {
        _logger.LogRetrievingEntity("inventory batch", "ItemId/InventBatchId", $"{itemId}/{inventBatchId}");
        
        var request = new GMKInventoryManagementServiceGetInventBatchRequest
        {
            CallContext = _callContextFactory.Create(),
            itemId = itemId,
            inventBatchId = inventBatchId
        };
        
        var response = await _inventoryManagementService.getInventBatchAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("inventory batch", "ItemId/InventBatchId", $"{itemId}/{inventBatchId}");
            return ServiceResponse.Failure("Failed to retrieve inventory batch. No response from service.");
        }
        
        if (!response.response.InventBatchId.Equals(inventBatchId, StringComparison.OrdinalIgnoreCase) ||
            !response.response.ItemId.Equals(itemId, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInventoryBatchRetrievedDoesNotMatchRequestedIdsRequestedItemIdInventBatchId(itemId, inventBatchId, response.response.ItemId, response.response.InventBatchId);
            return ServiceResponse.Failure("Inventory batch retrieved does not match requested IDs.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Inventory batch", "ItemId/InventBatchId", $"{itemId}/{inventBatchId}");
        return ServiceResponse<InventBatchDto>.Success(
            _mapper.MapToDto(response.response), "Inventory batch retrieved successfully.");
    }

    public async Task<ServiceResponse> GetInventBatchPagedListAsync(int pageNumber, int pageSize, string itemId, string? searchQuery = null)
    {
        _logger.LogRetrievingEntitiesPagedWithSearch("inventory batches", pageNumber, pageSize, searchQuery);
        
        var request = new GMKInventoryManagementServiceGetInventBatchPagedListRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber = pageNumber,
            pageSize = pageSize,
            itemId = itemId,
            searchQuery = searchQuery ?? string.Empty
        };
        
        var response = await _inventoryManagementService.getInventBatchPagedListAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponse("inventory batches");
            return ServiceResponse.Failure("Failed to retrieve inventory batches. No response from service.");
        }
        
        _logger.LogEntitiesRetrievedSuccessfully("Inventory batches", response.response.TotalCount);
        return ServiceResponse<PagedListDto<InventBatchDto>>.Success(
            _mapper.MapToDto(response.response), "Inventory batches retrieved successfully.");
    }
}

