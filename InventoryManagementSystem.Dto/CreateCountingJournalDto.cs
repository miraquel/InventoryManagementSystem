namespace InventoryManagementSystem.Dto;

public class CreateCountingJournalDto
{
    public string InventSiteId { get; init; } = string.Empty;
    public string InventLocationId { get; init; } = string.Empty;
    public string WMSLocationId { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}