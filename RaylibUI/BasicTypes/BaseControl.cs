using System.Numerics;
using Raylib_cs;
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
            _location = new Vector2(_bounds.x, _bounds.y);
            _width = (int)_bounds.width;
            _height = (int)_bounds.height;
        }
    }

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
            if (!Raylib.IsMouseButtonDown(_clickButton))
            {
                Click?.Invoke(this, new MouseEventArgs { Button = _clickButton});
            }   
        }
        if (_clickPossible)
        {
            _clickStart = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT);
            if (_clickStart)
            {
                _clickButton = MouseButton.MOUSE_BUTTON_LEFT;
                MouseDown?.Invoke(this, new MouseEventArgs { Button = _clickButton});
            }
            else
            {
                _clickStart = Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_RIGHT);
                if (_clickStart)
                {
                    _clickButton = MouseButton.MOUSE_BUTTON_RIGHT;
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
        _clickPossible = !Raylib.IsMouseButtonDown(MouseButton.MOUSE_BUTTON_LEFT) && !Raylib.IsMouseButtonDown(MouseButton.MOUSE_RIGHT_BUTTON);
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
        // This is used for debugging layout issues by drawing a box around the controls we can see where they think they are suppose to be and which is in the wrong place
        // Raylib.DrawRectangleLines((int)_bounds.x, (int)_bounds.y, _width,Height,Color.MAGENTA);
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
        return Raylib.GetMousePosition() - _location;
    }
}

public class MouseEventArgs
{
    public MouseButton Button { get; set; }
}