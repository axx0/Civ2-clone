using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes;

namespace RaylibUI.Initialization.GameControls;

public class MinimapPanel : BaseControl
{
    private readonly Game _game;
    private Texture2D? _backgroundImage;

    public MinimapPanel(GameScreen controller, Game game) : base(controller)
    {
        _game = game;
    }

    public override Size GetPreferredSize(int width, int height)
    {
        return new Size(GameScreen.MiniMapWidth, GameScreen.MiniMapHeight);
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