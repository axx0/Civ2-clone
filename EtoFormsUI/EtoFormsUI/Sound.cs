using System;
using System.IO;
using Civ2engine;
using Civ2engine.Events;
using Civ2engine.Enums;
using LibVLCSharp.Shared;

namespace EtoFormsUI
{
    public class Sound
    {
        private readonly LibVLC libVLC;
        private readonly LibVLC libVLCLoop;
        private readonly MediaPlayer player;
        private readonly MediaPlayer playerLoop;

        private readonly string location;

        public Sound(string loc)
        {
            location = loc;
            Game.OnUnitEvent += UnitEventHappened;

            libVLC = new LibVLC();
            player = new MediaPlayer(libVLC);

            libVLCLoop = new LibVLC("--input-repeat=65535"); // See https://github.com/ZeBobo5/Vlc.DotNet/issues/96
            playerLoop = new MediaPlayer(libVLCLoop);
        }

        private void PlaySound(string path){
            player.Play(new Media(libVLC, new Uri(path)));
        }

        private void PlayLoop(string path){
            playerLoop.Play(new Media(libVLC, new Uri(path)));
        }

        public void PlayMenuLoop()
        {
            this.PlayLoop(location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + "MENULOOP.WAV");
        }

        public void Stop()
        {
            player.Stop();
            playerLoop.Stop();
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                case UnitEventType.MoveCommand:
                    this.PlaySound(location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + "MOVPIECE.WAV");
                    break;
                case UnitEventType.Attack:
                    switch (e.Attacker.Type)
                    {
                        case UnitType.Catapult:
                            this.PlaySound(location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + "CATAPULT.WAV");
                            break;
                        case UnitType.Elephant:
                            this.PlaySound(location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + "ELEPHANT.WAV");
                            break;
                        default:
                            this.PlaySound(location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + "SWORDFGT.WAV");
                            break;
                    }
                    break;
            }
        }
    }
}
