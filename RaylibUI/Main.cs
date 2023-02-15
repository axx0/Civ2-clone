using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;
using RaylibControls;
using RaylibUI.Controls;

namespace RaylibUI
{
    public partial class Main
    {
        Texture2D tile;
        int selected_radio;
        bool isSAVselected = false;
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
            Raylib.InitWindow(1280, 800, "raylib - imgui - civ2");
            //Raylib.SetTargetFPS(60);
            rlImGui.Setup(true);
            Raylib.InitAudioDevice();

            //========== IMGUI STYLE
            ImGui.StyleColorsLight();
            var style = ImGui.GetStyle();
            style.Colors[(int)ImGuiCol.Text] = new Vector4(0, 0, 0, 1);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(1, 1, 1, 1);
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

            // Load some tile graphics
            int _frames = 0;
            var terrainGif = Raylib.LoadImageAnim(@"C:\\Program Files (x86)\\Civ2\\ICONS.GIF", out _frames);
            var tileImg1 = Raylib.ImageFromImage(terrainGif, new Rectangle(199, 322, 64, 32));
            var tileImg2 = Raylib.ImageFromImage(terrainGif, new Rectangle(298, 190, 32, 32));
            UI.tileTextureOuter = Raylib.LoadTextureFromImage(tileImg1);
            UI.tileTextureInner = Raylib.LoadTextureFromImage(tileImg2);
            Raylib.UnloadImage(terrainGif);
            Raylib.UnloadImage(tileImg1);
            Raylib.UnloadImage(tileImg2);

            UI.dialogPos = new Vector2(200, 200);  // initial dialog position
            UI.menuBar = new MenuBar();

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

                // IMGUI STUFF
                //rlImGui.Begin();
                //ImGui.ShowDemoWindow();
                _activeScreen.Draw(screenWidth, screenHeight);
                //ShowRadioIntroMenu();
                //rlImGui.End();

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, screenHeight - 20, 20, Raylib_cs.Color.BLACK);

                UI.MenuBar();
                UI.Dialog(new Vector2(332, 344), "Civilization II");

                Raylib.EndDrawing();
            }

            ShutdownApp();
        }

        public IUserInterface ActiveInterface { get; }

        public IList<IUserInterface> Interfaces { get; }

        void ShutdownApp()
        {
            rlImGui.Shutdown();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
