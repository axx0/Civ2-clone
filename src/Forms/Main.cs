using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using civ2.Enums;
using civ2.Events;
using civ2.Bitmaps;
using civ2.Sounds;

namespace civ2.Forms
{
    public partial class Main : Form
    {
        private Game Game => Game.Instance;
        private Map Map => Map.Instance;

        private MenuStrip MainMenuStrip;
        private PictureBox _mainPic;
        private MapPanel _mapPanel;
        private _MinimapPanel _minimapPanel;
        private StatusPanel _statusPanel;
        private readonly ToolStripMenuItem _buildMinesChangeForestItem, _cleanUpPollutionItem, _pillageItem, _unloadItem, _goToItem, _goHomeToNearestCityItem, _fortifyItem, _sleepItem, _disbandItem, _maxZoomInItem, _maxZoomOutItem, _activateUnitItem, _waitItem, _skipTurnItem, _endPlayerTurnItem, _buildNewCityItem, _automateSettlerItem, _paradropItem, _showMapGridItem, _ordersMenu;
        private readonly List<ToolStripItem> _settlerItems, _noSettlerItems;
        private readonly Civ2ToolStripMenuItem _taxRateItem, _viewThroneRoomItem, _findCityItem, _revolutionItem, _buildRoadItem, _buildIrrigationItem, _movePieceItem, _viewPieceItem, _zoomInItem, _zoomOutItem, _standardZoomItem, _mediumZoomOutItem, _arrangeWindowsItem, _showHiddenTerrainItem, _centerViewItem;
        public bool AreWeInIntroScreen, LoadGameCalled;
        public bool ViewPieceMode { get; set; }
        public static event EventHandler<MapEventArgs> OnMapEvent;

