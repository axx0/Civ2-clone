using System;

namespace Civ2engine.Events
{
    public class PopupboxEventArgs : EventArgs
    {
        public string BoxName;

        public PopupboxEventArgs(string boxName)
        {
            BoxName = boxName;
        }
    }
}
