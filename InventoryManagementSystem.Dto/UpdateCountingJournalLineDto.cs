namespace InventoryManagementSystem.Dto;

public class UpdateCountingJournalLineDto
{
    public string InventTransId { get; set; } = string.Empty;
    public decimal Counted { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public DateTime ModifiedDateTime { get; set; } = DateTime.MinValue;
}

