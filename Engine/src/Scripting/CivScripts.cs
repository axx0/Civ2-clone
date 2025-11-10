using System;
using System.Linq;
using System.Text;
using System.Threading;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Scripting.UI;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Core.Units;
using UnitType = Civ2engine.Scripting.ScriptObjects.UnitType;

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
            cosmic = new CosmicScripts(game);
        }

        public CosmicScripts cosmic { get; }

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
        
        public UnitApi createUnit(object unitType, object owner, object tileObject)
        {
            var definition =  unitType switch
            {
                UnitType ut => ut.BaseDefinition,
                UnitDefinition d => d,
                int i => _game.Rules.UnitTypes[i % _game.Rules.UnitTypes.Length],
                double i => _game.Rules.UnitTypes[(int)(i % _game.Rules.UnitTypes.Length)],
                string n => _game.Rules.UnitTypes.FirstOrDefault(u=>u.Name.Equals(n, StringComparison.InvariantCultureIgnoreCase)) ?? _game.Rules.UnitTypes[0],
                _ => _game.Rules.UnitTypes[0],
            };

            var civ = owner switch
            {
                AiInterface ai => ai.civ,
                Civilization c => c,
                Tribe t=> t.Civ,
                IPlayer p => p.Civilization,
                int i => _game.AllCivilizations[i % _game.AllCivilizations.Count],
                double i => _game.AllCivilizations[(int)(i % _game.AllCivilizations.Count)],
                string n => _game.AllCivilizations.FirstOrDefault(u =>
                                u.TribeName.Equals(n, StringComparison.InvariantCultureIgnoreCase))
                            ?? _game.AllCivilizations.FirstOrDefault(u =>
                                u.Adjective.Equals(n, StringComparison.InvariantCultureIgnoreCase))
                            ?? _game.AllCivilizations.FirstOrDefault(u =>
                                u.LeaderName.Equals(n, StringComparison.InvariantCultureIgnoreCase))
                            ?? (int.TryParse(n, out var idx) ? _game.AllCivilizations[idx % _game.AllCivilizations.Count] : _game.AllCivilizations[0]),
                _ => _game.AllCivilizations[0],
            };

            var tile = tileObject switch
            {
                TileApi apiTile => apiTile.BaseTile,
                Tile t => t,
                _ => civ.Cities.Count > 0 ? civ.Cities[0].Location : civ.Units.Count > 0 ? civ.Units[0].CurrentLocation : _game.Maps[0].Tile[0, 0]
            };

            var unit = new Unit
            {
                Counter = 0,
                Dead = false,
                Id = civ.Units.Count != 0 ? civ.Units.Max(u => u.Id) + 1 : 0,
                Order = (int)OrderType.NoOrders,
                Owner = civ,
                Veteran = false,
                X = tile.X,
                Y = tile.Y,
                CurrentLocation = tile,
                TypeDefinition = definition
            };
            civ.Units.Add(unit);
            unit.CurrentLocation.SetVisible(civ.Id);
            foreach (var neighbour in unit.CurrentLocation.Neighbours())
            {
                neighbour.SetVisible(civ.Id);
            }
            return new UnitApi(unit, _game);
        }
    }
}