using InventoryManagementSystem.Common.Logging;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;
using InventoryManagementSystem.Service.GMKInventoryManagementServiceGroup;
using InventoryManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service;

public class CountingService : ICountingService
{
    private readonly GMKInventoryManagementService _inventoryManagementService;
    private readonly ICallContextFactory _callContextFactory;
    private readonly ILogger<CountingService> _logger;
    private readonly MapperlyMapper _mapper = new();
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _defaultUser;

    public CountingService(
        IConfiguration configuration,
        GMKInventoryManagementService inventoryManagementService, 
        ICallContextFactory callContextFactory,
        ILogger<CountingService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _inventoryManagementService = inventoryManagementService;
        _callContextFactory = callContextFactory;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
        _defaultUser = configuration["DynamicsAXIntegration:DefaultUser"] ?? "axservices";
    }

    public async Task<ServiceResponse> CreateCountingJournalAsync(CreateCountingJournalDto dto)
    {
        _logger.LogCreatingCountingJournal(dto.InventSiteId, dto.InventLocationId, dto.WMSLocationId);
        
        var callContext = _callContextFactory.Create();
        var request = new GMKInventoryManagementServiceCreateCountingJournalRequest
        {
            CallContext = callContext,
            parm = new GMKInventJournalCreateDataContract
            {
                InventSiteId = dto.InventSiteId,
                InventLocationId = dto.InventLocationId,
                WMSLocationId = dto.WMSLocationId,
                SlsNo = GetUsernameFromClaims(),
                Description = $"{dto.Description} - {GetUsernameFromClaims()} WH {dto.InventLocationId}"
            }
        };
        var response = await _inventoryManagementService.createCountingJournalAsync(request);
        
        if (response?.response == null)
        {
            _logger.LogFailedToCreateEntityNoResponse("counting journal");
            return ServiceResponse.Failure("Failed to create counting journal. No response from service.");
        }
        
        if (!response.response.Success)
        {
            _logger.LogFailedToCreateEntity("counting journal", response.response.Message);
            return ServiceResponse.Failure(response.response.Message);
        }
        
        _logger.LogEntityCreatedSuccessfullyWithMessage("Counting journal", response.response.Message);
        return ServiceResponse<InventJournalTableDto>.Success(_mapper.MapToDto(response.response), response.response.Message);
    }

    public async Task<ServiceResponse> GetCountingJournalsAsync(int pageNumber, int pageSize)
    {
        _logger.LogRetrievingEntitiesPaged("counting journals", pageNumber, pageSize);
        
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
            _logger.LogFailedToRetrieveEntityNoResponse("counting journals");
            return ServiceResponse.Failure("Failed to retrieve counting journals. No response from service.");
        }

