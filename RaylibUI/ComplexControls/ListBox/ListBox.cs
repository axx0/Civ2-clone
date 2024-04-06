using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBox : BaseControl
{
    private readonly int _minEntries;
    private readonly bool _vertical;
    private readonly int? _maxColumns;
    private int _scrollIndex = 0;
    private List<ListBoxLabel> _allLabels;
    private int _maxChildWidth;
    private int _labelHeight;
    private ListBoxScrollBar? _scrollBar;
    private int _rows;
    private int _columns;

    public event EventHandler<ListBoxSelectionEventArgs>? ItemSelected;

    public ListBox(IControlLayout controller, int minEntries = -1, bool vertical = false, int? maxColumns = null,
        IList<string>? initialEntries = null) : base(controller)
    {
        _minEntries = minEntries;
        _vertical = vertical;
        _maxColumns = maxColumns;
        _allLabels = MakeLabels(initialEntries ?? Array.Empty<string>());
    }

    public override int GetPreferredWidth()
    {
        MeasureWidths();

        var screenWidth = Raylib.GetScreenWidth();

        var columns = (screenWidth / 2 - 22) / (_maxChildWidth + 5);
        if (columns < 1)
        {
            columns = 1;
        }

        if (_maxColumns < columns)
        {
            columns = _maxColumns.Value;
        }

        _columns = columns;

        var itemSpace = _columns * _maxChildWidth + 5;
        return _vertical ? ListBoxScrollBar.ScrollBarWidth + itemSpace : itemSpace;
    }

    public override int GetPreferredHeight()
    {
        MeasureHeights();
        
        var rows = 10;
        var requestedHeight = (_labelHeight + 1) * rows;
        var screenHeight = Raylib.GetScreenHeight();
        while (requestedHeight > screenHeight/2)
        {
            requestedHeight = (_labelHeight + 1) * --rows;
        }

        if (_columns * rows < _allLabels.Count)
        {
            requestedHeight += ListBoxScrollBar.ScrollBarWidth;
        }

        return requestedHeight;
    }

    private void MeasureWidths()
    {
        _maxChildWidth = _allLabels.Max(c => c.GetPreferredWidth());
    }

    private void MeasureHeights()
    {
        _labelHeight = 0;
        
        foreach (var l in _allLabels)
        {
            l.Width = _maxChildWidth;
            var height = l.GetPreferredHeight();

            if (height > _labelHeight)
            {
                _labelHeight = height;
            }
        }
    }

    public override void OnResize()
    {
        _rows = Height / _labelHeight;
        var renderHeight = Height - 4;
        var actualColumns = Width / _maxChildWidth + 1;
        if (_maxColumns < actualColumns)
        {
            actualColumns = _maxColumns.Value;
        }

        var totalVisible = actualColumns * _rows;
        if (_vertical)
        {
            var requiredRows = (int)Math.Ceiling(_allLabels.Count / (double)actualColumns);
            if (_rows < requiredRows)
            {
                _scrollBar = new ListBoxScrollBar(Controller, requiredRows - _rows,
                    new Rectangle(Location.X + Width - ListBoxScrollBar.ScrollBarWidth -2, Location.Y +2,
                        ListBoxScrollBar.ScrollBarWidth, Height - 4),
                    (position) => { SetupChildLabels(totalVisible, renderHeight, position); });
            }
        }
        else
        {
            var requiredColumns = (int)Math.Ceiling(_allLabels.Count / (double)_rows);
            if (requiredColumns > actualColumns)
            {
                renderHeight -= ListBoxScrollBar.ScrollBarWidth;
                _scrollBar = new ListBoxScrollBar(Controller, requiredColumns - actualColumns,
                    new Rectangle(Location.X + 2, Location.Y + Height - ListBoxScrollBar.ScrollBarWidth, Width - 4,
                        ListBoxScrollBar.ScrollBarWidth),
                    (position) => { SetupChildLabels(totalVisible, renderHeight, position); });
            }
        }

        SetupChildLabels(totalVisible, renderHeight, _scrollIndex);

        base.OnResize();
    }

    private void SetupChildLabels(int totalVisible, int renderHeight, int scrollPosition)
    {
        var skipAmount = _vertical ? scrollPosition * _columns : scrollPosition * _rows;
        var visibleLabels = _allLabels.Skip(skipAmount).Take(totalVisible).Cast<IControl>().ToList();

        var yOffset = 2;
        var xOffset = 2;
        foreach (var control in visibleLabels)
        {
            control.Bounds = new Rectangle(Location.X + xOffset, Location.Y + yOffset, _maxChildWidth, _labelHeight);
            yOffset += _labelHeight;
            if (yOffset + _labelHeight >= renderHeight)
            {
                yOffset = 2;
                xOffset += _maxChildWidth;
            }

            control.OnResize();
        }

        Children = _scrollBar != null ? visibleLabels.Concat(new[] { _scrollBar }).ToList() : visibleLabels.ToList();
        _scrollIndex = scrollPosition;
    }

    public void SetElements(IList<string> list, bool refresh, List<bool>? valid = null, bool scrollToEnd = false)
    {
        _scrollIndex = 0;
        _allLabels = MakeLabels(list);
        if (refresh)
        {
            MeasureWidths();
            MeasureHeights();
            OnResize();
        }

        if (scrollToEnd && _scrollBar != null)
        {
            _scrollBar.ScrollToEnd();
        }
    }

    private List<ListBoxLabel> MakeLabels(IList<string> list)
    {
        var entries =
            _minEntries > 0 && list.Count < _minEntries
                ? list.Concat(Enumerable.Repeat(string.Empty, _minEntries - list.Count))
                : list;

        return entries.Select(text => new ListBoxLabel(this.Controller, text, this)).ToList();
    }

    public void LabelClicked(string text)
    {
        this.ItemSelected?.Invoke(this, new ListBoxSelectionEventArgs(text));
    }
    public override void Draw(bool pulse)
    {
        var startX = Location.X + 2;
        var startY = Location.Y + 2;
        
        Raylib.DrawRectangle((int)startX, (int)startY, Width -4, Height -4, new Color(207,207,207,255));
        base.Draw(pulse);
        
        foreach (var control in Children)
        {
            control.Draw(pulse);
        }

    }
}