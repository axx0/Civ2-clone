using System;

namespace Civ2engine
{
    internal class CivEventArgs : EventArgs
    {
        public CivEventType EventType { get; }
        public Civilization Civ { get; }

        public CivEventArgs(CivEventType eventType, Civilization civ)
        {
            EventType = eventType;
            Civ = civ;
        }
    }
}