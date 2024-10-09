using System.Numerics;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;

namespace RaylibUI;

public abstract class BaseControl : IControl
{
    private Vector2 _location;
    private int _width;
    private Rectangle _bounds;
    private bool _clickPossible;
    private bool _clickStart;
    private MouseButton _clickButton;
    private int _height;
    protected IControlLayout Controller { get; }
    
    public bool EventTransparent { get; }

    protected BaseControl(IControlLayout controller, bool eventTransparent = false)
    {
        Controller = controller;
        EventTransparent = eventTransparent;
    }

    public Vector2 Location
    {
        get => _location;
        set
        {
            _location = value;
            _bounds = new Rectangle(value.X, value.Y, _width, _height);
        }
    }

    public int Width
    {
        get => _width;
        set
        {
            _width = value;
            _bounds = new Rectangle(_location.X, _location.Y, _width, _height);
        }
    }

    public int Height
    {
        get => _height;
        set
        {
            _height = value;
            _bounds = new Rectangle(_location.X, _location.Y, _width, _height);
        }
    }

    public Rectangle Bounds
    {
        get => _bounds;
        set
        {
            _bounds = value;
            _location = new Vector2(_bounds.X, _bounds.Y);
            _width = (int)_bounds.Width;
            _height = (int)_bounds.Height;
        }
    }

    public Rectangle? AbsolutePosition { get; set; }

    public virtual bool CanFocus => false;
    public IList<IControl>? Children { get; protected set; } = null;

    public virtual bool OnKeyPressed(KeyboardKey key)
    {
        return false;
    }

    public virtual void OnMouseMove(Vector2 moveAmount)
    {
        if (_clickStart)
        {
            if (!Input.IsMouseButtonDown(_clickButton))
            {
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
        _clickPossible = false;
        _clickStart = false;
    }

    public virtual void OnMouseEnter()
    {
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
        if (AbsolutePosition.HasValue)
        {
            var absolutePosition = AbsolutePosition.Value;
            Bounds = new Rectangle(Controller.Location.X + Controller.LayoutPadding.Left + absolutePosition.X,
                Controller.Location.Y + Controller.LayoutPadding.Top + absolutePosition.Y, absolutePosition.Width,
                absolutePosition.Height);
        }
    }

    public event EventHandler<MouseEventArgs> MouseDown;

    public event EventHandler<MouseEventArgs> Click;

    public virtual void Draw(bool pulse)
    {
        // This is used for debugging layout issues by drawing a box around the controls we can see where they think they are suppose to be and which is in the wrong place
        // Graphics.DrawRectangleLines((int)_bounds.X, (int)_bounds.Y, _width,Height,Color.Magenta);
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
        return Input.GetMousePosition() - _location;
    }
}

public class MouseEventArgs
{
    public MouseButton Button { get; set; }
}