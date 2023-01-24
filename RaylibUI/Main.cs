using Raylib_cs;
using rlImGui_cs;
using ImGuiNET;
using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;

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

        public Main()
        {
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

            //============ LOAD REQUIRED SAV GAME DATA
            LoadGame(savName);

            //============ LOAD SOUNDS
            //var sound = Raylib.LoadSound(Settings.Civ2Path + Path.DirectorySeparatorChar + "SOUND" + Path.DirectorySeparatorChar + "MENUOK.WAV");
            //Raylib.PlaySound(sound);

            while (!Raylib.WindowShouldClose())
            {
                MousePressedAction();
                KeyboardAction();

                Raylib.BeginDrawing();

                // Draw map & stuff
                DrawStuff();

                // IMGUI STUFF
                rlImGui.Begin();
                DrawMenuBar();
                ImGui.ShowDemoWindow();
                //ShowRadioIntroMenu();
                rlImGui.End();

                Raylib.DrawText($"{Raylib.GetFPS()} FPS", 5, 25, 20, Raylib_cs.Color.BLACK);

                Raylib.EndDrawing();
            }

            ShutdownApp();
        }

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

        private unsafe void ShowRadioIntroMenu()
        {
            fixed (int* ptr = &selected_radio) { };

            if (!isSAVselected)
            {
                if (ImGui.Begin("Radio btn"))
                {
                    if (ImGui.BeginTable("Table", 1))
                    {
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Start a New Game", ref selected_radio, 0)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Start on Premade World", ref selected_radio, 1)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Customize World", ref selected_radio, 2)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Begin Scenario", ref selected_radio, 3)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Load a Game", ref selected_radio, 4)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("Multiplayer Game", ref selected_radio, 5)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("View Hall of Fame", ref selected_radio, 6)) { }
                        ImGui.TableNextColumn();
                        if (ImGui.RadioButton("View Credits", ref selected_radio, 7)) { }

                        ImGui.EndTable();
                    }

                    ImGui.Text($"selected={selected_radio}");

                    if (ImGui.Button("OK") && selected_radio == 4)
                    {
                        isSAVselected = true;
                    }
                    if (ImGui.Button("Cancel")) 
                    {
                        ShutdownApp();
                    }


                    ImGui.End();
                }
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
