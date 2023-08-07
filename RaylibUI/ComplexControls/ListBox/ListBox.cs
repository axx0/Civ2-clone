using System.Numerics;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

public class ListBox : BaseControl
{
    private int _scrollIndex;
    private List<ListBoxLabel> _allLabels;
    private int _maxChildWidth;
    private int _labelHeight;

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

        return new Size(columns * _maxChildWidth, 10 * (_labelHeight+1));
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
        var actualColumns = Width / _maxChildWidth;
        var rows = Height / _labelHeight;
        var totalVisible = actualColumns * rows;
        if (totalVisible < _allLabels.Count)
        {
            //TODO: scrolling
        }

        Children = _allLabels.Take(rows).Cast<IControl>().ToList();

        var offset = 0;
        foreach (var control in Children)
        {
            control.Bounds = new Rectangle(Location.X, Location.Y + offset, _maxChildWidth, _labelHeight);
            offset += _labelHeight + 1;
            control.OnResize();
        }
        
        base.OnResize();
    }

    public void SetElements(List<string> list, List<bool> valid, bool refresh)
    {
        _scrollIndex = 0;
        _allLabels = list.Select((text, index) => new ListBoxLabel(this.Controller, text, this)).ToList();
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
        var startX = Location.X;
        var startY = Location.Y;
        
        Raylib.DrawRectangle((int)startX, (int)startY, Width, Height, Color.WHITE);
        base.Draw(pulse);
        
        foreach (var control in Children)
        {
            control.Draw(pulse);
        }

    }
}