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
        /// <param name="eButtons">The buttons clicked ion this event</param>
        /// <returns>True if the map should centre on the clicked location</returns>
        bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main main, MouseButtons eButtons);
        bool PanelClick(Game game, Main main);
        IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation);
        
        void DrawStatusPanel(Graphics eGraphics, PanelStyle panelStyle, int unitPanelHeight);
        void HandleKeyPress(Main main, KeyEventArgs keyEventArgs);
    }
}