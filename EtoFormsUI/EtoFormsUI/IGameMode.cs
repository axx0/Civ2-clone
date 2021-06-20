using System.Collections.Generic;
using Civ2engine.UnitActions;

namespace Civ2engine
{
    public interface IGameMode
    {
        IDictionary<Eto.Forms.Keys, IGameAction> Actions { get; set; }
    }
}