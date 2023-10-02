using System.Numerics;
using Raylib_cs;

namespace RaylibUI;

public abstract class BaseLayoutController : IControlLayout
{
    public Main MainWindow { get; }

    protected BaseLayoutController(Main main)
    {
        this.MainWindow = main;
    }
    
    private IControl? _focused;
    
    public IList<IControl> Controls { get; } = new List<IControl>();

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
        if (key == KeyboardKey.KEY_TAB)
        {
            if (_focused is null)
            {
                Focused = FindControl(Controls, (c) => c.CanFocus);
            }
            else
            {
                var allControls = Controls.SelectMany(c => c.Children ?? new[] { c }).ToArray();
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

    protected static IControl? FindControl(IEnumerable<IControl> controls, Func<IControl, bool> matching)
    {
        IControl? selected = null;
        var elements = controls;
        while (elements != null)
        {
            var candidate = elements.FirstOrDefault(matching);

            elements = candidate?.Children;
            
            if (candidate is { EventTransparent: false })
            {
                selected = candidate;
            }
        }

        return selected;
    }
}