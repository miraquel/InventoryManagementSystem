namespace InventoryManagementSystem.Dto;

public class OnHandDto
{
    public string ItemId { get; init; } = string.Empty;
    public string UnitId { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public string SearchName { get; init; } = string.Empty;
    public string InventSiteId { get; init; } = string.Empty;
    public string InventLocationId { get; init; } = string.Empty;
    public string WMSLocationId { get; init; } = string.Empty;
    public string InventBatchId { get; init; } = string.Empty;
    public string InventSerialId { get; init; } = string.Empty;
    public decimal PostedQty { get; init; }
    public decimal Registered { get; init; }
    public decimal Picked { get; init; }
    public decimal Deducted { get; init; }
    public decimal ReservPhysical { get; init; }
    public decimal ReservOrdered { get; init; }
    public decimal AvailPhysical { get; init; }
    public decimal PhysicalInvent { get; init; }
    public decimal OrderedSum { get; init; }
    public string StorageDimensionGroupName { get; init; } = string.Empty;
    public string TrackingDimensionGroupName { get; init; } = string.Empty;
}

