using System.Text;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting.UI
{
    public class UIScripts
    {
        private readonly IInterfaceCommands _uInterfaceCommands;
        private readonly StringBuilder _log;

        public UIScripts(IInterfaceCommands uInterfaceCommands, StringBuilder log)
        {
            _uInterfaceCommands = uInterfaceCommands;
            _log = log;
        }

        public void text(string text)
        {
            var pop = new PopupBox
            {
                Button = new[] { "OK" },
                Text = new[] { text },
                LineStyles = new[] { TextStyles.Left }
            };
            _uInterfaceCommands.ShowDialog(pop);
        }

        public Dialog createDialog()
        {
            return new Dialog(_uInterfaceCommands, _log);
        }
    }
}