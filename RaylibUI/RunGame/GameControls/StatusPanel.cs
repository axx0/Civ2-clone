using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly Game _game;
    private Texture2D? _backgroundImage;
    private HeaderLabel _headerLabel;
    private const int PaddingSide = 11;
    private const int Top = 38;
    private const int PaddingBtm = 11;

    public StatusPanel(IControlLayout controller, Game game) : base(controller)
    {
        _game = game;

        _headerLabel = new HeaderLabel(controller, "Status");
    }
    
    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, Top, PaddingBtm, PaddingSide);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, Top);
        _headerLabel.OnResize();

        base.OnResize();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.WHITE);
        _headerLabel.Draw(pulse);
        base.Draw(pulse);
    }
}