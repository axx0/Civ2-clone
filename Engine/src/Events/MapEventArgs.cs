using System;
using System.Collections.Generic;
using System.Drawing;
using Civ2engine.Enums;
using Civ2engine.Terrains;

namespace Civ2engine.Events
{
    public class MapEventArgs : EventArgs
    {
        public MapEventType EventType { get; }
        public List<Tile> TilesChanged { get; set; }

        public int[] CentrXY, MapStartXY, MapDrawSq;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }

        public MapEventArgs(MapEventType eventType, int[] centrXY)
        {
            EventType = eventType;
            CentrXY = centrXY;
        }

        public MapEventArgs(MapEventType eventType, int[] mapStartXY, int[] mapDrawSq)
        {
            EventType = eventType;
            MapStartXY = mapStartXY;
            MapDrawSq = mapDrawSq;
        }
    }
}
