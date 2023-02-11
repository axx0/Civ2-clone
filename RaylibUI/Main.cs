using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;

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
                    Raylib.ClearBackground(Color.WHITE);
                }
                else
                {
                    Raylib.ClearBackground(background.background);
                    Raylib.DrawTexture(background.CentreImage, (screenWidth- background.CentreImage.width)/2, (screenHeight-background.CentreImage.height)/2, Color.WHITE);
                }

                // IMGUI STUFF
                rlImGui.Begin();
                DrawMenuBar();
                ImGui.ShowDemoWindow();
                _activeScreen.Draw(screenWidth, screenHeight);
                //ShowRadioIntroMenu();
                rlImGui.End();

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, 25, 20, Raylib_cs.Color.BLACK);

                Raylib.EndDrawing();
            }

            ShutdownApp();
        }

        public IUserInterface ActiveInterface { get; }

        public IList<IUserInterface> Interfaces { get; }

        private void DrawMenuBar()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("Game"))
                {
                    if (ImGui.MenuItem("Game Options", "Ctrl+O")) { }
                    if (ImGui.MenuItem("Graphic Options", "Ctrl+P")) { }
                    if (ImGui.MenuItem("City Report Options", "Ctrl+E")) { }
                    if (ImGui.MenuItem("Multiplayer Options", "Ctrl+Y", false, false)) { }
                    if (ImGui.MenuItem("Game Profile", false)) { }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Pick Music")) { }
                    ImGui.Separator();
                    if (ImGui.MenuItem("Save Game", "Ctrl+S")) { }
                    if (ImGui.MenuItem("Load Game", "Ctrl+L")) { }
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Kingdom"))
                {
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("View"))
                {
                    ImGui.EndMenu();
                }
                if (ImGui.BeginMenu("Orders"))
                {
                    ImGui.EndMenu();
                }
                ImGui.EndMenuBar();
            }
        }

        void ShutdownApp()
        {
            rlImGui.Shutdown();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }
    }
}
