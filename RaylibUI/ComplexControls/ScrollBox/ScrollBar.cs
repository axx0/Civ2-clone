using Raylib_CSharp.Textures;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes;

namespace RaylibUI;

public class ScrollBar : BaseControl
{
    public const int ScrollBarWidth = 17;
    private readonly Action<int> _scrollAction;
    private readonly Texture2D[] _images;
    private int _scrollPos;
    private readonly int _positions;
    private int _increment;
    private readonly bool _vertical;

    public ScrollBar(IControlLayout controller, int positions, Rectangle bounds, Action<int> scrollAction) :  base(controller)
    {
        Bounds = bounds;
        _vertical = bounds.Height > bounds.Width;
        _scrollAction = scrollAction;
        _images = ImageUtils.GetScrollImages(ScrollBarWidth, _vertical).Select(Texture2D.LoadFromImage).ToArray();
        _scrollPos = 0;
        _positions = positions;
        Click += OnClick;
        UpdateIncrement();
    }

    public int ScrollPosition => _scrollPos;

    private void UpdateIncrement()
    {
        _increment = ((_vertical ? Height : Width) - 3 * ScrollBarWidth) / _positions;
    }

    public override int GetPreferredHeight()
    {
        return ScrollBarWidth;
    }
    public override int GetPreferredWidth()
    {
        return -1;
    }

    public override void OnResize()
    {
        UpdateIncrement();
        base.OnResize();
    }

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        var pos = GetRelativeMousePosition();
        CalculateScroll(_vertical ? pos.Y : pos.X);
    }

    private void CalculateScroll(float pos)
    {
        var breakPoint = _increment * _scrollPos + ScrollBarWidth;
        if (pos < breakPoint)
        {
            if (_scrollPos > 0)
            {
                _scrollPos--;
                _scrollAction(_scrollPos);
            }
        }else if (pos > breakPoint + ScrollBarWidth)
        {
            if (_scrollPos < _positions)
            {
                _scrollPos++;
                _scrollAction(_scrollPos);
            }
        }
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangle((int)Location.X, (int)Location.Y, Width, Height, Color.White);
        Graphics.DrawTexture(_images[0], (int)Location.X, (int)Location.Y, Color.White);
        if (_vertical)
        {
            Graphics.DrawTexture(_images[1], (int)Location.X, (int)Location.Y + _scrollPos * _increment + ScrollBarWidth, Color.White);
            Graphics.DrawTexture(_images[2], (int)Location.X, (int)Location.Y + Height - ScrollBarWidth, Color.White);
        }
        else
        {
            Graphics.DrawTexture(_images[1], (int)Location.X + _scrollPos * _increment+ ScrollBarWidth, (int)Location.Y, Color.White);
            Graphics.DrawTexture(_images[2], (int)Location.X + Width - ScrollBarWidth, (int)Location.Y, Color.White);
        }

        base.Draw(pulse);
    }

    public void ScrollToEnd()
    {
        _scrollPos = _positions;
        _scrollAction(_positions);
    }

    public void ScrollTo(int scrollPosition)
    {
        _scrollPos = scrollPosition;
        _scrollAction(_scrollPos);
    }
}