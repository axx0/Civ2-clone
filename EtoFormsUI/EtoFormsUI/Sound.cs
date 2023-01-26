using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Civ2engine;
using LibVLCSharp.Shared;

namespace EtoFormsUI
{
    public class Sound
    {
        private readonly LibVLC _libVlc;
        private readonly MediaPlayer player;
        private readonly MediaPlayer playerLoop;

        private Dictionary<GameSounds, string> _soundPaths;

        public Sound()
        {
            try
            {
                Core.Initialize();

                _libVlc = new LibVLC();
                player = new MediaPlayer(_libVlc);

                var libVlcLoop = new LibVLC("--input-repeat=65535");
                playerLoop = new MediaPlayer(libVlcLoop);
                LoadSounds(Settings.SearchPaths);
            }
            catch (Exception ex)
            {
                
            }
        }

        public void PlaySound(GameSounds gameSound, string soundPath = null)
        {
            Play(string.IsNullOrWhiteSpace(soundPath) ? _soundPaths[gameSound] : soundPath);
        }
        
        private void Play(string soundPath)
        {
            player?.Play(new Media(_libVlc, new Uri(soundPath)));
        }

        public void PlayMenuLoop()
        {
            //Play(_soundPaths[GameSounds.MenuLoop], playerLoop);
        }

        public void Stop()
        {
            player.Stop();
            playerLoop.Stop();
        }

        public void LoadSounds(IEnumerable<string> paths)
        {
            var filePaths = paths.SelectMany(p => new[] {p + Path.DirectorySeparatorChar + "Sound", p}).ToList();
            _soundPaths = new Dictionary<GameSounds, string>
            {
                {GameSounds.Move, Utils.GetFilePath("MOVPIECE.WAV", filePaths)},
                {GameSounds.Attack, Utils.GetFilePath("SWORDFGT.WAV", filePaths)},
                {GameSounds.MenuOk, Utils.GetFilePath("MENUOK.WAV", filePaths)},
                {GameSounds.MenuLoop, Utils.GetFilePath("MENULOOP.WAV", filePaths)}
            };
        }
    }
}
