using System;
using System.Collections.Generic;
using Civ2engine.Enums;
using Civ2engine.MapObjects;

namespace Civ2engine.Events
{
    public class MapEventArgs : EventArgs
    {
        public MapEventType EventType { get; }
        public List<Tile> TilesChanged { get; set; }

        public int[] CentrXy, MapStartXy, MapDrawSq;
        public int Zoom;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }

        public MapEventArgs(MapEventType eventType, int[] centrXy)
        {
            EventType = eventType;
            CentrXy = centrXy;
        }

        public MapEventArgs(MapEventType eventType, int[] mapStartXy, int[] mapDrawSq)
        {
            EventType = eventType;
            MapStartXy = mapStartXy;
            MapDrawSq = mapDrawSq;
        }
    }
}
