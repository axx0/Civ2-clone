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
    void OnMouseMove(Vector2 moveAmount);
    void OnMouseLeave();
    void OnMouseEnter();

    void OnFocus();

    void OnBlur();
    void Draw(bool pulse);

    int GetPreferredWidth();

    int GetPreferredHeight();
    void OnResize();
    
    bool EventTransparent { get; }
    bool Visible { get; set; }
}