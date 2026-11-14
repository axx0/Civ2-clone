using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace RaylibUI.BasicTypes.Controls;

public class ControlGroup : BaseControl
{
    internal const int NoFlex = -2;
    private const int EvenFlex = -1;

    public override bool CanFocus => false;
    public List<int> ChildWidths { get; private set; } = [];

    private readonly int _spacing, _flexElement;

    public ControlGroup(IControlLayout controller, int spacing = 3, int flexElement = EvenFlex, bool eventTransparent = true) : base(controller, eventTransparent: eventTransparent)
    {
        _spacing = spacing;
        _flexElement = flexElement; // id of element to be stretched (resized)
        //Controls = new List<IControl>();
    }

    //public override int Width => GetPreferredWidth();
    public override int Height => GetPreferredHeight();

    public override int GetPreferredWidth()
    {
        ChildWidths = Controls.Select(c => c.GetPreferredWidth()).ToList();
        return ChildWidths.Sum() + (ChildWidths.Count - 1) * _spacing;
    }

    public void SetChildWidths(List<int> childWidths)
    {
        ChildWidths = childWidths;
        Width = childWidths.Sum() + childWidths.Count * _spacing - _spacing;
    }

    public void ResizeChildWidths(int targetCumulativeWidth)
    {
        var factor = (double)(targetCumulativeWidth - Controls.Count * _spacing) / (double)Controls.Select(c => c.Width).Sum();
        foreach (var c in Controls)
            c.Width = (int)(c.Width * factor);
        ChildWidths = Controls.Select(c => c.Width).ToList();
        Width = ChildWidths.Sum() + ChildWidths.Count * _spacing - _spacing;
    }

    public override int GetPreferredHeight()
    {
        return Controls.Count == 0 ? 0 : Controls.Max(c => c.GetPreferredHeight());
    }

    public override void OnResize()
    {
        if (ChildWidths == null || ChildWidths.Count == 0)
        {
            GetPreferredWidth();
        }
        var offset = 0;
        if (_flexElement == NoFlex)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var child = Controls[i];
                var requestedWidth = ChildWidths[i];
                child.Location = new Vector2(offset, 0);
                child.Width = requestedWidth;
                child.Height = Height;
                offset += requestedWidth + _spacing;
            }
            return;
        }
        var totalWidth = ChildWidths.Sum() + (ChildWidths.Count - 1) * _spacing;
        var difference = Width - totalWidth;
        if (_flexElement != EvenFlex)
        {
            for (int i = 0; i < Controls.Count; i++)
            {
                var child = Controls[i];
                var requestedWidth = ChildWidths[i];
                var width = i == _flexElement ? requestedWidth + difference : requestedWidth;
                child.Location = new Vector2(offset, 0);
                child.Width = width;
                child.Height = Height;
                offset += width + _spacing;
            }

            return;
        }
        var controlWidth = ((Width + _spacing) / Controls.Count) - _spacing;
        var excess = ChildWidths.Sum(s => s - controlWidth);
        if (excess > 0)
        {
            controlWidth = (Width - excess + _spacing) / Controls.Count - _spacing;
        }
        for (int index = 0; index < Controls.Count; index++)
        {
            var child = Controls[index];
            var width = Math.Max(controlWidth, ChildWidths[index]);
            child.Location = new Vector2(offset, 0);
            child.Width = width;
            child.Height = Height; 
            offset += width + _spacing;
        }
    }

    public void AddChild(IControl control)
    {
        Controls.Add(control);
    }

    /// <summary>
    /// Desired size = children side by side (including margins and spacing), as tall as the tallest child.
    /// </summary>
    protected override Size MeasureCore(Size available)
    {
        if (Controls.Count == 0) return new Size(0, 0);

        foreach (var child in Controls)
        {
            child.Measure(available);
        }

        var width = Controls.Sum(OuterWidth) + (Controls.Count - 1) * _spacing;
        var height = Controls.Max(c => c.DesiredSize.Height + c.Margin.Top + c.Margin.Bottom);
        return new Size(width, height);
    }

    /// <summary>
    /// Place children in a row within the final width:
    /// NoFlex - each child gets its desired width,
    /// flex element - other children get their desired width, the flex child takes the rest,
    /// EvenFlex - all children get equal width and together fill the row (e.g. dialog buttons).
    /// </summary>
    protected override void ArrangeCore(Rectangle finalRect)
    {
        if (Controls.Count == 0) return;

        var available = Width - (Controls.Count - 1) * _spacing;
        var widths = Controls.Select(OuterWidth).ToList();

        if (_flexElement == EvenFlex)
        {
            var equalWidth = available / Controls.Count;
            for (var i = 0; i < widths.Count; i++)
            {
                widths[i] = equalWidth;
            }
            // Give the rounding remainder to the last child so the row fills exactly
            widths[^1] += available - equalWidth * Controls.Count;
        }
        else if (_flexElement != NoFlex && _flexElement < Controls.Count)
        {
            widths[_flexElement] = Math.Max(0, widths[_flexElement] + available - widths.Sum());
        }

        var x = 0;
        for (var i = 0; i < Controls.Count; i++)
        {
            var child = Controls[i];
            var margin = child.Margin;
            child.Arrange(new Rectangle(x + margin.Left, margin.Top,
                Math.Max(0, widths[i] - margin.Left - margin.Right),
                Math.Max(0, Height - margin.Top - margin.Bottom)));
            x += widths[i] + _spacing;
        }
    }

    private static int OuterWidth(IControl control)
    {
        return control.DesiredSize.Width + control.Margin.Left + control.Margin.Right;
    }
}