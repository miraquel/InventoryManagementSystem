using InventoryManagementSystem.API;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using InventoryManagementSystem.Service.Interface;

namespace InventoryManagementSystem.Service;

public class CountingService : ICountingService
{
    private readonly GMKInventoryManagementService _inventoryManagementService;
    private readonly MapperlyMapper _mapper = new();

    public CountingService(GMKInventoryManagementService inventoryManagementService)
    {
        _inventoryManagementService = inventoryManagementService;
    }

    public async Task<ServiceResponse> CreateCountingJournalAsync(CreateCountingJournalDto dto)
    {
        var request = new GMKInventoryManagementServiceCreateCountingJournalRequest
        {
            CallContext = new CallContext
            {
                Company = "GMK"
            },
            parm = new GMKInventJournalCreateDataContract
            {
                InventSiteId = dto.InventSiteId,
                InventLocationId = dto.InventLocationId,
                WMSLocationId = dto.WMSLocationId,
                SlsNo = "chaidir.ali"
            }
        };
        var response = await _inventoryManagementService.createCountingJournalAsync(request);
        
        if (response?.response == null)
        {
            return ServiceResponse.Failure("Failed to create counting journal. No response from service.");
        }
        
        return !response.response.Success ? ServiceResponse.Failure(response.response.Message) : ServiceResponse.Success(response.response.Message);
    }

    public async Task<ServiceResponse> GetCountingJournalsAsync(int pageNumber, int pageSize)
    {
        var request = new GMKInventoryManagementServiceGetInventJournalTablePagedRequest
        {
            CallContext = new CallContext
            {
                Company = "GMK"
            },
            pageNumber = pageNumber,
            pageSize = pageSize,
            parm = new GMKInventJournalTableDataContract()
        };
        var response = await _inventoryManagementService.getInventJournalTablePagedAsync(request);
        if (response?.response == null)
        {
            return ServiceResponse.Failure("Failed to retrieve counting journals. No response from service.");
        }

        return ServiceResponse<PagedListDto<InventJournalTableDto>>.Success( 
            _mapper.MapToDto(response.response), "Counting journals retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalAsync(string journalId)
    {
        var request = new GMKInventoryManagementServiceGetInventJournalTableRequest
        {
            CallContext = new CallContext
            {
                Company = "GMK"
            },
            journalId = journalId
        };
        
        var response = await _inventoryManagementService.getInventJournalTableAsync(request);
        if (response?.response == null)
        {
            return ServiceResponse.Failure("Failed to retrieve counting journal. No response from service.");
        }
        
        return ServiceResponse<InventJournalTableDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalLinesAsync(int pageNumber, int pageSize, string journalId)
    {
        var request = new GMKInventoryManagementServiceGetInventJournalTransPagedRequest
        {
            CallContext = new CallContext
            {
                Company = "GMK"
            },
            parm = new GMKInventJournalTransDataContract()
        };
        var response = await _inventoryManagementService.getInventJournalTransPagedAsync(request);
        if (response?.response == null)
        {
            return ServiceResponse.Failure("Failed to retrieve counting journal transactions. No response from service.");
        }

        return ServiceResponse<PagedListDto<InventJournalTransDto>>.Success( 
            _mapper.MapToDto(response.response), "Counting journal transactions retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalLineAsync(string inventTransId)
    {
        var request = new GMKInventoryManagementServiceGetInventJournalTransRequest
        {
            CallContext = new CallContext
            {
                Company = "GMK"
            },
            inventTransId = inventTransId
        };
        
        var response = await _inventoryManagementService.getInventJournalTransAsync(request);
        if (response?.response == null)
        {
            return ServiceResponse.Failure("Failed to retrieve counting journal transaction. No response from service.");
        }
        
        return ServiceResponse<InventJournalTransDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal transaction retrieved successfully.");
    }
}