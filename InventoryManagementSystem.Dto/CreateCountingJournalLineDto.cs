namespace InventoryManagementSystem.Dto;

public class CreateCountingJournalLineDto
{
    public string JournalId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string InventSiteId { get; set; } = string.Empty;
    public string InventLocationId { get; set; } = string.Empty;
    public string WMSLocationId { get; set; } = string.Empty;
    public string InventBatchId { get; set; } = string.Empty;
    public decimal Counted { get; set; }
    public decimal Qty { get; set; }
    public DateTime TransDate { get; set; }
}
