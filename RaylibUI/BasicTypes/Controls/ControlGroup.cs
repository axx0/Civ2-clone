using System.Runtime.CompilerServices;
using Raylib_cs;

namespace RaylibUI.Controls;

public class ControlGroup : BaseControl
{
    public override bool CanFocus => Children?.Any(c => c.CanFocus) ?? false;
    
    private List<Size> _childSizes;

    private int _spacing;
    public override IList<IControl>? Children { get; } = new List<IControl>();

    public ControlGroup(IControlLayout controller, int spacing = 3) : base(controller, eventTransparent: true)
    {
        _spacing = spacing;
    }

    public override Size GetPreferredSize(int width, int height)
    {
        var childSizes = new List<Size>(Children.Count);
        var maxHeight = 0;
        var totalWidth = -_spacing;
        foreach (var control in Children)
        {
            var size = control.GetPreferredSize(width, height);
            childSizes.Add(size);
            totalWidth += size.Width + _spacing;
            if (size.Height > maxHeight)
            {
                maxHeight = size.Height;
            }
        }

        _childSizes = childSizes;
        return new Size(totalWidth, maxHeight);
    }

    public override void OnResize()
    {
        var offset = 0;
        var controlWidth = ((Width + _spacing) / Children.Count) - _spacing;
        var excess = _childSizes.Sum(s =>
        {
            if (s.Width > controlWidth)
            {
                return s.Width - controlWidth;
            }

            return 0;
        });
        if (excess > 0)
        {
            controlWidth = (Width - excess + _spacing) / Children.Count - _spacing;
        }
        for (int index = 0; index < Children.Count; index++)
        {
            var child = Children[index];
            var size = _childSizes[index];
            var width = Math.Max(controlWidth, size.Width);
            child.Bounds = new Rectangle(Location.X + offset, Location.Y, width, size.Height);
            offset += width + _spacing;
        }
    }

    public void AddChild(IControl control)
    {
        Children.Add(control);
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        foreach (var control in Children)
        {
            control.Draw(pulse);
        }
    }
}