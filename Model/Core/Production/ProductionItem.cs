namespace Model.Core.Production;

public enum ItemStatus { Queued, InProgress, Completed }

public class ProductionItem
{
    public IProductionOrder Order { get; }
    public string Name => Order.Title;
    public int TotalCost { get; set; }
    public int RemainingCost { get; set; }
    public ItemStatus Status { get; set; } = ItemStatus.Queued;
    
    // Optional: Reference to a specific tile if required (Civ II Unique Improvements)
    public string? RequiredTileId { get; set; }

    public ProductionItem(IProductionOrder order, int shieldRows, string? tileId = null)
    {
        Order = order;
        TotalCost = order.Cost * shieldRows;
        RemainingCost = TotalCost;
        RequiredTileId = tileId;
    }

    public bool IsFinished => RemainingCost <= 0;
}
