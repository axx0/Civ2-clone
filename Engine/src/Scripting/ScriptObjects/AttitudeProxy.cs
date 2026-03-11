using System;

namespace Civ2engine.Scripting;

public class AttitudeProxy(Civilization civ)
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
            if (tribeId < 0 || tribeId >= civ.Attitude.Length) return 0;
            return civ.Attitude[tribeId];
        }
        set
        {
            if(tribeId < 0) return;
            
            if (tribeId > civ.Attitude.Length)
            {
                var newArray = new int[tribeId + 1];
                Array.Copy(civ.Attitude, newArray, civ.Attitude.Length);
                civ.Attitude = newArray;
            }
            civ.Attitude[tribeId] = value;
        }
    }
}