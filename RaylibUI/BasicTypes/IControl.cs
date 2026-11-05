using System.Net;
using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;

namespace RaylibUI;

public interface IControl : IComponent
{
    int Width { get; set; }
    
    int Height { get; set; }

    /// <summary>
    /// Empty space around the control. Containers include it when measuring and arranging their children.
    /// </summary>
    Padding Margin { get; set; }

    /// <summary>
    /// Bounds of control relative to game window.
    /// </summary>
    Rectangle Bounds { get; }

    bool CanFocus { get; }
    IList<IControl>? Controls { get; }

    IComponent Parent { get; }

    /**
     * Accepts a keystroke. Keystrokes are used for shortcuts and keyboard-based navigation.
     * Keep in mind that keystrokes are not the same as chars; for example LEFT_ARROW is not
     * associated with any char, and KeyboardKey.S may be used for entering lowercase 's' or uppercase 'S'.
     */
    bool OnKeyPressed(KeyboardKey key);
    /**
     * Accepts a UTF-16 character.
     * Keep in mind that keystrokes are not the same as chars; for example LEFT_ARROW is not
     * associated with any char, and KeyboardKey.S may be used for entering lowercase 's' or uppercase 'S'.
     * This is used by textboxes; most Controls should hook into OnKeyPressed rather than OnCharPressed.
     */
    bool OnCharPressed(char charPressed);

    /// <summary>
    /// Whether pressing a mouse button on this control captures the mouse until the button is released.
    /// </summary>
    bool CapturesMouse { get; }
    void OnMouseEnter();
    void OnMouseLeave();
    void OnMouseMove(MouseEventArgs e);
    void OnMouseDown(MouseEventArgs e);
    void OnMouseUp(MouseEventArgs e);
    /// <summary>
    /// Mouse button was pressed and released on this control.
    /// </summary>
    void OnClick(MouseEventArgs e);
    void OnMouseWheel(MouseEventArgs e);

    void OnFocus();

    void OnBlur();
    void Draw(bool pulse);

    int GetPreferredWidth();

    int GetPreferredHeight();
    void OnResize();

    /// <summary>
    /// Size this control wants, as calculated by the last <see cref="Measure"/> call.
    /// </summary>
    Size DesiredSize { get; }

    /// <summary>
    /// First layout pass: work out how big this control wants to be within the available space.
    /// </summary>
    Size Measure(Size available);

    /// <summary>
    /// Second layout pass: place this control at its final position and size (relative to its parent).
    /// </summary>
    void Arrange(Rectangle finalRect);
    
    bool EventTransparent { get; }
    bool Visible { get; set; }
}