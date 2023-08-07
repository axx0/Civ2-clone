using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.Initialization;

namespace RaylibUI.ComplexControls.Map;

public class MapControl : BaseControl
{
    private Texture2D? _backgroundImage;

    public MapControl(IControlLayout controller, Game game) : base(controller)
    {
    }

    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(width- GameScreen.MiniMapWidth, height);
    }

    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, 11,11,11);
        base.OnResize();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.WHITE);
        base.Draw(pulse);
    }
}