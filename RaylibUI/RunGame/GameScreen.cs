using System.Runtime.CompilerServices;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Raylib_cs;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUI.RunGame.GameControls.Mapping;
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

        _menu = new GameMenu(this);
        _menu.GetPreferredWidth();
        SetupOrders(game);

        Moving = new MovingPieces(this);
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

    private void SetupOrders(Game game)
    {
        var improvements = game.TerrainImprovements.Values;

        var orderText = MenuLoader.For("ORDERS");

        Orders = improvements.Select(i =>
            new ImprovementOrder(i, this, game, orderText.FirstOrDefault(mi => string.Equals(mi.Shortcut, i.Shortcut, StringComparison.InvariantCultureIgnoreCase)))).Cast<Order>()
               .ToList();

        Orders.Add(new BuildCity(this, orderText.First(mi => mi.Shortcut == "b").MenuText, game));
        Orders.Add(new PillageOrder(this, orderText.First(mi => mi.Shortcut == "Shift+P").MenuText, game));
        Orders.Add(new FortifyOrder(this, orderText.Last(mi => mi.Shortcut == "f").MenuText, game));
        Orders.Add(new SkipOrder(this, orderText.First(mi => mi.Shortcut == "SPACE").MenuText, game));
        Orders.Add(new WaitOrder(this, orderText.First(mi => mi.Shortcut == "w").MenuText, game));
        Orders.Add(new UnloadOrder(this, orderText.First(mi => mi.Shortcut == "u").MenuText));
        Orders.Add(new SleepOrder(this, orderText.First(mi => mi.Shortcut == "s").MenuText, game));
        Orders.Add(new GotoOrder(this, orderText.First(mi => mi.Shortcut == "g").MenuText, game));

        UpdateOrders(game.ActiveTile, game.ActiveUnit);
    }

    public void UpdateOrders(Tile activeTile, Unit activeUnit)
    {
        //_ordersMenu.Items.Clear();
        Orders.ForEach(o => o.Update(activeTile, activeUnit));
        //var groupedOrders = Orders.Select(o => o.Update(activeTile, activeUnit)).GroupBy(o => o.Group);

        //foreach (var groupedOrder in groupedOrders)
        //{
        //    if (_ordersMenu.Items.Count > 0)
        //    {
        //        _ordersMenu.Items.Add(new SeparatorMenuItem());
        //    }
        //    _ordersMenu.Items.AddRange(groupedOrder.OrderBy(o => o.ActivationCommand).Select(o => o.Command));
        //}

        // &Orders
        //     &Build New City|b
        // Build &Road|r
        // Build &Irrigation|i
        // Build &Mines|m
        // Transform to ...|o
        // Build &Airbase|e
        // Build &Fortress|f
        // Automate Settler|k
        // Clean Up &Pollution|p
        //     &Pillage|Shift+P
        //     &Unload|u
        //     &Go To|g
        //     &Paradrop|p
        // Air&lift|l
        //     &Set &Home City|h
        //     &Fortify|f
        //     &Sleep|s
        //     &Disband|Shift+D
        //     &Activate Unit|a
        //     &Wait|w
        // S&kip Turn|SPACE
        // End Player Tur&n|Ctrl+N

        // var BuildRoadCommand = new Command { MenuText = "Build Road", Shortcut = Keys.R };
        // var BuildIrrigationCommand = new Command { MenuText = "Build Irrigation", Shortcut = Keys.I };
        // var BuildMinesCommand = new Command { MenuText = "Build Mines", Shortcut = Keys.M };
        //var CleanPollutionCommand = new Command { MenuText = "Clean Up Pollution", Shortcut = Keys.P };
        //var PillageCommand = new Command { MenuText = "Pillage", Shortcut = Keys.Shift | Keys.P };
        //var UnloadCommand = new Command { MenuText = "Unload", Shortcut = Keys.U };
        //var GoToCommand = new Command { MenuText = "Go To", Shortcut = Keys.G };
        //var ParadropCommand = new Command { MenuText = "Paradrop", Shortcut = Keys.P };
        //var AirliftCommand = new Command { MenuText = "Airlift", Shortcut = Keys.L };
        //var GoHomeToNearestCityCommand = new Command { MenuText = "Go Home To Nearest City", Shortcut = Keys.H };
        //var FortifyCommand = new Command { MenuText = "Fortify", Shortcut = Keys.F };
        //var SleepCommand = new Command { MenuText = "Sleep", Shortcut = Keys.S };
        //var DisbandCommand = new Command { MenuText = "Disband", Shortcut = Keys.Shift | Keys.D };
        //var ActivateUnitCommand = new Command { MenuText = "Activate Unit", Shortcut = Keys.A };
        // var WaitCommand = new Command { MenuText = "Wait", Shortcut = Keys.W };
        //var SkipTurnCommand = new Command { MenuText = "Skip Turn", Shortcut = Keys.Space };
        //var EndPlayerTurn = new Command { MenuText = "End Player Turn", Shortcut = Keys.Control | Keys.N };

        //_ordersMenu.Items.AddRange(new MenuItem[]
        //{
        //        new SeparatorMenuItem(), GoToCommand,
        //        ParadropCommand, AirliftCommand, GoHomeToNearestCityCommand,
        //        new SeparatorMenuItem(), DisbandCommand, ActivateUnitCommand,
        //        new SeparatorMenuItem(), EndPlayerTurn
        //});
    }
}