using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBox : BaseControl
{
    private int _scrollIndex = 0;
    private List<ListBoxLabel> _allLabels;
    private int _maxChildWidth;
    private int _labelHeight;
    private ListBoxScrollBar? _scrollBar;
    private int _rows;
    private int _columns;

    public event EventHandler<ListBoxSelectionEventArgs> ItemSelected; 

    public ListBox(IControlLayout controller) : base(controller)
    {
    }

    public override int GetPreferredWidth()
    {
        MeasureSizes();

        var screenWidth = Raylib.GetScreenWidth();
        var columns = (screenWidth / 2 - 22) / (_maxChildWidth + 5);
        if (columns < 1)
        {
            columns = 1;
        }
        _columns = columns;
        return columns * _maxChildWidth + 5;
    }
    public override int GetPreferredHeight()
    {
        MeasureSizes();
        
        var rows = 10;
        var requestedHeight = (_labelHeight + 1) * rows;
        var screenHeight = Raylib.GetScreenHeight();
        while (requestedHeight > screenHeight/2)
        {
            requestedHeight = (_labelHeight + 1) * --rows;
        }

        if (_columns * rows < _allLabels.Count)
        {
            requestedHeight += ListBoxScrollBar.DefaultHeight;
        }

        return requestedHeight;
    }

    private void MeasureSizes()
    {
        _maxChildWidth = _allLabels.Max(c=>c.GetPreferredWidth());
        _labelHeight = 0;
        
        foreach (var l in _allLabels)
        {
            l.Width = _maxChildWidth;
            var Height = l.GetPreferredHeight();


            if (Height > _labelHeight)
            {
                _labelHeight = Height;
            }
        }
    }

    public override void OnResize()
    {
        var actualColumns = Width / _maxChildWidth + 1;
        _rows = Height / _labelHeight;
        var totalVisible = actualColumns * _rows;
        var renderHeight = Height;
        var requiredColumns = (int)Math.Ceiling(_allLabels.Count / (double)_rows);
        if (requiredColumns > actualColumns)
        {
            renderHeight -= ListBoxScrollBar.DefaultHeight;
            _scrollBar = new ListBoxScrollBar(Controller, actualColumns, requiredColumns, (position) =>
            {
                _scrollIndex = position;
                SetupChildLabels(totalVisible, renderHeight);
            } )
            {
                Bounds = new Rectangle(Location.X + 2, Location.Y + Height - ListBoxScrollBar.DefaultHeight, Width - 4,
                    ListBoxScrollBar.DefaultHeight)
            };
            _scrollBar.OnResize();
        }

        SetupChildLabels(totalVisible, renderHeight);

        base.OnResize();
    }

    private void SetupChildLabels(int totalVisible, int renderHeight)
    {
        var visibleLabels = _allLabels.Skip(_scrollIndex * _rows).Take(totalVisible).Cast<IControl>().ToList();

        var yOffset = 2;
        var xOffset = 2;
        foreach (var control in visibleLabels)
        {
            control.Bounds = new Rectangle(Location.X + xOffset, Location.Y + yOffset, _maxChildWidth, _labelHeight);
            yOffset += _labelHeight + 1;
            if (yOffset >= renderHeight)
            {
                yOffset = 2;
                xOffset += _maxChildWidth;
            }

            control.OnResize();
        }

        Children = _scrollBar != null ? visibleLabels.Concat(new[] { _scrollBar }).ToList() : visibleLabels.ToList();
    }

    public void SetElements(List<string> list, List<bool> valid, bool refresh)
    {
        _scrollIndex = 0;
        _allLabels = list.Select((text, index) => new ListBoxLabel(this.Controller, text, this)).ToList();
        if (refresh)
        {
            MeasureSizes();
            OnResize();
        }
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