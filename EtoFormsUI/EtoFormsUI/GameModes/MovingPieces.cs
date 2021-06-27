using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.UnitActions;
using Civ2engine.UnitActions.Move;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class MovingPieces : IGameMode
    {
        public IDictionary<Keys, Action> Actions { get; set; }

        public MovingPieces(Main main)
        {
            Actions = new Dictionary<Keys, Action>
            {
                {
                    Keys.B, BuildCity.CreateCityBuild((name) =>
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
                },

                {Keys.Keypad7, MovementFunctions.TryMoveNorthWest}, {Keys.Keypad8, MovementFunctions.TryMoveNorth},
                {Keys.Keypad9, MovementFunctions.TryMoveNorthEast},
                {Keys.Keypad1, MovementFunctions.TryMoveSouthWest}, {Keys.Keypad2, MovementFunctions.TryMoveSouth},
                {Keys.Keypad3, MovementFunctions.TryMoveSouthEast},
                {Keys.Keypad4, MovementFunctions.TryMoveWest}, {Keys.Keypad6, MovementFunctions.TryMoveEast},
                
                {Keys.Up, MovementFunctions.TryMoveNorth}, {Keys.Down, MovementFunctions.TryMoveSouth},
                {Keys.Left, MovementFunctions.TryMoveWest}, {Keys.Right, MovementFunctions.TryMoveEast}

            };
        }
    }
}