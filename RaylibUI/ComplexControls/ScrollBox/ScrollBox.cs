
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes;
using RaylibUI.ComplexControls.ScrollBox;

namespace RaylibUI;

public class ScrollBox : BaseControl
{
    private readonly bool _vertical;
    private readonly int? _maxColumns;
    private int _initialSelection;
    private int _scrollIndex;
    private IList<ScrollBoxElement> _allLabels;
    private int _maxChildWidth;
    private int _labelHeight;
    private ScrollBar? _scrollBar;
    private int _rows;
    private int _columns;
    private int _requiredColumns;

    public override bool CanFocus => Children?.Any(c => c.CanFocus) ?? false;

    public event EventHandler<ScrollBoxSelectionEventArgs>? ItemSelected;

    public ScrollBox(IControlLayout controller, bool vertical = false, int? maxColumns = null,
        IList<ScrollBoxElement>? initialEntries = null, int initialSelection  =-1) : base(controller)
    {
        _vertical = vertical;
        _maxColumns = maxColumns;
        _initialSelection = initialSelection;
        _allLabels = initialEntries ?? Array.Empty<ScrollBoxElement>();
        ConnectElements();
    }

    private void ConnectElements()
    {
        foreach (var control in _allLabels)
        {
            control.Scroll = Move;
            control.Selected = Selected;
        }
    }

    private void Selected(ScrollBoxElement control, bool soft)
    {
        ItemSelected?.Invoke(control,
            new ScrollBoxSelectionEventArgs(control.Text,  _allLabels.IndexOf(control), soft));
    }

    public override int GetPreferredWidth()
    {
        MeasureWidths();

        var screenWidth = Window.GetScreenWidth();

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
        var screenHeight = Window.GetScreenHeight();
        while (requestedHeight > screenHeight/2)
        {
            requestedHeight = (_labelHeight + 1) * --rows;
        }

        if (!_vertical && _columns * rows < _allLabels.Count)
        {
            requestedHeight += ScrollBar.ScrollBarWidth;
        }

        return requestedHeight;
    }

    private void MeasureWidths()
    {
        _maxChildWidth = _allLabels.Count == 0 ? 0 : _allLabels.Max(c => c.GetPreferredWidth());
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
            _requiredColumns = actualColumns;
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
            _requiredColumns = (int)Math.Ceiling(_allLabels.Count / (double)_rows);
            if (_requiredColumns > actualColumns)
            {
                renderHeight -= ScrollBar.ScrollBarWidth;
                _scrollBar = new ScrollBar(Controller, _requiredColumns - actualColumns,
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
        if (_initialSelection > -1 && _initialSelection < _allLabels.Count )
        {
            var scrollToElement = _initialSelection;
            _initialSelection = -1;
            ScrollToIndex(scrollToElement);
        }
    }

    /// <summary>
    /// Set the elements in the scroll box.
    ///     This method assumes its receiving new controls and reregisters events on them reusing controls previously
    ///       passed to the scroll box could result in multiple event registrations
    /// </summary>
    /// <param name="list"></param>
    /// <param name="refresh"></param>
    /// <param name="scrollToEnd"></param>
    public void SetElements(IList<ScrollBoxElement> list, bool refresh, bool scrollToEnd = false)
    {
        _allLabels = list;
        ConnectElements();
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

        if (Controller.Focused is ScrollBoxElement && !Children!.Contains(Controller.Focused))
        {
            Controller.Focused = Children?.First();
        }
    }

    public void Move(ScrollType direction)
    {
        if (Controller.Focused is not ScrollBoxElement selected) return;
        
        var selectedIndex = _allLabels.IndexOf(selected);

        if (selectedIndex == -1) return;

        int nextIndex;
        switch (direction)
        {
            case ScrollType.PreviousRow:
                nextIndex = selectedIndex - 1;
                break;
            case ScrollType.NextRow:
                nextIndex = selectedIndex + 1;
                break;
            case ScrollType.PreviousColumn:
                if (_requiredColumns > 1)
                {
                    if (selectedIndex < _rows)
                    {
                        nextIndex = selectedIndex + (_requiredColumns-1) * _rows -1;
                        if (nextIndex >= _allLabels.Count)
                        {
                            nextIndex -= _rows;
                        }
                    }
                    else
                    {
                        nextIndex = selectedIndex - _rows;
                    }
                }
                else
                {
                    nextIndex = selectedIndex - 1;
                }
                break;
            case ScrollType.NextColumn:
                if (_requiredColumns > 1)
                {
                    nextIndex = selectedIndex + _rows;
                    if (nextIndex >= _allLabels.Count)
                    {
                        nextIndex = selectedIndex % _rows +1;
                    }
                }
                else
                {
                    nextIndex = selectedIndex + 1;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        if (nextIndex == -1)
        {
            nextIndex = _allLabels.Count - 1;
        }else if (nextIndex == _allLabels.Count)
        {
            nextIndex = 0;
        }
        
        ScrollToIndex(nextIndex);
    }

    public void ScrollToIndex(int indexToShow)
    {
        if (!Children!.Contains(_allLabels[indexToShow]))
        {
            var increment = _vertical ? _columns : _rows;
            var currentPosition = _scrollBar!.ScrollPosition;
            var totalVisible = Children.Count - 1;
            
            var scrollPosition = currentPosition;
            bool loop;
            do
            {
                loop = false;
                var skipAmount = scrollPosition * increment;
                if (skipAmount > indexToShow)
                {
                    scrollPosition--;
                    loop = true;
                }
                else if (skipAmount + totalVisible <= indexToShow)
                {
                    scrollPosition++;
                    loop = true;
                }
            } while (loop);
            
            _scrollBar.ScrollTo(scrollPosition);
        }

        Controller.Focused = _allLabels[indexToShow];
    }

    public override void Draw(bool pulse)
    {
        var startX = Location.X + 2;
        var startY = Location.Y + 2;
        
        Graphics.DrawRectangle((int)startX, (int)startY, Width -4, Height -4, new Color(207,207,207,255));
        base.Draw(pulse);
        
        foreach (var control in Children)
        {
            control.Draw(pulse);
        }
    }
}