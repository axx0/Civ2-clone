using System;
using System.Text;
using System.Threading;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
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

        public CivScripts(IInterfaceCommands uInterfaceCommands, StringBuilder log, Game game)
        {
            _game = game;
            ui = new UIScripts(uInterfaceCommands, log);
            scen = new ScenarioHooks(game);
            core = new AxxExtensions(game);
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

        public Improvement getImprovement(int index)
        {
            return _game.Rules.Improvements[index];
        }
    }
}