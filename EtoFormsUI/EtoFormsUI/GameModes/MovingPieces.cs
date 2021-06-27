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
        public IDictionary<Keys, IGameAction> Actions { get; set; }

        public MovingPieces(Main main)
        {
            var moveUp = new MoveNorth();
            var moveDown = new MoveSouth();
            var moveLeft = new MoveWest();
            var moveRight = new MoveEast();
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
                },
                {Keys.Keypad8, moveUp}, {Keys.Up, moveUp}, {Keys.Keypad2, moveDown}, {Keys.Down, moveDown},
                {Keys.Keypad4, moveLeft}, {Keys.Left, moveLeft}, {Keys.Keypad6, moveRight}, {Keys.Right, moveRight},
                {Keys.Keypad3, new MoveSouthEast()}, {Keys.Keypad1, new MoveSouthWest()},
                {Keys.Keypad7, new MoveNorthWest()}, {Keys.Keypad9, new MoveNorthEast()}
            };
        }
    }
}