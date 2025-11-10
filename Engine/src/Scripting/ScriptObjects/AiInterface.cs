using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.UnitActions;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Units;
using Neo.IronLua;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.ScriptObjects;

/// <summary>
/// Provides the AI interface exposed to Lua scripts for controlling AI players.
/// This class bridges C# game logic with Lua AI scripts.
/// </summary>
public class AiInterface(AiPlayer player, Game game, StringBuilder log)
{
    private readonly Dictionary<string, Func<AiInterface, LuaTable, object>> _events= new();
    
    /// <summary>
    /// Access to the game's random number generator for AI decision-making
    /// </summary>
    public FastRandom Random => game.Random;

    /// <summary>
    /// The civilization this AI controls
    /// </summary>
    public Civilization civ => player.Civilization;

    /// <summary>
    /// The difficulty level of this AI player
    /// </summary>
    public int difficulty => player.DifficultyLevel;

    /// <summary>
    /// Checks if an event handler has been registered for the given event name
    /// </summary>
    /// <param name="eventName">The name of the event to check</param>
    /// <returns>True if the event has a registered handler</returns>
    public bool HasEvent(string eventName)
    {
        return _events.ContainsKey(eventName);
    }

    /// <summary>
    /// Invokes the registered event handler for the specified event
    /// </summary>
    /// <param name="eventName">The name of the event to call</param>
    /// <param name="args">Arguments to pass to the event handler</param>
    /// <returns>The result from the event handler, or null if no handler exists or an error occurs</returns>
    public object? Call(string eventName, LuaTable args)
    {
        try
        {
            return HasEvent(eventName) ? _events[eventName](this, args) : null;
        }
        catch (LuaException e)
        {
            log.AppendLine("Error running: " + eventName);
            log.AppendLine(e.Message);
            return null;
        }
    }

    /// <summary>
    /// Registers a Lua function to handle a specific AI event
    /// </summary>
    /// <param name="eventName">The name of the event (e.g., "Turn_Start", "Unit_Orders_Needed")</param>
    /// <param name="callback">The Lua function to call when the event occurs</param>
    public void RegisterEvent(string eventName, Func<AiInterface, LuaTable, object> callback)
    {
        _events.Add(eventName, callback);
    }

    /// <summary>
    /// Finds the nearest city to a given tile
    /// </summary>
    /// <param name="tile">The tile to search from</param>
    /// <param name="inRadiusOnly">If true, only searches within a 2-tile radius; otherwise searches entire map</param>
    /// <returns>The nearest city, or null if none found</returns>
    public City? GetNearestCity(Tile tile, bool inRadiusOnly = false)
    {
        // If there's a city on this tile, return it immediately
        if(tile.CityHere != null) return tile.CityHere;
        
        if (inRadiusOnly)
        {
            // Search only immediate neighbors and second ring
            return tile.Neighbours().FirstOrDefault(t => t.CityHere != null)?.CityHere ??
                   tile.SecondRing().FirstOrDefault(t => t.CityHere != null)?.CityHere;
        }
        
        // Search all cities on the map, ordered by distance
        return game.AllCities.OrderBy(c => Utilities.DistanceTo(tile, c.Location)).FirstOrDefault();
    }

    /// <summary>
    /// Evaluates whether a settler should move to a more fertile location or build a city
    /// </summary>
    /// <param name="currentTile">The settler's current location</param>
    /// <param name="unit">The settler unit</param>
    /// <returns>A move action to a more fertile tile, or a build city action</returns>
    public UnitAction CheckFertility(Tile currentTile, Unit unit)
    {
        // Look for nearby tiles with better fertility
        var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
            .Where(n => n.Fertility > currentTile.Fertility).OrderByDescending(n => n.Fertility)
            .FirstOrDefault();
            
        if (moreFertile != null)
        {
            // Move to the more fertile location
            return new MoveAction(unit, moreFertile, game);
        }
        
        // Current location is best, build a city here
        return new BuildCityAction(unit,game);
    }

    /// <summary>
    /// Selects a random tile from the map, optionally restricted to visible tiles
    /// </summary>
    /// <param name="args">Lua table with optional "global" parameter (bool) - if true, includes all tiles; if false, only visible tiles</param>
    /// <returns>A randomly selected tile</returns>
    public TileApi RandomTile(LuaTable args)
    {
        var tiles = game.Maps.SelectMany(m => m.Tile.OfType<Tile>());
        var global = args.ContainsKey("global") && (bool)args["global"];
        
        if (!global)
        {
            // Filter to only tiles visible to this civilization
            tiles = tiles.Where(t => t.Visibility[civ.Id]);
        }
        
        var tile = Random.ChooseFrom(tiles.ToList());

        return new TileApi(tile, game);
    }

