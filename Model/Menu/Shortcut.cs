using Raylib_CSharp.Interact;

namespace Model.Menu;

public readonly struct Shortcut
{
#pragma warning disable CA2211
    public static Shortcut None = new (KeyboardKey.Null);
#pragma warning restore CA2211

    public Shortcut(KeyboardKey key, bool shift = false, bool ctrl = false)
    {
        Key = key;
        Shift = shift;
        Ctrl = ctrl;
    }

    public KeyboardKey Key { get; }
    
    public bool Shift { get; }
    
    public bool Ctrl { get; }

    public static Shortcut Parse(string text)
    {
        var shortCutElements = text.Split(new[] {'|', '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
        return Enum.TryParse<KeyboardKey>( shortCutElements[^1], true, out var key)
            ? new Shortcut(key, shortCutElements.Contains("Shift"), shortCutElements.Contains("Ctrl"))
            : None;
    }

    public override int GetHashCode()
    {
        return (Key, Shift, Ctrl).GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj is Shortcut otherShortcut && otherShortcut.GetHashCode() == GetHashCode();
    }

    public override string ToString()
    {
        if (Ctrl)
        {
            if (Shift)
            {
                return "Ctrl+Shift+" + Key;
            }
            
            return "Ctrl+" + Key;
        }
        if (Shift)
        {

            return "Shift+" + Key;
        }
        return Key.ToString();
    }
}