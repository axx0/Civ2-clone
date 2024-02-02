using Civ2engine;
using Model;
using Raylib_cs;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusPanel : BaseControl
{
    private readonly GameScreen _gameScreen;
    private readonly Game _game;
    private readonly IUserInterface _active;
    private readonly HeaderLabel _headerLabel;
    private Texture2D? _backgroundImage;
    private Rectangle _internalBounds;
    private Padding _padding;

    public StatusPanel(GameScreen gameScreen, Game game) : base(gameScreen)
    {
        _gameScreen = gameScreen;
        _game = game;
        _active = gameScreen.MainWindow.ActiveInterface;

        _headerLabel = new HeaderLabel(gameScreen, _active.Look, "Status", fontSize: _active.Look.HeaderLabelFontSizeNormal);
        _padding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);
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
        _backgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, _padding);
        
        _headerLabel.Bounds = new Rectangle((int)Location.X, (int)Location.Y, Width, _padding.Top);
        _headerLabel.OnResize();
        _internalBounds = new Rectangle(Location.X + _padding.Left, Location.Y + _padding.Top, Width - _padding.Left - _padding.Right, Height - _padding.Top - _padding.Bottom);
        base.OnResize();
        Update();
    }

    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(_backgroundImage.Value,(int)Location.X, (int)Location.Y, Color.White);
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