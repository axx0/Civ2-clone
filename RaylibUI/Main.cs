using System.Numerics;
using Civ2engine;
using Civ2engine.MapObjects;
using Model;
using RaylibUI.Initialization;
using Raylib_CSharp;
using Raylib_CSharp.Windowing;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Audio;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Colors;

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
            Raylib.SetConfigFlags(ConfigFlags.Msaa4XHint| ConfigFlags.VSyncHint |
                                  ConfigFlags.ResizableWindow);
            Window.Init(1280, 800, "raylib - civ2");
            //Raylib.SetTargetFPS(60);
            AudioDevice.Init();

            Input.SetExitKey(KeyboardKey.F12);

            Shaders.Load();
            Helpers.LoadFonts();

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
                }, ShutdownApp);
            }

            //============ LOAD SOUNDS

            //prep this for a loop( should split that function out between loops and non loops)

        }

        private const float pulseTime = 30;
        
        private List<TimedEvent> _events = new List<TimedEvent>();
        
        public void RunLoop()
        {
            var counter = pulseTime;
            var pulse = false;

            while (!Window.ShouldClose() && !_shouldClose)
            {
                var frameTime = Time.GetFrameTime();
                Graphics.BeginDrawing();

                for (int i = 0; i < _events.Count; i++)
                {
                    _events[i].Remaining -= frameTime;
                    if (_events[i].Remaining < 0)
                    {
                        _events[i].Action();
                        _events.RemoveAt(i);
                        i--;
                    }
                }

                int screenHeight = Window.GetScreenHeight();

                 _activeScreen.Draw(pulse);

                Graphics.DrawText($"{Time.GetFPS()} FPS", 5, screenHeight - 20, 20, Color.Magenta);

                Graphics.EndDrawing();
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
            //Helpers.LoadFonts();
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
            Window.Close();
            AudioDevice.Close();
        }

        public void ReloadMain()
        {
            ActiveRuleSet = AllRuleSets.First(r => r.InterfaceIndex == _activeInterface.InterfaceIndex);
            TextureCache.Clear();
            ImageUtils.SetLook(_activeInterface);
            _activeScreen = new MainMenu(this,() => _shouldClose= true, StartGame, Soundman);
        }

        public void Schedule(string eventName, TimeSpan delay, Action action)
        {
            for (int i = 0; i < _events.Count; i++)
            {
                if (_events[i].Name == eventName)
                {
                    _events[i] = new TimedEvent(name: eventName, remaining: (float)delay.TotalSeconds, action: action);
                    return;
                }
            }
            _events.Add(new TimedEvent(name: eventName, remaining: (float)delay.TotalSeconds, action: action));
        }

        public void ClearSchedule(string eventName)
        {
            _events.RemoveAll(e => e.Name == eventName);
        }
    }

    internal class TimedEvent(string name, float remaining, Action action)
    {
        public float Remaining { get; set; } = remaining;
        public string Name { get; set; } = name;
        public Action Action { get; set; } = action;
    }
}
