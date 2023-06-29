using System.Numerics;
using Model;
using Raylib_cs;

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
    Size GetPreferredSize(int width, int height);
    void OnResize();
}

public record Size(int Width, int Height);