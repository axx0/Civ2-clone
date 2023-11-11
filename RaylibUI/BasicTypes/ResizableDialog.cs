using System.Numerics;
using Model;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Forms;
using Size = RaylibUI.BasicTypes.Size;

namespace RaylibUI;

public class ResizableDialog : BaseDialog
{
    private readonly int? _requestedWidth;


    private Size _size;

    private readonly HeaderLabel? _headerLabel;

    private ControlGroup? _buttons;

    protected ResizableDialog(Main host, string title, Point? position = null, int? requestedWidth = null) : base(host,
        position)
    {
        _requestedWidth = requestedWidth;
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

        SetLocation(width, maxWidth, height, totalHeight);


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

        BackgroundImage = ImageUtils.PaintDialogBase( _size.Width, _size.Height, _headerLabel?.Height ?? 11, 11 + _buttons?.Height ?? 0, 11);
    }
    

    protected void SetButtons(ControlGroup buttons)
    {
        _buttons = buttons;
    }

}
