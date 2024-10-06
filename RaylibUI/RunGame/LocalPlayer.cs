using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Advances;
using Model.Interface;

namespace RaylibUI.RunGame;

public class LocalPlayer : IPlayer
{
    private readonly GameScreen _gameScreen;

    public LocalPlayer(GameScreen gameScreen, Civilization civilization)
    {
        _gameScreen = gameScreen;
        Civilization = civilization;
    }

    public Civilization Civilization { get; }

    public Tile ActiveTile
    {
        get => _activeTile;
        set
        {
            if (_activeTile != value)
            {
                _activeTile = value;
            }
        }
    }

    private Unit? _activeUnit;
    
    private Tile _activeTile;

    public Unit? ActiveUnit
    {
        get { return _activeUnit; }
        set
        {
            if (value == null)
            {
                _activeUnit = null;
            }
            else if (value is { TurnEnded: false, Dead: false } && value.Owner == Civilization)
            {
                _activeTile = value.CurrentLocation;
                _activeUnit = value;
            }
            else
            {
#if DEBUG
                //     throw new NotSupportedException("Tried to set ended unit to active");
#endif
            }
        }
    }

    public void CivilDisorder(City city)
    {
        _gameScreen.ShowCityDialog("DISORDER", new[] { city.Name });
    }

    public void OrderRestored(City city)
    {
        _gameScreen.ShowCityDialog("RESTORED", new[] { city.Name });
    }

    public void WeLoveTheKingStarted(City city)
    {
        _gameScreen.ShowCityDialog("WELOVEKING", new[] { city.Name, city.Owner.LeaderTitle });
    }

    public void WeLoveTheKingCanceled(City city)
    {
        _gameScreen.ShowCityDialog("WEDONTLOVEKING", new[] { city.Name, city.Owner.LeaderTitle });
    }

    public void CantMaintain(City city, Improvement cityImprovement)
    {
        throw new NotImplementedException();
    }

    public void SelectNewAdvance(IGame game, List<Advance> researchPossibilities)
    {
        var activeInterface = _gameScreen.Main.ActiveInterface;
        _gameScreen.ShowPopup("RESEARCH", (s, i, arg3, arg4) =>
        {
            Civilization.ReseachingAdvance = researchPossibilities[i].Index;
        }, replaceStrings: new string [] { activeInterface.GetScientistName(Civilization.Epoch) },
            listBox: new ListBoxDefinition { Vertical = false, Entries  =  researchPossibilities.Select(a => new ListBoxEntry { Icon = activeInterface.GetAdvanceImage(a), LeftText = a.Name}).ToList() } );
    }

    public void CantProduce(City city, IProductionOrder? newItem)
    {
        ShowCityDialog(city, "BADBUILD");
    }

    public void CityProductionComplete(City city)
    {
        ShowCityDialog(city, "BUILT");
    }

    private void ShowCityDialog(City city, string dialogName)
    {
        // var popup = _main.popupBoxList[dialogName];
        // popup.Options ??= new List<string> { Labels.For(LabelIndex.ZoomToCity), Labels.For(LabelIndex.Continue) };
        // var dialog = new Civ2dialog(_main, popup,
        //     new List<string>
        //         { city.Name, city.ItemInProduction.GetDescription(), city.Owner.Adjective, Labels.For(LabelIndex.builds) });
        // dialog.ShowModal();
        // if (dialog.SelectedIndex == 0)
        // {
        //     _main.mapPanel.ShowCityWindow(city);
        // }
    }

    public IInterfaceCommands Ui { get; }
    public List<Unit> WaitingList { get; } = new();

    public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
    {
        var dialogKey = improvement.Levels[level].EnabledMessage;
        if (!string.IsNullOrWhiteSpace(dialogKey))
        {
            Ui.ShowDialog(dialogKey);
        }
    }

    public void MapChanged(List<Tile> tiles)
    {
        var t = tiles.SelectMany(t => t.Map.DirectNeighbours(t));

        var allTiles = tiles
            .Concat(tiles.SelectMany(t => t.Map.DirectNeighbours(t).Where(n => n.IsVisible(Civilization.Id))))
            .Distinct();
        foreach (var tile in allTiles)
        {
            _gameScreen.TileCache.Redraw(tile, Civilization.Id);
        }

        _gameScreen.ForceRedraw();
    }

    public void WaitingAtEndOfTurn()
    {
        _gameScreen.ActiveMode = _gameScreen.ViewPiece;
    }

    public void NotifyAdvanceResearched(int advance)
    {
        var activeInterface = _gameScreen.Main.ActiveInterface;
        _gameScreen.ShowPopup("CIVADVANCE",
            replaceStrings: new[]
            {
                Civilization.Adjective, activeInterface.GetScientistName(Civilization.Epoch),
                _gameScreen.Game.Rules.Advances[advance].Name
            });
    }
}