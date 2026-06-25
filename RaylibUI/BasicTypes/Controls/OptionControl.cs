using Model.Images;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI.BasicTypes;

public class OptionControl : LabelControl
{
    public Action<OptionControl> Selected { get; set; }
    private readonly IImageSource[] _icons;
    private readonly int _iconWidth, _iconHeight;
    private readonly OptionsPanel _parent;
    private readonly IControlLayout _controller;

    public override bool CanFocus => false;

    public OptionControl(IControlLayout controller, OptionsPanel parent, string text, int index, bool isChecked, IImageSource[] icons) : 
        base(controller, text, eventTransparent: false, verticalAlignment: Model.Controls.VerticalAlignment.Center,
        padding: new (0, Images.GetImageWidth(icons[0], controller.MainWindow.ActiveInterface, parent.Looks.IconScale), 0, 0),
        font: parent.Looks.Font, fontSize: parent.Looks.FontSize, colorFront: parent.Looks.TextColorFront,
        colorShadow: parent.Looks.TextColorShadow, shadowOffset: parent.Looks.TextShadowOffset)
    {
        _controller = controller;
        Index = index;
        Checked = isChecked;
        _parent = parent;
        _icons = icons;
        _iconWidth = Images.GetImageWidth(icons[0], controller.MainWindow.ActiveInterface, parent.Looks.IconScale);
        _iconHeight = Images.GetImageHeight(icons[0], controller.MainWindow.ActiveInterface, parent.Looks.IconScale);

        Click += OnClick;
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        Selected(this);
        _controller.Focused = (OptionsPanel)Parent!;
    }

    public int Index { get; }
    public bool Checked { get; set; }
    
    public void Clear()
    {
        Checked = false;
    }

    public override int GetPreferredHeight()
    {
        var baseHeight = base.GetPreferredHeight();
        return baseHeight < _iconHeight ? _iconHeight : baseHeight;
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        Graphics.DrawTextureEx(TextureCache.GetImage(_icons[Checked || _icons.Length == 1 ? 0 : 1]), 
            new(Bounds.X, Bounds.Y), 0f, _parent.Looks.IconScale, Color.White);
        base.Draw(pulse);
        if (_parent.SelectedId == Index)
        {
            Graphics.DrawRectangleLinesEx(new Rectangle(Bounds.X + _iconWidth - 1, Bounds.Y + 1, Bounds.Width - _iconWidth, Bounds.Height - 2), 0.5f, Color.Black);
        }
    }
}   