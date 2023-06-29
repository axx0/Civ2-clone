using System.Runtime.CompilerServices;
using Raylib_cs;

namespace RaylibUI.Controls;

public class ControlGroup : BaseControl
{
    public override bool CanFocus => Children?.Any(c => c.CanFocus) ?? false;
    
    private List<Size> _childSizes;
    public override IList<IControl>? Children { get; } = new List<IControl>();

    public ControlGroup(IControlLayout controller) : base(controller)
    {
    }

    public override Size GetPreferredSize(int width, int height)
    {
        var childSizes = new List<Size>(Children.Count);
        var maxHeight = 0;
        var totalWidth = -11;
        foreach (var control in Children)
        {
            var size = control.GetPreferredSize(width, height);
            childSizes.Add(size);
            totalWidth += size.Width + 11;
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
        for (int index = 0; index < Children.Count; index++)
        {
            var child = Children[index];
            var size = _childSizes[index];
            child.Bounds = new Rectangle(Location.X + offset, Location.Y, size.Width, size.Height);
            offset += size.Width + 11;
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