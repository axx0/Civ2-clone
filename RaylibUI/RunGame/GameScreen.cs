using System.Runtime.CompilerServices;
using Civ2.Menu;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Interface;
using Raylib_cs;
using RaylibUI.RunGame.Commands.Orders;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Menu;
using RaylibUI.RunGame.GameModes;
using RaylibUI.RunGame.GameModes.Orders;

namespace RaylibUI.RunGame;

public class GameScreen : BaseScreen
{
    public Main Main { get; }
    public Game Game { get; }
    public Sound Soundman { get; }
    public List<Order> Orders { get; set; } = new();

    private readonly MinimapPanel _minimapPanel;
    private readonly MapControl _mapControl;
    private readonly StatusPanel _statusPanel;
    private readonly LocalPlayer _player;
    private readonly GameMenu _menu;

    public IGameMode ActiveMode
    {
        get => _activeMode ??= ViewPiece;
        set
        {
            if (value.Activate())
            {
                _activeMode = value;
            }
        }
    }

    public TileTextureCache TileCache { get; }
    
    public LocalPlayer Player => _player;

    public StatusPanel StatusPanel => _statusPanel;

    public GameMenu MenuBar => _menu;
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

        var commands = SetupCommands(game);
        var menuElements = main.ActiveInterface.ConfigureGameCommands(commands);
        _menu = new GameMenu(this, menuElements);
        _menu.GetPreferredWidth();

        Moving = new MovingPieces(this, commands);
        ViewPiece = new ViewPiece(this);
        Processing = new ProcessingMode(this);
               
        if (Game.GetActiveCiv == Game.GetPlayerCiv)
        {
            ActiveMode = _player.ActiveUnit is not {MovePoints: > 0} ? ViewPiece : Moving;
        }
        else
        {
            ActiveMode = Processing;
        }
        
        var width = Raylib.GetScreenWidth();
        var height = Raylib.GetScreenHeight();
        
        var menuHeight = _menu.GetPreferredHeight();
        var mapWidth = width - MiniMapWidth;
        
        _statusPanel = new StatusPanel(this, game);
        
        _minimapPanel = new MinimapPanel(this, game);
        _mapControl = new MapControl(this, game, new Rectangle(0, menuHeight, mapWidth, height - menuHeight));

        // The order of these is important as MapControl can overdraw so must be drawn first
        Controls.Add(_mapControl);
        Controls.Add(_menu);
        Controls.Add(_minimapPanel);
        Controls.Add(_statusPanel);
    }

    public ProcessingMode Processing { get; }

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key is KeyboardKey.KEY_LEFT_ALT or KeyboardKey.KEY_RIGHT_ALT)
        {
            Focused = MenuBar.Children!.First();
            return;
        }
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

    public void ForceRedraw()
    {
        _mapControl.ForceRedraw = true;
    }

    private IList<IGameCommand> SetupCommands(Game game)
    {
        var commandInterface = typeof(IGameCommand);
        var improvementCommand = typeof(ImprovementOrder);
        var args = new object[] { this };
        var improvements = game.TerrainImprovements.Values;
        var commands = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t != commandInterface && commandInterface.IsAssignableFrom(t) && !t.IsAbstract &&
                        t != improvementCommand)
            .Select(t => Activator.CreateInstance(t, args: args)).OfType<IGameCommand>()
            .Concat(improvements.Select(i => new ImprovementOrder(i, this, game))).ToList();

        return commands;
    }

    public void ShowPopup(string dialogName,
        Action<string, int, IList<bool>?, IDictionary<string, string>?>? handleButtonClick = null,
        List<TextBoxDefinition>? textBoxes = null)
    {
        var dialog = MainWindow.ActiveInterface.GetDialog(dialogName);
        if (dialog != null)
        {
            ShowDialog(new CivDialog(MainWindow, dialog, new Point(0, 0), handleButtonClick, textBoxDefs: textBoxes),
                stack: true);
        }
    }
}