        _logger.LogEntitiesRetrievedSuccessfully("Counting journals", response.response.TotalCount);
        return ServiceResponse<PagedListDto<InventJournalTableDto>>.Success( 
            _mapper.MapToDto(response.response), "Counting journals retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalAsync(string journalId)
    {
        _logger.LogRetrievingEntity("counting journal", "JournalId", journalId);
        
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
            _logger.LogFailedToRetrieveEntityNoResponseWithId("counting journal", "JournalId", journalId);
            return ServiceResponse.Failure("Failed to retrieve counting journal. No response from service.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Counting journal", "JournalId", journalId);
        return ServiceResponse<InventJournalTableDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalSummaryAsync(string journalId)
    {
        _logger.LogRetrievingEntity("counting journal summary", "JournalId", journalId);
        
        var request = new GMKInventoryManagementServiceGetCountingJournalSummaryRequest
        {
            CallContext = _callContextFactory.Create(),
            journalId = journalId
        };
        
        var response = await _inventoryManagementService.getCountingJournalSummaryAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("counting journal summary", "JournalId", journalId);
            return ServiceResponse.Failure("Failed to retrieve counting journal summary. No response from service.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Counting journal summary", "JournalId", journalId);
        return ServiceResponse<CountingJournalSummaryDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal summary retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalLinesAsync(int pageNumber, int pageSize, string journalId, string? itemId = null, string? inventBatchId = null)
    {
        _logger.LogRetrievingCountingJournalLines(journalId, pageNumber, pageSize);
        
        var request = new GMKInventoryManagementServiceGetInventJournalTransPagedRequest
        {
            CallContext = _callContextFactory.Create(),
            pageNumber =  pageNumber,
            pageSize = pageSize,
            parm = new GMKInventJournalTransDataContract
            {
                JournalId = journalId,
                ItemId = itemId,
                InventBatchId = inventBatchId
            }
        };
        var response = await _inventoryManagementService.getInventJournalTransPagedAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("counting journal lines", "JournalId", journalId);
            return ServiceResponse.Failure("Failed to retrieve counting journal transactions. No response from service.");
        }

        _logger.LogCountingJournalLinesRetrievedSuccessfully(journalId, response.response.TotalCount);
        return ServiceResponse<PagedListDto<InventJournalTransDto>>.Success( 
            _mapper.MapToDto(response.response), "Counting journal transactions retrieved successfully.");
    }

    public async Task<ServiceResponse> GetCountingJournalLineAsync(string inventTransId)
    {
        _logger.LogRetrievingEntity("counting journal line", "InventTransId", inventTransId);
        
        var request = new GMKInventoryManagementServiceGetInventJournalTransRequest
        {
            CallContext = _callContextFactory.Create(),
            inventTransId = inventTransId
        };
        
        var response = await _inventoryManagementService.getInventJournalTransAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("counting journal line", "InventTransId", inventTransId);
            return ServiceResponse.Failure("Failed to retrieve counting journal transaction. No response from service.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Counting journal line", "InventTransId", inventTransId);
        return ServiceResponse<InventJournalTransDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal transaction retrieved successfully.");
    }

    public async Task<ServiceResponse> CreateCountingJournalLineAsync(CreateCountingJournalLineDto dto)
    {
        _logger.LogCreatingCountingJournalLine(dto.JournalId, dto.ItemId, dto.Counted);
        
        var request = new GMKInventoryManagementServiceCreateCountingJournalLineRequest
        {
            CallContext = _callContextFactory.Create(),
            parm = new GMKInventJournalTransDataContract
            {
                JournalId = dto.JournalId,
                ItemId = dto.ItemId,
                InventSiteId = dto.InventSiteId,
                InventLocationId = dto.InventLocationId,
                WMSLocationId = dto.WMSLocationId,
                InventBatchId = dto.InventBatchId,
                Counted = dto.Counted,
                Qty = dto.Qty,
                TransDate = dto.TransDate
            }
        };
        
        var response = await _inventoryManagementService.createCountingJournalLineAsync(request);
        
        if (response?.response == null)
        {
            _logger.LogFailedToCreateEntityNoResponseWithId("counting journal line", "JournalId", dto.JournalId);
            return ServiceResponse.Failure("Failed to create counting journal line. No response from service.");
        }
        
        if (!response.response.Success)
        {
            _logger.LogFailedToCreateEntityWithId("counting journal line", "JournalId", dto.JournalId, response.response.Message);
            return ServiceResponse.Failure(response.response.Message);
        }
        
        _logger.LogCountingJournalLineCreatedSuccessfully(dto.JournalId, dto.ItemId);
        return ServiceResponse.Success(response.response.Message);
    }

    public async Task<ServiceResponse> UpdateCountingJournalLineAsync(UpdateCountingJournalLineDto dto)
    {
        _logger.LogUpdatingCountingJournalLine(dto.InventTransId, dto.Counted);
        
        var request = new GMKInventoryManagementServiceUpdateCountingJournalLineRequest
        {
            CallContext = _callContextFactory.Create(),
            parm = new GMKInventJournalTransDataContract
            {
                InventTransId = dto.InventTransId,
                Counted = dto.Counted,
                ModifiedBy = dto.ModifiedBy,
                ModifiedDateTime = dto.ModifiedDateTime
            }
        };
        
        var response = await _inventoryManagementService.updateCountingJournalLineAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToUpdateEntityNoResponse("counting journal line", "InventTransId", dto.InventTransId);
            return ServiceResponse.Failure("Failed to update counting journal line. No response from service.");
        }
        
        if (!response.response.Success)
        {
            _logger.LogFailedToUpdateEntity("counting journal line", "InventTransId", dto.InventTransId, response.response.Message);
            return ServiceResponse.Failure(response.response.Message);
        }
        
        _logger.LogEntityUpdatedSuccessfully("Counting journal line", "InventTransId", dto.InventTransId);
        return ServiceResponse.Success(response.response.Message);
    }

    public async Task<ServiceResponse> DeleteCountingJournalLineAsync(string inventTransId)
    {
        _logger.LogDeletingEntity("counting journal line", "InventTransId", inventTransId);
        
        var request = new GMKInventoryManagementServiceDeleteCountingJournalLineRequest
        {
            CallContext = _callContextFactory.Create(),
            inventTransId = inventTransId
        };
        
        var response = await _inventoryManagementService.deleteCountingJournalLineAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToDeleteEntityNoResponse("counting journal line", "InventTransId", inventTransId);
            return ServiceResponse.Failure("Failed to delete counting journal line. No response from service.");
        }
        
        if (!response.response.Success)
        {
            _logger.LogFailedToDeleteEntity("counting journal line", "InventTransId", inventTransId, response.response.Message);
            return ServiceResponse.Failure(response.response.Message);
        }
        
        _logger.LogEntityDeletedSuccessfully("Counting journal line", "InventTransId", inventTransId);
        return ServiceResponse.Success(response.response.Message);
    }

