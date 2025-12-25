namespace InventoryManagementSystem.Dto;

public class CountingJournalSummaryDto
{
    public int CountedLines { get; set; }
    public int PendingLines { get; set; }
    public int TotalLines { get; set; }
    public decimal VarianceAmount { get; set; }
    public int VarianceLines { get; set; }
    public decimal VarianceQty { get; set; }
}

