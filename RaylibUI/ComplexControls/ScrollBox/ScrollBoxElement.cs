using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.ComplexControls.ScrollBox;

namespace RaylibUI;

public class ScrollBoxElement : ControlGroup
{
    public override bool CanFocus => true;
    
    public string Text { get; }

    public ScrollBoxElement(IControlLayout controller, IEnumerable<IControl> displayElements, int spacing = 3, int flexElement = -1) : base(controller, spacing, flexElement, eventTransparent: false)
    {
        foreach (var control in displayElements)
        {
            AddChild(control);
        }

        Text = Children?.OfType<LabelControl>().FirstOrDefault()?.Text ?? "";
        Click += OnClick;
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        Controller.Focused = this;
        Selected(this, false);
    }

    public override void OnFocus()
    {
        base.OnFocus();
        Selected(this, true);
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        if (key == KeyboardKey.Up || key == KeyboardKey.Kp2)
        {
            Scroll(ScrollType.PreviousRow);
            return true;
        }
        if (key == KeyboardKey.Down || key == KeyboardKey.Kp8)
        {
            Scroll(ScrollType.NextRow);
            return true;
        }
        
        if (key == KeyboardKey.Left || key == KeyboardKey.Kp4)
        {
            Scroll(ScrollType.PreviousColumn);
            return true;
        }
        if (key == KeyboardKey.Right || key == KeyboardKey.Kp6)
        {
            Scroll(ScrollType.NextColumn);
            return true;
        }      
        if (key == KeyboardKey.Enter || key == KeyboardKey.KpEnter)
        {
            Selected(this, false);
            return true;
        }
        return base.OnKeyPressed(key);
    }

    internal Action<ScrollType> Scroll { get; set; }
    public Action<ScrollBoxElement, bool> Selected { get; set; }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        if (Controller.Focused == this)
        {
            Graphics.DrawRectangleLinesEx(Bounds, 0.2f, Color.Gray);
        }
    }
}