    /// <summary>
    /// Performs a breadth-first search to find the nearest enemy unit or city
    /// </summary>
    /// <param name="args">Lua table with parameters:
    ///   - tile (TileApi, required): Starting tile for the search
    ///   - distance (int, optional): Maximum search distance (0 or -1 for unlimited, default 1)
    ///   - same_landmass (bool, optional): If true, restricts search to same landmass (default false)
    /// </param>
    /// <returns>The tile containing the nearest enemy, or null if none found</returns>
    public TileApi? NearestEnemy(LuaTable args)
    {
        var tile = args.ContainsKey("tile") ? args["tile"] as TileApi : null;
        if(tile == null) return null;
        
        var distance = (args.ContainsKey("distance") ? Convert.ToInt32(args["distance"]) : 1);
        var same_landmass = (args.ContainsKey("same_landmass") ? args["same_landmass"] as bool? : false) ?? false;

        var land = tile.BaseTile.Island;
        var isOcean = tile.BaseTile.Terrain.Type  == TerrainType.Ocean;
        var ourId = civ.Id;
        var seen = new HashSet<Tile> { tile.BaseTile };
        var candidates = new List<Tuple<Tile, int>> { new(tile.BaseTile, 0) };
        
        // Breadth-first search outward from starting tile
        while (candidates.Count > 0)
        {
            var (currentTile, currentDistance) = candidates.First();
            candidates.Remove(candidates.First());
            
            // Check if this tile contains an enemy city or unit
            if ((currentTile.CityHere != null && currentTile.CityHere.OwnerId != ourId) ||
                (currentTile.UnitsHere.Count > 0 && currentTile.UnitsHere[0].Owner.Id != ourId))
            {
                return new TileApi(currentTile, game);
            }
            
            // Stop expanding if we've reached the distance limit
            if (currentDistance >= distance && distance > 0) continue;
            
            var tiles = currentTile.Neighbours().Where(n =>!seen.Contains(n));
            
            if (same_landmass)
            {
                if (isOcean)
                {   
                    // For ocean tiles, include coastal tiles when finding enemies but don't expand from them
                    if (currentTile.Island != land) continue;
                }
                else
                {
                    // For land tiles, only include tiles on the same landmass
                    tiles = tiles.Where(t => t.Island == land);
                }
            }

            // Add all valid neighbors to the search queue
            foreach (var neighbour in tiles)
            {
                seen.Add(neighbour);
                candidates.Add(new Tuple<Tile, int>(neighbour, currentDistance+1));
            }
        }

        return null;
    }

    /// <summary>
    /// Performs a breadth-first search to find the nearest friendly unit
    /// </summary>
    /// <param name="args">Lua table with parameters:
    ///   - tile (TileApi, required): Starting tile for the search
    ///   - distance (int, optional): Maximum search distance (0 or -1 for unlimited, default 1)
    ///   - same_landmass (bool, optional): If true, restricts search to same landmass (default true)
    ///   - unit_type (int, optional): If >= 0, only finds friendly units of a DIFFERENT type; if -1, finds any friendly unit
    ///   - in_horde (bool, optional): If true, only finds friendly units in a horde (default false)
    /// </param>
    /// <returns>The tile containing the nearest friendly unit, or null if none found</returns>
    public TileApi? NearestFriend(LuaTable args)
    {
        var tile = args.ContainsKey("tile") ? args["tile"] as TileApi : null;
        if(tile == null) return null;
        
        var distance = (args.ContainsKey("distance") ? Convert.ToInt32(args["distance"]) : 1);
        
        var same_landmass = (args.ContainsKey("same_landmass") ? args["same_landmass"] as bool? : true) ?? true;

        var unit_type = args.ContainsKey("unit_type") ? Convert.ToInt32(args["unit_type"]) : -1;
        
        var inHorde = args.ContainsKey("in_horde") && (bool)args["in_horde"];

        var land = tile.BaseTile.Island;
        var isOcean = tile.BaseTile.Terrain.Type == TerrainType.Ocean;
        var ourId = civ.Id;
        var seen = new HashSet<Tile> { tile.BaseTile };
        var candidates = new List<Tuple<Tile, int>> { new(tile.BaseTile, 0) };
        
        // Breadth-first search outward from starting tile
        while (candidates.Count > 0)
        {
            var (currentTile, currentDistance) = candidates.First();
            candidates.Remove(candidates.First());
            
            // Check if there's a friendly unit on this tile
            if (currentTile.UnitsHere.Count > 0 && currentTile.UnitsHere[0].Owner.Id == ourId)
            {
                if (inHorde)
                {
                    if (currentTile.UnitsHere.Any(u => u.ExtendedData.ContainsKey("horde")))
                    {
                        return new TileApi(currentTile, game);
                    }
                }
                else
                {
                    if (unit_type >= 0)
                    {
                        // If unit_type is specified, check if any unit has a different type
                        if (currentTile.UnitsHere.Any(u => u.Type != unit_type))
                        {
                            return new TileApi(currentTile, game);
                        }
                    }
                    else
                    {
                        // No unit type specified, any friendly unit counts
                        return new TileApi(currentTile, game);
                    }
                }
            }
            
            // Stop expanding if we've reached the distance limit
            if (currentDistance >= distance && distance > 0) continue;
            
            var tiles = currentTile.Neighbours().Where(n => !seen.Contains(n));
            
            if (same_landmass)
            {
                if (isOcean)
                {
                    // For ocean tiles, include coastal tiles when finding friends but don't expand from them
                    if (currentTile.Island != land) continue;
                }
                else
                {
                    // For land tiles, only include tiles on the same landmass
                    tiles = tiles.Where(t => t.Island == land);
                }
            }

            // Add all valid neighbors to the search queue in random order
            foreach (var neighbour in Random.Shuffle(tiles))
            {
                seen.Add(neighbour);
                candidates.Add(new Tuple<Tile, int>(neighbour, currentDistance + 1));
            }
        }

        return null;
    }
    
