using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI;

public class ListBoxScrollBar : BaseControl
{
    public const int DefaultHeight = 17;
    private readonly int _actualColumns;
    private readonly int _requiredColumns;
    private readonly Action<int> _scrollAction;
    private readonly Texture2D[] _images;
    private int _scroolPos;
    private readonly int _positions;
    private int _increment;

    public ListBoxScrollBar(IControlLayout controller, int actualColumns, int requiredColumns, Action<int> scrollAction) :  base(controller)
    {
        _actualColumns = actualColumns;
        _requiredColumns = requiredColumns;
        _scrollAction = scrollAction;
        _images = ImageUtils.GetScrollImages(DefaultHeight).Select(Raylib.LoadTextureFromImage).ToArray();
        _scroolPos = 0;
        _positions = requiredColumns - _actualColumns;
    }

    public override int GetPreferredHeight()
    {
        return DefaultHeight;
    }
    public override int GetPreferredWidth()
    {
        return -1;
    }

    public override void OnResize()
    {
        _increment = (Width - 2 * DefaultHeight) / _positions;
        base.OnResize();
    }

    public override void OnClick()
    {
        var pos = GetRelativeMousePosition();
        var breakPoint = _increment * _scroolPos + DefaultHeight;
        if (pos.X < breakPoint)
        {
            if (_scroolPos > 0)
            {
                _scroolPos--;
                _scrollAction(_scroolPos);
            }
        }else if (pos.X > breakPoint + DefaultHeight)
        {
            if (_scroolPos < _positions)
            {
                _scroolPos++;
                _scrollAction(_scroolPos);
            }
        }
        
        base.OnClick();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawRectangle((int)Location.X, (int)Location.Y, Width, Height, Color.WHITE);
        Raylib.DrawTexture(_images[0], (int)Location.X, (int)Location.Y, Color.WHITE);
        Raylib.DrawTexture(_images[1], (int)Location.X + _scroolPos * _increment, (int)Location.Y, Color.WHITE);
        Raylib.DrawTexture(_images[2], (int)Location.X + Width - DefaultHeight, (int)Location.Y, Color.WHITE);
        base.Draw(pulse);
    }
}