using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using RTciv2.Events;

namespace RTciv2.Sounds
{
    public static class Sound
    {
        private static SoundPlayer MOVPIECE;

        public static void LoadSounds(string directoryPath)
        {
            MOVPIECE = new SoundPlayer(directoryPath + "Sound\\MOVPIECE.WAV");

            Actions.OnMoveUnitCommand += UnitHasMoved;
        }

        private static void UnitHasMoved(object sender, MoveUnitCommandEventArgs e)
        {
            if (e.MoveUnit)
            {
                MOVPIECE.Play();
            }
                
        }
    }
}
