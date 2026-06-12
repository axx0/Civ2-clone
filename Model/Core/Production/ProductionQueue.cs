using System;
using System.Collections.Generic;

namespace Model.Core.Production;

public class ProductionQueue
{
    private readonly List<ProductionItem> _queue = new();

    public IReadOnlyList<ProductionItem> Items => _queue.AsReadOnly();
    public ProductionItem? Current => _queue.Count == 0 ? null : _queue[0];

    /// <summary>
    /// Adds a new project to the end of the construction queue.
    /// </summary>
    public void Enqueue(IProductionOrder order, int shieldRows, string? tileId = null)
    {
        _queue.Add(new ProductionItem(order, shieldRows, tileId));
    }

    /// <summary>
    /// Processes one turn of production.
    /// </summary>
    /// <param name="pointsAvailable">The amount of production the city generates this turn.</param>
    /// <returns>The number of points actually spent.</returns>
    public int ProcessTurn(int pointsAvailable)
    {
        int spent = 0;

        // We iterate through the queue. In Civ II, only the front item produces unless it's finished.
        for (int i = 0; i < _queue.Count; i++)
        {
            var item = _queue[i];

            if (item.IsFinished)
            {
                _queue.RemoveAt(i);
                i--; // Adjust index because we removed an item
                continue;
            }

            int deduction = Math.Min(pointsAvailable, item.RemainingCost);
            item.RemainingCost -= deduction;
            spent += deduction;
            pointsAvailable -= deduction;

            if (item.RemainingCost <= 0)
            {
                item.Status = ItemStatus.Completed;
                // Note: In a real game loop, you might trigger a "Construction Complete" event here.
            }

            // If we ran out of production points this turn, stop processing the queue.
            if (pointsAvailable <= 0) break;
        }

        return spent;
    }

    public void Clear() => _queue.Clear();
}
