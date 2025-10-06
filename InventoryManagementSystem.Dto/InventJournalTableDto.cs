using InventoryManagementSystem.Service.DTO;
using InventoryManagementSystem.Service.DTO.Enums;

namespace InventoryManagementSystem.Dto;

public class InventJournalTableDto
{
    public string JournalNameId { get; set; } = string.Empty;
    public string JournalId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int NumOfLines { get; set; }
    public NoYes Posted { get; set; }
    public string PostedUserId { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public IEnumerable<InventJournalTransDto> InventJournalTrans { get; set; } = [];
}