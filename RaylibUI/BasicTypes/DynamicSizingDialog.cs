using System.Numerics;
using Model;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
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
            _headerLabel = _active != null ? 
                new HeaderLabel(this, _active.Look, title, fontSize: _active?.Look.HeaderLabelFontSizeNormal ?? 28) :
                new HeaderLabel(this, title, 28);
            Controls.Add(_headerLabel);
        }

        LayoutPadding = _active?.GetPadding(_headerLabel?.TextSize.Y ?? 0, true) ?? new Padding(28, 11, 11, 11);
    }


    public override void Resize(int width, int height)
    {
        var heights = new int[Controls.Count];  // height of control
        var topCtrl = new int[Controls.Count];  // y-distance of control from dialog top
        var widths = new int[Controls.Count];
        var leftCtrl = new int[Controls.Count];
        var maxWidth = Controls.Max(c => c.GetPreferredWidth());
        var imageBox = Controls.OfType<ImageBox>().FirstOrDefault();
        var imageWidth = imageBox?.GetPreferredWidth() ?? 0;
        var imageHeight = imageBox?.GetPreferredHeight() ?? 0;
        if (_requestedWidth.HasValue && _requestedWidth.Value > maxWidth)
        {
            maxWidth = _requestedWidth.Value;
        }
        var imageBoxIndex = 0;
        for (int index = 0; index < Controls.Count; index++)
        {
            var control = Controls[index];

            // Heights
            var controlHeight = control.GetPreferredHeight();
            heights[index] = controlHeight;
            if (index == 0)
            {
                topCtrl[index] = _headerLabel != null ? 0 : LayoutPadding.Top;
            }
            else if (index > 0)
            {
                if (Controls[index - 1] is ImageBox)
                {
                    topCtrl[index] = topCtrl[index - 1];
                    imageBoxIndex = index - 1;
                }
                else if (imageBox != null && (Controls[index].Children?.OfType<Button>().Any() ?? false))
                {
                    var middleControlsHeight = Controls.
                        Where(c => c is not ImageBox && c is not HeaderLabel && (!c.Children?.OfType<Button>().Any() ?? true)).ToList().
                        Sum(c => c.GetPreferredHeight());
                    topCtrl[index] = topCtrl[imageBoxIndex] + Math.Max(heights[imageBoxIndex], middleControlsHeight) + 3;
                }
                else
                {
                    topCtrl[index] = topCtrl[index - 1] + heights[index - 1];
                    if (Controls[index].Children?.OfType<Button>().Any() ?? false)
                    {
                        topCtrl[index] = topCtrl[index - 1] + heights[index - 1] + 3;
                    }
                }
            }

            // Widths
            if (Controls[index] is HeaderLabel)
            {
                leftCtrl[index] = 0;
                widths[index] = maxWidth + imageWidth + LayoutPadding.Left + LayoutPadding.Right;
            }
            else if (Controls[index] is ImageBox)
            {
                leftCtrl[index] = LayoutPadding.Left;
                widths[index] = control.GetPreferredWidth();
            }
            else if (Controls[index].Children?.OfType<Button>().Any() ?? false)
            {
                leftCtrl[index] = LayoutPadding.Left - 1;
                widths[index] = maxWidth + imageWidth + 2;
            }
            else
            {
                leftCtrl[index] = imageBox != null ? leftCtrl[imageBoxIndex] + widths[imageBoxIndex] : LayoutPadding.Left;
                widths[index] = maxWidth;
            }
        }

        var totalHeight = topCtrl[^1] + LayoutPadding.Bottom;
        if (_buttons != null)
        {
            totalHeight -= 3;
            if (!_active?.IsButtonInOuterPanel ?? true)
                totalHeight += heights[^1];
        }

        SetLocation(width, maxWidth + imageWidth + LayoutPadding.Left + LayoutPadding.Right, height, totalHeight);

        for (int index = 0; index < Controls.Count; index++)
        {
            Controls[index].Bounds = new Rectangle(Location.X + leftCtrl[index], Location.Y + topCtrl[index], widths[index], heights[index]);
            Controls[index].OnResize();
        }

        _size = new Size(maxWidth + imageWidth + LayoutPadding.Left + LayoutPadding.Right, totalHeight);

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
