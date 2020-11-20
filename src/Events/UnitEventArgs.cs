using System;
using civ2.Enums;

namespace civ2.Events
{
    public class UnitEventArgs : EventArgs
    {
        public UnitEventType EventType;
        public int Counter;

        public UnitEventArgs(UnitEventType eventType)
        {
            EventType = eventType;
        }

        public UnitEventArgs(UnitEventType eventType, int counter)
        {
            EventType = eventType;
            Counter = counter;
        }
    }
}
