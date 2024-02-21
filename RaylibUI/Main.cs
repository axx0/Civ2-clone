using Raylib_cs;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;
using RaylibUI.Forms;
using JetBrains.Annotations;

namespace RaylibUI
{
    public partial class Main
    {
        private Map _map;

        private IScreen _activeScreen;
        private bool _shouldClose;


        internal readonly Sound Soundman;

        public Main()
        {
            var hasCivDir = Settings.LoadConfigSettings();

            //========= RAYLIB WINDOW SETTINGS
            Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint |
                                  ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1280, 800, "raylib - civ2");
            //Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();
            Soundman = new Sound();

            //========== IMGUI STYLE

            Raylib.SetExitKey(KeyboardKey.F12);

            Shaders.Load();

            //============ LOAD REQUIRED SAV GAME DATA
            if (hasCivDir)
            {
                _activeScreen = SetupMainScreen();
            }
            else
            {
                _activeScreen = new GameFileLocatorScreen(this,() =>
                {
                    hasCivDir = true;
                    _activeScreen = SetupMainScreen();
                });
            }

            //============ LOAD SOUNDS

            //prep this for a loop( should split that function out between loops and non loops)

            //play a sound
            //soundman.PlayCIV2DefaultSound("DIVEBOMB");
        }

        public void RunLoop()
        {
            var counter = 0;
            var pulse = false;

            while (!Raylib.WindowShouldClose() && !_shouldClose)
            {

                Raylib.BeginDrawing();

                int screenHeight = Raylib.GetScreenHeight();

                _activeScreen.Draw(pulse);

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, screenHeight - 20, 20, Color.Magenta);

                Raylib.EndDrawing();
                if (counter++ >= 30)
                {
                    pulse = !pulse;
                    counter = 0;
                }
            }

            ShutdownApp();
        }

        private MainMenu SetupMainScreen()
        {                
            Labels.UpdateLabels(null);
            Helpers.LoadFonts();
            Interfaces = Helpers.LoadInterfaces();

            ActiveInterface = Helpers.GetInterface(Settings.Civ2Path, Interfaces);
            return new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }


        public IUserInterface ActiveInterface { get; set; }

        public IList<IUserInterface> Interfaces { get; set; }

        public IScreen ActiveScreen => _activeScreen;

        void ShutdownApp()
        {
            Shaders.Unload();
            Soundman.Dispose();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
