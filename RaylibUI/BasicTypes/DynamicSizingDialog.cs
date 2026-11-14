using System.Numerics;
using Model;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using Button = RaylibUI.Controls.Button;

namespace RaylibUI;

public class DynamicSizingDialog : BaseDialog
{
    private readonly int _requestedWidth;

    private readonly HeaderLabel? _headerLabel;
    private TableLayoutPanel _innerPanel;
    private ImageBox? _image;
    private int _contentWidth;
    private int _maxHeight;

    private ControlGroup? _buttons;

    private IUserInterface _active;

    protected DynamicSizingDialog(Main host, string title, int requestedWidth, Point? position = null) :
        base(host, position)
    {
        _active = host.ActiveInterface;

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


    /// <summary>
    /// Standard dialog layout: header on top, optional image left of the content table, buttons below.
    /// Width = max(header, image + max(table content, requested width)); buttons never widen the dialog.
    /// </summary>
    public override void Resize(int width, int height)
    {
        _innerPanel = Controls.OfType<TableLayoutPanel>().First();
        _innerPanel.MaxControlRows = 999;

        var unlimited = new Size(int.MaxValue, int.MaxValue);
        var imageWidth = _image?.GetPreferredWidth() ?? 0;
        var imageHeight = _image?.GetPreferredHeight() ?? 0;
        var headerWidth = _headerLabel?.Measure(unlimited).Width ?? 0;
        var table = _innerPanel.Measure(unlimited);

        _contentWidth = Math.Max(headerWidth, imageWidth + Math.Max(table.Width, _requestedWidth));

        _image?.Arrange(new Rectangle(LayoutPadding.Left, LayoutPadding.Top, imageWidth, imageHeight));
        _innerPanel.Arrange(new Rectangle(LayoutPadding.Left + imageWidth, LayoutPadding.Top,
            _contentWidth - imageWidth, table.Height));

        // A stretched listbox may get taller when it gets wider, so use the arranged height
        _maxHeight = Math.Max(_innerPanel.Height, imageHeight);

        var menu = Controls.OfType<ControlGroup>().FirstOrDefault();
        if (menu is not null)
        {
            menu.Measure(new Size(_contentWidth, int.MaxValue));
            menu.Arrange(new Rectangle(LayoutPadding.Left, LayoutPadding.Top + _maxHeight + 3,
                _contentWidth, menu.DesiredSize.Height));
        }

        // Header spans the whole dialog width; its text is centered within it
        _headerLabel?.Arrange(new Rectangle(0, 0, Width, LayoutPadding.Top));

        SetLocation(width, Width, height, Height);
        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding);
    }

    public override int Width => _contentWidth + PaddingSide;
    public override int Height => _maxHeight + LayoutPadding.Top + LayoutPadding.Bottom;

    /// <summary>
    /// Image shown left of the content table.
    /// </summary>
    protected void SetImage(ImageBox image)
    {
        _image = image;
        Controls.Add(image);
    }


    protected void SetButtons(ControlGroup buttons)
    {
        _buttons = buttons;
    }

    protected bool ButtonExists(string text)
    {
        return _buttons?.Controls?.OfType<Button>().Any(b => b.Text == text) ?? false;
    }
}