using System.Collections.Generic;
using Civ2engine;
using Civ2engine.UnitActions;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class Processing : IGameMode
    {
        public IDictionary<Keys, IGameAction> Actions { get; set; } = new Dictionary<Keys, IGameAction>();
    }
}