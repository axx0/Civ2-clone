using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Units;
using Neo.IronLua;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.ScriptObjects;

public class AiInterface
{
    private readonly AiPlayer _player;
    private readonly Game _game;
    private readonly StringBuilder _log;

    private readonly Dictionary<string, Func<AiInterface, LuaTable, object>> _events= new();
    
    public FastRandom Random => _game.Random;

    public AiInterface(AiPlayer player, Game game, StringBuilder log)
    {
        _player = player;
        _game = game;
        _log = log;
    }
    
    public Civilization civ => _player.Civilization;

    public int difficulty => _player.DifficultyLevel;

    public bool HasEvent(string eventName)
    {
        return _events.ContainsKey(eventName);
    }

    public object? Call(string eventName, LuaTable args)
    {
        try
        {
            return HasEvent(eventName) ? _events[eventName](this, args) : null;
        }
        catch (LuaException e)
        {
            _log.AppendLine("Error running: " + eventName);
            _log.AppendLine(e.Message);
            return null;
        }
    }

    public void RegisterEvent(string eventName, Func<AiInterface, LuaTable, object> callback)
    {
        _events.Add(eventName, callback);
    }

    public City? GetNearestCity(Tile tile, bool inRadiusOnly = false)
    {
        if(tile.CityHere != null) return tile.CityHere;
        if (inRadiusOnly)
        {
            return tile.Neighbours().FirstOrDefault(t => t.CityHere != null)?.CityHere ??
                   tile.SecondRing().FirstOrDefault(t => t.CityHere != null)?.CityHere;
        }
        //TODO: search from current location may be faster
        return _game.AllCities.OrderBy(c => Utilities.DistanceTo(tile, c)).FirstOrDefault();
    }

    public UnitAction CheckFertility(Tile currentTile, Unit unit)
    {
        var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
            .Where(n => n.Fertility > currentTile.Fertility).OrderByDescending(n => n.Fertility)
            .FirstOrDefault();
        if (moreFertile != null)
        {
            return new MoveAction(unit, moreFertile);
        }
        return new BuildCityAction(unit,_game);
    }

    public TileApi RandomTile(LuaTable args)
    {
        var tiles = _game.Maps.SelectMany(m => m.Tile.OfType<Tile>());
        var global = args.ContainsKey("global") && (bool)args["global"];
        if (!global)
        {
            tiles = tiles.Where(t => t.Visibility[civ.Id]);
        }
        
        var tile = Random.ChooseFrom(tiles.ToList());

        return new TileApi(tile, _game);
    }

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
        while (candidates.Count > 0)
        {
            var (currentTile, currentDistance) = candidates.First();
            candidates.Remove(candidates.First());
            if ((currentTile.CityHere != null && currentTile.CityHere.OwnerId != ourId) ||
                (currentTile.UnitsHere.Count > 0 && currentTile.UnitsHere[0].Owner.Id != ourId))
            {
                return new TileApi(currentTile, _game);
            }
            
            if (currentDistance >= distance && distance > 0) continue;
            
            var tiles =currentTile.Neighbours().Where(n =>!seen.Contains(n));
            if (same_landmass)
            {
                if (isOcean)
                {   
                    // for ocean include the coasts when finding enemies but don't add its neighbours
                    if (currentTile.Island != land) continue;
                }
                else
                {
                    tiles = tiles.Where(t => t.Island == land);
                }
            }

            foreach (var neighbour in tiles)
            {
                seen.Add(neighbour);
                candidates.Add(new Tuple<Tile, int>(neighbour, currentDistance+1));
            }
        }

        return null;
    }
    
    public LuaTable GetPossibleMoves(Unit unit)
    {
        var result = new LuaTable { new NothingAction(unit) };

        if (unit.CurrentLocation == null) return result;
        
        if (UnitFunctions.CanFortifyHere(unit, unit.CurrentLocation))
        {
            result.Add(new FortifyAction(unit));
        }

        foreach (var possibleMove in MovementFunctions.GetPossibleMoves(unit.CurrentLocation, unit))
        {
            if (possibleMove.UnitsHere.Count == 0 || possibleMove.UnitsHere[0].Owner == unit.Owner)
            {
                if (possibleMove.CityHere != null && possibleMove.CityHere.Owner != unit.Owner)
                {
                    result.Add(new CaptureAction(unit, possibleMove.CityHere));
                }

                result.Add(new MoveAction(unit, possibleMove));
            }
            else
            {
                result.Add(new AttackAction(unit, possibleMove));
            }
        }
        return result;
    }
}