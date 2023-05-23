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

        private Game Game => Game.Instance;
        private Map map;

        private bool hasCivDir;
        private IScreen _activeScreen;
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
                Labels.UpdateLabels(null); 
                Interfaces = Helpers.LoadInterfaces();

                ActiveInterface = Helpers.GetInterface(Settings.Civ2Path, Interfaces);
                _activeScreen = new MainMenu(ActiveInterface,() => shouldClose= true);
            }
            else
            {
                _activeScreen = new GameFileLocatorScreen();
            }
            //LoadGame(savName);

            //============ LOAD SOUNDS
            var sound = Raylib.LoadSound(Settings.Civ2Path + Path.DirectorySeparatorChar + "SOUND" + Path.DirectorySeparatorChar + "DIVEBOMB.WAV");
            soundman = new Sound();
     
            //prep this for a loop( should split that function out between loops and non loops)
            sndMenuLoop =  soundman.PlayCIV2DefaultSound("MENULOOP",true);

            //play a sound
            soundman.PlayCIV2DefaultSound("DIVEBOMB");

            var background = _activeScreen.GetBackground();

            FormManager.Initialize();

            while (!Raylib.WindowShouldClose() && !shouldClose)
            {
                if (sndMenuLoop != null)
                  sndMenuLoop.MusicUpdateCall();


                //ToStop Music you would call
                sndMenuLoop.Stop();

        // MousePressedAction();
        // KeyboardAction();

        Raylib.BeginDrawing();
                int screenWidth = Raylib.GetScreenWidth();
                int screenHeight = Raylib.GetScreenHeight();

                if (background == null)
                {
                    Raylib.ClearBackground(new Color(143, 123, 99, 255));
                }
                else
                {
                    Raylib.ClearBackground(background.background);
                    Raylib.DrawTexture(background.CentreImage, (screenWidth- background.CentreImage.width)/2, (screenHeight-background.CentreImage.height)/2, Color.WHITE);
                }

                _activeScreen.Draw(screenWidth, screenHeight);

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, screenHeight - 20, 20, Raylib_cs.Color.BLACK);

                Raylib.EndDrawing();

            }

            ShutdownApp();
        }

        public IUserInterface ActiveInterface { get; }

        public IList<IUserInterface> Interfaces { get; }

        void ShutdownApp()
        {
            soundman.Dispose();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
