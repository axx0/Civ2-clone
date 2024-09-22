using System;
using System.Collections.Generic;
using System.Text;
using Civ2engine.IO;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting
{
    /// <summary>
    /// This class exposes the popup functionality to LUA script in a TOTPP compatible way
    ///
    /// We violate normal C# naming rules for compatibility with LUA code
    /// </summary>
    public class Dialog
    {
        private readonly IInterfaceCommands _uInterfaceCommands;
        private readonly StringBuilder _log;
        private readonly PopupBox _popup;
        private IList<bool> _checkResults;
        private string _selectedButton;
        private List<int> _optionValues;
        private List<bool> _checkBoxOptionStates;

        public string title
        {
            get => _popup.Title;
            set => _popup.Title = value;
        }

        public int width
        {
            get => _popup.Width;
            set => _popup.Width = value;
        }
        
        public int height
        {
            get => 0;
            set => _log.AppendLine("Warning: Attempted to set height to " +value +" on dialog -> not supported");
        }

        public Dialog(IInterfaceCommands uInterfaceCommands, StringBuilder log)
        {
            _uInterfaceCommands = uInterfaceCommands;
            _log = log;
            _popup = new PopupBox();
        }

        public void addCheckbox(string text, int id, bool initial = false)
        {
            _popup.Checkbox = true;
            (_popup.Options ??= new List<string>()).Add(text);
            (_checkBoxOptionStates ??= new List<bool>()).Add(initial);
            (_optionValues ??= new List<int>()).Add(id);
        }

        public void addOption(string option, int value)
        {
            (_popup.Options ??= new List<string>()).Add(option);
            (_optionValues ??= new List<int>()).Add(value);
        }

        public void addText(string text)
        {
            _popup.AddText(text);
        }
        
        public void addButton(string text)
        {
            (_popup.Button ??= new List<string>()).Add(text);
        }

        public string getSelectedButton()
        {
            return _selectedButton;
        }

        public bool getCheckboxState(int id)
        {
            if (_optionValues == null) return false;
            var index = _optionValues.IndexOf(id);
            return _checkResults != null && index < _checkResults.Count && _checkResults[index];
        }

        public int show()
        {
            _popup.Button ??= new List<string> { Labels.Ok };
            var (selectedButton, selectedIndex, checkBoxes) = _uInterfaceCommands.ShowDialog(_popup, _checkBoxOptionStates);
            _checkResults = checkBoxes;
            _selectedButton = selectedButton;
            return _optionValues != null && _optionValues.Count > selectedIndex ? _optionValues[selectedIndex] : selectedIndex;
        }

        /// <summary>
        /// Save the created dialog as a popup that can be shown in other places such as terrain improvement Errors
        /// </summary>
        /// <param name="key"></param>
        public void saveAsPopup(string key)
        {
            _popup.Button ??= new List<string> { Labels.Ok };
            _uInterfaceCommands.SavePopup(key, _popup);
        }
    }
}