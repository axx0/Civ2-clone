using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Scripting;
using Model.Core.Units;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting.ScriptObjects;

public class UnitApi
{
    private readonly Unit _unit;
    private readonly Game _game;

    public UnitApi(Unit unit, Game game)
    {
        _unit = unit;
        _game = game;
    }

    /// <summary>
    /// Returns the number of attacks spent by the unit (from the 'Attacks per turn' patch).
    /// </summary>
    public int attackSpent
    {
        get => _unit.AttacksSpent;
        set => _unit.AttacksSpent = value;
    }

    // /// <summary>
    // /// Returns the attributes of the unit (bitmask).
    // /// </summary>
    // public int attributes
    // {
    //     get => _unit.Attributes;
    //     set => _unit.Attributes = value;
    // }

    /// <summary>
    /// Returns the carrying unit if this unit is currently on board, null otherwise.
    /// The game shares the memory location of this field with gotoTile.x,
    /// so don't use this field if gotoTile is not null.
    /// </summary>
    public UnitApi? carriedBy
    {
        get => _unit.InShip != null ? new UnitApi(_unit.InShip, _game) : null;
        set => _unit.InShip = value?._unit;
    }

    /// <summary>
    /// Returns the damage taken by the unit in hitpoints.
    /// </summary>
    public int damage
    {
        get => _unit.HitPointsLost;
        set => _unit.HitPointsLost = value;
    }

    /// <summary>
    /// Returns the value of the 'domain-specific counter' of the unit.
    /// </summary>
    public int domainSpec
    {
        get => _unit.Counter;
        set => _unit.Counter = value;
    }

    /// <summary>
    /// Returns the tile the unit is moving to under the goto order, or null if it doesn't have the goto order.
    /// </summary>
    public TileApi? gotoTile
    {
        get => _unit.Order == (int)OrderType.GoTo && _unit.CurrentLocation.Map.IsValidTileC2(_unit.GoToX, _unit.GoToY) ? new TileApi(_unit.CurrentLocation.Map.TileC2(_unit.GoToX, _unit.GoToY), _game) : null;
        set
        {
            if (value?.BaseTile.Z == _unit.CurrentLocation.Z && (value.BaseTile.X != _unit.CurrentLocation.X || value.BaseTile.Y != _unit.CurrentLocation.Y))
            {
                _unit.Order = (int)OrderType.GoTo;
                _unit.GoToX = value.BaseTile.X;
                _unit.GoToY = value.BaseTile.Y;
            }
        }
    }

    /// <summary>
    /// Returns the number of hitpoints left. It is defined as unit.type.hitpoints - unit.damage.
    /// </summary>
    public int hitpoints => _unit.RemainingHitpoints;

    /// <summary>
    /// Returns the unit's home city, or null if it doesn't have one.
    /// </summary>
    public CityApi? homeCity
    {
        get => _unit.HomeCity != null ? new CityApi(_unit.HomeCity, _game) : null;
        set => _unit.HomeCity = value?.BaseCity;
    }


    /// <summary>
    /// Returns the unit's id.
    /// </summary>
    public int id => _unit.Id;

    /// <summary>
    /// Returns the unit's location.
    /// </summary>
    public TileApi location => new(_unit.CurrentLocation, _game);

    /// <summary>
    /// Returns the number of moves spent by the unit.
    /// </summary>
    public int moveSpent
    {
        get => _unit.MovePointsLost;
        set => _unit.MovePointsLost = value;
    }

    /// <summary>
    /// Returns the current order of the unit.
    /// </summary>
    public int order
    {
        get => _unit.Order;
        set => _unit.Order = value;
    }

    /// <summary>
    /// Returns the unit's owner.
    /// </summary>
    public Tribe owner
    {
        get => new(_unit.Owner);
        set => _unit.Owner = value.Civ;
    }

    /// <summary>
    /// Returns the unit's type.
    /// </summary>
    public UnitType type => new(_unit.TypeDefinition, _game);

    /// <summary>
    /// Returns the veteran status of the unit.
    /// </summary>
    public bool veteran
    {
        get => _unit.Veteran;
        set => _unit.Veteran = value;
    }

    // /// <summary>
    // /// Returns the unit visibility mask.
    // /// </summary>
    // public int visibility
    // {
    //     get => _unit.TypeDefinition..Visibility;
    //     set => _unit.Visibility = value;
    // }

    /// <summary>
    /// Activates a unit, clearing its orders, and, if it has a human owner and movement points left,
    /// selects it on the map.
    /// </summary>
    public void activate()
    {
        // TODO: implement 
    }

    /// <summary>
    /// Alias for civ.teleportUnit(unit, tile).
    /// </summary>
    /// <param name="tile">The destination tile</param>
    public void teleport(TileApi tile)
    {
        _unit.CurrentLocation = tile.BaseTile;
    }
}