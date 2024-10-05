using Raylib_cs;
using System.Numerics;
using Civ2engine;
using Civ2engine.IO;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;
using RaylibUI.Forms;
using JetBrains.Annotations;
using System.Diagnostics;

namespace RaylibUI
{
    public partial class Main : IMain
    {
        private Map _map;

        private IScreen _activeScreen;
        private bool _shouldClose;


        private Sound Soundman;
        private IUserInterface _activeInterface;

        public Main()
        {
            var hasCivDir = Settings.LoadConfigSettings();

            //========= RAYLIB WINDOW SETTINGS
            Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.VSyncHint |
                                  ConfigFlags.ResizableWindow);
            Raylib.InitWindow(1280, 800, "raylib - civ2");
            //Raylib.SetTargetFPS(60);
            Raylib.InitAudioDevice();

            Raylib.SetExitKey(KeyboardKey.F12);

            Shaders.Load();

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
            Helpers.LoadFonts();
            Interfaces = Helpers.LoadInterfaces(this);
            AllRuleSets =  Interfaces.SelectMany((userInterface, idx) =>
                {
                    userInterface.InterfaceIndex = idx; 
                    var sets = userInterface.FindRuleSets(Settings.SearchPaths);
                    
                    foreach (var ruleset in sets)
                    {
                        ruleset.InterfaceIndex = idx;
                    }
                    return sets;
                })
                .ToArray();
            ActiveInterface = Helpers.GetInterface(Settings.Civ2Path, Interfaces, AllRuleSets);
            return new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }



        public IUserInterface ActiveInterface
        {
            get => _activeInterface;
            private set
            {
                if(value == _activeInterface) return;
                
                _activeInterface = value;
                
                ActiveRuleSet ??= AllRuleSets.First(r => r.InterfaceIndex == _activeInterface.InterfaceIndex);
                
                _activeInterface.Initialize();
                TextureCache.Clear();
                Labels.UpdateLabels(ActiveRuleSet);
                ImageUtils.SetLook(_activeInterface);
                Soundman?.Dispose();
                Soundman = new Sound(_activeInterface.Title);
                _activeScreen?.InterfaceChanged(Soundman);
            }
        }

        public IList<IUserInterface> Interfaces { get; set; }

        void ShutdownApp()
        {
            Shaders.Unload();
            Soundman?.Dispose();
            Raylib.CloseWindow();
            Raylib.CloseAudioDevice();
        }

        public void ReloadMain()
        {
            ActiveRuleSet = AllRuleSets.First(r => r.InterfaceIndex == _activeInterface.InterfaceIndex);
            TextureCache.Clear();
            ImageUtils.SetLook(_activeInterface);
            _activeScreen = new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }
    }
}
