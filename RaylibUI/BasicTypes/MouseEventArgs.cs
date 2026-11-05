using System.Numerics;
using Raylib_CSharp.Interact;

namespace RaylibUI;

/// <summary>
/// Data for mouse events (similar to WinForms' MouseEventArgs).
/// </summary>
public class MouseEventArgs : EventArgs
{
    /// <summary>
    /// Button that was pressed/released, or held during a move. Null = no button (e.g. plain mouse move).
    /// </summary>
    public MouseButton? Button { get; init; }

    /// <summary>
    /// Mouse position relative to the control receiving the event.
    /// </summary>
    public Vector2 Location { get; init; }

    /// <summary>
    /// Mouse position relative to the game window.
    /// </summary>
    public Vector2 ScreenLocation { get; init; }

    /// <summary>
    /// Mouse movement since the last frame (used for dragging).
    /// </summary>
    public Vector2 Movement { get; init; }

    /// <summary>
    /// Mouse wheel movement (positive = away from the user).
    /// </summary>
    public float WheelDelta { get; init; }
}
