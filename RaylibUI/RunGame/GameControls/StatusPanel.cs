using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly GameScreen _gameScreen;
    private readonly Game _game;
    private Texture2D? _backgroundImage;
    private HeaderLabel _headerLabel;
    private Rectangle _internalBounds;
    private const int PaddingSide = 11;
    private const int Top = 38;
    private const int PaddingBtm = 11;

    public StatusPanel(GameScreen gameScreen, Game game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;

        _headerLabel = new HeaderLabel(gameScreen, "Status");
        Click += OnClick;
    }

 

    public void Update()
    {
        Children = _gameScreen.ActiveMode.GetSidePanelContents(_internalBounds);
    }

    private void OnClick(object? sender, MouseEventArgs e)
    {
        _gameScreen.ActiveMode.PanelClick();
    }

    public override void OnResize()
    {
        _backgroundImage = ImageUtils.PaintDialogBase(Width, Height, Top, PaddingBtm, PaddingSide);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, Top);
        _headerLabel.OnResize();
        _internalBounds = new Rectangle(Location.X + PaddingSide, Location.Y + Top, Width - PaddingSide * 2,
            Height - Top - PaddingBtm);
        base.OnResize();
        Update();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.WHITE);
        _headerLabel.Draw(pulse);
        base.Draw(pulse);
        if (Children != null)
        {
            foreach (var control in Children)
            {
                control.Draw(pulse);
            }
        }
    }
}