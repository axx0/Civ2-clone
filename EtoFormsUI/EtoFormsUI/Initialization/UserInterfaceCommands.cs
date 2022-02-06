using System;
using System.Collections.Generic;
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

        public Tuple<string, int, List<bool>> ShowDialog(PopupBox popupBox, List<bool> checkBoxOptionStates = null)
        {
            var popup = new Civ2dialog(_main, popupBox, checkboxOptionState: checkBoxOptionStates);
            popup.ShowModal(_main);
            return Tuple.Create(popup.SelectedButton, popup.SelectedIndex, popup.CheckboxReturnStates);
        }
    }
}