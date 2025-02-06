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

    public Tile ActiveTile { get; set; }

    private Unit? _activeUnit;

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
                if (value.CurrentLocation != null) ActiveTile = value.CurrentLocation;
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
        _gameScreen.ShowCityDialog("DISORDER", city);
    }

    public void OrderRestored(City city)
    {
        _gameScreen.ShowCityDialog("RESTORED", city);
    }

    public void WeLoveTheKingStarted(City city)
    {
        _gameScreen.ShowCityDialog("WELOVEKING", city);
    }

    public void WeLoveTheKingCanceled(City city)
    {
        _gameScreen.ShowCityDialog("WEDONTLOVEKING", city);
    }

    public void CantMaintain(City city, Improvement cityImprovement)
    {
        _gameScreen.ShowCityDialog("INHOCK", city, new[] { city.Name, cityImprovement.Name },
            new[] { cityImprovement.Cost });
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
        _gameScreen.ShowCityDialog("BADBUILD", city);
    }

    public void CityProductionComplete(City city)
    {
        _gameScreen.ShowCityDialog("BUILT", city);
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
        // var t = tiles.SelectMany(t => t.Map.DirectNeighbours(t));

        var allTiles = tiles
            .Concat(tiles.SelectMany(t => t.Map.DirectNeighbours(t).Where(n => n.IsVisible(_gameScreen.VisibleCivId))))
            .Distinct();
        foreach (var tile in allTiles)
        {
            _gameScreen.TileCache.Redraw(tile, _gameScreen.VisibleCivId);
        }

        _gameScreen.ForceRedraw();
    }

    public void WaitingAtEndOfTurn(IGame game)
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

    public void FoodShortage(City city)
    {
        _gameScreen.ShowCityDialog("FOODSHORTAGE", city);
    }

    public void CityDecrease(City city)
    {
        _gameScreen.ShowCityDialog("DECREASE", city);
    }

    public void TurnStart(int turnNumber)
    {
        _gameScreen.TurnStarting(turnNumber);
    }

    public void SetUnitActive(Unit? unit, bool move)
    {
        ActiveUnit = unit;
        if (_gameScreen.Game.GetActiveCiv == this.Civilization)
        {
            _gameScreen.ActiveMode = _gameScreen.Moving;
        }
    }
}