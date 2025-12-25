namespace InventoryManagementSystem.Dto;

public class InventBatchDto
{
    public string ItemId { get; set; } = string.Empty;
    public string InventBatchId { get; set; } = string.Empty;
    public DateTime ProdDate { get; set; }
    public DateTime ExpDate { get; set; }
}

