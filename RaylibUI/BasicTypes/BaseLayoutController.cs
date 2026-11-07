using System.Numerics;
using Civ2engine;
using Model;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Transformations;

namespace RaylibUI;

public abstract class BaseLayoutController : IControlLayout
{
    private Vector2 _location;

    public Main MainWindow { get; }

    protected BaseLayoutController(Main main, Padding layoutPadding)
    {
        LayoutPadding = layoutPadding;
        MainWindow = main;
        Location = new Vector2(0);
        Controls = new ControlCollection(this);
    }

    private IControl? _focused;

    public IList<IControl> Controls { get; }

    public Rectangle Bounds => new(Location.X, Location.Y, Width, Height);

    public IControl? Focused
    {
        get => _focused;
        set
        {
            if(_focused == value) return;
            // Disabled controls can't have the focus
            if (value is { Enabled: false }) return;
            _focused?.OnBlur();
            _focused = value;
            _focused?.OnFocus();
        }
    }

    public IControl? Hovered { get; set; }

    /// <summary>
    /// Control that currently gets all mouse events (between a mouse press and its release).
    /// </summary>
    public IControl? Captured { get; set; }

    public abstract void Resize(int width, int height);
    public abstract void Draw(bool pulse);
    public abstract void Move(Vector2 moveAmount);

    public virtual void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Tab)
        {
            var backwards = Input.IsKeyDown(KeyboardKey.LeftShift) || Input.IsKeyDown(KeyboardKey.RightShift);
            var ctrl = Input.IsKeyDown(KeyboardKey.LeftControl) || Input.IsKeyDown(KeyboardKey.RightControl);

            // Ctrl+Tab switches the tabs of the tab control the focus is in (as in Windows)
            if (ctrl && FindTabControl() is { } tabs)
            {
                tabs.SelectNextTab(!backwards);
                return;
            }
            SelectNextControl(!backwards);
        }
    }

    /// <summary>
    /// Tab control holding the focused control (or focused itself); with nothing focused, the first one in the dialog.
    /// </summary>
    private BasicTypes.TabControl? FindTabControl()
    {
        for (var control = _focused; control != null; control = control.Parent as IControl)
        {
            if (control is BasicTypes.TabControl tabs) return tabs;
        }
        return _focused == null ? FindFirst(Controls) : null;

        static BasicTypes.TabControl? FindFirst(IEnumerable<IControl>? controls)
        {
            foreach (var control in controls ?? [])
            {
                if (control is BasicTypes.TabControl tabs) return tabs;
                if (FindFirst(control.Controls) is { } inner) return inner;
            }
            return null;
        }
    }

    /// <summary>
    /// Move the focus to the next (or previous) control in tab order, wrapping around at the ends.
    /// Tab order = all controls that can take the focus, in the order they were added (searching the whole tree);
    /// invisible and disabled controls are skipped. With nothing focused it starts at the first (or last) one.
    /// Returns false if no control can take the focus.
    /// </summary>
    public bool SelectNextControl(bool forward = true)
    {
        var all = new List<IControl>();
        CollectFocusable(Controls, all);
        if (all.Count == 0) return false;

        var n = all.Count;
        var start = _focused == null ? -1 : all.IndexOf(_focused);
        if (start == -1 && !forward)
        {
            start = n;
        }

        for (var i = 1; i <= n; i++)
        {
            var candidate = all[((start + (forward ? i : -i)) % n + n) % n];
            if (candidate.Visible && candidate.Enabled)
            {
                Focused = candidate;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// All controls that can take the focus (visible or not), depth-first in the order they were added.
    /// </summary>
    private static void CollectFocusable(IEnumerable<IControl>? controls, List<IControl> result)
    {
        if (controls == null) return;

        foreach (var control in controls)
        {
            if (control.CanFocus)
            {
                result.Add(control);
            }
            CollectFocusable(control.Controls, result);
        }
    }

    public Padding LayoutPadding { get; set; }
    public Vector2 Location 
    {
        get => _location;
        set 
        {
            _location = value;
        }
    }
    
    public virtual int Width { get; }
    public virtual int Height { get; }
    public bool Visible => true;

    public virtual bool OnMouseDownOutside(MouseEventArgs e)
    {
        return false;
    }

    public virtual void OnMouseMoveOutside(MouseEventArgs e)
    {
    }

    public virtual void OnMouseUpOutside(MouseEventArgs e)
    {
    }
}