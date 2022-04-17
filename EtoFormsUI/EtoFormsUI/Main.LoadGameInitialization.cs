using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.ImageLoader;
using EtoFormsUI.Initialization;
using EtoFormsUI.Players.Orders;
using Order = EtoFormsUI.Players.Orders.Order;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        public bool LoadGameInitialization(Ruleset ruleset, string saveFileName)
        {
            var rules = RulesParser.ParseRules(ruleset);
            CityLoader.LoadCities(ruleset);
            // Read SAV file & RULES.txt
            CurrentPlayer = new LocalPlayer(this);

            ClassicSaveLoader.LoadSave(ruleset, saveFileName, rules, CurrentPlayer);
            Images.LoadGraphicsAssetsFromFiles(ruleset, rules);
            //ViewPiece.ActiveXY = gameData.ActiveCursorXY;
            return true;
        }

        public bool LoadScenarioInit(Ruleset ruleset, string scenarioFileName)
        {
            return false;
        }
        
        public bool StartPremadeInit(Ruleset ruleset, string mapFileName)
        {
            return NewGame.StartPreMade(this, ruleset, mapFileName);
        }

        public void StartGame()
        {
            SetupGameModes(Game.Instance);
            
            // Generate map tile graphics
            Images.MapTileGraphic = new Bitmap[Map.XDim, Map.YDim];
            for (var col = 0; col < Map.XDim; col++)
            {
                for (var row = 0; row < Map.YDim; row++)
                {
                    Images.MapTileGraphic[col, row] = Draw.MakeTileGraphic(Map.Tile[col, row], col, row, Game.Options.FlatEarth, MapImages.Terrains[Map.MapIndex]);
                }
            }
            
            foreach (MenuItem item in Menu.Items) item.Enabled = true;

            minimapPanel = new MinimapPanel(this, 262, 149);
            layout.Add(minimapPanel, ClientSize.Width - 262, 0);

            mapPanel = new MapPanel(this, ClientSize.Width - 262, ClientSize.Height);
            layout.Add(mapPanel, 0, 0);

            StatusPanel = new StatusPanel(this, 262, ClientSize.Height - 148);
            layout.Add(StatusPanel, ClientSize.Width - 262, 148);

            Content = layout;

            Game.OnPlayerEvent += (sender, e) =>
            {
                if (Game.GetActiveCiv == Game.GetPlayerCiv)
                {
                    CurrentGameMode = Game.GetActiveCiv.AnyUnitsAwaitingOrders ? Moving : ViewPiece;
                }
                else
                {
                    CurrentGameMode = Processing;
                }
            };
            _cheatCommands.ForEach(c =>
            {
                if (c != _openLuaConsoleCommand)
                {
                    c.Enabled = Game.Options.CheatMenu;
                }
            });

            SetupOrders(Game.Instance);
            
            BringToFront();
        }
        
        
        
        private void SetupOrders(Game instance)
        {
            var improvements = instance.TerrainImprovements;

            Orders = improvements.Select(i =>  new ImprovementOrder(i, this, instance).Update(CurrentPlayer.ActiveTile, CurrentPlayer.ActiveUnit)).ToList();
            

            var groupedOrders = Orders.GroupBy(o => o.Group);

            foreach (var groupedOrder in groupedOrders)
            {
                if (_ordersMenu.Items.Count > 0)
                {
                    _ordersMenu.Items.Add(new SeparatorMenuItem());
                }
                _ordersMenu.Items.AddRange(groupedOrder.Select(o=>o.Command));
            }
            
            
            // var BuildRoadCommand = new Command { MenuText = "Build Road", Shortcut = Keys.R };
            // var BuildIrrigationCommand = new Command { MenuText = "Build Irrigation", Shortcut = Keys.I };
            // var BuildMinesCommand = new Command { MenuText = "Build Mines", Shortcut = Keys.M };
            var CleanPollutionCommand = new Command { MenuText = "Clean Up Pollution", Shortcut = Keys.P };
            var PillageCommand = new Command { MenuText = "Pillage", Shortcut = Keys.Shift | Keys.P };
            var UnloadCommand = new Command { MenuText = "Unload", Shortcut = Keys.U };
            var GoToCommand = new Command { MenuText = "Go To", Shortcut = Keys.G };
            var ParadropCommand = new Command { MenuText = "Paradrop", Shortcut = Keys.P };
            var AirliftCommand = new Command { MenuText = "Airlift", Shortcut = Keys.L };
            var GoHomeToNearestCityCommand = new Command { MenuText = "Go Home To Nearest City", Shortcut = Keys.H };
            var FortifyCommand = new Command { MenuText = "Fortify", Shortcut = Keys.F };
            var SleepCommand = new Command { MenuText = "Sleep", Shortcut = Keys.S };
            var DisbandCommand = new Command { MenuText = "Disband", Shortcut = Keys.Shift | Keys.D };
            var ActivateUnitCommand = new Command { MenuText = "Activate Unit", Shortcut = Keys.A };
            var WaitCommand = new Command { MenuText = "Wait", Shortcut = Keys.W };
            var SkipTurnCommand = new Command { MenuText = "Skip Turn", Shortcut = Keys.Space };
            var EndPlayerTurn = new Command { MenuText = "End Player Turn", Shortcut = Keys.Control | Keys.N };

            _ordersMenu.Items.AddRange( new MenuItem[]
            {
                new SeparatorMenuItem(),
                CleanPollutionCommand, PillageCommand, new SeparatorMenuItem(), UnloadCommand, GoToCommand,
                ParadropCommand, AirliftCommand, GoHomeToNearestCityCommand, FortifyCommand, SleepCommand,
                new SeparatorMenuItem(), DisbandCommand, ActivateUnitCommand, WaitCommand, SkipTurnCommand,
                new SeparatorMenuItem(), EndPlayerTurn
            });
        }
    }
}
