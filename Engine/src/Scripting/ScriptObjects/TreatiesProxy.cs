using System;

namespace Civ2engine.Scripting;

public class TreatiesProxy(Civilization civ)
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
            if (tribeId < 0 || tribeId >= civ.Relations.Length || civ.Relations[tribeId] == null) return 0;
            return civ.Relations[tribeId]!.Summary;
        }
        set
        {
            if (tribeId < 0) return;

            if (tribeId > civ.Relations.Length)
            {
                var newArray = new Relation[tribeId + 1];
                Array.Copy(civ.Relations, newArray, civ.Relations.Length);
                civ.Relations = newArray;
            }
            civ.Relations[tribeId] ??= new Relation();
            civ.Relations[tribeId]!.UpdateFrom(value);
        }
    }
}