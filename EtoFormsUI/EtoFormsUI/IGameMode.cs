using System;
using System.Collections.Generic;
using Eto.Forms;
using EtoFormsUI;
using EtoFormsUI.Animations;

namespace Civ2engine
{
    public interface IGameMode
    {
        bool Init(IGameMode previous, Game game);
        IDictionary<Keys, Action> Actions { get; set; }
        bool MapClicked(int[] clickedXy, MapPanel mapPanel, Main buttons, MouseButtons eButtons);
        bool PanelClick(Game game, Main main);
        IAnimation GetDefaultAnimation(Game game, IAnimation currentAnimation);
        
        int[] ActiveXY { get; }
    }
}