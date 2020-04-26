using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Events
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
