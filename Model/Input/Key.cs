namespace Model.Input;

/// <summary>
/// UI-agnostic key identifiers used by Model. UI layers map these to real platform key codes.
/// </summary>
public enum Key
{
    None = 0,

    // Common control keys
    Enter,
    Escape,
    Space,
    Tab,
    Backspace,
    Delete,
    Home,
    End,
    PageUp,
    PageDown,

    // Modifiers (if you want them as keys too; optional)
    Shift,
    Control,
    Alt,
    LeftShift,
    RightShift,
    LeftControl,
    RightControl,
    LeftAlt,
    RightAlt,

    // Arrows
    Up,
    Down,
    Left,
    Right,

    // Letters
    A, B, C, D, E, F, G, H, I, J, K, L, M,
    N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

    // Digits
    D0, D1, D2, D3, D4, D5, D6, D7, D8, D9,

    // Function keys
    F1, F2, F3, F4, F5, F6,
    F7, F8, F9, F10, F11, F12
}
