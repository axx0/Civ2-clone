using System;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Eto.Forms;

namespace EtoFormsUI.Players.Orders
{
    public abstract class Order
    {
        private readonly Main _mainForm;
        
        private readonly string _defaultLabel;

        protected Order(Main mainForm, Keys activationCommand, string defaultLabel, int @group)
        {
            _mainForm = mainForm;
            _defaultLabel = defaultLabel.Split("|")[0];
            ActivationCommand = activationCommand;
            Command = new Command
            {
                MenuText = defaultLabel,
                Shortcut = activationCommand
            };
            Group = group;
            Command.Executed += (_, _) => ExecuteCommand();
            ErrorPopup = _mainForm.popupBoxList["CANTDO"];
        }

        public void ExecuteCommand()
        {
            if (Status == OrderStatus.Active)
            {
                Execute((LocalPlayer)_mainForm.CurrentPlayer);
            }
            else
            {
                var dialog = new Civ2dialog(_mainForm, ErrorPopup);
                dialog.ShowModal();
            }
        }

        public Command Command { get;  }
        
        public Keys ActivationCommand { get; }
        
        public int Group { get; }

        public OrderStatus Status { get; set; }

        private PopupBox ErrorPopup { get; set; }
        
        public abstract Order Update(Tile activeTile, Unit activeUnit);


        protected abstract void Execute(LocalPlayer player);
        
        protected void SetCommandState(OrderStatus status = OrderStatus.Disabled, string menuText = null, string errorPopupKeyword = null)
        {
            Command.MenuText = string.IsNullOrWhiteSpace(menuText) ? _defaultLabel : menuText;
            Command.Enabled = status == OrderStatus.Active;
            Status = status;
            ErrorPopup = !string.IsNullOrWhiteSpace(errorPopupKeyword) && _mainForm.popupBoxList.ContainsKey(errorPopupKeyword)
                ? _mainForm.popupBoxList[errorPopupKeyword]
                : _mainForm.popupBoxList["CANTDO"];
        }
    
    }
}