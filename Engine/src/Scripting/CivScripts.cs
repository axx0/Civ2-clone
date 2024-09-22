using System;
using System.Text;
using System.Threading;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Scripting.UI;
using Civ2engine.Units;

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

        /// <summary>
        /// For compatibility with existing scripts, new lua scripts could set CurrentLocation directly
        /// </summary>
        public Action<Unit, Tile> teleportUnit = (unit, tile) =>
        {
            unit.CurrentLocation = tile;
        };

        public CityImprovement getImprovement(int index)
        {
            return new CityImprovement(_game.Rules.Improvements[index]);
        }

        public Tech getTech(int index)
        {
            return new Tech(_game.Rules.Advances,index);
        }

        public UnitType getUnitType(int index)
        {
            return new UnitType(_game.Rules.UnitTypes[index]);
        }
    }
}