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
    private ListBoxScrollBar _scrollBar;
    private int _rows;

    public event EventHandler<ListBoxSelectionEventArgs> ItemSelected; 

    public ListBox(IControlLayout controller) : base(controller)
    {
    }

    public override Size GetPreferredSize(int width, int height)
    {
        MeasureSizes(width, height);

        var columns = (width / 2 - 22) / (_maxChildWidth + 5);
        if (columns < 1)
        {
            columns = 1;
        }

        var rows = 10;
        var requestedHeight = (_labelHeight + 1) * rows;
        while (requestedHeight > height/2)
        {
            requestedHeight = (_labelHeight + 1) * --rows;
        }

        if (columns * rows < _allLabels.Count)
        {
            requestedHeight += ListBoxScrollBar.DefaultHeight;
        }

        return new Size(columns * _maxChildWidth +5, requestedHeight);
    }

    private void MeasureSizes(int width, int height)
    {
        _maxChildWidth = -4;
        _labelHeight = 0;
        foreach (var l in _allLabels)
        {
            var size = l.GetPreferredSize(width, height);
            if (size.Width > _maxChildWidth)
            {
                _maxChildWidth = size.Width;
            }

            if (size.Height > _labelHeight)
            {
                _labelHeight = size.Height;
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
            _scrollBar = new ListBoxScrollBar(GameScreen, actualColumns, requiredColumns, (position) =>
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

        Children = visibleLabels.Concat(new[] { _scrollBar }).ToList();
    }

    public void SetElements(List<string> list, List<bool> valid, bool refresh)
    {
        _scrollIndex = 0;
        _allLabels = list.Select((text, index) => new ListBoxLabel(this.GameScreen, text, this)).ToList();
        if (refresh)
        {
            var width = Raylib.GetScreenWidth();
            var height = Raylib.GetScreenHeight();
            MeasureSizes(width,height);
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