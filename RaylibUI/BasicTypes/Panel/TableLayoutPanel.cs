using Neo.IronLua;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.BasicTypes;

public class TableLayoutPanel : BaseControl
{
    public override bool CanFocus => false;

    private const int defaultHeight = 300;

    private readonly IControlLayout _controller;
    private readonly ScrollBar _VscrollBar, _HscrollBar;
    private int _startingControlRow, _startingControlCol;

    public Color BackgroundColour { get; set; } = Color.Blank;

    /// <summary>
    /// Maximum rows of controls to be displayed on panel.
    /// </summary>
    public int MaxControlRows { get; set; } = 20;

    /// <summary>
    /// Maximum columns of controls to be displayed on panel.
    /// </summary>
    public int MaxControlColumns { get; set; } = 20;

    private TableLayout _tableLayout;
    public TableLayout TableLayout
    {
        get { return _tableLayout; }
        set
        {
            _tableLayout = value;
            _tableLayout.Cells.Select(cell => cell.Control).ToList().ForEach(c => Controls.Add(c));
        }
    }

    public TableLayoutPanel(IControlLayout controller) : base(controller)
    {
        _controller = controller;

        _startingControlRow = 0;
        _startingControlCol = 0;

        _VscrollBar = new ScrollBar(_controller, (position) =>
        {
            _startingControlRow = position;
            OnResize();
        });
        Controls.Add(_VscrollBar);

        _HscrollBar = new ScrollBar(_controller, (position) =>
        {
            _startingControlCol = position;
            OnResize();
        }, false);
        Controls.Add(_HscrollBar);
    }

    public override void OnResize()
    {
        if (_tableLayout == null) return;

        _tableLayout.CalculateDimensions(_startingControlRow, _startingControlCol, MaxControlRows, MaxControlColumns);

        var controls = Controls.Where(c => c is not ScrollBar).ToList();

        Height = _tableLayout.GetHeight(_startingControlRow, MaxControlRows);

        _VscrollBar.Visible = MaxControlRows < _tableLayout.RowCount;
        _VscrollBar.Maximum = _tableLayout.RowCount - MaxControlRows;
        _VscrollBar.Location = new(Width - _VscrollBar.Width - 2, 2);
        _VscrollBar.Height = Height - 4;

        if (MaxControlColumns < _tableLayout.ColumnCount)
        {
            _HscrollBar.Location = new(2, Height - _HscrollBar.Height - 2);
            _HscrollBar.Maximum = _tableLayout.ColumnCount - MaxControlColumns;
            _HscrollBar.Width = Width - 4;
        }

        // Change width of controls so they don't overlap with scrollbar
        if (_VscrollBar.Visible)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].Location.X + controls[i].Width > _VscrollBar.Location.X)
                {
                    controls[i].Width = (int)(_VscrollBar.Location.X - controls[i].Location.X);
                }
            }
        }

        // Adjust height of panel so controls don't overlap with scrollbar
        if (_HscrollBar.Visible && Controls.Where(c => c.Location.Y + c.Height > _HscrollBar.Location.Y).Any())
        {
            Height += _HscrollBar.Height;
            _HscrollBar.Location = new(_HscrollBar.Location.X, Height - _HscrollBar.Height);
        }
    }

    /// <summary>
    /// Desired size = columns and rows built from the cells' desired sizes (plus cell padding),
    /// limited to the visible rows/columns, plus room for the horizontal scrollbar if needed.
    /// </summary>
    protected override Size MeasureCore(Size available)
    {
        if (_tableLayout == null) return new Size(0, 0);

        foreach (var cell in _tableLayout.Cells.Where(c => c.Control is not null))
        {
            cell.Control!.Measure(new Size(
                Math.Max(0, available.Width - cell.Padding.Left - cell.Padding.Right),
                Math.Max(0, available.Height - cell.Padding.Top - cell.Padding.Bottom)));
        }

        var columnWidths = _tableLayout.GetColumnWidths(MeasuredWidth);
        var rowHeights = _tableLayout.GetRowHeights(c => c.Control!.DesiredSize.Height);

        var width = columnWidths.Skip(_startingControlCol).Take(MaxControlColumns).Sum();
        var height = rowHeights.Skip(_startingControlRow).Take(MaxControlRows).Sum();
        if (MaxControlColumns < _tableLayout.ColumnCount)
        {
            height += _HscrollBar.Height;
        }

        return new Size(width, height);
    }

    /// <summary>
    /// Size each control from its desired size (stretched cells fill their column; extra width goes
    /// to the last column), then position them and the scrollbars as in <see cref="OnResize"/>.
    /// </summary>
    protected override void ArrangeCore(Rectangle finalRect)
    {
        if (_tableLayout == null) return;

        var columnWidths = _tableLayout.GetColumnWidths(MeasuredWidth);
        var extra = Width - columnWidths.Skip(_startingControlCol).Take(MaxControlColumns).Sum();
        if (extra > 0)
        {
            columnWidths[^1] += extra;
        }

        foreach (var cell in _tableLayout.Cells.Where(c => c.Control is not null))
        {
            var control = cell.Control!;
            var width = cell.Stretch
                ? Math.Max(0, columnWidths[cell.Column] - cell.Padding.Left - cell.Padding.Right)
                : control.DesiredSize.Width;
            // Location is set by OnResize below
            control.Arrange(new Rectangle(0, 0, width, control.DesiredSize.Height));
        }

        // Rows are built from the arranged heights, since a stretched control
        // (e.g. a listbox) may change its height when its width changes
        OnResize();
    }

    /// <summary>
    /// Width a cell asks for. A stretched control without a width of its own (e.g. a listbox)
    /// only fills the column, so it doesn't make the column wider. One with a preferred width
    /// (e.g. an options panel) widens the column to it, and fills it if it's wider.
    /// </summary>
    private static int MeasuredWidth(Cell cell)
    {
        var control = cell.Control!;
        return cell.Stretch && control.GetPreferredWidth() < 0 ? 0 : control.DesiredSize.Width;
    }

    public override void Draw(bool pulse)
    {
        //Graphics.DrawRectangleRec(Bounds, BackgroundColour);

        base.Draw(pulse);
    }
}