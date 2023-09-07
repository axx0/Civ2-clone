using Raylib_cs;

namespace RaylibUI.BasicTypes.Controls;

public class ControlGroup : BaseControl
{
    public override bool CanFocus => false;
    
    private List<Size> _childSizes;

    private readonly int _spacing;
    private readonly int _flexElement;
    private int _totalWidth;

    public ControlGroup(IControlLayout controller, int spacing = 3, int flexElement = -1) : base(controller, eventTransparent: true)
    {
        _spacing = spacing;
        _flexElement = flexElement;
        _totalWidth = -_spacing;
        Children = new List<IControl>();
    }

    public override Size GetPreferredSize(int width, int height)
    {
        var childSizes = new List<Size>(Children.Count);
        var maxHeight = 0;
        _totalWidth = -_spacing;
        foreach (var control in Children)
        {
            var size = control.GetPreferredSize(width, height);
            childSizes.Add(size);
            _totalWidth += size.Width + _spacing;
            if (size.Height > maxHeight)
            {
                maxHeight = size.Height;
            }
        }

        _childSizes = childSizes;
        
        return new Size(_totalWidth, maxHeight);
    }

    public override void OnResize()
    {
        var offset = 0;
        var difference = _totalWidth - Width;
        if (_flexElement != -1)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var size = _childSizes[i];
                var width = i == _flexElement ? size.Width + difference : size.Width;
                child.Bounds = new Rectangle(Location.X + offset, Location.Y, width, size.Height);
                offset += width + _spacing;
            }

            return;
        }
        var controlWidth = ((Width + _spacing) / Children.Count) - _spacing;
        var excess = _childSizes.Sum(s => s.Width - controlWidth);
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