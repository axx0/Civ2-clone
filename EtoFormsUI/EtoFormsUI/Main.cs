using System;
using Eto.Drawing;
using Eto.Forms;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.MapObjects;
using EtoFormsUI.Cheat_menu;
using EtoFormsUI.GameModes;
using EtoFormsUI.Menu;
using Model;
using DialogResult = Eto.Forms.DialogResult;
using Order = EtoFormsUI.Players.Orders.Order;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private Game Game => Game.Instance;
        private Map Map => Game.CurrentMap;

        public IGameMode CurrentGameMode
        {
            get => _currentGameMode;
            set
            {
                if (value.Activate(_currentGameMode, CurrentPlayer))
                {
                    _currentGameMode = value;
                }
            }
        }

        private Processing Processing = new();
        internal MovingPieces Moving;
        internal ViewPiece ViewPiece;

        public PixelLayout layout;
        internal MapPanel mapPanel;
        private MinimapPanel minimapPanel;
        public StatusPanel StatusPanel { get; private set; }
        public List<Order> Orders { get; set; } = new();
        public IPlayer CurrentPlayer { get; set; }

        internal Dictionary<string, PopupBox> popupBoxList;
        public Sound Sounds;
        private List<Command> _cheatCommands;
        
        private ButtonMenuItem _ordersMenu;
        
        private IGameMode _currentGameMode;
        private Command _openLuaConsoleCommand;
        public event EventHandler<MapEventArgs> OnMapEvent;
        
        public Main()
        {
            this.Load += (_, _) =>
            {
                Content = layout;
                Sounds.PlayMenuLoop();
            };
            this.Shown += (_, _) => MainMenu();
            this.KeyDown += KeyPressedEvent;
            LoadInitialAssets();

            Title = ActiveInterface.Title;
            BackgroundColor = Color.FromArgb(143, 123, 99);
            WindowState = WindowState.Maximized;
            var iconPath = Utils.GetFilePath("civ2.ico", Settings.SearchPaths);
            if(iconPath != null)
            {
                Icon = new Icon(iconPath);
            }

            layout = new PixelLayout();

            var imgV = new ImageView { Image = Images.ExtractBitmap(DLLs.Tiles, "introScreenSymbol") };
            layout.Add(imgV, (int)Screen.PrimaryScreen.Bounds.Width / 2 - imgV.Image.Width / 2, 
                (int)Screen.PrimaryScreen.Bounds.Height / 2 - imgV.Image.Height / 2);


            // Game menu commands
            var GameOptionsCommand = new Command { MenuText = "Game Options", Shortcut = Keys.Control | Keys.O };
            GameOptionsCommand.Executed += (_, _) =>
            {
                var optionsDialog = new Civ2dialog(this, popupBoxList["GAMEOPTIONS"], new List<string> { "Patch xxx" }, checkboxOptionState: new List<bool> { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn, Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp, Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen });
                optionsDialog.ShowModal(this);
                Game.Options.SoundEffects = optionsDialog.CheckboxReturnStates[0];
                Game.Options.Music = optionsDialog.CheckboxReturnStates[1];
                Game.Options.AlwaysWaitAtEndOfTurn = optionsDialog.CheckboxReturnStates[2];
                Game.Options.AutosaveEachTurn = optionsDialog.CheckboxReturnStates[3];
                Game.Options.ShowEnemyMoves = optionsDialog.CheckboxReturnStates[4];
                Game.Options.NoPauseAfterEnemyMoves = optionsDialog.CheckboxReturnStates[5];
                Game.Options.FastPieceSlide = optionsDialog.CheckboxReturnStates[6];
                Game.Options.InstantAdvice = optionsDialog.CheckboxReturnStates[7];
                Game.Options.TutorialHelp = optionsDialog.CheckboxReturnStates[8];
                Game.Options.MoveUnitsWithoutMouse = optionsDialog.CheckboxReturnStates[9];
                Game.Options.EnterClosestCityScreen = optionsDialog.CheckboxReturnStates[10];
            };
            var GraphicOptionsCommand = new Command { MenuText = "Graphic Options", Shortcut = Keys.Control | Keys.P };
            GraphicOptionsCommand.Executed += (_, _) =>
            {
                var optionsDialog = new Civ2dialog(this, popupBoxList["GRAPHICOPTIONS"], checkboxOptionState: new List<bool> { Game.Options.ThroneRoomGraphics, Game.Options.DiplomacyScreenGraphics, Game.Options.AnimatedHeralds, Game.Options.CivilopediaForAdvances, Game.Options.HighCouncil, Game.Options.WonderMovies });
                optionsDialog.ShowModal(this);
                Game.Options.ThroneRoomGraphics = optionsDialog.CheckboxReturnStates[0];
                Game.Options.DiplomacyScreenGraphics = optionsDialog.CheckboxReturnStates[1];
                Game.Options.AnimatedHeralds = optionsDialog.CheckboxReturnStates[2];
                Game.Options.CivilopediaForAdvances = optionsDialog.CheckboxReturnStates[3];
                Game.Options.HighCouncil = optionsDialog.CheckboxReturnStates[4];
                Game.Options.WonderMovies = optionsDialog.CheckboxReturnStates[5];
            };
            var CityReportOptionsCommand = new Command { MenuText = "City Report Options", Shortcut = Keys.Control | Keys.E };
            CityReportOptionsCommand.Executed += (_, _) =>
            {
                var optionsDialog = new Civ2dialog(this, popupBoxList["MESSAGEOPTIONS"], checkboxOptionState: new List<bool> { Game.Options.WarnWhenCityGrowthHalted, Game.Options.ShowCityImprovementsBuilt, Game.Options.ShowNonCombatUnitsBuilt, Game.Options.ShowInvalidBuildInstructions, Game.Options.AnnounceCitiesInDisorder, Game.Options.AnnounceOrderRestored, Game.Options.AnnounceWeLoveKingDay, Game.Options.WarnWhenFoodDangerouslyLow, Game.Options.WarnWhenPollutionOccurs, Game.Options.WarnChangProductWillCostShields, Game.Options.ZoomToCityNotDefaultAction });
                optionsDialog.ShowModal(this);
                Game.Options.WarnWhenCityGrowthHalted = optionsDialog.CheckboxReturnStates[0];
                Game.Options.ShowCityImprovementsBuilt = optionsDialog.CheckboxReturnStates[1];
                Game.Options.ShowNonCombatUnitsBuilt = optionsDialog.CheckboxReturnStates[2];
                Game.Options.ShowInvalidBuildInstructions = optionsDialog.CheckboxReturnStates[3];
                Game.Options.AnnounceCitiesInDisorder = optionsDialog.CheckboxReturnStates[4];
                Game.Options.AnnounceOrderRestored = optionsDialog.CheckboxReturnStates[5];
                Game.Options.AnnounceWeLoveKingDay = optionsDialog.CheckboxReturnStates[6];
                Game.Options.WarnWhenFoodDangerouslyLow = optionsDialog.CheckboxReturnStates[7];
                Game.Options.WarnWhenPollutionOccurs = optionsDialog.CheckboxReturnStates[8];
                Game.Options.WarnChangProductWillCostShields = optionsDialog.CheckboxReturnStates[9];
                Game.Options.ZoomToCityNotDefaultAction = optionsDialog.CheckboxReturnStates[10];
            };
            var MultiplayerOptionsCommand = new Command { MenuText = "Multiplayer Options", Shortcut = Keys.Control | Keys.Y, Enabled = false };
            var GameProfileCommand = new Command { MenuText = "Game Profile", Enabled = false };
            var PickMusicCommand = new Command { MenuText = "Pick Music" };
            var SaveGameCommand = new Command { MenuText = "Save Game", Shortcut = Keys.Control | Keys.S };
            var LoadGameCommand = new Command { MenuText = "Load Game", Shortcut = Keys.Control | Keys.L };
            var JoinGameCommand = new Command { MenuText = "Join Game", Shortcut = Keys.Control | Keys.J, Enabled = false };
            var SetPasswordCommand = new Command { MenuText = "Set Password", Shortcut = Keys.Control | Keys.W };
            var ChangeTimerCommand = new Command { MenuText = "Change Timer", Shortcut = Keys.Control | Keys.T, Enabled = false };
            var RetireCommand = new Command { MenuText = "Retire", Shortcut = Keys.Control | Keys.R };
            var QuitCommand = new Command { MenuText = "Quit", Shortcut = Keys.Control | Keys.Q };
            QuitCommand.Executed += (sender, e) => Application.Instance.Quit();

            // Kingdom menu commands
            var TaxRateCommand = new Command { MenuText = "Tax Rate", Shortcut = Keys.Shift & Keys.T };
            TaxRateCommand.Executed += (_, _) =>
            {
                var taxWin = new TaxRateWindow(this);
                taxWin.ShowModal();
            };
            var ViewThroneRoomCommand = new Command { MenuText = "View Throne Room", Shortcut = Keys.Shift | Keys.H };
            var FindCityCommand = new Command { MenuText = "Find City", Shortcut = Keys.Shift | Keys.C };
            var RevolutionCommand = new Command { MenuText = "REVOLUTION", Shortcut = Keys.Shift | Keys.R };

            // View menu commands
            var MovePiecesCommand = new Command { MenuText = "Move Pieces", Shortcut = Keys.V };
            var ViewPiecesCommand = new Command { MenuText = "View Pieces", Shortcut = Keys.V };
            var ZoomInCommand = new Command { MenuText = "Zoom In", Shortcut = Keys.Z };
            var ZoomOutCommand = new Command { MenuText = "Zoom Out", Shortcut = Keys.X };
            var MaxZoomInCommand = new Command { MenuText = "Max Zoom In", Shortcut = Keys.Control | Keys.Z };
            var StandardZoomCommand = new Command { MenuText = "Standard Zoom", Shortcut = Keys.Shift | Keys.Z };
            var MediumZoomOutCommand = new Command { MenuText = "Medium Zoom Out", Shortcut = Keys.Shift | Keys.X };
            var MaxZoomOutCommand = new Command { MenuText = "Max Zoom Out", Shortcut = Keys.Control | Keys.X };
            var ShowMapGridCommand = new Command { MenuText = "Show Map Grid", Shortcut = Keys.Control | Keys.G };
            var ArrangeWindowsCommand = new Command { MenuText = "ArrangeWindows" };
            var ShowHiddenTerrainCommand = new Command { MenuText = "Show Hidden Terrain", Shortcut = Keys.T };
            var CenterViewCommand = new Command { MenuText = "Center View", Shortcut = Keys.C };

            // Orders menu commands


            // Advisors menu commands
            var ChatWithKingsCommand = new Command { MenuText = "Chat With Kings", Shortcut = Keys.Control | Keys.C };
            var ConsultHighCouncilCommand = new Command { MenuText = "Consult High Council" };
            var CityStatusCommand = new Command { MenuText = "City Status", Shortcut = Keys.F1 };
            CityStatusCommand.Executed += (_, _) =>
            {
                var win = new CityStatusWindow(this);
                win.Location = new Point((ClientSize.Width / 2) - (win.Width / 2), (ClientSize.Height / 2) - (win.Height / 2));
                win.Show();
            };
            var DefenseMinisterCommand = new Command { MenuText = "Defense Minister", Shortcut = Keys.F2 };
            DefenseMinisterCommand.Executed += (_, _) =>
            {
                var win = new DefenseMinisterWindow(this);
                win.Location = new Point((ClientSize.Width / 2) - (win.Width / 2), (ClientSize.Height / 2) - (win.Height / 2));
                win.Show();
            };
            var ForeignMinisterCommand = new Command { MenuText = "Foreign Minister", Shortcut = Keys.F3 };
            var AttitudeAdvisorCommand = new Command { MenuText = "Attitude Advisor", Shortcut = Keys.F4 };
            AttitudeAdvisorCommand.Executed += (_, _) =>
            {
                var win = new AttitudeAdvisorWindow(this);
                win.Location = new Point((ClientSize.Width / 2) - (win.Width / 2), (ClientSize.Height / 2) - (win.Height / 2));
                win.Show();
            };
            var TradeAdvisorCommand = new Command { MenuText = "Trade Advisor", Shortcut = Keys.F5 };
            TradeAdvisorCommand.Executed += (_, _) =>
            {
                var win = new TradeAdvisorWindow(this);
                win.Location = new Point((ClientSize.Width / 2) - (win.Width / 2), (ClientSize.Height / 2) - (win.Height / 2));
                win.Show();
            }; 
            var ScienceAdvisorCommand = new Command { MenuText = "Science Advisor", Shortcut = Keys.F6 };
            var CasualtyTimelineCommand = new Command { MenuText = "Casualty Timeline", Shortcut = Keys.Control | Keys.D };

            // World menu commands
            var WondersCommand = new Command { MenuText = "Wonders of the World", Shortcut = Keys.F7 };
            var Top5citiesCommand = new Command { MenuText = "Top 5 Cities", Shortcut = Keys.F8 };
            var CivScoreCommand = new Command { MenuText = "Civilization Score", Shortcut = Keys.F9 };
            var DemographicsCommand = new Command { MenuText = "Demographics", Shortcut = Keys.F11 };
            var SpaceshipsCommand = new Command { MenuText = "Spaceships", Shortcut = Keys.F12 };

            // Cheat menu commands
            var ToggleCheatModeCommand = new Command { MenuText = "Toggle Cheat Mode", Shortcut = Keys.Control | Keys.K };
            ToggleCheatModeCommand.Executed += (s, e) =>
            {
                if (!Game.Options.CheatPenaltyWarning)
                {
                    var dialog = new Civ2dialog(this, popupBoxList["REALLYCHEAT"]);

                    dialog.ShowModal(this);

                    if (dialog.SelectedButton != "OK" || dialog.SelectedIndex != 1)
                    {
                        return;
                    }

                    Game.Options.CheatPenaltyWarning = true;
                }

                Game.Options.CheatMenu = !Game.Options.CheatMenu;
                _cheatCommands.ForEach(c=>c.Enabled = Game.Options.CheatMenu);
            };
            
            var CreateUnitCommand = new Command { MenuText = "Create Unit", Shortcut = Keys.Shift | Keys.F1 };
            var RevealMapCommand = new Command { MenuText = "Reveal Map", Shortcut = Keys.Shift | Keys.F2 };
            RevealMapCommand.Executed += (sender, e) =>
            {
                var options = Game.ActiveCivs.Select(c => c.Adjective).ToList();
                options.AddRange(new string[] { "Entire Map", "No Special View" });
                var popupbox = new RevealMapPanel(this, options.ToArray());
                popupbox.ShowModal(Parent);
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.UpdateMap));
            };
            var SetHumanPlayerCommand = new Command { MenuText = "Set Human Player", Shortcut = Keys.Shift | Keys.F3, Enabled = false};
            var SetGameYearCommand = new Command { MenuText = "Set Game Year", Shortcut = Keys.Shift | Keys.F4, Enabled = false };
            var KillCivilizationCommand = new Command { MenuText = "Kill Civilization", Shortcut = Keys.Shift | Keys.F5, Enabled = false };
            var TechnologyAdvanceCommand = new Command { MenuText = "Technology Advance", Shortcut = Keys.Shift | Keys.F6, Enabled = false };
            var EditTechsCommand = new Command { MenuText = "Edit Technologies", Shortcut = Keys.Control | Keys.Shift | Keys.F6, Enabled = false };
            var ForceGovernmentCommand = new Command { MenuText = "Force Government", Shortcut = Keys.Shift | Keys.F7, Enabled = false };
            var ChangeTerrainCursorCommand = new Command { MenuText = "Change Terrain at Cursor", Shortcut = Keys.Shift | Keys.F8, Enabled = false };
            var DestroyUnitsCursorCommand = new Command { MenuText = "Destroy All Units At Cursor", Shortcut = Keys.Control | Keys.Shift | Keys.D, Enabled = false };
            var ChangeMoneyCommand = new Command { MenuText = "Change Money", Shortcut = Keys.Shift | Keys.F9, Enabled = false };
            var EditUnitCommand = new Command { MenuText = "Edit Unit", Shortcut = Keys.Control | Keys.Shift | Keys.U, Enabled = false };
            var EditCityCommand = new Command { MenuText = "Edit City", Shortcut = Keys.Control | Keys.Shift | Keys.C, Enabled = false };
            var EditKingCommand = new Command { MenuText = "Edit King", Shortcut = Keys.Control | Keys.Shift | Keys.K, Enabled = false };
            var ScenarioParamsCommand = new Command { MenuText = "Scenario Parameters", Shortcut = Keys.Control | Keys.Shift | Keys.P, Enabled = false };
            var SaveAsScenCommand = new Command { MenuText = "Save As Scenario", Shortcut = Keys.Control | Keys.Shift | Keys.S, Enabled = false };
            _openLuaConsoleCommand = new Command { MenuText = "Open Lua Console", Shortcut = Keys.Control | Keys.Shift | Keys.F3, Enabled = true };
            _openLuaConsoleCommand.Executed += (sender, args) =>
            {
                var luaConsole = new LuaConsoleDialog(this, Game.Options.CheatMenu);
                luaConsole.ShowModal();
            };
            
            _cheatCommands = new List<Command>
            {
                CreateUnitCommand,
                RevealMapCommand,
                SetHumanPlayerCommand,
                SetGameYearCommand,
                KillCivilizationCommand,
                TechnologyAdvanceCommand,
                EditTechsCommand,
                ForceGovernmentCommand,
                ChangeTerrainCursorCommand,
                DestroyUnitsCursorCommand,
                ChangeMoneyCommand,
                EditUnitCommand,
                EditCityCommand,
                EditKingCommand,
                ScenarioParamsCommand,
                SaveAsScenCommand,
                _openLuaConsoleCommand
            };
            // Editor menu commands
            var ToggleScenFlagCommand = new Command { MenuText = "Toggle Scenario Flag", Shortcut = Keys.Control | Keys.F };
            var AdvancesEditorCommand = new Command { MenuText = "Advances Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D1 };
            var CitiesEditorCommand = new Command { MenuText = "Cities Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D2 };
            var EffectsEditorCommand = new Command { MenuText = "Effects Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D3 };
            var ImprovEditorCommand = new Command { MenuText = "Improvements Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D4 };
            var TerrainEditorCommand = new Command { MenuText = "Terrain Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D5 };
            var TribeEditorCommand = new Command { MenuText = "Tribe Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D6 };
            var UnitsEditorCommand = new Command { MenuText = "Units Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D7 };
            var EventsEditorCommand = new Command { MenuText = "Events Editor", Shortcut = Keys.Control | Keys.Shift | Keys.D8 };

            // Civilopedia menu commands
            var CivAdvancesFlagCommand = new Command { MenuText = "Civilization Advances" };
            var CityImprovFlagCommand = new Command { MenuText = "City Improvements" };
            var WondersWorldCommand = new Command { MenuText = "Wonders of the World" };
            var MilitaryUnitsCommand = new Command { MenuText = "Military Units" };
            var GovernmentsCommand = new Command { MenuText = "Governments" };
            var TerrainTypesCommand = new Command { MenuText = "Terrain Types" };
            var GameConceptsCommand = new Command { MenuText = "Game Concepts" };
            var AboutCommand = new Command { MenuText = "About Civilization II" };

            _ordersMenu = new ButtonMenuItem
            {
                Text = "&Orders",
            };

            Menu = new MenuBar
            {
                Items =
                {
                    // File submenu
                    new ButtonMenuItem { Text = "&Game", Items = { GameOptionsCommand, GraphicOptionsCommand, CityReportOptionsCommand, MultiplayerOptionsCommand, GameProfileCommand, new SeparatorMenuItem(), PickMusicCommand, new SeparatorMenuItem(), SaveGameCommand, LoadGameCommand, JoinGameCommand, new SeparatorMenuItem(), SetPasswordCommand, ChangeTimerCommand, new SeparatorMenuItem(), RetireCommand, QuitCommand } },
                    new ButtonMenuItem { Text = "&Kingdom", Items = { TaxRateCommand, new SeparatorMenuItem(), ViewThroneRoomCommand, FindCityCommand, new SeparatorMenuItem(), RevolutionCommand } },
                    new ButtonMenuItem { Text = "&View", Items = { MovePiecesCommand, ViewPiecesCommand, new SeparatorMenuItem(), ZoomInCommand, ZoomOutCommand, new SeparatorMenuItem(), MaxZoomInCommand, StandardZoomCommand, MediumZoomOutCommand, MaxZoomOutCommand, new SeparatorMenuItem(), ShowMapGridCommand, ArrangeWindowsCommand, ShowHiddenTerrainCommand, CenterViewCommand } },
                    _ordersMenu,
                    new ButtonMenuItem { Text = "&Advisors", Items = { ChatWithKingsCommand, ConsultHighCouncilCommand, new SeparatorMenuItem(), CityStatusCommand, DefenseMinisterCommand, ForeignMinisterCommand, new SeparatorMenuItem(), AttitudeAdvisorCommand, TradeAdvisorCommand, ScienceAdvisorCommand, new SeparatorMenuItem(), CasualtyTimelineCommand } },
                    new ButtonMenuItem { Text = "&World", Items = { WondersCommand, Top5citiesCommand, CivScoreCommand, new SeparatorMenuItem(), DemographicsCommand, SpaceshipsCommand } },
                    new ButtonMenuItem { Text = "&Cheat", Items = { ToggleCheatModeCommand, new SeparatorMenuItem(), CreateUnitCommand, RevealMapCommand, SetHumanPlayerCommand, new SeparatorMenuItem(), SetGameYearCommand, KillCivilizationCommand, new SeparatorMenuItem(), TechnologyAdvanceCommand, EditTechsCommand, ForceGovernmentCommand, ChangeTerrainCursorCommand, DestroyUnitsCursorCommand, ChangeMoneyCommand, new SeparatorMenuItem(), EditUnitCommand, EditCityCommand, EditKingCommand, new SeparatorMenuItem(), ScenarioParamsCommand, SaveAsScenCommand, new SeparatorMenuItem(), _openLuaConsoleCommand } },
                    new ButtonMenuItem { Text = "&Editor", Items = { ToggleScenFlagCommand, new SeparatorMenuItem(), AdvancesEditorCommand, CitiesEditorCommand, EffectsEditorCommand, ImprovEditorCommand, TerrainEditorCommand, TribeEditorCommand, UnitsEditorCommand, EventsEditorCommand } },
                    new ButtonMenuItem { Text = "&Civilopedia", Items = { CivAdvancesFlagCommand, CityImprovFlagCommand, WondersWorldCommand, MilitaryUnitsCommand, new SeparatorMenuItem(), GovernmentsCommand, TerrainTypesCommand, new SeparatorMenuItem(), GameConceptsCommand, new SeparatorMenuItem(), AboutCommand } },
                },
            };

            // Make a sound player
            Sounds = new Sound();
        }
        
        private void KeyPressedEvent(object sender, KeyEventArgs e)
        {
            CurrentGameMode.HandleKeyPress(this, e);
        }

        // Load assets at start of Civ2 program
        private void LoadInitialAssets()
        {
            if (Settings.LoadConfigSettings())
            {
                if (!PromptForCivDirectory())
                {
                    Environment.Exit(0);
                }
            }

            Interfaces = Initialization.Helpers.LoadInterfaces();

            ActiveInterface = Initialization.Helpers.GetInterface(Settings.Civ2Path, Interfaces);
            
            if (ActiveInterface == null)
            {
                Environment.Exit(0);
            }

            ActiveInterface.Initialize();


            // Load images
            InterfaceUtils.ImportWallpapersFromIconsFile(Settings.Civ2Path);
            
            Labels.UpdateLabels(null);

            // Load popup boxes info (Game.txt)
            popupBoxList = PopupBoxReader.LoadPopupBoxes(Settings.Civ2Path, "game.txt");
        }

        public IUserInterface ActiveInterface { get; set; }

        public IList<IUserInterface> Interfaces { get; set; }

        private static bool PromptForCivDirectory()
        {
            var directoryFound = false;
            var browseButton = new Button { Text = "Browse for Directory" };

            var closeButton = new Button { Text = "Close" };
            var dialog = new Dialog
            {
                Padding = 10,
                Title = "Civ directory not found",
                Content = new Label { Text = "No civ directory specified. Click " },
                NegativeButtons = { closeButton },
                PositiveButtons = { browseButton }
            };
            browseButton.Command = new Command((_, _) =>
            {
                var fileDialog = new SelectFolderDialog();

                var result = fileDialog.ShowDialog(dialog);
                
                if (result == DialogResult.Ok && Settings.AddPath(fileDialog.Directory))
                {
                    directoryFound = true;
                }
                dialog.Close();
            });
            closeButton.Command = new Command((_, _) => { dialog.Close(); });
            dialog.ShowModal();
            return directoryFound;
        }

        private void SetupGameModes(Game game)
        {
            ViewPiece = new ViewPiece(game, this);
            Moving = new MovingPieces(this, game);
            CurrentGameMode = game.ActiveUnit != null ? Moving : ViewPiece;
        }

        public void TriggerMapEvent(MapEventArgs args)
        {
            OnMapEvent?.Invoke(this, args);
        }
    }
}
