using System.Numerics;
using Raylib_cs;
using RaylibUI.Controls;

namespace RaylibUI;

public interface IControlLayout
{
    IList<IControl> Controls { get; }
    IControl? Focused { get; set; }
    IControl? Hovered { get; set; }

    void Resize(int width, int height);
    void Draw(bool pulse);
    void Move(Vector2 moveAmount);
    void OnKeyPress(KeyboardKey key);
}