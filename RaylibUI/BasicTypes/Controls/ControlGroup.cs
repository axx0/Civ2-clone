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
        _flexElement = flexElement;
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
}