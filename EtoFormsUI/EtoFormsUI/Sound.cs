using System.Media;
using Civ2engine;
using Civ2engine.Events;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public class Sound
    {
        private readonly SoundPlayer player;
        private readonly string location;

        public Sound(string loc)
        {
            location = loc;
            Game.OnUnitEvent += UnitEventHappened;

            player = new SoundPlayer();
        }

        public void PlayMenuLoop()
        {
            player.SoundLocation = location + "\\Sound\\MENULOOP.WAV";
            player.PlayLooping();
        }

        public void Stop()
        {
            player.Stop();
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                case UnitEventType.MoveCommand:
                    player.SoundLocation = location + "\\Sound\\MOVPIECE.WAV";
                    player.Play();
                    break;
            }
        }
    }
}
