namespace InventoryManagementSystem.Dto;

public class WMSLocationDto
{
    public string WMSLocationId { get; set; } = string.Empty;
    public string InventLocationId { get; set; } = string.Empty;
    public int LocationType { get; set; }
    public int MaxPalletCount { get; set; }
    public decimal MaxVolume { get; set; }
    public decimal MaxWeight { get; set; }
}

