using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;

namespace RaylibUI;

public interface IControlLayout : IComponent
{
    Main MainWindow { get; }
    new IList<IControl> Controls { get; }
    IControl? Focused { get; set; }
    IControl? Hovered { get; set; }

    void Resize(int width, int height);
    void Draw(bool pulse);
    void Move(Vector2 moveAmount);
    void OnKeyPress(KeyboardKey key);
    
    Padding LayoutPadding { get; set; }
    
    new Vector2 Location { get; }
    int Width { get; }
    int Height { get; }
    void MouseOutsideControls(Vector2 mousePos);
}
