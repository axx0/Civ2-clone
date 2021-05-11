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
            Main.OnPopupboxEvent += PopupboxEventHappened;

            LibVLCSharp.Shared.Core.Initialize();

            libVLC = new LibVLC();
            player = new MediaPlayer(libVLC);

            libVLCLoop = new LibVLC("--input-repeat=65535"); // See https://github.com/ZeBobo5/Vlc.DotNet/issues/96
            playerLoop = new MediaPlayer(libVLCLoop);
        }

        public void PlaySound(string soundName)
        {
            string path = location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + soundName;
            player.Play(new Media(libVLC, new Uri(path)));
        }

        public void PlayLoop(string soundName)
        {
            string path = location + Path.DirectorySeparatorChar + "Sound" + Path.DirectorySeparatorChar + soundName;
            playerLoop.Play(new Media(libVLC, new Uri(path)));
        }

        public void PlayMenuLoop()
        {
            this.PlayLoop("MENULOOP.WAV");
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
                    this.PlaySound("MOVPIECE.WAV");
                    break;
                case UnitEventType.Attack:
                    PlaySound(string.IsNullOrWhiteSpace(e.Attacker.AttackSound) ?  "SWORDFGT.WAV" : e.Attacker.AttackSound);
                    break;
            }
        }

        private void PopupboxEventHappened(object sender, PopupboxEventArgs e)
        {
            //switch (e.BoxName)
            //{
            //    case "LOADOK":
            //        Stop();
            //        this.PlaySound("MENUOK.WAV");
            //        break;
            //}
        }
    }
}
