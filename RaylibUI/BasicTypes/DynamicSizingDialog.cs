using System.Numerics;
using Civ2Gold;
using Model;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.Forms;
using Button = RaylibUI.Controls.Button;
using Size = RaylibUI.BasicTypes.Size;

namespace RaylibUI;

public class DynamicSizingDialog : BaseDialog
{
    private readonly int? _requestedWidth;
    
    private Size _size;

    private readonly HeaderLabel? _headerLabel;

    private ControlGroup? _buttons;

    private IUserInterface _active;

    protected DynamicSizingDialog(Main host, string title, Point? position = null, int? requestedWidth = null) : 
        base(host, position)
    {
        _active = host.ActiveInterface;

        _size = new Size(0, 0);
        _requestedWidth = requestedWidth;
        if (!string.IsNullOrEmpty(title))
        {
            _headerLabel = new HeaderLabel(this, _active.Look, title, fontSize: _active.Look.HeaderLabelFontSizeNormal);
            Controls.Add(_headerLabel);
        }

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, true);
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
            if (control.Children?.OfType<Button>().Any() ?? false)
            {
                totalHeight += LayoutPadding.Bottom;
                
                if (!_active.IsButtonInOuterPanel)
                {
                    totalHeight += controlHeight;
                }
            }
            else
            {
                totalHeight += controlHeight;
            }
        }

        SetLocation(width, maxWidth, height, totalHeight);


        int left = LayoutPadding.Left + (int)Location.X;
        int top = LayoutPadding.Top + (int)Location.Y;
        var sidePadding = LayoutPadding.Left + LayoutPadding.Right;
        index = 0;
        if (_headerLabel != null)
        {
            _headerLabel.Bounds = new Rectangle(Location.X, Location.Y, maxWidth + sidePadding, LayoutPadding.Top);
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

        _size = new Size(maxWidth + sidePadding, totalHeight);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _size.Width, _size.Height, LayoutPadding);
    }
    

    protected void SetButtons(ControlGroup buttons)
    {
        _buttons = buttons;
    }

    protected bool ButtonExists(string text)
    {
        return _buttons?.Children?.OfType<Button>().Any(b => b.Text == text) ?? false;
    }
}
