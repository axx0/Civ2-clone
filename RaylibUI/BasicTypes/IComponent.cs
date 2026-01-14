using Raylib_CSharp.Transformations;
using System.Numerics;

namespace RaylibUI;

public interface IComponent
{
    IList<IControl>? Controls { get; }

    /// <summary>
    /// Location of control relative to its parent.
    /// </summary>
    Vector2 Location { get; set; }
    Rectangle Bounds { get; }
    bool Visible { get; }
}