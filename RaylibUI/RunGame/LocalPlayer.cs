using Civ2engine;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Model.Controls;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Mapping;
using Model.Core.Units;
using Model.Events;
using Model.Core.Player;
using Model.Core.Production;
using Model.Images;

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

    public Tile ActiveTile { get; set; } = null!;

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
        _gameScreen.ShowCityDialog("INHOCK", city, [city.Name, cityImprovement.Name],
            [cityImprovement.Cost]);
    }

    public void SelectNewAdvance(List<Advance> researchPossibilities)
    {
        var activeInterface = _gameScreen.Main.ActiveInterface;
        _gameScreen.ShowPopup("RESEARCH", (s, i, arg3, arg4) =>
            {
                if (researchPossibilities.Count == 0)
                {
                    return;
                }

                var selectedIndex = Math.Clamp(i, 0, researchPossibilities.Count - 1);
                Civilization.ReseachingAdvance = researchPossibilities[selectedIndex].Index;
                if (Civilization.ScienceRate <= 0)
                {
                    Civilization.ScienceRate = 60;
                    Civilization.TaxRate = Math.Min(Civilization.TaxRate, 40);
                }
            }, replaceStrings: [activeInterface.GetScientistName(Civilization.Epoch)],
            listBox: new ListboxDefinition
            {
                VerticalScrollbar = false,
                ImageShift = false,
                Rows = Math.Min(10, researchPossibilities.Count),
                Looks = new ListboxLooks
                {
                    Font = activeInterface.Look.DefaultFont,
                    FontSize = 20,
                    TextColorFront = Raylib_CSharp.Colors.Color.Black,
                    TextColorShadow = Raylib_CSharp.Colors.Color.Blank,
                    TextShadowOffset = System.Numerics.Vector2.Zero,
                    SelectedTextFont = activeInterface.Look.DefaultFont,
                    SelectedTextBackgroundColor = new Raylib_CSharp.Colors.Color(107, 107, 107, 255),
                    SelectedTextColorFront = Raylib_CSharp.Colors.Color.White,
                    SelectedTextColorShadow = Raylib_CSharp.Colors.Color.Black
                },
                Groups = researchPossibilities.Select(a => new ListboxGroup
                {
                    Elements = [new() { Icon = GetClassicAdvanceIcon(a), Width = 2 * 36 + 2 },
                                new() { Text = a.Name, VerticalAlignment = VerticalAlignment.Center } ],
                    Height = 36
                }).ToList()
            });
    }

    private static IImageSource GetClassicAdvanceIcon(Advance advance)
    {
        var x = 343 + advance.KnowledgeCategory * 37;
        var y = 211 + advance.Epoch * 21;
        return new BitmapStorage("icons", x, y, 36, 20);
    }

    public void CantProduce(City city, IProductionOrder? newItem)
    {
        _gameScreen.ShowCityDialog("BADBUILD", city);
    }

    public void CityProductionComplete(City city)
    {
        _gameScreen.ShowCityDialog("BUILT", city);
    }

    public IInterfaceCommands Ui { get; } = null!;
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

    public void UnitLost(Unit unit, Unit? killedBy)
    {
        //TODO: How do we use this
    }

    public void UnitsLost(List<Unit> deadUnits, Unit? killedBy)
    {
        //TODO: How do we use this
    }

    public void UnitMoved(Unit unit, Tile tileTo, Tile tileFrom)
    {
        OnUnitEvent?.Invoke(this, new MovementEventArgs(unit, tileFrom, tileTo));
    }

    public void CombatHappened(CombatEventArgs combatEventArgs)
    {
        OnUnitEvent?.Invoke(this, combatEventArgs);
    }

    public void MoveBlocked(Unit unit, BlockedReason blockedReason)
    {
        OnUnitEvent?.Invoke(this, new MovementBlockedEventArgs(unit, blockedReason));
    }

    public event EventHandler<UnitEventArgs>? OnUnitEvent;

    public void GoodyHutTriggered(Unit unit, GoodyHutOutcomeResult outcome)
    {
        var args = new GoodyHutOutcomeEventArgs(unit, outcome);
        OnUnitEvent?.Invoke(this, args);

        var popupName = outcome.OutcomeType switch
        {
            "Gold" => "SURPRISEMETALS",
            "Scrolls" => "SURPRISESCROLLS",
            "Tribe" => "SURPRISENOMADS",
            "Barbarians" => "SURPRISEBARB",
            "AbandonedVillage" => "SURPRISENOTHING",
            "Mercenaries" => "SURPRISEMERCS",
            _ => "GOODYHUT_DEFAULT"
        };

        _gameScreen.ShowPopup(popupName, replaceNumbers: [50]);
    }

    public void SelectTechFromConquest(List<Advance> techs)
    {
        var advance = _gameScreen.Game.Random.ChooseFrom(techs);
        _gameScreen.Game.GiveAdvance(advance.Index, Civilization);
        
        //TODO: Show popup
    }

    public void CityLost(City city)
    {
        //TODO: Show info ? is game over?
    }

    public void CityCaptured(City city)
    {
       //TODO: Show popup?? what does the game do here? 
    }
}
