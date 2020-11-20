using System;
using civ2.Enums;

namespace civ2.Events
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
