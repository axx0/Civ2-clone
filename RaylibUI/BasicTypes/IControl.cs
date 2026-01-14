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

    bool OnKeyPressed(KeyboardKey key);
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