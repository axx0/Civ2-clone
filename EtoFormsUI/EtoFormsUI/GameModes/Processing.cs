using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.Terrains;
using Civ2engine.UnitActions;
using Eto.Forms;
using EtoFormsUI.Animations;

namespace EtoFormsUI.GameModes
{
    public class Processing : IGameMode
    {
        public bool Init(IGameMode previous, Game game)
        {
            ActiveTile = previous.ActiveTile;
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

        public Tile ActiveTile { get; set; }
    }
}