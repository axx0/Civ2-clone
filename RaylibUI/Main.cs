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
        private string savName = "he_a1770.sav";

        private Map map;

        private bool hasCivDir;
        private IScreen _activeScreen;
        private bool _shouldClose;


        internal Sound soundman;
        private Sound.SoundData sndMenuLoop;
        public Main()
        {
            hasCivDir = Settings.LoadConfigSettings();

            //========= RAYLIB WINDOW SETTINGS
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            Raylib.InitWindow(1280, 800, "raylib - civ2");
            //Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();

            //========== IMGUI STYLE
            //rlImGui.Setup(true);
            //ImGui.StyleColorsLight();
            //var style = ImGui.GetStyle();
            //style.Colors[(int)ImGuiCol.Text] = new Vector4(0, 0, 0, 1);
            //style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(1, 1, 1, 1);
            var shouldClose = false;

            Raylib.SetExitKey(KeyboardKey.KEY_F12);

            //============ LOAD REQUIRED SAV GAME DATA
            if (hasCivDir)
            {
                _activeScreen = SetupMainScreen();
            }
            else
            {
                _activeScreen = new GameFileLocatorScreen(() =>
                {
                    hasCivDir = true;
                    _activeScreen = SetupMainScreen();
                });
            }
            //LoadGame(savName);

            //============ LOAD SOUNDS
            var sound = Raylib.LoadSound(Settings.Civ2Path + Path.DirectorySeparatorChar + "SOUND" + Path.DirectorySeparatorChar + "DIVEBOMB.WAV");
            soundman = new Sound();
     
            //prep this for a loop( should split that function out between loops and non loops)
            sndMenuLoop =  soundman.PlayCIV2DefaultSound("MENULOOP",true);

            //play a sound
            //soundman.PlayCIV2DefaultSound("DIVEBOMB");

            FormManager.Initialize();

            var counter = 0;
            var pulse = false;

            while (!Raylib.WindowShouldClose() && !_shouldClose)
            {
                if (sndMenuLoop != null)
                  sndMenuLoop.MusicUpdateCall();


                //ToStop Music you would call
               // sndMenuLoop.Stop();

        // MousePressedAction();
        // KeyboardAction();

        Raylib.BeginDrawing();
                
                int screenHeight = Raylib.GetScreenHeight();

                _activeScreen.Draw(pulse);

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, screenHeight - 20, 20, Raylib_cs.Color.BLACK);

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
            Interfaces = Helpers.LoadInterfaces();

            ActiveInterface = Helpers.GetInterface(Settings.Civ2Path, Interfaces);
            return new MainMenu(ActiveInterface,() => _shouldClose= true, StartGame);
        }


        public IUserInterface ActiveInterface { get; set; }

        public IList<IUserInterface> Interfaces { get; set; }

        void ShutdownApp()
        {
            soundman.Dispose();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
