using System;
using System.Collections.Generic;
using Civ2engine;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class ViewPiece : IGameMode
    {
        public IDictionary<Keys, Action> Actions { get; set; } = new Dictionary<Keys, Action>
        {
            {Keys.Enter, Game.Instance.ChoseNextCiv}
        };
    }
}