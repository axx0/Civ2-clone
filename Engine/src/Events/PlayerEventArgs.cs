using System;
using Civ2engine.Enums;

namespace Civ2engine.Events
{
    public class PlayerEventArgs : EventArgs
    {
        public PlayerEventType EventType;

        public PlayerEventArgs(PlayerEventType eventType)
        {
            EventType = eventType;
        }
    }
}
