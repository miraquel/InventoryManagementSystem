using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;

namespace InventoryManagementSystem.Service.Interface;

public interface ICountingService
{
    Task<ServiceResponse> CreateCountingJournalAsync(CreateCountingJournalDto dto);
    Task<ServiceResponse> GetCountingJournalsAsync(int pageNumber, int pageSize);
    Task<ServiceResponse> GetCountingJournalAsync(string journalId);
    Task<ServiceResponse> GetCountingJournalSummaryAsync(string journalId);
    Task<ServiceResponse> GetCountingJournalLinesAsync(int pageNumber, int pageSize, string journalId, string? itemId = null, string? inventBatchId = null);
    Task<ServiceResponse> GetCountingJournalLineAsync(string inventTransId);
    Task<ServiceResponse> CreateCountingJournalLineAsync(CreateCountingJournalLineDto dto);
    Task<ServiceResponse> UpdateCountingJournalLineAsync(UpdateCountingJournalLineDto dto);
    Task<ServiceResponse> DeleteCountingJournalLineAsync(string inventTransId);
    Task<ServiceResponse> UpdateBlockInventJournalTableAsync(string journalId, bool lockRecord);
    Task<ServiceResponse> GetSessionIdAsync();
    Task<ServiceResponse> GetInventJournalTransByBatchAsync(string journalId, string itemId, string inventSiteId, string inventLocationId, string wmsLocationId, string inventBatchId);
}