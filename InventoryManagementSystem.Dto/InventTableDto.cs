namespace InventoryManagementSystem.Dto;

public class InventTableDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string SearchName { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public int ProductType { get; set; }
    public int ProductionType { get; set; }
    public string TrackingDimensionGroupName { get; set; } = string.Empty;
}

