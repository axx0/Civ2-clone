using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.UnitActions;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class MovingPieces : IGameMode
    {
        public IDictionary<Keys, IGameAction> Actions { get; set; }

        public MovingPieces(Main main)
        {
            Actions = new Dictionary<Keys, IGameAction>
            {
                {
                    Keys.B, new BuildCityAction((name) =>
                    {
                        var cityNameDialog = new Civ2dialogV2(main, main.popupBoxList["NAMECITY"],
                            textBoxes: new List<TextBoxDefinition>
                            {
                                new()
                                {
                                    index = 0,
                                    InitialValue = name,
                                    Name = "CityName"
                                }
                            });
                        cityNameDialog.ShowModal(main);
                        return new BuildCityConfirmResult
                        {
                            Build = cityNameDialog.SelectedIndex != int.MinValue,
                            Name = cityNameDialog.TextValues["CityName"]
                        };
                    })
                }
            };
        }
    }
}