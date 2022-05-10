using System;
using System.Collections.Generic;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI;
using EtoFormsUI.Animations;

namespace Civ2engine
{
    public interface IGameMode
    {
        bool Activate(IGameMode previous, IPlayer currentPlayer);

        /// <summary>
        ///  A tile was clicked how sould this game mode respond
        /// </summary>
        /// <param name="clickedXy">The tile clicked</param>
        /// <param name="mapPanel"></param>
        /// <param name="main">The app main form</param>
        /// <param name="longClick">True if the mouse was clicked and held for some time</param>
        /// <param name="mouseEventArgs">The mouse event args</param>
        /// <param name="mouseDownTile">The coords of the map where mouse down occured (could be same as clickedXy</param>
        /// <returns>True if the map should centre on the clicked location</returns>
        bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, bool longClick, MouseEventArgs mouseEventArgs,
            int[] mouseDownTile);
        bool PanelClick(Game game, Main main);
        IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation);
        
        void DrawStatusPanel(Graphics eGraphics, PanelStyle panelStyle, int unitPanelHeight);
        void HandleKeyPress(Main main, KeyEventArgs keyEventArgs);
    }
}