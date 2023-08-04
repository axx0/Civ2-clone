using System.Numerics;
using Raylib_cs;

namespace RaylibUI;

public abstract class BaseScreen : BaseLayoutController, IScreen
{
    public override void Draw(bool pulse)
    {
        var layoutController = _dialogs.LastOrDefault(this);
        var width = Raylib.GetScreenWidth();
        var height = Raylib.GetScreenHeight();

        if (_renderedWidth != width || _renderedHeight != height)
        {
            _renderedWidth = width;
            _renderedHeight = height;
            Resize(width, height);
        }
        else
        {
            ControlEvents(layoutController);
        }

        foreach (var dialog in _dialogs)
        {
            dialog.Draw(pulse);
        }
    }

    public override void Resize(int width, int height)
    {
        //TODO: background?

        foreach (var control in Controls)
        {
            control.OnResize();
        }

        foreach (var dialog in _dialogs)
        {
            dialog.Resize(width, height);
        }
    }

    public override void Move(Vector2 moveAmount)
    {
    }

    private void ControlEvents(IControlLayout layoutController)
    {
        var key = (KeyboardKey) Raylib.GetKeyPressed();
        while (key > 0)
        {
            if (layoutController.Focused == null || !layoutController.Focused.OnKeyPressed(key))
            {
                layoutController.OnKeyPress(key);
            }
            key = (KeyboardKey)Raylib.GetKeyPressed();
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
    }

    private static void FindHovered(IControlLayout layoutController, Vector2 mousePos)
    {
        layoutController.Hovered = FindControl(layoutController.Controls,
            control => Raylib.CheckCollisionPointRec(mousePos, control.Bounds));
        layoutController.Hovered?.OnMouseEnter();
    }
    

    protected void ShowDialog(IControlLayout dialog, bool stack = false)
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
}