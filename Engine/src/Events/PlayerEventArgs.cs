using System;
using Civ2engine.Enums;

namespace Civ2engine.Events
{
    public class PlayerEventArgs : EventArgs
    {
        public int PlayerNo { get; }
        public PlayerEventType EventType { get; }

        public PlayerEventArgs(PlayerEventType eventType, int playerNo)
        {
            PlayerNo = playerNo;
            EventType = eventType;
        }
    }
}
