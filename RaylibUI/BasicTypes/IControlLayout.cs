using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;
using RaylibUI.Forms;

namespace RaylibUI;

public interface IControlLayout
{
    Main MainWindow { get; }
    IList<IControl> Controls { get; }
    IControl? Focused { get; set; }
    IControl? Hovered { get; set; }

    void Resize(int width, int height);
    void Draw(bool pulse);
    void Move(Vector2 moveAmount);
    void OnKeyPress(KeyboardKey key);
    
    Padding LayoutPadding { get; set; }
    
    Vector2 Location { get; }
    void MouseOutsideControls(Vector2 mousePos);
}