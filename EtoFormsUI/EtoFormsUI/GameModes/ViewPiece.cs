using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
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
                this.ActiveXY = previous.ActiveXY;
            }
            return true;
        }

        public IDictionary<Keys, Action> Actions { get; set; } = new Dictionary<Keys, Action>
        {
            {Keys.Enter, Game.Instance.ChoseNextCiv}
        };

        public bool MapClicked(int[] clickedXy, MapPanel mapPanel, Main main, MouseButtons eButtons)
        {
            
            ActiveXY = clickedXy;

            if (eButtons == MouseButtons.Primary)
            {

                var city = Game.Instance.GetCities.FirstOrDefault(c =>
                    c.X == clickedXy[0] && c.Y == clickedXy[1]);
                if (city != null)
                {
                    mapPanel.ShowCityWindow(city);
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
            if (game.ActiveUnit != null)
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
            if (currentAnimation is not WaitingAnimation animation) return new WaitingAnimation(game, null, ActiveXY);
            if (animation.Unit != null) return new WaitingAnimation(game, null, ActiveXY);
            animation.Reset();
            return animation;
        }

        public int[] ActiveXY { get; set; }
    }
}