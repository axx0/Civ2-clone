using Civ2engine;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using System.Diagnostics;
using System.Numerics;

namespace RaylibUI;

public abstract class BaseControl : IControl
{
    private Vector2 _location;
    private bool _clickPossible;
    private bool _clickStart;
    private MouseButton _clickButton;
    private bool _visible;
    private readonly ControlCollection _controls;
    protected IControlLayout Controller { get; }

    public bool EventTransparent { get; }

    protected BaseControl(IControlLayout controller, bool eventTransparent = false)
    {
        Controller = controller;
        EventTransparent = eventTransparent;
        _controls = new ControlCollection(this);
        Visible = true;
    }

    public Vector2 Location
    {
        get => _location;
        set
        {
            _location = value;
        }
    }

    public virtual int Width { get; set; }

    public virtual int Height { get; set; }

    public Rectangle Bounds => new(
        (Parent?.Bounds.X ?? 0) + Location.X,
        (Parent?.Bounds.Y ?? 0) + Location.Y,
        Width,
        Height);

    public bool Visible
    {
        get
        {
            // A control that hasn't been added to a container isn't shown
            if (Parent == null)
            {
                return false;
            }
            else if (Location.X + Width <= 0 ||
                Location.X > Parent.Bounds.Width ||
                Location.Y + Height <= 0 ||
                Location.Y > Parent.Bounds.Height)
            {
                return false; 
            }
            else if (!Parent.Visible)
            {
                return false;
            }
            else
            {
                return _visible;
            }
            
        }
        set => _visible = value;
    }

    public virtual bool CanFocus => false;
    /// <summary>
    /// Child controls. Adding a control here sets its <see cref="Parent"/>; assigning a new list replaces all children.
    /// </summary>
    public IList<IControl> Controls
    {
        get => _controls;
        protected set => _controls.ReplaceWith(value);
    }

    /// <summary>
    /// Container this control was added to (a control or a layout controller). Set by <see cref="ControlCollection"/>.
    /// </summary>
    public IComponent? Parent { get; internal set; }


    public virtual bool OnKeyPressed(KeyboardKey key)
    {
        return false;
    }

    public virtual bool OnCharPressed(Char charPressed)
    {
        return false;
    }

    public virtual void OnMouseMove(Vector2 moveAmount)
    {
        if (!Visible) return;

        if (_clickStart)
        {
            if (!Input.IsMouseButtonDown(_clickButton))
            {
                _clickStart = false;
                Click?.Invoke(this, new MouseEventArgs { Button = _clickButton});
            }   
        }
        if (_clickPossible)
        {
            _clickStart = Input.IsMouseButtonDown(MouseButton.Left);
            if (_clickStart)
            {
                _clickButton = MouseButton.Left;
                MouseDown?.Invoke(this, new MouseEventArgs { Button = _clickButton});
            }
            else
            {
                _clickStart = Input.IsMouseButtonDown(MouseButton.Right);
                if (_clickStart)
                {
                    _clickButton = MouseButton.Right;
                    MouseDown?.Invoke(this, new MouseEventArgs { Button = _clickButton});
                }
            }
        }
    }

    public virtual void OnMouseLeave()
    {
        if (!Visible) return;

        _clickPossible = false;
        _clickStart = false;
    }

    public virtual void OnMouseEnter()
    {
        if (!Visible) return;

        _clickPossible = !Input.IsMouseButtonDown(MouseButton.Left) && !Input.IsMouseButtonDown(MouseButton.Right);

        _clickStart = false;
    }

    public virtual void OnFocus()
    {
        
    }

    public virtual void OnBlur()
    {
    }

    public virtual void OnResize()
    {
    }

    public event EventHandler<MouseEventArgs> MouseDown;

    public event EventHandler<MouseEventArgs> Click;

    public virtual void Draw(bool pulse)
    {
        if (!Visible) return;

        // This is used for debugging layout issues by drawing a box around the controls we can see where they think they are suppose to be and which is in the wrong place
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        foreach (var control in Controls ?? Enumerable.Empty<IControl>())
        {
            control.Draw(pulse);
        }
    }

    public virtual int GetPreferredWidth()
    {
        return -1;
    }

    public virtual int GetPreferredHeight()
    {
        return -1;
    }

    protected Vector2 GetRelativeMousePosition()
    {
        return Input.GetMousePosition() - new Vector2(Bounds.X, Bounds.Y);
    }
}

public class MouseEventArgs
{
    public MouseButton Button { get; set; }
}