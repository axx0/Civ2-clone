using Civ2engine;
using Raylib_cs;
using RaylibUI.ComplexControls.Map;
using RaylibUI.Initialization.GameControls;

namespace RaylibUI.Initialization;

public class GameScreen : BaseScreen
{
    public Game Game { get; }
    
    private readonly MinimapPanel _minimapPanel;
    private readonly MapControl _mapControl;
    private readonly StatusPanel _statusPanel;


    internal const int MiniMapHeight = 148;
    internal const int MiniMapWidth = 262;

    public GameScreen(Game game)
    {
        Game = game;
        
        _mapControl = new MapControl(this, game);
        Controls.Add(_mapControl);
        
        _minimapPanel = new MinimapPanel(this, game);
        Controls.Add(_minimapPanel);
        
        _statusPanel = new StatusPanel(this, game);
        Controls.Add(_statusPanel);
    }

    public override void Resize(int width, int height)
    {
        var mapWidth = width - MiniMapWidth;
        _mapControl.Bounds = new Rectangle(0, 0, mapWidth, height);
        _minimapPanel.Bounds = new Rectangle( mapWidth, 0, MiniMapWidth, MiniMapHeight);
        _statusPanel.Bounds = new Rectangle(mapWidth, MiniMapHeight, MiniMapWidth, height - MiniMapHeight);
        
        base.Resize(width, height);
    }
}