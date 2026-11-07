using System.Numerics;
using Raylib_CSharp.Collision;
using Raylib_CSharp.Interact;

namespace RaylibUI;

/// <summary>
/// Reads the mouse once per frame and turns it into mouse events (enter/leave, move, down/up, click, wheel)
/// for the controls of the active layout controller:
/// - the topmost, deepest visible control under the mouse gets the events,
/// - pressing a button captures the mouse, so the pressed control keeps getting events until release,
/// - Click = press and release on the same control.
/// </summary>
public class MouseDispatcher
{
    private static readonly MouseButton[] Buttons = [MouseButton.Left, MouseButton.Right];

    private IControlLayout? _pressedLayout;
    private IControl? _pressedControl;
    private MouseButton _pressedButton;

    /// <summary>
    /// Handle this frame's mouse input for the given (topmost) dialog or screen.
    /// Returns true if the dialog passed a mouse press on to whatever is underneath it
    /// (see <see cref="IControlLayout.OnMouseDownOutside"/>); the caller then processes the new topmost one.
    /// </summary>
    public bool Process(IControlLayout layout)
    {
        var passPressThrough = false;
        var position = Input.GetMousePosition();
        var movement = Input.GetMouseDelta();
        var wheel = Input.GetMouseWheelMove();

        // A press that started in another dialog (e.g. a dialog opened or closed mid-click) is dropped
        if (_pressedLayout != null && _pressedLayout != layout)
        {
            _pressedLayout.Captured = null;
            ClearPress();
        }

        // Leftover capture without a held button, or the captured control got disabled
        if (layout.Captured != null &&
            ((!Buttons.Any(Input.IsMouseButtonDown) && !Buttons.Any(Input.IsMouseButtonReleased)) || !layout.Captured.Enabled))
        {
            layout.Captured = null;
        }

        var (target, overDisabled) = FindTarget(layout, position);
        if (layout.Captured == null)
        {
            UpdateHover(layout, target);
        }

        foreach (var button in Buttons.Where(Input.IsMouseButtonPressed))
        {
            var receiver = layout.Captured ?? target;
            if (receiver == null)
            {
                // A disabled control swallows the press; it isn't "outside" the dialog's controls
                if (!overDisabled)
                {
                    passPressThrough |= layout.OnMouseDownOutside(MakeOutsideArgs(layout, button, position, movement));
                }
                continue;
            }

            if (_pressedControl == null)
            {
                _pressedLayout = layout;
                _pressedControl = receiver;
                _pressedButton = button;
                if (layout.Captured == null && receiver.CapturesMouse)
                {
                    layout.Captured = receiver;
                }
            }
            receiver.OnMouseDown(MakeArgs(receiver, button, position, movement));
        }

        if (movement != Vector2.Zero)
        {
            var receiver = layout.Captured ?? target;
            var held = _pressedControl != null ? _pressedButton : (MouseButton?)null;
            if (receiver != null)
            {
                receiver.OnMouseMove(MakeArgs(receiver, held, position, movement));
            }
            else if (!overDisabled)
            {
                layout.OnMouseMoveOutside(MakeOutsideArgs(layout, held, position, movement));
            }
        }

        foreach (var button in Buttons.Where(Input.IsMouseButtonReleased))
        {
            var receiver = layout.Captured ?? target;
            if (receiver != null)
            {
                receiver.OnMouseUp(MakeArgs(receiver, button, position, movement));
            }
            else if (!overDisabled)
            {
                layout.OnMouseUpOutside(MakeOutsideArgs(layout, button, position, movement));
            }

            if (_pressedControl == null || button != _pressedButton) continue;

            // Release the capture before the click, so a click handler can safely close its dialog
            var pressed = _pressedControl;
            layout.Captured = null;
            ClearPress();

            if (pressed.Visible && pressed.Enabled && ShapeHelper.CheckCollisionPointRec(position, pressed.Bounds))
            {
                pressed.OnClick(MakeArgs(pressed, button, position, movement));
            }

            (target, overDisabled) = FindTarget(layout, position);
            UpdateHover(layout, target);
        }

        if (wheel != 0)
        {
            target?.OnMouseWheel(MakeArgs(target, null, position, movement, wheel));
        }

        return passPressThrough;
    }

    private void ClearPress()
    {
        _pressedLayout = null;
        _pressedControl = null;
    }

    private static void UpdateHover(IControlLayout layout, IControl? target)
    {
        if (layout.Hovered == target) return;

        layout.Hovered?.OnMouseLeave();
        layout.Hovered = target;
        target?.OnMouseEnter();
    }

    /// <summary>
    /// Control that gets the mouse events at this position. A disabled control under the mouse gets none,
    /// but still blocks the mouse from what's behind it (overDisabled = true).
    /// </summary>
    private static (IControl? target, bool overDisabled) FindTarget(IControlLayout layout, Vector2 position)
    {
        var hit = HitTest(layout.Controls, position);
        return hit is { Enabled: false } ? (null, true) : (hit, false);
    }

    /// <summary>
    /// Deepest visible control under the mouse, searching from the topmost (last drawn) control down.
    /// Event-transparent controls pass the mouse to their parent.
    /// </summary>
    private static IControl? HitTest(IEnumerable<IControl>? controls, Vector2 position)
    {
        IControl? hit = null;
        while (controls != null)
        {
            var candidate = controls.LastOrDefault(c => c.Visible && ShapeHelper.CheckCollisionPointRec(position, c.Bounds));
            if (candidate == null) break;

            if (!candidate.EventTransparent)
            {
                hit = candidate;
            }
            controls = candidate.Controls;
        }

        return hit;
    }

    private static MouseEventArgs MakeOutsideArgs(IControlLayout layout, MouseButton? button, Vector2 position, Vector2 movement)
    {
        return new MouseEventArgs
        {
            Button = button,
            Location = position - layout.Location,
            ScreenLocation = position,
            Movement = movement
        };
    }

    private static MouseEventArgs MakeArgs(IControl control, MouseButton? button, Vector2 position, Vector2 movement, float wheel = 0)
    {
        return new MouseEventArgs
        {
            Button = button,
            Location = position - new Vector2(control.Bounds.X, control.Bounds.Y),
            ScreenLocation = position,
            Movement = movement,
            WheelDelta = wheel
        };
    }
}
