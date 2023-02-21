using Raylib_cs;
using rlImGui_cs;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;
using RaylibUI.Controls;
using JetBrains.Annotations;

namespace RaylibUI
{
    public partial class Main
    {
        private string savName = "re_b239.sav";

        private Game Game => Game.Instance;
        private Map map;

        private bool hasCivDir;
        private IScreen _activeScreen;


        public Main()
        {
            hasCivDir = Settings.LoadConfigSettings();

            //========= RAYLIB WINDOW SETTINGS
            Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
            //Raylib.SetConfigFlags(ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
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
            var sound = Raylib.LoadSound(Settings.Civ2Path + Path.DirectorySeparatorChar + "SOUND" + Path.DirectorySeparatorChar + "MENUOK.WAV");
            Raylib.PlaySound(sound);
            var background = _activeScreen.GetBackground();

            var menuBar = new MenuBar();

            while (!Raylib.WindowShouldClose() && !shouldClose)
            {
                // MousePressedAction();
                // KeyboardAction();

                Raylib.BeginDrawing();
                int screenWidth = Raylib.GetScreenWidth();
                int screenHeight = Raylib.GetScreenHeight();

                // Draw map & stuff
                //DrawStuff();
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

                menuBar.Draw();

                Raylib.EndDrawing();
            }

            ShutdownApp();
        }

        public IUserInterface ActiveInterface { get; }

        public IList<IUserInterface> Interfaces { get; }

        void ShutdownApp()
        {
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
