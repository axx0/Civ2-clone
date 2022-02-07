using System.Text;
using Civ2engine.Scripting.UI;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting
{
    public class CivScripts
    {
        public CivScripts(IInterfaceCommands uInterfaceCommands, StringBuilder log, Game game)
        {
            ui = new UIScripts(uInterfaceCommands, log);
            scen = new ScenarioHooks(game);
        }

        // ReSharper disable once IdentifierTypo
        public ScenarioHooks scen { get; }

        public UIScripts ui { get; }
    }
}