        public Main()
        {
            LoadInitialAssets();

            #region INITIAL SETTINGS
            InitializeComponent();
            Text = "Civilization II Multiplayer Gold";
            BackColor = Color.FromArgb(143, 123, 99);
            this.Icon = Images.Civ2Icon;
            WindowState = FormWindowState.Maximized;

            // Symbol image in the center of screen
            var backimage = Images.MainScreenSymbol;
            _mainPic = new PictureBox
            {
                Image = backimage,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.None,
                Location = new Point((ClientSize.Width / 2) - (backimage.Width / 2), (ClientSize.Height / 2) - (backimage.Height / 2))
            };
            Controls.Add(_mainPic);
            _mainPic.SendToBack();
            #endregion

            #region MENUS
            MainMenuStrip = new MenuStrip
            {
                BackColor = Color.White
            };
            Controls.Add(MainMenuStrip);
            // Game menu
            var GameMenu = new ToolStripMenuItem("Game");
            var GameOptionsItem = new ToolStripMenuItem("Game Options", null, GameOptions_Click, (Keys)Shortcut.CtrlO);
            var GraphicOptionsItem = new ToolStripMenuItem("Graphic Options", null, GraphicOptions_Click, (Keys)Shortcut.CtrlP);
            var CityReportOptionsItem = new ToolStripMenuItem("City Report Options", null, CityReportOptions_Click, (Keys)Shortcut.CtrlE);
            var MultiplayerOptionsItem = new ToolStripMenuItem("Multiplayer Options", null, MultiplayerOptions_Click, (Keys)Shortcut.CtrlY);
            var GameProfileItem = new ToolStripMenuItem("Game Profile", null, GameProfile_Click);
            var PickMusicItem = new ToolStripMenuItem("Pick Music", null, PickMusic_Click);
            var SaveGameItem = new ToolStripMenuItem("Save Game", null, SaveGame_Click, (Keys)Shortcut.CtrlS);
            var LoadGameItem = new ToolStripMenuItem("Load Game", null, LoadGame_Click, (Keys)Shortcut.CtrlL);
            var JoinGameItem = new ToolStripMenuItem("Join Game", null, JoinGame_Click, (Keys)Shortcut.CtrlJ);
            var SetPasswordItem = new ToolStripMenuItem("Set Password", null, SetPassword_Click, (Keys)Shortcut.CtrlW);
            var ChangeTimerItem = new ToolStripMenuItem("Change Timer", null, ChangeTimer_Click, (Keys)Shortcut.CtrlT);
            var RetireItem = new ToolStripMenuItem("Retire", null, Retire_Click, (Keys)Shortcut.CtrlR);
            var QuitItem = new ToolStripMenuItem("Quit", null, Quit_Click, (Keys)Shortcut.CtrlQ);
            MainMenuStrip.Items.Add(GameMenu);
            GameMenu.DropDownItems.Add(GameOptionsItem);
            GameMenu.DropDownItems.Add(GraphicOptionsItem);
            GameMenu.DropDownItems.Add(CityReportOptionsItem);
            GameMenu.DropDownItems.Add(MultiplayerOptionsItem);
            GameMenu.DropDownItems.Add(GameProfileItem);
            GameMenu.DropDownItems.Add(new ToolStripSeparator());
            GameMenu.DropDownItems.Add(PickMusicItem);
            GameMenu.DropDownItems.Add(new ToolStripSeparator());
            GameMenu.DropDownItems.Add(SaveGameItem);
            GameMenu.DropDownItems.Add(LoadGameItem);
            GameMenu.DropDownItems.Add(JoinGameItem);
            GameMenu.DropDownItems.Add(new ToolStripSeparator());
            GameMenu.DropDownItems.Add(SetPasswordItem);
            GameMenu.DropDownItems.Add(ChangeTimerItem);
            GameMenu.DropDownItems.Add(new ToolStripSeparator());
            GameMenu.DropDownItems.Add(RetireItem);
            GameMenu.DropDownItems.Add(QuitItem);

            // View menu
            var ViewMenu = new ToolStripMenuItem("View");
            _movePieceItem = new Civ2ToolStripMenuItem("Move Pieces", MovePieces_Click, "v");
            _viewPieceItem = new Civ2ToolStripMenuItem("View Pieces", ViewPieces_Click, "v");
            _zoomInItem = new Civ2ToolStripMenuItem("Zoom In", ZoomIn_Click, "z");
            _zoomOutItem = new Civ2ToolStripMenuItem("Zoom Out", ZoomOut_Click, "x");
            _maxZoomInItem = new ToolStripMenuItem("Max Zoom In", null, MaxZoomIn_Click, (Keys)Shortcut.CtrlZ);
            _standardZoomItem = new Civ2ToolStripMenuItem("Standard Zoom", StandardZoom_Click, "Shift+Z");
            _mediumZoomOutItem = new Civ2ToolStripMenuItem("Medium Zoom Out", MediumZoomOut_Click, "Shift+X");
            _maxZoomOutItem = new ToolStripMenuItem("Max Zoom Out", null, MaxZoomOut_Click, (Keys)Shortcut.CtrlX);
            _showMapGridItem = new ToolStripMenuItem("Show Map Grid", null, ShowMapGrid_Click, (Keys)Shortcut.CtrlG);
            _arrangeWindowsItem = new Civ2ToolStripMenuItem("Arrange Windows", ArrangeWindows_Click, "");
            _showHiddenTerrainItem = new Civ2ToolStripMenuItem("Show Hidden Terrain", ShowHiddenTerrain_Click, "t");
            _centerViewItem = new Civ2ToolStripMenuItem("Center View", CenterView_Click, "c");
            MainMenuStrip.Items.Add(ViewMenu);
            ViewMenu.DropDownItems.Add(_movePieceItem);
            ViewMenu.DropDownItems.Add(_viewPieceItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(_zoomInItem);
            ViewMenu.DropDownItems.Add(_zoomOutItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(_maxZoomInItem);
            ViewMenu.DropDownItems.Add(_standardZoomItem);
            ViewMenu.DropDownItems.Add(_mediumZoomOutItem);
            ViewMenu.DropDownItems.Add(_maxZoomOutItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(_showMapGridItem);
            ViewMenu.DropDownItems.Add(_arrangeWindowsItem);
            ViewMenu.DropDownItems.Add(_showHiddenTerrainItem);
            ViewMenu.DropDownItems.Add(_centerViewItem);

            // Kingdom menu
            var KingdomMenu = new ToolStripMenuItem("Kingdom");
            _taxRateItem = new Civ2ToolStripMenuItem("Tax Rate", TaxRate_Click, "Shift+T");
            _viewThroneRoomItem = new Civ2ToolStripMenuItem("View Throne Room", ViewThroneRoom_Click, "Shift+H");
            _findCityItem = new Civ2ToolStripMenuItem("Find City", FindCity_Click, "Shift+C");
            _revolutionItem = new Civ2ToolStripMenuItem("REVOLUTION", Revolution_Click, "Shift+R");
            MainMenuStrip.Items.Add(KingdomMenu);
            KingdomMenu.DropDownItems.Add(_taxRateItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(_viewThroneRoomItem);
            KingdomMenu.DropDownItems.Add(_findCityItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(_revolutionItem);

            // Orders
            _ordersMenu = new ToolStripMenuItem("Orders");
            _buildRoadItem = new Civ2ToolStripMenuItem("Build Road", BuildRoad_Click, "r");
            _buildIrrigationItem = new Civ2ToolStripMenuItem("Build Irrigation", BuildIrrigation_Click, "i");
            _buildMinesChangeForestItem = new ToolStripMenuItem("Build Mines", null, BuildMinesChangeForest_Click);
            _cleanUpPollutionItem = new ToolStripMenuItem("Clean Up Pollution", null, CleanUpPollution_Click);
            _pillageItem = new ToolStripMenuItem("Pillage", null, Pillage_Click);
            _unloadItem = new ToolStripMenuItem("Unload", null, Unload_Click);
            _goToItem = new ToolStripMenuItem("Go To", null, GoTo_Click);
            _goHomeToNearestCityItem = new ToolStripMenuItem("Go Home To Nearest City", null, GoHomeToNearestCity_Click);
            _fortifyItem = new ToolStripMenuItem("Fortify", null, Fortify_Click);
            _sleepItem = new ToolStripMenuItem("Sleep", null, Sleep_Click);
            _disbandItem = new ToolStripMenuItem("Disband", null, Disband_Click);
            _activateUnitItem = new ToolStripMenuItem("Activate Unit", null, ActivateUnit_Click);
            _waitItem = new Civ2ToolStripMenuItem("Wait", Wait_Click, "w");
            _skipTurnItem = new Civ2ToolStripMenuItem("Skip Turn", SkipTurn_Click, "SPACE");
            _endPlayerTurnItem = new ToolStripMenuItem("End Player Turn", null, EndPlayerTurn_Click, (Keys)Shortcut.CtrlN);
            _buildNewCityItem = new ToolStripMenuItem("Build New City", null, BuildNewCity_Click);   //Settlers only items
            _automateSettlerItem = new ToolStripMenuItem("Automate Settler", null, AutomateSettler_Click);
            _paradropItem = new ToolStripMenuItem("Paradrop", null, Paradrop_Click);   //Paratroopers only item
            MainMenuStrip.Items.Add(_ordersMenu);

            _settlerItems = new List<ToolStripItem> { _buildNewCityItem, _buildRoadItem, _buildIrrigationItem, _buildMinesChangeForestItem, new ToolStripSeparator(), _automateSettlerItem, _cleanUpPollutionItem, new ToolStripSeparator(), _goToItem, _goHomeToNearestCityItem, new ToolStripSeparator(), _sleepItem, new ToolStripSeparator(), _disbandItem, _activateUnitItem, _waitItem, _skipTurnItem, new ToolStripSeparator(), _endPlayerTurnItem };

            _noSettlerItems = new List<ToolStripItem> { _buildRoadItem, _buildIrrigationItem, _buildMinesChangeForestItem, new ToolStripSeparator(), _cleanUpPollutionItem, _pillageItem, new ToolStripSeparator(), _unloadItem, _goToItem, _paradropItem, _goHomeToNearestCityItem, new ToolStripSeparator(), _fortifyItem, _sleepItem, new ToolStripSeparator(), _disbandItem, _activateUnitItem, _waitItem, _skipTurnItem, new ToolStripSeparator(), _endPlayerTurnItem };

            _ordersMenu.DropDownItems.Add(_buildNewCityItem);
            _ordersMenu.DropDownItems.Add(_buildRoadItem);
            _ordersMenu.DropDownItems.Add(_buildIrrigationItem);
            _ordersMenu.DropDownItems.Add(_buildMinesChangeForestItem);
            _ordersMenu.DropDownItems.Add(new ToolStripSeparator());
            _ordersMenu.DropDownItems.Add(_automateSettlerItem);
            _ordersMenu.DropDownItems.Add(_cleanUpPollutionItem);
            _ordersMenu.DropDownItems.Add(_pillageItem);
            _ordersMenu.DropDownItems.Add(new ToolStripSeparator());
            _ordersMenu.DropDownItems.Add(_unloadItem);
            _ordersMenu.DropDownItems.Add(_goToItem);
            _ordersMenu.DropDownItems.Add(_paradropItem);
            _ordersMenu.DropDownItems.Add(_goHomeToNearestCityItem);
            _ordersMenu.DropDownItems.Add(new ToolStripSeparator());
            _ordersMenu.DropDownItems.Add(_fortifyItem);
            _ordersMenu.DropDownItems.Add(_sleepItem);
            _ordersMenu.DropDownItems.Add(new ToolStripSeparator());
            _ordersMenu.DropDownItems.Add(_disbandItem);
            _ordersMenu.DropDownItems.Add(_activateUnitItem);
            _ordersMenu.DropDownItems.Add(_waitItem);
            _ordersMenu.DropDownItems.Add(_skipTurnItem);
            _ordersMenu.DropDownItems.Add(new ToolStripSeparator());
            _ordersMenu.DropDownItems.Add(_endPlayerTurnItem);

            // Advisors menu
            var AdvisorsMenu = new ToolStripMenuItem("Advisors");
            var ChatWithKingsItem = new ToolStripMenuItem("Chat with Kings", null, ChatWithKings_Click, (Keys)Shortcut.CtrlC);
            var ConsultHighCouncilItem = new ToolStripMenuItem("Consult High Council", null, ConsultHighCouncil_Click);
            var CityStatusItem = new ToolStripMenuItem("City Status", null, CityStatus_Click, (Keys)Shortcut.F1);
            var DefenseMinisterItem = new ToolStripMenuItem("Defense Minister", null, DefenseMinister_Click, (Keys)Shortcut.F2);
            var ForeignMinisterItem = new ToolStripMenuItem("Foreign Minister", null, ForeignMinister_Click, (Keys)Shortcut.F3);
            var AttitudeAdvisorItem = new ToolStripMenuItem("Attitude Advisor", null, AttitudeAdvisor_Click, (Keys)Shortcut.F4);
            var TradeAdvisorItem = new ToolStripMenuItem("Trade Advisor", null, TradeAdvisor_Click, (Keys)Shortcut.F5);
            var ScienecAdvisorItem = new ToolStripMenuItem("Science Advisor", null, ScienceAdvisor_Click, (Keys)Shortcut.F6);
            var CasualtyTimelineItem = new ToolStripMenuItem("Casualty Timeline", null, CasualtyTimeline_Click, (Keys)Shortcut.CtrlD);
            MainMenuStrip.Items.Add(AdvisorsMenu);
            AdvisorsMenu.DropDownItems.Add(ChatWithKingsItem);
            AdvisorsMenu.DropDownItems.Add(ConsultHighCouncilItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(CityStatusItem);
            AdvisorsMenu.DropDownItems.Add(DefenseMinisterItem);
            AdvisorsMenu.DropDownItems.Add(ForeignMinisterItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(AttitudeAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(TradeAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(ScienecAdvisorItem);
            AdvisorsMenu.DropDownItems.Add(new ToolStripSeparator());
            AdvisorsMenu.DropDownItems.Add(CasualtyTimelineItem);

            // World menu
            var WorldMenu = new ToolStripMenuItem("World");
            var WondersOfWorldItem = new ToolStripMenuItem("Wonders of the World", null, WondersOfWorld_Click, (Keys)Shortcut.F7);
            var Top5CitiesItem = new ToolStripMenuItem("Top 5 Cities", null, Top5Cities_Click, (Keys)Shortcut.F8);
            var CivScoreItem = new ToolStripMenuItem("Civilization Score", null, CivScore_Click, (Keys)Shortcut.F9);
            var DemographicsItem = new ToolStripMenuItem("Demographics", null, Demographics_Click, (Keys)Shortcut.F11);
            var SpaceshipsItem = new ToolStripMenuItem("Spaceships", null, Spaceships_Click, (Keys)Shortcut.F12);
            MainMenuStrip.Items.Add(WorldMenu);
            WorldMenu.DropDownItems.Add(WondersOfWorldItem);
            WorldMenu.DropDownItems.Add(Top5CitiesItem);
            WorldMenu.DropDownItems.Add(CivScoreItem);
            WorldMenu.DropDownItems.Add(new ToolStripSeparator());
            WorldMenu.DropDownItems.Add(DemographicsItem);
            WorldMenu.DropDownItems.Add(SpaceshipsItem);

            // Cheat menu
            var CheatMenu = new ToolStripMenuItem("Cheat");
            var ToggleCheatModeItem = new ToolStripMenuItem("Toggle Cheat Mode", null, ToggleCheatMode_Click, (Keys)Shortcut.CtrlK);
            var CreateUnitItem = new ToolStripMenuItem("Create Unit", null, CreateUnit_Click, (Keys)Shortcut.ShiftF1);
            var RevealMapItem = new ToolStripMenuItem("Reveal Map", null, RevealMap_Click, (Keys)Shortcut.ShiftF2);
            var SetHumanPlayerItem = new ToolStripMenuItem("Set Human Player", null, SetHumanPlayer_Click, (Keys)Shortcut.ShiftF3);
            var SetGameYearItem = new ToolStripMenuItem("Set Game Year", null, SetGameYear_Click, (Keys)Shortcut.ShiftF4);
            var KillCivilizationItem = new ToolStripMenuItem("Kill Civilization", null, KillCivilization_Click, (Keys)Shortcut.ShiftF5);
            var TechnologyAdvanceItem = new ToolStripMenuItem("Technology Advance", null, TechnologyAdvance_Click, (Keys)Shortcut.ShiftF6);
            var EditTechnologiesItem = new ToolStripMenuItem("Edit Technologies", null, EditTechnologies_Click, (Keys)Shortcut.CtrlShiftF6);
            var ForceGovernmentItem = new ToolStripMenuItem("Force Government", null, ForceGovernment_Click, (Keys)Shortcut.ShiftF7);
            var ChangeTerrainAtCursorItem = new ToolStripMenuItem("Change Terrain At Cursor", null, ChangeTerrainAtCursor_Click, (Keys)Shortcut.ShiftF8);
            var DestroyAllUnitsAtCursorItem = new ToolStripMenuItem("Destroy All Units At Cursor", null, DestroyAllUnitsAtCursor_Click, (Keys)Shortcut.CtrlShiftD);
            var ChangeMoneyItem = new ToolStripMenuItem("Change Money", null, ChangeMoney_Click, (Keys)Shortcut.ShiftF9);
            var EditUnitItem = new ToolStripMenuItem("Edit Unit", null, EditUnit_Click, (Keys)Shortcut.CtrlShiftU);
            var EditCityItem = new ToolStripMenuItem("Edit City", null, EditCity_Click, (Keys)Shortcut.CtrlShiftC);
            var EditKingItem = new ToolStripMenuItem("Edit King", null, EditKing_Click, (Keys)Shortcut.CtrlShiftK);
            var ScenarioParametersItem = new ToolStripMenuItem("Scenario Parameters", null, ScenarioParameters_Click, (Keys)Shortcut.CtrlShiftP);
            var SaveAsScenarioItem = new ToolStripMenuItem("Save As Scenario", null, SaveAsScenario_Click, (Keys)Shortcut.CtrlShiftS);
            MainMenuStrip.Items.Add(CheatMenu);
            CheatMenu.DropDownItems.Add(ToggleCheatModeItem);
            CheatMenu.DropDownItems.Add(new ToolStripSeparator());
            CheatMenu.DropDownItems.Add(CreateUnitItem);
            CheatMenu.DropDownItems.Add(RevealMapItem);
            CheatMenu.DropDownItems.Add(SetGameYearItem);
            CheatMenu.DropDownItems.Add(new ToolStripSeparator());
            CheatMenu.DropDownItems.Add(SetGameYearItem);
            CheatMenu.DropDownItems.Add(KillCivilizationItem);
            CheatMenu.DropDownItems.Add(new ToolStripSeparator());
            CheatMenu.DropDownItems.Add(TechnologyAdvanceItem);
            CheatMenu.DropDownItems.Add(EditTechnologiesItem);
            CheatMenu.DropDownItems.Add(ForceGovernmentItem);
            CheatMenu.DropDownItems.Add(ChangeTerrainAtCursorItem);
            CheatMenu.DropDownItems.Add(DestroyAllUnitsAtCursorItem);
            CheatMenu.DropDownItems.Add(ChangeMoneyItem);
            CheatMenu.DropDownItems.Add(new ToolStripSeparator());
            CheatMenu.DropDownItems.Add(EditUnitItem);
            CheatMenu.DropDownItems.Add(EditCityItem);
            CheatMenu.DropDownItems.Add(EditKingItem);
            CheatMenu.DropDownItems.Add(new ToolStripSeparator());
            CheatMenu.DropDownItems.Add(ScenarioParametersItem);
            CheatMenu.DropDownItems.Add(SaveAsScenarioItem);

            // Disable some item
            MultiplayerOptionsItem.Enabled = false;
            GameProfileItem.Enabled = false;
            JoinGameItem.Enabled = false;
            ChangeTimerItem.Enabled = false;
            #endregion
        }

        private void Main_Load(object sender, EventArgs e)
        {
            ShowIntroScreen();
        }

        // Load assets at start of Civ2 program
        private void LoadInitialAssets()
        {
            // Load images
            Images.LoadGraphicsAssetsAtIntroScreen();

            // Load sounds
            // ...
        }

        #region GAME MENU EVENTS
        private void GameOptions_Click(object sender, EventArgs e)
        {
            var GameOptionsPanel = new GameOptionsPanel(this, 746, 440);
            Controls.Add(GameOptionsPanel);
            GameOptionsPanel.Location = new Point((ClientSize.Width / 2) - (GameOptionsPanel.Size.Width / 2), (ClientSize.Height / 2) - (GameOptionsPanel.Size.Height / 2));
            GameOptionsPanel.Show();
            GameOptionsPanel.BringToFront();
        }

        private void GraphicOptions_Click(object sender, EventArgs e)
        {
            var GraphicOptionsPanel = new GraphicOptionsPanel(this, 746, 280);
            Controls.Add(GraphicOptionsPanel);
            GraphicOptionsPanel.Location = new Point((ClientSize.Width / 2) - (GraphicOptionsPanel.Size.Width / 2), (ClientSize.Height / 2) - (GraphicOptionsPanel.Size.Height / 2));
            GraphicOptionsPanel.Show();
            GraphicOptionsPanel.BringToFront();
        }

        private void CityReportOptions_Click(object sender, EventArgs e)
        {
            var CityreportOptionsPanel = new CityreportOptionsPanel(this, 746, 440);
            Controls.Add(CityreportOptionsPanel);
            CityreportOptionsPanel.Location = new Point((ClientSize.Width / 2) - (CityreportOptionsPanel.Size.Width / 2), (ClientSize.Height / 2) - (CityreportOptionsPanel.Size.Height / 2));
            CityreportOptionsPanel.Show();
            CityreportOptionsPanel.BringToFront();
        }

        private void MultiplayerOptions_Click(object sender, EventArgs e) { }
        private void GameProfile_Click(object sender, EventArgs e) { }
        private void PickMusic_Click(object sender, EventArgs e) { }
        private void SaveGame_Click(object sender, EventArgs e) { }

        private void LoadGame_Click(object sender, EventArgs e) {
            //LoadGameCalled = true;
            //ChoiceMenu.ChoseResult(); 
        }

        private void JoinGame_Click(object sender, EventArgs e) { }
        private void SetPassword_Click(object sender, EventArgs e) { }
        private void ChangeTimer_Click(object sender, EventArgs e) { }
        private void Retire_Click(object sender, EventArgs e) { }
        private void Quit_Click(object sender, EventArgs e) {
            //ShowIntroScreen(); 
            Close(); }
        #endregion
        #region VIEW MENU EVENTS
        private void MovePieces_Click(object sender, EventArgs e)
        {
            ViewPieceMode = false;
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
            _viewPieceItem.Enabled = true;
            _movePieceItem.Enabled = false;
        }

        private void ViewPieces_Click(object sender, EventArgs e)
        {
            ViewPieceMode = true;
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
            _movePieceItem.Enabled = true;
            _viewPieceItem.Enabled = false;
        }

        private void ZoomIn_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != 8)
            {
                Game.Zoom++;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void ZoomOut_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != -7)
            {
                Game.Zoom--;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void MaxZoomIn_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != 8)
            {
                Game.Zoom = 8;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void MaxZoomOut_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != -7)
            {
                Game.Zoom = -7;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void StandardZoom_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != 0)
            {
                Game.Zoom = 0;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void MediumZoomOut_Click(object sender, EventArgs e)
        {
            if (Game.Zoom != -4)
            {
                Game.Zoom = -4;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ZoomChanged));
            }
        }

        private void ShowMapGrid_Click(object sender, EventArgs e)
        {
            Game.Options.Grid = !Game.Options.Grid;
            _showMapGridItem.Checked = Game.Options.Grid;
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.ToggleGrid));
        }

        private void ArrangeWindows_Click(object sender, EventArgs e) { }
        private void ShowHiddenTerrain_Click(object sender, EventArgs e) { }
        private void CenterView_Click(object sender, EventArgs e)
        {
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.CenterView));
        }
        #endregion
        #region KINGDOM MENU EVENTS
        private void TaxRate_Click(object sender, EventArgs e) { }
        private void ViewThroneRoom_Click(object sender, EventArgs e) { }
        private void FindCity_Click(object sender, EventArgs e) { }
        private void Revolution_Click(object sender, EventArgs e) { }
        #endregion
        #region ORDERS MENU EVENTS
        private void BuildRoad_Click(object sender, EventArgs e) { }

        private void BuildIrrigation_Click(object sender, EventArgs e)
        {
            if (_buildIrrigationItem.Enabled) Game.IssueUnitOrder(OrderType.BuildIrrigation);
        }

        private void BuildMinesChangeForest_Click(object sender, EventArgs e)
        {
            if (_buildMinesChangeForestItem.Enabled) Game.IssueUnitOrder(OrderType.BuildMine);
        }

        private void CleanUpPollution_Click(object sender, EventArgs e) { }
        private void Pillage_Click(object sender, EventArgs e) { }
        private void Unload_Click(object sender, EventArgs e) { }

        private void GoTo_Click(object sender, EventArgs e)
        {
            if (_goToItem.Enabled) Game.IssueUnitOrder(OrderType.GoTo);   //TODO: implement goto
        }

        private void GoHomeToNearestCity_Click(object sender, EventArgs e)
        {
            if (_goHomeToNearestCityItem.Enabled) Game.IssueUnitOrder(OrderType.GoHome);
        }

        private void Fortify_Click(object sender, EventArgs e)
        {
            if (_fortifyItem.Enabled) Game.IssueUnitOrder(OrderType.Fortify);
        }

        private void Sleep_Click(object sender, EventArgs e)
        {
            if (_sleepItem.Enabled) Game.IssueUnitOrder(OrderType.Sleep);
        }

        private void Disband_Click(object sender, EventArgs e) { }

        private void ActivateUnit_Click(object sender, EventArgs e)
        {
            //if (_activateUnitItem.Enabled) Actions.GiveCommand("Activate unit"); 
        }

        private void Wait_Click(object sender, EventArgs e)
        {
            if (Game.Instance.ActiveUnit != null) Game.ChooseNextUnit();
        }

        private void SkipTurn_Click(object sender, EventArgs e)
        {
            Game.IssueUnitOrder(OrderType.SkipTurn);
        }

        private void EndPlayerTurn_Click(object sender, EventArgs e) { }

        private void BuildNewCity_Click(object sender, EventArgs e)
        {
            if(_buildNewCityItem.Enabled) Game.IssueUnitOrder(OrderType.BuildCity);
        }

        private void AutomateSettler_Click(object sender, EventArgs e)
        {
            if (_automateSettlerItem.Enabled) Game.IssueUnitOrder(OrderType.Automate);
        }

        private void Paradrop_Click(object sender, EventArgs e) { }
        #endregion
        #region ADVISORS MENU EVENTS
        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }

        private void CityStatus_Click(object sender, EventArgs e)
        {
            var CityStatusPanel = new CityStatusPanel(this, 622, 421);
            Controls.Add(CityStatusPanel);
            CityStatusPanel.Location = new Point((ClientSize.Width / 2) - (CityStatusPanel.Size.Width / 2), (ClientSize.Height / 2) - (CityStatusPanel.Size.Height / 2));
            CityStatusPanel.Show();
            CityStatusPanel.BringToFront();
        }

        private void DefenseMinister_Click(object sender, EventArgs e)
        {
            var DefenseMinisterPanel = new DefenseMinisterPanel(this, 622, 421);
            Controls.Add(DefenseMinisterPanel);
            DefenseMinisterPanel.Location = new Point((ClientSize.Width / 2) - (DefenseMinisterPanel.Size.Width / 2), (ClientSize.Height / 2) - (DefenseMinisterPanel.Size.Height / 2));
            DefenseMinisterPanel.Show();
            DefenseMinisterPanel.BringToFront();
        }

        private void ForeignMinister_Click(object sender, EventArgs e) { }

        private void AttitudeAdvisor_Click(object sender, EventArgs e)
        {
            var AttitudeAdvisorPanel = new AttitudeAdvisorPanel(this, 622, 421);
            Controls.Add(AttitudeAdvisorPanel);
            AttitudeAdvisorPanel.Location = new Point((ClientSize.Width / 2) - (AttitudeAdvisorPanel.Size.Width / 2), (ClientSize.Height / 2) - (AttitudeAdvisorPanel.Size.Height / 2));
            AttitudeAdvisorPanel.Show();
            AttitudeAdvisorPanel.BringToFront();
        }

        private void TradeAdvisor_Click(object sender, EventArgs e)
        {
            var TradeAdvisorPanel = new TradeAdvisorPanel(this, 622, 421);
            Controls.Add(TradeAdvisorPanel);
            TradeAdvisorPanel.Location = new Point((ClientSize.Width / 2) - (TradeAdvisorPanel.Size.Width / 2), (ClientSize.Height / 2) - (TradeAdvisorPanel.Size.Height / 2));
            TradeAdvisorPanel.Show();
            TradeAdvisorPanel.BringToFront();
        }

        private void ScienceAdvisor_Click(object sender, EventArgs e)
        {
            var ScienceAdvisorPanel = new ScienceAdvisorPanel(this, 622, 421);
            Controls.Add(ScienceAdvisorPanel);
            ScienceAdvisorPanel.Location = new Point((ClientSize.Width / 2) - (ScienceAdvisorPanel.Size.Width / 2), (ClientSize.Height / 2) - (ScienceAdvisorPanel.Size.Height / 2));
            ScienceAdvisorPanel.Show();
            ScienceAdvisorPanel.BringToFront();
        }

        private void CasualtyTimeline_Click(object sender, EventArgs e) { }
        #endregion
        #region WORLD MENU EVENTS
        private void WondersOfWorld_Click(object sender, EventArgs e)
        {
            var WondersOfWorldPanel = new WondersOfWorldPanel(this, 622, 421);
            Controls.Add(WondersOfWorldPanel);
            WondersOfWorldPanel.Location = new Point((ClientSize.Width / 2) - (WondersOfWorldPanel.Size.Width / 2), (ClientSize.Height / 2) - (WondersOfWorldPanel.Size.Height / 2));
            WondersOfWorldPanel.Show();
            WondersOfWorldPanel.BringToFront();
        }

        private void Top5Cities_Click(object sender, EventArgs e) { }
        private void CivScore_Click(object sender, EventArgs e) { }

        private void Demographics_Click(object sender, EventArgs e)
        {
            var DemographicsPanel = new DemographicsPanel(this, 622, 421);
            Controls.Add(DemographicsPanel);
            DemographicsPanel.Location = new Point((ClientSize.Width / 2) - (DemographicsPanel.Size.Width / 2), (ClientSize.Height / 2) - (DemographicsPanel.Size.Height / 2));
            DemographicsPanel.Show();
            DemographicsPanel.BringToFront();
        }
        private void Spaceships_Click(object sender, EventArgs e) { }
        #endregion
        #region CHEAT MENU EVENTS
        private void ToggleCheatMode_Click(object sender, EventArgs e) { }

        private void CreateUnit_Click(object sender, EventArgs e)
        {
            //CreateUnitForm CreateUnitForm = new CreateUnitForm();
            //CreateUnitForm.Load += new EventHandler(CreateUnitForm_Load);   //so you set the correct size of form
            //CreateUnitForm.ShowDialog(); 
        }

        private void CreateUnitForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Location = new Point(300, 200);
            frm.Width = 746;
            frm.Height = 459; }

        private void RevealMap_Click(object sender, EventArgs e) { }
        private void SetHumanPlayer_Click(object sender, EventArgs e) { }

        private void SetGameYear_Click(object sender, EventArgs e)
        {
            //SetGameYearForm SetGameYearForm = new SetGameYearForm();
            //SetGameYearForm.Load += new EventHandler(SetGameYearForm_Load);   //so you set the correct size of form
            //SetGameYearForm.ShowDialog(); 
        }

        private void SetGameYearForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Location = new Point(400, 350);
            frm.Width = 476;
            frm.Height = 158; }

