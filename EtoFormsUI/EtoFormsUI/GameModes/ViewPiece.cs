using System.Collections.Generic;
using Civ2engine;
using Civ2engine.UnitActions;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class ViewPiece : IGameMode
    {
        public IDictionary<Keys, IGameAction> Actions { get; set; }
    }
}