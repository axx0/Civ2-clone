
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ScrollBox : BaseControl
{
    private readonly bool _vertical;
    private readonly int? _maxColumns;
    private int _scrollIndex;
    private IList<BaseControl> _allLabels;
    private int _maxChildWidth;
    private int _labelHeight;
    private ScrollBar? _scrollBar;
    private int _rows;
    private int _columns;

    public event EventHandler<ScrollBoxSelectionEventArgs>? ItemSelected;

    public ScrollBox(IControlLayout controller, bool vertical = false, int? maxColumns = null,
        IList<BaseControl>? initialEntries = null) : base(controller)
    {
        _vertical = vertical;
        _maxColumns = maxColumns;
        _allLabels = initialEntries ?? Array.Empty<BaseControl>();
        foreach (var control in _allLabels)
        {
            control.Click += OnControlClick;
        }
    }

    private void OnControlClick(object? sender, MouseEventArgs args)
    {
        ItemSelected?.Invoke(sender, new ScrollBoxSelectionEventArgs(args));
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
        return _vertical ? ScrollBar.ScrollBarWidth + itemSpace : itemSpace;
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
            requestedHeight += ScrollBar.ScrollBarWidth;
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
                _scrollBar = new ScrollBar(Controller, requiredRows - _rows,
                    new Rectangle(Location.X + Width - ScrollBar.ScrollBarWidth -2, Location.Y +2,
                        ScrollBar.ScrollBarWidth, Height - 4),
                    (position) => { SetupChildControls(totalVisible, renderHeight, position); });
            }
        }
        else
        {
            var requiredColumns = (int)Math.Ceiling(_allLabels.Count / (double)_rows);
            if (requiredColumns > actualColumns)
            {
                renderHeight -= ScrollBar.ScrollBarWidth;
                _scrollBar = new ScrollBar(Controller, requiredColumns - actualColumns,
                    new Rectangle(Location.X + 2, Location.Y + Height - ScrollBar.ScrollBarWidth, Width - 4,
                        ScrollBar.ScrollBarWidth),
                    (position) => { SetupChildControls(totalVisible, renderHeight, position); });
            }
        }

        SetupChildControls(totalVisible, renderHeight, _scrollIndex);

        base.OnResize();
    }

    private void SetupChildControls(int totalVisible, int renderHeight, int scrollPosition)
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

    /// <summary>
    /// Set the elements in the scroll box.
    ///     This method assumes its receiving new controls and reregisters events on them reusing controls previously
    ///       passed to the scroll box could result in multiple event registrations
    /// </summary>
    /// <param name="list"></param>
    /// <param name="refresh"></param>
    /// <param name="scrollToEnd"></param>
    public void SetElements(IList<BaseControl> list, bool refresh, bool scrollToEnd = false)
    {
        _allLabels = list;
        foreach (var control in _allLabels)
        {
            control.Click += OnControlClick;
        }
        _scrollIndex = 0;
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