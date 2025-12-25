using InventoryManagementSystem.Enum;

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
    public DateTime CreatedDateTime { get; set; } = DateTime.MinValue;
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDateTime { get; set; } = DateTime.MinValue;
    public string BlockUserId { get; set; } = string.Empty;
    public string JournalSessionId { get; set; } = string.Empty;
    public IEnumerable<InventJournalTransDto> InventJournalTrans { get; set; } = [];
}