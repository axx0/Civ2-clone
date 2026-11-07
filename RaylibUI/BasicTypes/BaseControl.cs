using Civ2engine;
using Model;
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

    /// <summary>
    /// Empty space around the control. Containers include it when measuring and arranging their children.
    /// </summary>
    public Padding Margin { get; set; }

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

    // Mouse handling. The MouseDispatcher calls these; override them
    // to react to the mouse, or subscribe to the matching events.

    /// <summary>
    /// Whether pressing a mouse button on this control captures the mouse, so that the control keeps
    /// getting mouse events until the button is released, even outside its bounds.
    /// </summary>
    public virtual bool CapturesMouse => true;

    /// <summary>
    /// True while this control has captured the mouse. Set to false to release the capture early.
    /// </summary>
    public bool Capture
    {
        get => Controller.Captured == this;
        set
        {
            if (value)
            {
                Controller.Captured = this;
            }
            else if (Controller.Captured == this)
            {
                Controller.Captured = null;
            }
        }
    }

    public virtual void OnMouseEnter()
    {
        MouseEnter?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseLeave()
    {
        MouseLeave?.Invoke(this, EventArgs.Empty);
    }

    public virtual void OnMouseMove(MouseEventArgs e)
    {
        MouseMove?.Invoke(this, e);
    }

    public virtual void OnMouseDown(MouseEventArgs e)
    {
        MouseDown?.Invoke(this, e);
    }

    public virtual void OnMouseUp(MouseEventArgs e)
    {
        MouseUp?.Invoke(this, e);
    }

    /// <summary>
    /// Mouse button was pressed and released on this control.
    /// </summary>
    public virtual void OnClick(MouseEventArgs e)
    {
        Click?.Invoke(this, e);
    }

    public virtual void OnMouseWheel(MouseEventArgs e)
    {
        MouseWheel?.Invoke(this, e);
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

    public event EventHandler? MouseEnter;

    public event EventHandler? MouseLeave;

    public event EventHandler<MouseEventArgs>? MouseMove;

    public event EventHandler<MouseEventArgs>? MouseDown;

    public event EventHandler<MouseEventArgs>? MouseUp;

    public event EventHandler<MouseEventArgs>? Click;

    public event EventHandler<MouseEventArgs>? MouseWheel;

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

    /// <summary>
    /// Size this control wants, as calculated by the last <see cref="Measure"/> call.
    /// </summary>
    public Size DesiredSize { get; private set; } = new(0, 0);

    /// <summary>
    /// First layout pass: work out how big this control wants to be within the available space.
    /// </summary>
    public Size Measure(Size available)
    {
        DesiredSize = MeasureCore(available);
        return DesiredSize;
    }

    /// <summary>
    /// Second layout pass: place this control at its final position and size (relative to its parent).
    /// </summary>
    public void Arrange(Rectangle finalRect)
    {
        Location = new Vector2(finalRect.X, finalRect.Y);
        Width = (int)finalRect.Width;
        Height = (int)finalRect.Height;
        ArrangeCore(finalRect);
    }

    /// <summary>
    /// Override to calculate the desired size. Containers should measure their children here.
    /// Defaults to the preferred width/height so existing controls keep working;
    /// controls without a preference (-1) keep their current size.
    /// </summary>
    protected virtual Size MeasureCore(Size available)
    {
        var width = GetPreferredWidth();
        var height = GetPreferredHeight();
        return new Size(width < 0 ? Width : width, height < 0 ? Height : height);
    }

    /// <summary>
    /// Override to arrange children inside this control. Defaults to <see cref="OnResize"/>.
    /// </summary>
    protected virtual void ArrangeCore(Rectangle finalRect)
    {
        OnResize();
    }
}