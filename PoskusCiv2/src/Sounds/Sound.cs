using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace PoskusCiv2.Sounds
{
    public static class Sound
    {
        public static SoundPlayer MoveSound;

        public static void LoadSounds(string soundsLoc)
        {
            MoveSound = new SoundPlayer(soundsLoc + "MOVPIECE.WAV");
        }
    }
}
