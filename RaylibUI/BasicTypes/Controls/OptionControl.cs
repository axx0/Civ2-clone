using System.Net.Mime;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

internal class OptionControl : LabelControl
{
    private readonly Texture2D[] _images;

    public override bool CanFocus => true;

    public OptionControl(IControlLayout controller, string text, int index, bool isChecked, Texture2D[] images) : base(controller, text, eventTransparent: false, offset: images[0].Width, font: controller.MainWindow.ActiveInterface.Look.LabelFont, colorFront: controller.MainWindow.ActiveInterface.Look.LabelColour)
    {
        Index = index;
        Checked = isChecked;
        _images = images;
    }

    public int Index { get; }
    public bool Checked { get; set; }
    
    public void Clear()
    {
        Checked = false;
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_images[Checked || _images.Length == 1 ? 0: 1], (int)Location.X,(int)Location.Y, Color.WHITE);
        base.Draw(pulse);
        if (Controller.Focused == this)
        {
            Raylib.DrawRectangleLinesEx(new Rectangle(Bounds.X + _images[0].Width-1, Bounds.Y + 1, Bounds.Width - _images[0].Width, Bounds.Height -2), 0.5f, Color.BLACK);
        }
    }

    public override int GetPreferredHeight()
    {
        var baseHeight = base.GetPreferredHeight();
        return baseHeight < _images[0].Height ? _images[0].Height : baseHeight;
    }
}   