using System.Runtime.CompilerServices;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUI.RunGame.GameControls.GameModes;
using RaylibUI.RunGame.GameControls.Mapping;

namespace RaylibUI.RunGame;

public class GameScreen : BaseScreen
{
    public Main Main { get; }
    public Game Game { get; }
    public Sound Soundman { get; }

    private readonly MinimapPanel _minimapPanel;
    private readonly MapControl _mapControl;
    private readonly StatusPanel _statusPanel;
    private readonly IPlayer _player;
    private readonly GameMenu _menu;

    public IGameMode ActiveMode
    {
        get => _activeMode;
        set
        {
            if (value.Activate())
            {
                _activeMode = value;
            }
        }
    }

    public TileTextureCache TileCache { get; }
    
    public IPlayer Player => _player;
    public IGameMode Moving { get; }
    public IGameMode ViewPiece { get; }

    private const int MiniMapWidth = 262;
    private readonly int _miniMapHeight;
    private IGameMode _activeMode;

    public event EventHandler<MapEventArgs>? OnMapEvent = null;

    public GameScreen(Main main, Game game, Sound soundman): base(main)
    {
        TileCache = new TileTextureCache(this);
        Main = main;
        Game = game;
        Soundman = soundman;

        _miniMapHeight = Math.Max(100, game.CurrentMap.YDim) + 38 + 11;

        var civ = game.GetPlayerCiv;
        _player = new LocalPlayer(this, civ);
        game.ConnectPlayer(_player);
        
        Moving = new MovingPieces(this);
        ViewPiece = new ViewPiece(this);
        ActiveMode = _player.ActiveUnit is not {MovePoints: > 0} ? ViewPiece : Moving;
        
        var width = Raylib.GetScreenWidth();
        var height = Raylib.GetScreenHeight();

        _menu = new GameMenu(this);
        _menu.GetPreferredWidth();
        
        var menuHeight = _menu.GetPreferredHeight();
        var mapWidth = width - MiniMapWidth;
        
        
        _mapControl = new MapControl(this, game, new Rectangle(0, menuHeight, mapWidth, height - menuHeight));
        Controls.Add(_mapControl);
        
        Controls.Add(_menu);
        _minimapPanel = new MinimapPanel(this, game);
        Controls.Add(_minimapPanel);
        
        _statusPanel = new StatusPanel(this, game);
        Controls.Add(_statusPanel);
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        ActiveMode.HandleKeyPress(key);
    }

    public override void Resize(int width, int height)
    {
        _menu.GetPreferredWidth();
        var menuHeight = _menu.GetPreferredHeight();
        var mapWidth = width - MiniMapWidth;
        _menu.Bounds = new Rectangle(0, 0, width, menuHeight);
        _mapControl.Bounds = new Rectangle(0, menuHeight, mapWidth, height - menuHeight);
        _minimapPanel.Bounds = new Rectangle( mapWidth, menuHeight, MiniMapWidth, _miniMapHeight);
        _statusPanel.Bounds = new Rectangle(mapWidth, _miniMapHeight + menuHeight, MiniMapWidth, height - _miniMapHeight - menuHeight);
        
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

    public void ShowCityWindow(City city)
    {
        //TODO: City window
        var cityDialog = new CityWindow(this, city);
        ShowDialog(cityDialog);
    }

    public void TriggerMapEvent(MapEventArgs args)
    {
        OnMapEvent?.Invoke(this, args);
    }
    
    public bool ActivateUnits(Tile tile)
    {
        var unitsHere = tile.UnitsHere;
        if (unitsHere.Count == 0)
        {
            Game.ActiveTile = tile;
            return true;
        }

        var unit = unitsHere[0];
        if (unitsHere.Count > 1)
        {
            //TODO: Multiple units on this square => open unit selection dialogBox
            //
            // var selectUnitDialog = new SelectUnitDialog(main, unitsHere);
            // selectUnitDialog.ShowModal(main);
            //
            // if (selectUnitDialog.SelectedIndex < 0)
            // {
            //     return false;
            // }
            //
            // unit = unitsHere[selectUnitDialog.SelectedIndex];
        }

            
        if (!unit.TurnEnded)
        {
            _player.ActiveUnit = unit;
        }

        unit.Order = OrderType.NoOrders; // Always clear order when clicked, no matter if the unit is activated
        ActiveMode = Moving;
        return true;
    }
}