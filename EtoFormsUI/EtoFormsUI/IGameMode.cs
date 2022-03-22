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
        bool Init(IGameMode previous, Game game);
        IDictionary<Keys, Action> Actions { get; set; }
        bool MapClicked(Tile clickedXy, MapPanel mapPanel, Main buttons, MouseButtons eButtons);
        bool PanelClick(Game game, Main main);
        IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation);
        
        Tile ActiveTile { get; }
        void DrawStatusPanel(Graphics eGraphics, PanelStyle panelStyle, int unitPanelHeight);
    }
}