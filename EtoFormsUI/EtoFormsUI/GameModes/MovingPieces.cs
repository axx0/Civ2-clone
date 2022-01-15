using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Civ2engine.UnitActions.Move;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class MovingPieces : IGameMode
    {
        private Game _game; 
        public bool Init(IGameMode previous, Game game)
        {
            _game = game;
            if (game.ActiveUnit is not {MovePoints: > 0})
            {
                game.ChooseNextUnit();
            }
            return true;
        }

        public IDictionary<Keys, Action> Actions { get; set; }
        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, MouseButtons eButtons)
        {
            if (eButtons == MouseButtons.Primary)
            {
                var city = clickedXy.CityHere;
                if (city == null)
                {
                    return mapPanel.ActivateUnits(clickedXy);
                }
                mapPanel.ShowCityWindow(city);
            }
            else
            {
                main.CurrentGameMode = main.ViewPiece;              
                main.ViewPiece.ActiveTile = clickedXy;
            }

            return true;
        }

        public bool PanelClick(Game game, Main main)
        {
            main.CurrentGameMode = main.ViewPiece;
            return true;
        }

        public IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation)
        {
            if (currentAnimation is not WaitingAnimation animation) return new WaitingAnimation(game, game.ActiveUnit, game.ActiveUnit.CurrentLocation);
            if (animation.Unit != game.ActiveUnit) return new WaitingAnimation(game, game.ActiveUnit, game.ActiveUnit.CurrentLocation);
            animation.Reset();
            return animation;
        }

        public Tile ActiveTile => _game.ActiveUnit.CurrentLocation;

        public MovingPieces(Main main)
        {
            Actions = new Dictionary<Keys, Action>
            {
                {
                    Keys.B, CityActions.CreateCityBuild((name) =>
                    {
                        var box = main.popupBoxList["NAMECITY"];
                        if (box.Options is not null)
                        {
                            box.Text = box.Options;
                            box.Options = null; 
                        }
                        var cityNameDialog = new Civ2dialog(main, main.popupBoxList["NAMECITY"],
                            textBoxes: new List<TextBoxDefinition>
                            {
                                new()
                                {
                                    index = 0,
                                    InitialValue = name,
                                    Name = "CityName",
                                    Width = 225
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
                {Keys.Left, MovementFunctions.TryMoveWest}, {Keys.Right, MovementFunctions.TryMoveEast},

                {Keys.Space, () => Game.Instance.ActiveUnit.SkipTurn()},
                {Keys.S, () => Game.Instance.ActiveUnit.Sleep()}

            };
        }
    }
}