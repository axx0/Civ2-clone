using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Neo.IronLua;
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.ScriptObjects;

public class AiInterface
{
    private readonly AiPlayer _player;
    private readonly Game _game;

    private readonly Dictionary<string, Func<AiInterface, LuaTable, object>> _events= new();
    
    public FastRandom Random => _game.Random;

    public AiInterface(AiPlayer player, Game game)
    {
        _player = player;
        _game = game;
    }
    
    public Civilization civ => _player.Civilization;

    public int difficulty => _player.DifficultyLevel;

    public bool HasEvent(string eventName)
    {
        return _events.ContainsKey(eventName);
    }

    public object? Call(string eventName, LuaTable args)
    {
        return HasEvent(eventName) ? _events[eventName](this, args) : null;
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


    public List<UnitAction> GetPossibleMoves(Unit unit)
    {
        var result = new List<UnitAction>
        {
            new NothingAction(unit)
        };
        if (unit.CurrentLocation != null)
        {
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
        }

        return result;
    }
}