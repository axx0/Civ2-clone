using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class ViewPiece : IGameMode
    {
        public bool Init(IGameMode previous,Game game)
        {
            if (previous is MovingPieces)
            {
                this.ActiveTile = previous.ActiveTile;
            }

            if (ActiveTile == null)
            {
                var firstCity = game.GetActiveCiv.Cities.FirstOrDefault();
                if (firstCity != null)
                {
                    ActiveTile = firstCity.Location;
                }
            }
            return true;
        }

        public IDictionary<Keys, Action> Actions { get; set; } = new Dictionary<Keys, Action>
        {
            {Keys.Enter, Game.Instance.ChoseNextCiv}
        };

        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, MouseButtons eButtons)
        {
            
            ActiveTile = clickedXy;

            if (eButtons == MouseButtons.Primary)
            {

                if (ActiveTile.CityHere != null)
                {
                    mapPanel.ShowCityWindow(ActiveTile.CityHere);
                }
                else
                {
                    mapPanel.ActivateUnits(clickedXy);
                }

            }

            return true;
        }

        public bool PanelClick(Game game, Main main)
        {
            if (game.ActiveUnit is {Dead: false})
            {
                main.CurrentGameMode = main.Moving;
            }
            else
            {
                game.ChoseNextCiv();
            }
            return true;
        }

        public IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation)
        {
            if (currentAnimation is not WaitingAnimation animation) return new WaitingAnimation(game, null, ActiveTile);
            if (animation.Unit != null) return new WaitingAnimation(game, null, ActiveTile);
            animation.Reset();
            return animation;
        }

        public Tile ActiveTile { get; set; }
    }
}