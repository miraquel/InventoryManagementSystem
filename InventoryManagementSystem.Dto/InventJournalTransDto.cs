namespace InventoryManagementSystem.Dto;

public class InventJournalTransDto
{
    public string InventTransId { get; init; } = string.Empty;
    public string JournalId { get; init; } = string.Empty;
    public string ItemId { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public DateTime TransDate { get; init; } = DateTime.MinValue;
    public string InventSiteId { get; init; } = string.Empty;
    public string InventLocationId { get; init; } = string.Empty;
    public string WMSLocationId { get; init; } = string.Empty;
    public string InventBatchId { get; init; } = string.Empty;
    public decimal InventOnHand { get; init; }
    public decimal Counted { get; init; }
    public decimal Qty { get; init; }
    public string UnitId { get; init; } = string.Empty;
    public string CreatedBy { get; init; } = string.Empty;
    public DateTime CreatedDateTime { get; init; } = DateTime.MinValue;
    public string ModifiedBy { get; init; } = string.Empty;
    public DateTime ModifiedDateTime { get; init; } = DateTime.MinValue;
}