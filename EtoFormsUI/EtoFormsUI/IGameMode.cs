using System;
using System.Collections.Generic;

namespace Civ2engine
{
    public interface IGameMode
    {
        IDictionary<Eto.Forms.Keys, Action> Actions { get; set; }
    }
}