    /// <summary>
    /// Generates all possible actions a unit can take from its current position
    /// </summary>
    /// <param name="unit">The unit to get moves for</param>
    /// <returns>A Lua table containing all possible unit actions (move, attack, fortify, etc.)</returns>
    public LuaTable GetPossibleMoves(UnitApi unittoMove)
    {
        var unit = unittoMove.BaseUnit;
        // Start with the "do nothing" action
        var result = new LuaTable { new NothingAction(unit, game) };

        if (unit.CurrentLocation == null) return result;
        
        // Add fortify option if applicable
        if (UnitFunctions.CanFortifyHere(unit, unit.CurrentLocation))
        {
            result.Add(new FortifyAction(unit, game));
        }
        
        // Add build city option if applicable
        if (unit.AiRole == AiRoleType.Settle)
        {
            result.Add(new BuildCityAction(unit, game));       
        }

        // Generate all possible movement and combat actions
        foreach (var possibleMove in MovementFunctions.GetPossibleMoves(unit.CurrentLocation, unit))
        {
             if (possibleMove.UnitsHere.Count == 0 || possibleMove.UnitsHere[0].Owner == unit.Owner)
            {
                // Tile is empty or contains friendly units
                if (unit.Domain == UnitGas.Sea && possibleMove.Terrain.Type != TerrainType.Ocean)
                {
                    result.Add(new UnloadAction(unit, possibleMove, game));
                }else if (possibleMove.CityHere != null && possibleMove.CityHere.Owner != unit.Owner)
                {
                    // Can capture an empty enemy city
                    result.Add(new CaptureAction(unit, possibleMove.CityHere, game));
                }

                // Can move to this tile
                result.Add(new MoveAction(unit, possibleMove, game));
            }
            else
            {
                // Tile contains enemy units - add attack action
                if (unit.Domain == UnitGas.Sea && possibleMove.Terrain.Type != TerrainType.Ocean)
                {
                    // Is a seabourne invasion
                    if (unit.CarriedUnits.Any(u => u.CanMakeAmphibiousAssaults))
                    {
                        result.Add(new InvasionAction(unit, possibleMove, game));
                    }
                    // Invalid move so don't add
                }
                else
                {
                    result.Add(new AttackAction(unit, possibleMove, game));
                }
            }
        }
        
        return result;
    }

    public TileApi? LocationTowards(LuaTable args)
    {
        var target = args.ContainsKey("target") ? args["target"] as TileApi : null;
        var location = args.ContainsKey("location") ? args["location"] as TileApi : null;
        var speed = args.ContainsKey("speed") ? Convert.ToInt32(args["speed"]) / game.Rules.Cosmic.MovementMultiplier : 1;
        if (target == null || location == null) return null;

        var path = Path.CalculatePathBetween(game, location.BaseTile, target.BaseTile,
            location.BaseTile.Terrain.Type == TerrainType.Ocean ? UnitGas.Sea : UnitGas.Ground, 
            speed, civ, false,
            false, false);
        if (path == null || path.Tiles.Length <= speed)
        {
            return target;
        }
        return new TileApi(path.Tiles[speed], game);
    }

    public double Distance(TileApi from, TileApi to)
    {
        var dist= Utilities.DistanceTo(from.BaseTile, to.BaseTile);
        return dist;
    }
}