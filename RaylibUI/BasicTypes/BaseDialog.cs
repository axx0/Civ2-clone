using System.Numerics;
using Model;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Forms;
using Size = RaylibUI.BasicTypes.Size;

namespace RaylibUI;

public class BaseDialog : BaseLayoutController
{
    private readonly int? _requestedWidth;


    private Size _size;
    private Vector2 _location;
    private Texture2D? _backgroundImage;
    public Point Position { get; }

    private HeaderLabel? _headerLabel;

    private ControlGroup? _buttons;
    
    protected BaseDialog(Main host, string title, Point? position = null, int? requestedWidth = null): base(host) 
    {
        _requestedWidth = requestedWidth;
        Position = position ?? new Point(0,0);
        if (!string.IsNullOrWhiteSpace(title))
        {
            _headerLabel = new HeaderLabel(this, title);
            Controls.Add(_headerLabel);
        }
    }


    public override void Resize(int width, int height)
    {
        var heights = new int[Controls.Count];
        var maxWidth = Controls.Max(c=>c.GetPreferredWidth());
        if (_requestedWidth.HasValue && _requestedWidth.Value > maxWidth)
        {
            maxWidth = _requestedWidth.Value;
        }
        var totalHeight = 0;
        int index;
        for (index = 0; index < Controls.Count; index++)
        {
            var control = Controls[index];
            control.Width = maxWidth;

            var controlHeight = control.GetPreferredHeight();

            heights[index] = controlHeight;
            totalHeight += controlHeight;
        }

        _location.X = Position.X switch
        {
            0 => (width - maxWidth) / 2f,
            < 0 => width + (int)Position.X,
            _ => _location.X
        };

        _location.Y = Position.Y switch
        {
            0 => (height - totalHeight) / 2f,
            < 0 => height + (int)Position.Y,
            _ => _location.Y
        };

        int left = 11 + (int)_location.X;
        int top = 11 + (int)_location.Y;
        index = 0;
        if (_headerLabel != null)
        {
            _headerLabel.Bounds = new Rectangle(_location.X, _location.Y, maxWidth + 22, heights[0] + 11);
            top += heights[0];
            index = 1;
            _headerLabel.OnResize();
        }

        var mainControlsCount = _buttons == null ? Controls.Count : Controls.Count - 1;
        for (; index < mainControlsCount; index++)
        {
            Controls[index].Bounds = new Rectangle(left, top, maxWidth, heights[index]);
            top += heights[index];
            Controls[index].OnResize();
        }

        if (_buttons != null)
        {
            _buttons.Bounds = new Rectangle(left-1, top + 3, maxWidth +2, heights[^1]);
            _buttons.OnResize();
        }

        _size = new Size(maxWidth + 11 * 2, totalHeight + 22);

        _backgroundImage = ImageUtils.PaintDialogBase( _size.Width, _size.Height, _headerLabel?.Height ?? 11, 11 + _buttons?.Height ?? 0, 11);
    }

    protected void SetButtons(ControlGroup buttons)
    {
        _buttons = buttons;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)_location.X, (int)_location.Y, Color.WHITE);
        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }
    }

    public override void Move(Vector2 moveAmount)
    {
        _location += moveAmount;
        MoveChildren(moveAmount, Controls);
    }


    private static void MoveChildren(Vector2 moveAmount, IEnumerable<IControl> controls)
    {
        foreach (var control in controls)
        {
            control.Location += moveAmount;
            if (control.Children != null)
            {
                MoveChildren(moveAmount, control.Children);
            }
        }
    }
}