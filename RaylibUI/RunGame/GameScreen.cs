using System.Runtime.CompilerServices;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;
using Model.Core;
using Model.Dialog;
using Model.Images;
using Model.Interface;
using Model.Menu;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Transformations;
using RaylibUI.RunGame.Commands.Orders;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Menu;
using RaylibUI.RunGame.GameModes;
using RaylibUI.RunGame.GameModes.Orders;
using Raylib_CSharp.Interact;

namespace RaylibUI.RunGame;

public class GameScreen : BaseScreen
{
    public Main Main { get; }
    public IGame Game { get; }
    public Sound Soundman { get; }

    private readonly MinimapPanel _minimapPanel;
    private readonly MapControl _mapControl;
    private readonly StatusPanel _statusPanel;
    private readonly LocalPlayer _player;
    private readonly GameMenu _menu;
    private bool _ToTPanelLayout;

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
    
    public int Zoom     // -7 (min) ... 8 (max), 0=std.
    {
        get => _zoom;
        set => _zoom = Math.Max(Math.Min(value, 8), -7);
    }
    public TileTextureCache TileCache { get; }
    
    public LocalPlayer Player => _player;

    public StatusPanel StatusPanel => _statusPanel;
    public bool ToTPanelLayout => _ToTPanelLayout;
    public GameMenu MenuBar => _menu;
    public IGameMode Moving { get; }
    public IGameMode ViewPiece { get; }

    private const int MiniMapWidth = 262;
    private readonly int _miniMapHeight;
    private IGameMode _activeMode;
    
    private CivDialog _currentPopupDialog;
    private Action<string,int,IList<bool>?,IDictionary<string,string>?>? _popupClicked;

    private int _zoom;

    public event EventHandler<MapEventArgs>? OnMapEvent = null;

    public GameScreen(Main main, IGame game, Sound soundman, IDictionary<string, string?>? viewData): base(main)
    {
        TileCache = new TileTextureCache(this);
        Main = main;
        Game = game;
        Soundman = soundman;

        if (viewData != null && viewData.TryGetValue("Zoom", out var value) && int.TryParse(value, out var zoom))
        {
            //Use the property to ensure range validation is run
            Zoom = zoom;
        }
        
        Moving = new MovingPieces(this);
        ViewPiece = new ViewPiece(this);
        Processing = new ProcessingMode(this);

        var civ = game.GetPlayerCiv;
        _player = new LocalPlayer(this, civ);
        VisibleCivId = _player.Civilization.Id;
        game.ConnectPlayer(_player);

        _ToTPanelLayout = false;
        _miniMapHeight = Math.Max(100, game.Maps[_player.ActiveTile.Z].YDim) + 38 + 11;

        var commands = SetupCommands(game);
        var menuElements = main.ActiveInterface.ConfigureGameCommands(commands);
        _menu = new GameMenu(this, menuElements);
        _menu.GetPreferredWidth();

        if (Game.GetActiveCiv == Game.GetPlayerCiv)
        {
            ActiveMode = _player.ActiveUnit is not {MovePoints: > 0} ? ViewPiece : Moving;
        }
        else
        {
            ActiveMode = Processing;
        }
        
        var width = Window.GetScreenWidth();
        var height = Window.GetScreenHeight();
        
        var menuHeight = _menu.GetPreferredHeight();
        
        _statusPanel = new StatusPanel(this, game);
        _minimapPanel = new MinimapPanel(this, game);

        var mapWidth = width - MiniMapWidth;
        var mapRect = new Rectangle(0, menuHeight, mapWidth, height - menuHeight);
        if (_ToTPanelLayout)
        {
            mapWidth = width;
            mapRect = new Rectangle(0, menuHeight + _miniMapHeight, mapWidth, height - menuHeight - _miniMapHeight);
        }
        _mapControl = new MapControl(this, game, mapRect);

        // The order of these is important as MapControl can overdraw so must be drawn first
        Controls.Add(_mapControl);
        Controls.Add(_menu);
        Controls.Add(_minimapPanel);
        Controls.Add(_statusPanel);

        var lookup = new Dictionary<Shortcut, IList<IGameCommand>>();
        foreach (var command in commands)
        {
            foreach (var shortcut in command.ActivationKeys)
            {
                if (shortcut.Equals(Shortcut.None)) continue;
                
                if (lookup.ContainsKey(shortcut))
                {
                        lookup[shortcut].Add(command);
                }
                else
                {
                    lookup.Add(shortcut, new List<IGameCommand> { command});
                }
            }
        }

        GameCommands = lookup;
    }

    private Dictionary<Shortcut, IList<IGameCommand>> GameCommands { get; }

    private void TryExecuteCommand(IList<IGameCommand> commands)
    {
        foreach (var command in commands)
        {
            command.Update();
        }

        var activeCommand = commands.MinBy(c => c.Status);

        if (activeCommand == null || activeCommand.Status == CommandStatus.Invalid) return;

        if (activeCommand.Status <= CommandStatus.Default)
        {
            activeCommand.Action();
        }
        else
        {
            ShowPopup(activeCommand.ErrorDialog, dialogImage: activeCommand.ErrorImage);
        }
    }

