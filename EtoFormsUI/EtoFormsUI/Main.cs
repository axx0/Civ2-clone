using System;
using Eto.Drawing;
using Eto.Forms;
using Civ2engine;
using System.Diagnostics;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private Game Game => Game.Instance;
        private Map Map => Map.Instance;
        private readonly PixelLayout layout;
        private MapPanel mapPanel;
        private MinimapPanel minimapPanel;
        private StatusPanel statusPanel;
        private bool suppressKeyEvent;
        public bool ViewPieceMode { get; set; }
        public Sound Sounds;
        private int[] _activeXY;
        public int[] ActiveXY   // Coords of either active unit or view piece
        {
            get
            {
                if (!ViewPieceMode) _activeXY = new int[] { Game.GetActiveUnit.X, Game.GetActiveUnit.Y };
                return _activeXY;
            }
            set { _activeXY = value; }
        }

        public Main()
        {
            this.Load += LoadEvent;
            this.KeyDown += KeyPressedEvent;
            LoadInitialAssets();

            Title = "Civilization II Multiplayer Gold";
            BackgroundColor = Color.FromArgb(143, 123, 99);
            WindowState = WindowState.Maximized;
            Icon = new Icon(Settings.Civ2Path + "civ2.ico");
            suppressKeyEvent = false;

            layout = new PixelLayout();
            var image = new ImageView { Image = Images.MainScreenSymbol };
            layout.Add(image, (int)Screen.PrimaryScreen.Bounds.Width / 2 - Images.MainScreenSymbol.Width / 2, (int)Screen.PrimaryScreen.Bounds.Height / 2 - Images.MainScreenSymbol.Height / 2);

            // Game menu commands
            var GameOptionsCommand = new Command { MenuText = "Game Options", Shortcut = Keys.Control | Keys.O };
            var GraphicOptionsCommand = new Command { MenuText = "Graphic Options", Shortcut = Keys.Control | Keys.P };
            var CityReportOptionsCommand = new Command { MenuText = "City Report Options", Shortcut = Keys.Control | Keys.E };
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
            var TaxRateCommand = new Command { MenuText = "Tax Rate", Shortcut = Keys.Shift | Keys.T };
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
            var BuildRoadCommand = new Command { MenuText = "Build Road", Shortcut = Keys.R };
            var BuildIrrigationCommand = new Command { MenuText = "Build Irrigation", Shortcut = Keys.I };
            var BuildMinesCommand = new Command { MenuText = "Build Mines", Shortcut = Keys.M };
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

            // Advisors menu commands
            var ChatWithKingsCommand = new Command { MenuText = "Chat With Kings", Shortcut = Keys.Control | Keys.C };
            var ConsultHighCouncilCommand = new Command { MenuText = "Consult High Council" };
            var CityStatusCommand = new Command { MenuText = "City Status", Shortcut = Keys.F1 };
            var DefenseMinisterCommand = new Command { MenuText = "Defense Minister", Shortcut = Keys.F2 };
            var ForeignMinisterCommand = new Command { MenuText = "Foreign Minister", Shortcut = Keys.F3 };
            var AttitudeAdvisorCommand = new Command { MenuText = "Attitude Advisor", Shortcut = Keys.F4 };
            var TradeAdvisorCommand = new Command { MenuText = "Trade Advisor", Shortcut = Keys.F5 };
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
            var CreateUnitCommand = new Command { MenuText = "Toggle Cheat Mode", Shortcut = Keys.Shift | Keys.F1 };
            var RevealMapCommand = new Command { MenuText = "Reveal Map", Shortcut = Keys.Shift | Keys.F2 };
            var SetHumanPlayerCommand = new Command { MenuText = "Set Human Player", Shortcut = Keys.Shift | Keys.F3 };
            var SetGameYearCommand = new Command { MenuText = "Set Game Year", Shortcut = Keys.Shift | Keys.F4 };
            var KillCivilizationCommand = new Command { MenuText = "Kill Civilization", Shortcut = Keys.Shift | Keys.F5 };
            var TechnologyAdvanceCommand = new Command { MenuText = "Technology Advance", Shortcut = Keys.Shift | Keys.F6 };
            var EditTechsCommand = new Command { MenuText = "Edit Technologies", Shortcut = Keys.Control | Keys.Shift | Keys.F6 };
            var ForceGovernmentCommand = new Command { MenuText = "Force Government", Shortcut = Keys.Shift | Keys.F7 };
            var ChangeTerrainCursorCommand = new Command { MenuText = "Change Terrain at Cursor", Shortcut = Keys.Shift | Keys.F8 };
            var DestroyUnitsCursorCommand = new Command { MenuText = "Destroy All Units At Cursor", Shortcut = Keys.Control | Keys.Shift | Keys.D };
            var ChangeMoneyCommand = new Command { MenuText = "Change Money", Shortcut = Keys.Shift | Keys.F9 };
            var EditUnitCommand = new Command { MenuText = "Edit Unit", Shortcut = Keys.Control | Keys.Shift | Keys.U };
            var EditCityCommand = new Command { MenuText = "Edit City", Shortcut = Keys.Control | Keys.Shift | Keys.C };
            var EditKingCommand = new Command { MenuText = "Edit King", Shortcut = Keys.Control | Keys.Shift | Keys.K };
            var ScenarioParamsCommand = new Command { MenuText = "Scenario Parameters", Shortcut = Keys.Control | Keys.Shift | Keys.P };
            var SaveAsScenCommand = new Command { MenuText = "Save As Scenario", Shortcut = Keys.Control | Keys.Shift | Keys.S };

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

            Menu = new MenuBar
            {
                Items =
                {
                    // File submenu
                    new ButtonMenuItem { Text = "&Game", Items = { GameOptionsCommand, GraphicOptionsCommand, CityReportOptionsCommand, MultiplayerOptionsCommand, GameProfileCommand, PickMusicCommand, SaveGameCommand, LoadGameCommand, JoinGameCommand, SetPasswordCommand, ChangeTimerCommand, RetireCommand, QuitCommand } },
                    new ButtonMenuItem { Text = "&Kingdom", Items = { TaxRateCommand, ViewThroneRoomCommand, FindCityCommand, RevolutionCommand } },
                    new ButtonMenuItem { Text = "&View", Items = { MovePiecesCommand, ViewPiecesCommand, ZoomInCommand, ZoomOutCommand, MaxZoomInCommand, StandardZoomCommand, MediumZoomOutCommand, MaxZoomOutCommand, ShowMapGridCommand, ArrangeWindowsCommand, ShowHiddenTerrainCommand, CenterViewCommand } },
                    new ButtonMenuItem { Text = "&Orders", Items = { BuildRoadCommand, BuildIrrigationCommand, BuildMinesCommand, CleanPollutionCommand, PillageCommand, UnloadCommand, GoToCommand, ParadropCommand, AirliftCommand, GoHomeToNearestCityCommand, FortifyCommand, SleepCommand, DisbandCommand, ActivateUnitCommand, WaitCommand, SkipTurnCommand, EndPlayerTurn } },
                    new ButtonMenuItem { Text = "&Advisors", Items = { ChatWithKingsCommand, ConsultHighCouncilCommand, CityStatusCommand, DefenseMinisterCommand, ForeignMinisterCommand, AttitudeAdvisorCommand, TradeAdvisorCommand, ScienceAdvisorCommand, CasualtyTimelineCommand } },
                    new ButtonMenuItem { Text = "&World", Items = { WondersCommand, Top5citiesCommand, CivScoreCommand, DemographicsCommand, SpaceshipsCommand } },
                    new ButtonMenuItem { Text = "&Cheat", Items = { ToggleCheatModeCommand, CreateUnitCommand, RevealMapCommand, SetHumanPlayerCommand, SetGameYearCommand, KillCivilizationCommand, TechnologyAdvanceCommand, EditTechsCommand, ForceGovernmentCommand, ChangeTerrainCursorCommand, DestroyUnitsCursorCommand, ChangeMoneyCommand, EditUnitCommand, EditCityCommand, EditKingCommand, ScenarioParamsCommand, SaveAsScenCommand } },
                    new ButtonMenuItem { Text = "&Editor", Items = { ToggleScenFlagCommand, AdvancesEditorCommand, CitiesEditorCommand, EffectsEditorCommand, ImprovEditorCommand, TerrainEditorCommand, TribeEditorCommand, UnitsEditorCommand, EventsEditorCommand } },
                    new ButtonMenuItem { Text = "&Civilopedia", Items = { CivAdvancesFlagCommand, CityImprovFlagCommand, WondersWorldCommand, MilitaryUnitsCommand, GovernmentsCommand, TerrainTypesCommand, GameConceptsCommand, AboutCommand } },
                },
            };

            // Make a sound player
            Sounds = new Sound(Settings.Civ2Path);
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            ShowIntroScreen();
            Sounds.PlayMenuLoop();
        }

        // Load assets at start of Civ2 program
        private void LoadInitialAssets()
        {
            Settings.LoadConfigSettings();

            // Load images
            Images.LoadGraphicsAssetsAtIntroScreen();
        }
    }
}
