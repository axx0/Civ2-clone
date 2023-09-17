using System.Net.Mime;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI;

internal class OptionControl : LabelControl
{
    private readonly Action<OptionControl> _action;
    private readonly Texture2D[] _images;
    

    public OptionControl(IControlLayout controller, string text, int index, Action<OptionControl> optionAction, bool isChecked, Texture2D[] images) : base(controller, text, images[0].width)
    {
        Index = index;
        _action = optionAction;
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
    }

    public override Size GetPreferredSize(int width, int height)
    {
        var baseSize = base.GetPreferredSize(width, height);
        return baseSize.Height < _images[0].height ? new Size(baseSize.Width, _images[0].height) : baseSize;
    }

    public override void OnClick()
    {
        base.OnClick();
        _action(this);
        Checked = true;
    }
}   