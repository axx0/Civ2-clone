using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

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
            switch (value)
            {
                case null:
                    _activeUnit = null;
                    break;
                case { TurnEnded: false, Dead: false }:
                    _activeTile = value.CurrentLocation;
                    _activeUnit = value;
                    break;
                default:
#if DEBUG
                    //     throw new NotSupportedException("Tried to set ended unit to active");
#endif
                    break;
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

    public void SelectNewAdvance(Game game, List<Advance> researchPossibilities)
    {
        // var popup = _main.popupBoxList["RESEARCH"];
        // var dialog = new Civ2dialog(_main, popup, new List<string> { "wise men" },
        //     listbox: new ListboxDefinition { LeftText = researchPossibilities.Select(a => a.Name).ToList() });
        // dialog.ShowModal();
        // Civ.ReseachingAdvance = researchPossibilities[dialog.SelectedIndex].Index;
    }

    public void CantProduce(City city, ProductionOrder newItem)
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
}