using System.Text;
using Civ2engine.Scripting.UI;
// ReSharper disable MemberCanBePrivate.Global

namespace Civ2engine.Scripting
{
    public class CivScripts
    {
        public CivScripts(IInterfaceCommands uInterfaceCommands, StringBuilder log)
        {
            ui = new UIScripts(uInterfaceCommands, log);
        }

        public UIScripts ui { get; }
    }
}