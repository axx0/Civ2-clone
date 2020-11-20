using System;
using civ2.Enums;

namespace civ2.Events
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
