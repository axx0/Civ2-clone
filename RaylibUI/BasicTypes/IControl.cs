using System.Net;
using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;

namespace RaylibUI;

public interface IControl
{
    Vector2 Location { get; set; }

    int Width { get; set; }

    int Height { get; }
    Rectangle Bounds { get; set; }

    bool CanFocus { get; }
    IList<IControl>? Children { get; }

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
}