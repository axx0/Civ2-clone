using System.Diagnostics;
using System.Numerics;
using Model;
using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;

namespace RaylibUI;

public abstract class BaseScreen : BaseLayoutController, IScreen
{
    public override void Draw(bool pulse)
    {
        var layoutController = _dialogs.LastOrDefault(this);
        var width = Window.GetScreenWidth();
        var height = Window.GetScreenHeight();

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

        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }

        foreach (var dialog in _dialogs)
        {
            dialog.Draw(pulse);
        }
    }

    public abstract void InterfaceChanged(Sound soundManager);

    public override void Resize(int width, int height)
    {
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
        foreach (var key in _keys)
        {
            if (!Input.IsKeyPressed(key)) continue;
            if (layoutController.Focused == null || !layoutController.Focused.OnKeyPressed(key))
            {
                layoutController.OnKeyPress(key);
            }
        }

        var mousePos = Input.GetMousePosition();
        var control = layoutController.Hovered; 
        if (control != null)
        {
            control.OnMouseMove(Input.GetMouseDelta());
            if (control.Children != null)
            {
                var hoverChild = FindControl(control.Children,
                    child => ShapeHelper.CheckCollisionPointRec(mousePos, child.Bounds));
                if (hoverChild != null)
                {
                    control.OnMouseLeave();
                    layoutController.Hovered = hoverChild;
                    hoverChild.OnMouseEnter();
                }
            }
            if (!ShapeHelper.CheckCollisionPointRec(mousePos, control.Bounds))
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
            control => ShapeHelper.CheckCollisionPointRec(mousePos, control.Bounds));
        layoutController.Hovered?.OnMouseEnter();
    }

    public void CloseDialog(IControlLayout? dialog)
    {
        if (dialog != null)
        {
            _dialogs.Remove(dialog);
        }
    }
    

    public void ShowDialog(IControlLayout dialog, bool stack = false)
    {
        if (!stack)
        {
            _dialogs.Clear();
        }

        _dialogs.Add(dialog);
        if (_renderedWidth > 0 && _renderedHeight > 0)
        {
            dialog.Resize(_renderedWidth, _renderedHeight);
        }
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