        private void KillCivilization_Click(object sender, EventArgs e) { }
        private void TechnologyAdvance_Click(object sender, EventArgs e) { }
        private void EditTechnologies_Click(object sender, EventArgs e) { }
        private void ForceGovernment_Click(object sender, EventArgs e) { }
        private void ChangeTerrainAtCursor_Click(object sender, EventArgs e) { }
        private void DestroyAllUnitsAtCursor_Click(object sender, EventArgs e) { }
        private void ChangeMoney_Click(object sender, EventArgs e) { }
        private void EditUnit_Click(object sender, EventArgs e) { }
        private void EditCity_Click(object sender, EventArgs e) { }
        private void EditKing_Click(object sender, EventArgs e) { }
        private void ScenarioParameters_Click(object sender, EventArgs e) { }
        private void SaveAsScenario_Click(object sender, EventArgs e) { }
        #endregion

        #region If a new unit or no unit is active, update orders menu accordingly
        public void Update_ordersMenu()
        {
            //if (MapForm.ViewingPiecesMode)  //disable all menus except disband & activate unit
            if (1 == 1) //disable all menus except disband & activate unit
            {
                foreach (ToolStripItem item in _ordersMenu.DropDownItems) item.Enabled = false;
                _disbandItem.Enabled = true;
                _activateUnitItem.Enabled = true;
            }
            else if (Game.Instance.ActiveUnit.Type == UnitType.Settlers)
            {
                _ordersMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in _settlerItems)
                {
                    _ordersMenu.DropDownItems.Add(item);
                    item.Enabled = true;
                }
            }
            else
            {
                _ordersMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in _noSettlerItems)
                {
                    _ordersMenu.DropDownItems.Add(item);
                    item.Enabled = true;
                }
                if (Game.Instance.ActiveUnit.Type != UnitType.Paratroopers) _ordersMenu.DropDownItems.Remove(_paradropItem);
                //if (Game.Instance.ActiveUnit.GAS == (UnitGAS.Air | UnitGAS.Sea)) _pillageItem.Enabled = false;
                _buildRoadItem.Enabled = false;
                _buildIrrigationItem.Enabled = false;
                _buildMinesChangeForestItem.Enabled = false;
                _cleanUpPollutionItem.Enabled = false;
                _unloadItem.Enabled = false;
            }
        }
        #endregion

        // If view pieces mode is toggled on/off
        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.SwitchViewMovePiece:
                    {
                        if (ViewPieceMode)
                        {
                            _movePieceItem.Enabled = true;
                            _viewPieceItem.Enabled = false;
                        }
                        else
                        {
                            _movePieceItem.Enabled = false;
                            _viewPieceItem.Enabled = true;
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
