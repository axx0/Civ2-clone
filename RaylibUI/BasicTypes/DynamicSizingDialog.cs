using System.Numerics;
using Model;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using Button = RaylibUI.Controls.Button;
using Size = RaylibUI.BasicTypes.Size;

namespace RaylibUI;

public class DynamicSizingDialog : BaseDialog
{
    private readonly int? _requestedWidth;

    private readonly HeaderLabel? _headerLabel;
    private TableLayoutPanel _innerPanel;

    private ControlGroup? _buttons;

    private IUserInterface _active;

    protected DynamicSizingDialog(Main host, string title, Point? position = null, int? requestedWidth = null) :
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


    public override void Resize(int width, int height)
    {
        _innerPanel = Controls.OfType<TableLayoutPanel>().FirstOrDefault();

        var imageBox = _innerPanel.Controls.OfType<ImageBox>().FirstOrDefault();
        var imageWidth = imageBox?.GetPreferredWidth() ?? 0;
        int maxTextWidth = 0;
        _innerPanel.Width = Math.Max(_headerLabel?.Width ?? 0 - PaddingSide, imageWidth + Math.Max(maxTextWidth, _requestedWidth ?? 0));

        var options = _innerPanel.Controls.OfType<OptionsPanel>().FirstOrDefault();
        if (options is not null)
        {
            options.Width = _innerPanel.Width - imageWidth;
            options.OnResize();
        }

        var listbox = _innerPanel.Controls.OfType<Listbox>().FirstOrDefault();
        if (listbox is not null)
        {
            var cell = _innerPanel.TableLayout.Cells.FirstOrDefault(c => c.Control == listbox);
            listbox.Width = _innerPanel.Width - cell.Padding.Left - cell.Padding.Right - imageWidth;
            listbox.OnResize();
        }

        _innerPanel.OnResize();

        var menu = Controls.OfType<ControlGroup>().FirstOrDefault();
        if (menu is not null)
        {
            menu.ResizeChildWidths(_innerPanel.Width);
            menu.OnResize();
            menu.Location = new Vector2(LayoutPadding.Left, _innerPanel.Location.Y + _innerPanel.Height + 3);
        }

        // Adjust header label position
        if (_headerLabel is not null)
        {
            _headerLabel.Location = new Vector2((_innerPanel.Width + PaddingSide) / 2 - _headerLabel.Width / 2, _headerLabel.Location.Y);
        }

        SetLocation(width, Width, height, Height);
        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding);
    }

    public override int Width => _innerPanel.Width + PaddingSide;
    public override int Height => _innerPanel.Height + LayoutPadding.Top + LayoutPadding.Bottom;


    protected void SetButtons(ControlGroup buttons)
    {
        _buttons = buttons;
    }

    protected bool ButtonExists(string text)
    {
        return _buttons?.Controls?.OfType<Button>().Any(b => b.Text == text) ?? false;
    }
}