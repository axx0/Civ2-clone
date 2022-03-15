using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Eto.Drawing;
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

        public void DrawStatusPanel(Graphics eGraphics, PanelStyle style, int unitPanelHeight)
        {
            Draw.Text(eGraphics, "Viewing Pieces", style.Font, Colors.White, new Point(119, 10), true, true,
                Colors.Black, 1, 0);

            // Draw location & tile type on active square
            if (ActiveTile != null)
            {
                Draw.Text(eGraphics,
                    $"Loc: ({ActiveTile.X}, {ActiveTile.Y}) {ActiveTile.Island}",
                    style.Font, style.FrontColor, new Point(5, 27), false, false, style.BackColor, 1, 1);
                Draw.Text(eGraphics,
                    $"({ActiveTile.Type})", style.Font,
                    style.FrontColor, new Point(5, 45), false, false, style.BackColor, 1, 1);
            }
            //int count;
            //for (count = 0; count < Math.Min(_unitsOnThisTile.Count, maxUnitsToDraw); count++)
            //{
            //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), (int)Math.Round(64 * 1.15), (int)Math.Round(48 * 1.15)), 6, 70 + count * 56);
            //    //e.Graphics.DrawImage(ModifyImage.Resize(Draw.Unit(UnitsOnThisTile[count], false, 0), 0), 6, 70 + count * 56);  // TODO: do this again!!!
            //    Draw.Text(e.Graphics, _unitsOnThisTile[count].HomeCity.Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 70 + count * 56), _backColor, 1, 1);
            //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Order.ToString(), _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 88 + count * 56), _backColor, 1, 1); // TODO: give proper conversion of orders to string
            //    Draw.Text(e.Graphics, _unitsOnThisTile[count].Name, _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(79, 106 + count * 56), _backColor, 1, 1);
            //}
            //if (count < _unitsOnThisTile.Count)
            //{
            //    string _moreUnits = (_unitsOnThisTile.Count - count == 1) ? "More Unit" : "More Units";
            //    Draw.Text(e.Graphics, $"({_unitsOnThisTile.Count() - count} {_moreUnits})", _font, StringAlignment.Near, StringAlignment.Near, _frontColor, new Point(5, UnitPanel.Height - 27), _backColor, 1, 1);
            //}
        }
    }
}