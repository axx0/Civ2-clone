using System.Net.Mime;
using Model.Images;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI;

internal class OptionControl : LabelControl
{
    private readonly IImageSource[] _images;
    private readonly int _imageWidth, _imageHeight;

    public override bool CanFocus => true;

    public OptionControl(IControlLayout controller, string text, int index, bool isChecked, IImageSource[] images) : base(
        controller, text, eventTransparent: false, offset: Images.GetImageWidth(images[0]),
        fontSize: controller.MainWindow.ActiveInterface.Look.LabelFontSize,
        font: controller.MainWindow.ActiveInterface.Look.LabelFont,
        colorFront: controller.MainWindow.ActiveInterface.Look.LabelColour,
        colorShadow: controller.MainWindow.ActiveInterface.Look.LabelShadowColour,
        shadowOffset: new System.Numerics.Vector2(1, 1))
    {
        Index = index;
        Checked = isChecked;
        _images = images;
        _imageWidth = Images.GetImageWidth(images[0]);
        _imageHeight = Images.GetImageHeight(images[0]);
    }

    public int Index { get; }
    public bool Checked { get; set; }
    
    public void Clear()
    {
        Checked = false;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(TextureCache.GetImage(_images[Checked || _images.Length == 1 ? 0: 1]), (int)Location.X,(int)Location.Y, Color.White);
        base.Draw(pulse);
        if (Controller.Focused == this)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(Bounds.X + _imageWidth - 1, Bounds.Y + 1, Bounds.Width - _imageWidth, Bounds.Height -2), 0.5f, Color.Black);
        }
    }

    public override int GetPreferredHeight()
    {
        var baseHeight = base.GetPreferredHeight();
        return baseHeight < _imageHeight ? _imageHeight : baseHeight;
    }
}   