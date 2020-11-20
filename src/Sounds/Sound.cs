using System.Media;
using civ2.Events;
using civ2.GameActions;
using civ2.Enums;

namespace civ2.Sounds
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
