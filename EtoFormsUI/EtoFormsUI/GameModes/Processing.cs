using System;
using System.Collections.Generic;
using Civ2engine;
using Civ2engine.UnitActions;
using Eto.Forms;

namespace EtoFormsUI.GameModes
{
    public class Processing : IGameMode
    {
        public IDictionary<Keys, Action> Actions { get; set; } = new Dictionary<Keys, Action>();
    }
}