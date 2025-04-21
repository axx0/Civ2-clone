using System;
using System.Text;
using System.Threading;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Scripting.UI;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Core.Units;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting
{
    public class CivScripts
    {
        private readonly Game _game;

        public CivScripts(StringBuilder log, Game game)
        {
            _game = game;
            ui = new UIScripts(log);
            scen = new ScenarioHooks(game);
            core = new AxxExtensions(game, log);
        }

        internal void Connect(IInterfaceCommands interfaceCommands)
        {
            ui.Connect(interfaceCommands);
        }
        
        public AxxExtensions core { get; }

        // ReSharper disable once IdentifierTypo
        public ScenarioHooks scen { get; }

        public UIScripts ui { get; }
        
        public Action<int> sleep = Thread.Sleep;
        
        // public void teleportUnit(object unitObj, object tileObject)
        // {
        //     if(tileObject is Tile tile)
        //     unit.CurrentLocation = tile;
        // };

        public CityImprovement? getImprovement(int index)
        {
            if(index < 0 || index >= _game.Rules.Improvements.Length) return null;
            return new CityImprovement(_game.Rules.Improvements[index]);
        }

        public Tech? getTech(int index)
        {
            if(index < 0 || index >= _game.Rules.Advances.Length) return null;
            return new Tech(_game.Rules.Advances,index);
        }

        public UnitType? getUnitType(int index)
        {
            if(index < 0 || index >= _game.Rules.UnitTypes.Length) return null;
            return  new UnitType(_game.Rules.UnitTypes[index], _game) ;
        }

        public TileApi? getTile(int x, int y, int z)
        {
            if (z < 0 || z >= _game.Maps.Count || !_game.Maps[z].IsValidTileC2(x,y)) return null;
            return new TileApi(_game.Maps[z].TileC2(x, y), _game);
        }

        public bool canEnter(object unit, object tileObject)
        {
            Tile tile;
            if (tileObject is Tile apiTile)
            {
                tile = apiTile;
            }else if (tileObject is Tile coreTile)
            {
                tile = coreTile;
            }
            else
            {
                return false;
            }

            return unit switch
            {
                UnitType ut => UnitFunctions.CanEnter(ut.BaseDefinition.Domain, tile),
                Unit u => UnitFunctions.CanEnter(u.Domain, tile),
                UnitDefinition d => UnitFunctions.CanEnter(d.Domain, tile),
                _ => false
            };
        }
    }
}