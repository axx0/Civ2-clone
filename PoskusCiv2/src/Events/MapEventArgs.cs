using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Events
{
    public class MapEventArgs : EventArgs
    {
        public MapEventType EventType;
        public int[] CenterSqXY;
        public int[] ActiveSqXY;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }

        public MapEventArgs(MapEventType eventType, int[] centerSqXY)
        {
            EventType = eventType;
            CenterSqXY = centerSqXY;
        }
    }
}
