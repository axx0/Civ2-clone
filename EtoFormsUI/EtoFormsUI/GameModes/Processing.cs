using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class Processing : IGameMode
    {
        public bool Activate(IGameMode previous, IPlayer currentPlayer)
        {
            return true;
        }

        public IDictionary<Keys, Action> Actions { get; set; } = new Dictionary<Keys, Action>();
        public bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main buttons, MouseButtons eButtons)
        {
            return true;
        }

        public bool PanelClick(Game game, Main main)
        {
            return false;
        }

        public IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation)
        {
            return null;
        }

        public void DrawStatusPanel(Graphics eGraphics, PanelStyle panelStyle, int unitPanelHeight)
        {
        }

        public void HandleKeyPress(Main eventArgs, KeyEventArgs keyEventArgs)
        {
            // Cannot press keys while processing
        }

        public void HandleCommand(Command command)
        {
            
        }
    }
}