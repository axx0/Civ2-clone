using Model;
using Raylib_CSharp;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.RunGame.GameControls;
using System.Diagnostics;
using System.Numerics;

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

            // Dialogs shown before the first frame only now have their layout (and visible controls)
            foreach (var dialog in _dialogsWaitingForFocus.Where(d => _dialogs.Contains(d) && d.Focused == null))
            {
                dialog.SelectNextControl();
            }
            _dialogsWaitingForFocus.Clear();
        }
        else
        {
            ControlEvents(layoutController);
        }

        foreach (var control in Controls.Where(c => c.Visible))
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
        // Handle up to 16 characters per frame
        for (int i = 0; i < 16; i++)
        {
            var charPressed = Convert.ToChar(Input.GetCharPressed());
            if (charPressed > char.MinValue)
            {
                layoutController.Focused?.OnCharPressed(charPressed);
            }
        }
        foreach (var key in _keys)
        {
            if (!Input.IsKeyPressed(key)) continue;
            if (layoutController.Focused == null || !layoutController.Focused.OnKeyPressed(key))
            {
                layoutController.OnKeyPress(key);
            }
        }

        if (_mouse.Process(layoutController))
        {
            // The dialog passed a press on (e.g. a dropdown menu that closed on a click outside it):
            // let the new topmost dialog/screen handle the same press in this frame
            var next = _dialogs.LastOrDefault(this);
            if (next != layoutController)
            {
                _mouse.Process(next);
            }
        }
    }

    public void CloseDialog(IControlLayout? dialog)
    {
        if (dialog != null)
        {
            _dialogs.Remove(dialog);
        }
    }
    

    /// <summary>
    /// Show a dialog (on top of the others if stack = true, otherwise replacing them).
    /// Starting focus: a dialog that sets <see cref="IControlLayout.Focused"/> itself (e.g. in its constructor)
    /// keeps that control; otherwise the first control in tab order gets the focus.
    /// </summary>
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

            // Starting focus: the first control in tab order (e.g. the options panel, otherwise the first button),
            // unless the dialog already chose one
            if (dialog.Focused == null)
            {
                dialog.SelectNextControl();
            }
        }
        else
        {
            // No layout yet, so no control is visible; focus it after the first resize
            _dialogsWaitingForFocus.Add(dialog);
        }
    }

    private readonly List<IControlLayout> _dialogs = new();
    private readonly List<IControlLayout> _dialogsWaitingForFocus = new();
    
    private int _renderedWidth;
    private int _renderedHeight;
    private readonly KeyboardKey[] _keys;
    private readonly MouseDispatcher _mouse = new();

    protected BaseScreen(Main main) : base(main, Padding.None)
    {
        _keys = (KeyboardKey[])Enum.GetValues(typeof(KeyboardKey));
    }
}