    public async Task<ServiceResponse> UpdateBlockInventJournalTableAsync(string journalId, bool lockRecord)
    {
        _logger.LogUpdatingJournalBlockStatus(journalId, lockRecord);
        
        var request = new GMKInventoryManagementServiceUpdateBlockInventJournalTableRequest
        {
            CallContext = _callContextFactory.Create(),
            inventJournalId = journalId,
            lockRecord = lockRecord
        };
        
        var response = await _inventoryManagementService.updateBlockInventJournalTableAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToUpdateEntityNoResponse("journal block status", "JournalId", journalId);
            return ServiceResponse.Failure("Failed to update journal block status. No response from service.");
        }
        
        if (!response.response.Success)
        {
            _logger.LogFailedToUpdateEntity("journal block status", "JournalId", journalId, response.response.Message);
            return ServiceResponse.Failure(response.response.Message);
        }
        
        _logger.LogJournalBlockStatusUpdatedSuccessfully(journalId, lockRecord);
        return ServiceResponse.Success(response.response.Message);
    }

    public async Task<ServiceResponse> GetSessionIdAsync()
    {
        _logger.LogRetrievingSessionId();
        
        var request = new GMKInventoryManagementServiceGetSessionIdRequest
        {
            CallContext = _callContextFactory.Create()
        };

        try
        {
            var response = await _inventoryManagementService.getSessionIdAsync(request);
            if (response?.response != null)
            {
                _logger.LogSessionIdRetrievedSuccessfully(response.response);
                return ServiceResponse<int>.Success(response.response, "Session ID retrieved successfully.");
            }
            
            _logger.LogFailedToRetrieveSessionIdNoResponse();
            return ServiceResponse.Failure("Failed to retrieve session ID. No response from service.");
        }
        catch (Exception ex)
        {
            _logger.LogFailedToRetrieveSessionId(ex);
            return ServiceResponse.Failure($"Failed to retrieve session ID: {ex.Message}");
        }
    }

    public async Task<ServiceResponse> GetInventJournalTransByBatchAsync(string journalId, string itemId, string inventSiteId, string inventLocationId, string wmsLocationId, string inventBatchId)
    {
        _logger.LogRetrievingEntity("counting journal line by batch", "JournalId", journalId);
        
        var request = new GMKInventoryManagementServiceGetInventJournalTransByBatchRequest
        {
            CallContext = _callContextFactory.Create(),
            journalId = journalId,
            itemId = itemId,
            inventSiteId = inventSiteId,
            inventLocationId = inventLocationId,
            wmsLocationId = wmsLocationId,
            inventBatchId = inventBatchId
        };
        
        var response = await _inventoryManagementService.getInventJournalTransByBatchAsync(request);
        if (response?.response == null)
        {
            _logger.LogFailedToRetrieveEntityNoResponseWithId("counting journal line by batch", "JournalId", journalId);
            return ServiceResponse.Failure("Failed to retrieve counting journal transaction by batch. No response from service.");
        }
        
        _logger.LogEntityRetrievedSuccessfully("Counting journal line by batch", "JournalId", journalId);
        return ServiceResponse<InventJournalTransDto>.Success(
            _mapper.MapToDto(response.response), "Counting journal transaction by batch retrieved successfully.");
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