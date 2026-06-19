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
    }
    
    private IControl? _focused;
    
    public IList<IControl> Controls { get; } = [];

    public Rectangle Bounds => new(Location.X, Location.Y, Width, Height);

    public IControl? Focused
    {
        get => _focused;
        set
        {
            if(_focused == value) return;
            _focused?.OnBlur();
            _focused = value;
            _focused?.OnFocus();
        }
    }

    public IControl? Hovered { get; set; }
    public abstract void Resize(int width, int height);
    public abstract void Draw(bool pulse);
    public abstract void Move(Vector2 moveAmount);

    public virtual void OnKeyPress(KeyboardKey key)
    { 
        if (key == KeyboardKey.Tab)
        {
            if (_focused is null)
            {
                Focused = FindControl(Controls, (c) => c.CanFocus);
            }
            else
            {
                var allControls = Controls.SelectMany(c => c.Controls ?? new[] { c }).ToArray();
                var pos = Array.IndexOf(allControls, Focused);
                do
                {
                    pos++;
                    if (pos == allControls.Length)
                    {
                        pos = 0;
                    }
                } while (!allControls[pos].CanFocus);

                Focused = allControls[pos];
            }
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

    public virtual void MouseOutsideControls(Vector2 mousePos)
    {
        
    }

    protected static IControl? FindControl(IEnumerable<IControl> controls, Func<IControl, bool> matching)
    {
        // Controls are drawn in list order, so the last matching sibling is visually on top.
        // Walk backwards and keep searching through event-transparent containers so a fitted
        // image or decorative panel cannot steal mouse focus from a button drawn above it.
        foreach (var control in controls.Reverse())
        {
            if (!matching(control))
            {
                continue;
            }

            if (control.Controls is { Count: > 0 })
            {
                var child = FindControl(control.Controls, matching);
                if (child != null)
                {
                    return child;
                }
            }

            if (!control.EventTransparent)
            {
                return control;
            }
        }

        return null;
    }
}