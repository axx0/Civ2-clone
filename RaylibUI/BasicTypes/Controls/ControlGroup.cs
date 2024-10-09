using Raylib_CSharp.Transformations;

namespace RaylibUI.BasicTypes.Controls;

public class ControlGroup : BaseControl
{
    internal const int NoFlex = -2;
    private const int EvenFlex = -1;
    public override bool CanFocus => false;

    public List<int> ChildWidths { get; private set; }

    private readonly int _spacing;
    private readonly int _flexElement;

    public ControlGroup(IControlLayout controller, int spacing = 3, int flexElement = EvenFlex, bool eventTransparent = true) : base(controller, eventTransparent: eventTransparent)
    {
        _spacing = spacing;
        _flexElement = flexElement;
        Children = new List<IControl>();
    }

    public override int GetPreferredWidth()
    {
        ChildWidths = Children.Select(c => c.GetPreferredWidth()).ToList();
        return ChildWidths.Sum() + (ChildWidths.Count - 1) * _spacing;
    }

    public void SetChildWidths(List<int> childWidths)
    {
        ChildWidths = childWidths;
        Width = childWidths.Sum() + childWidths.Count * _spacing - _spacing;
    }

    public override int GetPreferredHeight()
    {
        return Children.Max(c => c.GetPreferredHeight());
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
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var requestedWidth = ChildWidths[i];
                child.Bounds = new Rectangle(Location.X + offset, Location.Y, requestedWidth, Height);
                offset += requestedWidth + _spacing;
            }
            return;
        }
        var totalWidth = ChildWidths.Sum() + (ChildWidths.Count - 1) * _spacing;
        var difference = Width - totalWidth;
        if (_flexElement != EvenFlex)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                var requestedWidth = ChildWidths[i];
                var width = i == _flexElement ? requestedWidth + difference : requestedWidth;
                child.Bounds = new Rectangle(Location.X + offset, Location.Y, width, Height);
                offset += width + _spacing;
            }

            return;
        }
        var controlWidth = ((Width + _spacing) / Children.Count) - _spacing;
        var excess = ChildWidths.Sum(s => s - controlWidth);
        if (excess > 0)
        {
            controlWidth = (Width - excess + _spacing) / Children.Count - _spacing;
        }
        for (int index = 0; index < Children.Count; index++)
        {
            var child = Children[index];
            var width = Math.Max(controlWidth, ChildWidths[index]);
            child.Bounds = new Rectangle(Location.X + offset, Location.Y, width, Height);
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