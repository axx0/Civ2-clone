using Civ2engine;

namespace EtoFormsUI
{
    public class UserInterfaceCommands : IInterfaceCommands
    {
        private readonly Main _main;

        public UserInterfaceCommands(Main main)
        {
            _main = main;
        }

        public void ShowDialog(PopupBox popupBox)
        {
            var popup = new Civ2dialog(_main, popupBox);
            popup.ShowModal(_main);
        }
    }
}