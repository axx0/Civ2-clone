using System.Runtime.CompilerServices;
using Civ2engine;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame;

public class GameScreen : BaseScreen
{
    public Main Main { get; }
    public Game Game { get; }
    
    private readonly MinimapPanel _minimapPanel;
    private readonly MapControl _mapControl;
    private readonly StatusPanel _statusPanel;
    private readonly IPlayer _player;
    private readonly IGameMode _activeMode;
    private readonly GameMenu _menu;


    internal const int MiniMapHeight = 148;
    internal const int MiniMapWidth = 262;

    public event EventHandler<MapEventArgs>? OnMapEvent = null;

    public GameScreen(Main main, Game game, Sound soundman): base(main)
    {
        Main = main;
        Game = game;

        var civ = game.GetPlayerCiv;
        _player = new LocalPlayer(this, civ);

        game.ConnectPlayer(_player);
        
        _mapControl = new MapControl(this, game);
        Controls.Add(_mapControl);
        
        _minimapPanel = new MinimapPanel(this, game);
        Controls.Add(_minimapPanel);
        
        _statusPanel = new StatusPanel(this, game);
        Controls.Add(_statusPanel);

        _menu = new GameMenu(this);
        Controls.Add(_menu);

        if (_player.ActiveUnit != null)
        {
            _activeMode = new MovingPieces(this);
        }
        else
        {
            _activeMode = new ViewPiece(this);
        }
    }

    public override void Resize(int width, int height)
    {
        _menu.GetPreferredWidth();
        var menuHeight = _menu.GetPreferredHeight();
        var mapWidth = width - MiniMapWidth;
        _menu.Bounds = new Rectangle(0, 0, width, menuHeight);
        _mapControl.Bounds = new Rectangle(0, menuHeight, mapWidth, height - menuHeight);
        _minimapPanel.Bounds = new Rectangle( mapWidth, menuHeight, MiniMapWidth, MiniMapHeight);
        _statusPanel.Bounds = new Rectangle(mapWidth, MiniMapHeight + menuHeight, MiniMapWidth, height - MiniMapHeight - menuHeight);
        
        base.Resize(width, height);
    }

    public void UpdateOrders(Tile activeTile, Unit activeUnit)
    {
    }
    
    public void ShowCityDialog(string dialog, IList<string> replaceStrings)
    {
        // var dialogBox = new CivDialog(this, popupBoxList[dialog], replaceStrings);
        // this.ShowDialog(dialogBox, true);
    }

    public void TriggerMapEvent(MapEventArgs args)
    {
        OnMapEvent?.Invoke(this, args);
    }
}