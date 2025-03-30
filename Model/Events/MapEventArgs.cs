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

        public int[] MapStartXy, MapDrawSq;
        public int Zoom, Xshift;

        public MapEventArgs(MapEventType eventType)
        {
            EventType = eventType;
        }
    }
}
