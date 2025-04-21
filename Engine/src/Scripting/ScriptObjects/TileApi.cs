using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Core.Units;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting;

public class TileApi(Tile tile, Game game)
{
    internal Tile BaseTile => tile;
    
    /// Returns the baseterrain object associated with the tile.
    public BaseTerrain baseTerrain
    {
        get => new(tile.Terrain, tile.Z);
        set
        {
            if (value is not null)
            {
                var terrain = value.Terrain;
                tile.Terrain = terrain;
            }
        }
    }
    
    /// <summary>
    ///Returns the city at the tile's location, or `nil` if there's no city there.
    /// </summary>
    public City city => tile.CityHere;

    /// <summary>
    /// Returns the tile's defender.
    /// </summary>
    public Tribe defender => tile.UnitsHere.Count > 0 ? new Tribe(tile.UnitsHere[0].Owner) : null;

    /// <summary>
    /// Returns the tile's fertility.
    /// </summary>
    public decimal fertility
    {
        get =>
            tile.Fertility;
        set => tile.Fertility = value;
    }

    /// <summary>
    /// Returns `true` if the tile would have a shield when changed to grassland, `false` otherwise.
    /// </summary>
    public bool grasslandShield => tile.HasShield;

    /// <summary>
    /// Returns `true` if the tile has a goodie hut, `false` otherwise.
    /// </summary>
    public bool hasGoodieHut => tile.HasGoodieHut;


    /// <summary>
    /// Returns the tile's improvements (bitmask).
    /// </summary>
    public int improvements { get; set; }

    /// <summary>
    /// Returns the tile's landmass index.
    /// </summary>
    public int landmass
    {
        get => tile.Island;
        set => tile.Island = value;
    }

    /// <summary>
    /// Returns the tribe owning the tile.
    /// </summary>
    public Tribe? owner
    {
        get => tile.Owner == -1 ? null : new Tribe(game.AllCivilizations[tile.Owner]);
        set
        {
            if (value != null) tile.Owner = value.Id;
        }
    }

    /// <summary>
    /// Returns `true` if the tile has a river, `false` otherwise.
    /// </summary>
    public bool river
    {
        get => tile.River;
        set => tile.River = value;
    }

    /// <summary>
    /// Returns the terrain object associated with the tile.
    /// </summary>
    public TerrainApi terrain => new TerrainApi(tile);
    
    public int terrainType
    {
        get => (int)tile.Terrain.Type;
        set => tile.Terrain = game.Rules.Terrains[tile.Z][value];
    }

    /// <summary>
    /// Returns an iterator yielding all units at the tile's location.
    /// </summary>
    public IEnumerable<Unit> units => tile.UnitsHere;

    /// <summary>
    /// Returns the tile's visibility for each tribe (bitmask).
    /// </summary>
    public int visibility
    {
        get => Utils.ToBitmask(tile.Visibility);
        set => tile.Visibility = Utils.FromBitmask(value);
    }

    /// <summary>
    /// Returns the tile's improvements as known by the given tribe (bitmask).
    /// </summary>
    public int[] visibleImprovements
    {
        get => game.ImprovementEncoder.EncodeKnowledge(tile.PlayerKnowledge, game.AllCivilizations.Count);
    }

    /// <summary>
    /// Returns the `x` coordinate of the tile.
    /// </summary>
    public int x => tile.X;

    /// <summary>
    /// Returns the `y` coordinate of the tile.
    /// </summary>
    public int y => tile.Y;

    /// <summary>
    /// Returns the `z` coordinate of the tile (map number).
    /// </summary>
    public int z => tile.Z;

    public override bool Equals(object? obj)
    {
        if (obj is not TileApi other)
        {
            return false;
        }
        return other.BaseTile == BaseTile;
    }

    public override int GetHashCode()
    {
        return BaseTile.GetHashCode();
    }
}

public class TerrainApi(Tile tile)
{
}

public class Tribe  
{
    public Tribe(Civilization civ)
    {
        throw new System.NotImplementedException();
    }

    public int Id { get; set; }
}

public class BaseTerrain
{
    private readonly int _map;

    public BaseTerrain(Terrain terrain, int map)
    {
        _map = map;
        Terrain = terrain;
    }

    public Terrain Terrain { get; }
}