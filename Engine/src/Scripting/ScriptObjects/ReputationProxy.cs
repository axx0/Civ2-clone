using System;

namespace Civ2engine.Scripting;

public class ReputationProxy(Civilization civ)
{
    public int this[Tribe other]
    {
        get => this[other.id];
        set => this[other.id] = value;
    }

    public int this[int tribeId]
    {
        get
        {
            if (tribeId < 0 || tribeId >= civ.Reputation.Length) return 0;
            return civ.Reputation[tribeId];
        }
        set
        {
            if(tribeId < 0) return;
            
            if (tribeId > civ.Reputation.Length)
            {
                var newArray = new int[tribeId + 1];
                Array.Copy(civ.Reputation, newArray, civ.Reputation.Length);
                civ.Reputation = newArray;
            }
            civ.Reputation[tribeId] = value;
        }
    }
}