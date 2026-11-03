using Model;

namespace RaylibUI.BasicTypes;

public class TableLayout
{
    private int[] _maxWidths, _maxHeights;

    public List<Cell> Cells;
    public int RowCount { get; set; } = 1;
    public int ColumnCount { get; set; } = 1;

    public TableLayout()
    {
        Cells = [];
    }

    public void Add(IControl control, int row, int column, Padding? padding = null, bool stretch = false)
    {
        Cells.Add(new Cell(control, row, column, padding, stretch));
        RowCount = Math.Max(RowCount, row + 1);
        ColumnCount = Math.Max(ColumnCount, column + 1);
    }

    /// <summary>
    /// Height of each row = tallest cell in it (control height + cell padding).
    /// </summary>
    public int[] GetRowHeights(Func<IControl, int> controlHeight)
    {
        var heights = new int[RowCount];
        for (int row = 0; row < RowCount; row++)
        {
            var cells = Cells.Where(c => c.Row == row && c.Control is not null);
            if (cells.Any())
            {
                heights[row] = cells.Max(c => controlHeight(c.Control!) + c.Padding.Top + c.Padding.Bottom);
            }
        }
        return heights;
    }

    /// <summary>
    /// Width of each column = widest cell in it (control width + cell padding).
    /// </summary>
    public int[] GetColumnWidths(Func<IControl, int> controlWidth)
    {
        var widths = new int[ColumnCount];
        for (int col = 0; col < ColumnCount; col++)
        {
            var cells = Cells.Where(c => c.Column == col && c.Control is not null);
            if (cells.Any())
            {
                widths[col] = cells.Max(c => controlWidth(c.Control!) + c.Padding.Left + c.Padding.Right);
            }
        }
        return widths;
    }


    // StartingRow&Col determines which control has Location=(0,0)
    public void CalculateDimensions(int startingRow, int startingCol, int visibleRows, int visibleCols)
    {
        _maxHeights = GetRowHeights(c => c.Height);
        _maxWidths = GetColumnWidths(c => c.Width);

        // Get locations of cells
        foreach (var cell in Cells)
        {
            cell.Location = new(_maxWidths.Take(cell.Column).Sum() - _maxWidths.Take(startingCol).Sum(),
                _maxHeights.Take(cell.Row).Sum() - _maxHeights.Take(startingRow).Sum());
            cell.Width = _maxWidths[cell.Column];
            cell.Height = _maxHeights[cell.Row];
            if (cell.Control is not null)
            {
                cell.Control.Location = cell.Location + new System.Numerics.Vector2(cell.Padding.Left, cell.Padding.Top);
            }

            // Make out of bounds controls unvisible
            if (cell.Row < startingRow || cell.Column < startingCol ||
                cell.Row >= startingRow + visibleRows || cell.Column >= startingCol + visibleCols)
            {
                cell.Control.Visible = false;
            }
            else
            {
                cell.Control.Visible = true;
            }
        }

    }

    public int GetHeight(int startingRow, int maxRows)
    {
        return _maxHeights.Skip(startingRow).Take(maxRows).Sum();
    }
}