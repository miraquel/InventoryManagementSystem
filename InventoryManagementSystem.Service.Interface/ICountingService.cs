using InventoryManagementSystem.API;
using InventoryManagementSystem.Dto;
using InventoryManagementSystem.Dto.Common;

namespace InventoryManagementSystem.Service.Interface;

public interface ICountingService
{
    Task<ServiceResponse> CreateCountingJournalAsync(CreateCountingJournalDto dto);
    Task<ServiceResponse> GetCountingJournalsAsync(int pageNumber, int pageSize);
    Task<ServiceResponse> GetCountingJournalAsync(string journalId);
    Task<ServiceResponse> GetCountingJournalLinesAsync(int pageNumber, int pageSize, string journalId);
    Task<ServiceResponse> GetCountingJournalLineAsync(string inventTransId);
}