    public ProcessingMode Processing { get; }
    public Map CurrentMap => Player.ActiveTile.Map;

    /// <summary>
    /// The CivId of the currently displayed map normally the same as the player civId but can be changed via reveal map
    /// </summary>
    public int VisibleCivId { get; set; }

    public MapControl MapControl => _mapControl;

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key is KeyboardKey.LeftAlt or KeyboardKey.RightAlt)
        {
            Focused = MenuBar.Children!.First();
            return;
        }
        var command = new Shortcut(key, Input.IsKeyDown(KeyboardKey.RightShift) ||
                                        Input.IsKeyDown(KeyboardKey.LeftShift)
            , Input.IsKeyDown(KeyboardKey.LeftControl) ||
              Input.IsKeyDown(KeyboardKey.RightControl)
        );

        if (!ActiveMode.HandleKeyPress(command) && GameCommands.ContainsKey(command))
        {
            TryExecuteCommand(GameCommands[command]);
        }
        
    }

    public override void InterfaceChanged(Sound man)
    {
        //Some of the initialization logic should be here.... not sure exactly what currently this shouldn't ce called
    }

    public override void Resize(int width, int height)
    {
        GetPanelBounds(width, height);
        base.Resize(width, height);
    }

    private void GetPanelBounds(int width, int height)
    {
        _menu.GetPreferredWidth();
        var menuHeight = _menu.GetPreferredHeight();
        var mapWidth = width - MiniMapWidth;
        var mapControlRect = new Rectangle(0, menuHeight, mapWidth, height - menuHeight);
        var minimapRect = new Rectangle(mapWidth, menuHeight, MiniMapWidth, _miniMapHeight);
        var statusRect = new Rectangle(mapWidth, _miniMapHeight + menuHeight, MiniMapWidth, height - _miniMapHeight - menuHeight);
        if (_ToTPanelLayout)
        {
            mapWidth = width;
            mapControlRect = new Rectangle(0, menuHeight + _miniMapHeight, mapWidth, height - menuHeight - _miniMapHeight);
            minimapRect = new Rectangle(mapWidth - MiniMapWidth, menuHeight, MiniMapWidth, _miniMapHeight);
            statusRect = new Rectangle(0, menuHeight, mapWidth - MiniMapWidth, _miniMapHeight);
        }
        _menu.Bounds = new Rectangle(0, 0, width, menuHeight);
        _mapControl.Bounds = mapControlRect;
        _minimapPanel.Bounds = minimapRect;
        _statusPanel.Bounds = statusRect;
    }

    public void ShowCityDialog(string dialog, City city, IList<string>? replaceStrings = null,
        IList<int>? replaceNumbers = null)
    {
        replaceStrings ??= new List<string>
            { city.Name, city.ItemInProduction.GetDescription(), city.Owner.Adjective, Labels.For(LabelIndex.builds) };
        ShowPopup(dialog,
            handleButtonClick: (s, i, arg3, arg4) =>
            {
                if (i == 0)
                {
                    ShowCityWindow(city);
                }
            },
            replaceNumbers: replaceNumbers,
            options: new List<string> { Labels.For(LabelIndex.ZoomToCity), Labels.For(LabelIndex.Continue) },
            replaceStrings: replaceStrings);
    }

    public void ShowCityWindow(City city)
    {
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
            Game.ActivePlayer.ActiveTile = tile;
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

        unit.Order = (int)OrderType.NoOrders; // Always clear order when clicked, no matter if the unit is activated
        ActiveMode = Moving;
        return true;
    }

    public void ForceRedraw()
    {
        _mapControl.ForceRedraw = true;
    }

    private IList<IGameCommand> SetupCommands(IGame game)
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
        IList<int>? replaceNumbers = null,
        IList<bool>? checkboxStates = null,
        List<string>? options = null,
        List<TextBoxDefinition>? textBoxes = null,
        DialogImageElements? dialogImage = null,
        IList<string>? replaceStrings = null,
        ListBoxDefinition? listBox = null)
    {
        var popupBox = MainWindow.ActiveInterface.GetDialog(dialogName);
        if (popupBox != null)
        {
            popupBox.Options ??= options;
            _popupClicked = handleButtonClick;
            _currentPopupDialog = new CivDialog(MainWindow, popupBox, new Point(0, 0),
                ClosePopup, textBoxDefs: textBoxes, image: dialogImage, replaceStrings: replaceStrings,
                replaceNumbers: replaceNumbers, listBox: listBox, checkboxStates: checkboxStates);
            ShowDialog(_currentPopupDialog, stack: true);
        }
    }

    private void ClosePopup(string arg1, int arg2, IList<bool>? arg3, IDictionary<string, string>? arg4)
    {
        CloseDialog(_currentPopupDialog);
        _popupClicked?.Invoke(arg1, arg2, arg3, arg4);
    }

    public void ToggleMapLayout()
    {
        _ToTPanelLayout = !_ToTPanelLayout;
        Resize(Window.GetScreenWidth(), Window.GetScreenHeight());
    }

    public void TurnStarting(int turnNumber)
    {
        _statusPanel.Update();
    }
}