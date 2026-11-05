using System.Numerics;
using Model;
using Raylib_CSharp.Interact;
using RaylibUI.Controls;

namespace RaylibUI;

public interface IControlLayout : IComponent
{
    Main MainWindow { get; }
    IList<IControl> Controls { get; }
    IControl? Focused { get; set; }
    IControl? Hovered { get; set; }

    /// <summary>
    /// Control that currently gets all mouse events (between a mouse press and its release).
    /// </summary>
    IControl? Captured { get; set; }

    void Resize(int width, int height);
    void Draw(bool pulse);
    void Move(Vector2 moveAmount);
    void OnKeyPress(KeyboardKey key);
    
    Padding LayoutPadding { get; set; }
    
    Vector2 Location { get; }
    int Width { get; }
    int Height { get; }
    void MouseOutsideControls(Vector2 mousePos);
}