using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using RTciv2.Events;
using RTciv2.GameActions;
using RTciv2.Enums;

namespace RTciv2.Sounds
{
    public static class Sound
    {
        private static SoundPlayer MOVPIECE;

        public static void LoadSounds(string directoryPath)
        {
            MOVPIECE = new SoundPlayer(directoryPath + "Sound\\MOVPIECE.WAV");

            Actions.OnUnitEvent += UnitEventHappened;
        }

        private static void UnitEventHappened(object sender, UnitEventArgs e)
        {
            if (e.EventType == UnitEventType.MoveCommand && e.Counter == 0) MOVPIECE.Play();
        }
    }
}
