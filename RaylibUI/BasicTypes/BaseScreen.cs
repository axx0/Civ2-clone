using System.Diagnostics;
using System.Numerics;
using Model;
using Raylib_cs;

namespace RaylibUI;

public abstract class BaseScreen : BaseLayoutController, IScreen
{
    public override void Draw(bool pulse)
    {
        var layoutController = _dialogs.LastOrDefault(this);
        var Width = Raylib.GetScreenWidth();
        var Height = Raylib.GetScreenHeight();

        if (_renderedWidth != Width || _renderedHeight != Height)
        {
            _renderedWidth = Width;
            _renderedHeight = Height;
            Resize(Width, Height);
        }
        else
        {
            ControlEvents(layoutController);
        }

        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }

        foreach (var dialog in _dialogs)
        {
            dialog.Draw(pulse);
        }
    }

    public override void Resize(int Width, int Height)
    {
        foreach (var control in Controls)
        {
            control.OnResize();
        }

        foreach (var dialog in _dialogs)
        {
            dialog.Resize(Width, Height);
        }
    }

    public override void Move(Vector2 moveAmount)
    {
    }

    private void ControlEvents(IControlLayout layoutController)
    {
        foreach (var key in _keys)
        {
            if (!Raylib.IsKeyPressed(key)) continue;
            if (layoutController.Focused == null || !layoutController.Focused.OnKeyPressed(key))
            {
                layoutController.OnKeyPress(key);
            }
        }

        var mousePos = Raylib.GetMousePosition();
        var control = layoutController.Hovered; 
        if (control != null)
        {
            control.OnMouseMove(Raylib.GetMouseDelta());
            if (control.Children != null)
            {
                var hoverChild = FindControl(control.Children,
                    child => Raylib.CheckCollisionPointRec(mousePos, child.Bounds));
                if (hoverChild != null)
                {
                    control.OnMouseLeave();
                    layoutController.Hovered = hoverChild;
                    hoverChild.OnMouseEnter();
                }
            }
            if (!Raylib.CheckCollisionPointRec(mousePos, control.Bounds))
            {
                control.OnMouseLeave();
                FindHovered(layoutController, mousePos);
            }
        }
        else
        {
            FindHovered(layoutController, mousePos);
        }

        if (layoutController.Hovered == null)
        {
            layoutController.MouseOutsideControls(mousePos);
        }
    }

    private static void FindHovered(IControlLayout layoutController, Vector2 mousePos)
    {
        layoutController.Hovered = FindControl(layoutController.Controls,
            control => Raylib.CheckCollisionPointRec(mousePos, control.Bounds));
        layoutController.Hovered?.OnMouseEnter();
    }

    public void CloseDialog(IControlLayout dialog)
    {
        _dialogs.Remove(dialog);
    }
    

    public void ShowDialog(IControlLayout dialog, bool stack = false)
    {
        if (!stack)
        {
            _dialogs.Clear();
        }

        _dialogs.Add(dialog);
        dialog.Resize(_renderedWidth,_renderedHeight);
    }

    private readonly List<IControlLayout> _dialogs = new();
    
    private int _renderedWidth;
    private int _renderedHeight;
    private readonly KeyboardKey[] _keys;

    protected BaseScreen(Main main) : base(main, Padding.None)
    {
        _keys = (KeyboardKey[])Enum.GetValues(typeof(KeyboardKey));
    }
}