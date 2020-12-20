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
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        MenuStrip MainMenuStrip;
        PictureBox MainPic;
        MapPanel MapPanel;
        //CityForm cityForm;
        _MinimapPanel MinimapPanel;
        StatusPanel StatusPanel;
        ToolStripMenuItem OrdersMenu;
        ToolStripMenuItem BuildMinesChangeForestItem, CleanUpPollutionItem, PillageItem, UnloadItem, GoToItem, GoHomeToNearestCityItem, FortifyItem, SleepItem, DisbandItem, MaxZoomInItem, MaxZoomOutItem, ActivateUnitItem, WaitItem, SkipTurnItem, EndPlayerTurnItem, BuildNewCityItem, AutomateSettlerItem, ParadropItem;
        List<ToolStripItem> SettlerItems, NoSettlerItems;
        Civ2ToolStripMenuItem TaxRateItem, ViewThroneRoomItem, FindCityItem, RevolutionItem, BuildRoadItem, BuildIrrigationItem, MovePiecesItem, ViewPiecesItem, ZoomInItem, ZoomOutItem, StandardZoomItem, MediumZoomOutItem, ArrangeWindowsItem, ShowHiddenTerrainItem, CenterViewItem;
        public bool AreWeInIntroScreen, LoadGameCalled;
        ToolStripMenuItem ShowMapGridItem;

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
            Bitmap backimage = Images.MainScreenSymbol;
            MainPic = new PictureBox
            {
                Image = backimage,
                SizeMode = PictureBoxSizeMode.AutoSize,
                Anchor = AnchorStyles.None,
                Location = new Point((ClientSize.Width / 2) - (backimage.Width / 2), (ClientSize.Height / 2) - (backimage.Height / 2))
            };
            Controls.Add(MainPic);
            MainPic.SendToBack();
            #endregion

            #region MENUS
            MainMenuStrip = new MenuStrip
            {
                BackColor = Color.White
            };
            Controls.Add(MainMenuStrip);
            //Game menu
            ToolStripMenuItem GameMenu = new ToolStripMenuItem("Game");
            ToolStripMenuItem GameOptionsItem = new ToolStripMenuItem("Game Options", null, GameOptions_Click, (Keys)Shortcut.CtrlO);
            ToolStripMenuItem GraphicOptionsItem = new ToolStripMenuItem("Graphic Options", null, GraphicOptions_Click, (Keys)Shortcut.CtrlP);
            ToolStripMenuItem CityReportOptionsItem = new ToolStripMenuItem("City Report Options", null, CityReportOptions_Click, (Keys)Shortcut.CtrlE);
            ToolStripMenuItem MultiplayerOptionsItem = new ToolStripMenuItem("Multiplayer Options", null, MultiplayerOptions_Click, (Keys)Shortcut.CtrlY);
            ToolStripMenuItem GameProfileItem = new ToolStripMenuItem("Game Profile", null, GameProfile_Click);
            ToolStripMenuItem PickMusicItem = new ToolStripMenuItem("Pick Music", null, PickMusic_Click);
            ToolStripMenuItem SaveGameItem = new ToolStripMenuItem("Save Game", null, SaveGame_Click, (Keys)Shortcut.CtrlS);
            ToolStripMenuItem LoadGameItem = new ToolStripMenuItem("Load Game", null, LoadGame_Click, (Keys)Shortcut.CtrlL);
            ToolStripMenuItem JoinGameItem = new ToolStripMenuItem("Join Game", null, JoinGame_Click, (Keys)Shortcut.CtrlJ);
            ToolStripMenuItem SetPasswordItem = new ToolStripMenuItem("Set Password", null, SetPassword_Click, (Keys)Shortcut.CtrlW);
            ToolStripMenuItem ChangeTimerItem = new ToolStripMenuItem("Change Timer", null, ChangeTimer_Click, (Keys)Shortcut.CtrlT);
            ToolStripMenuItem RetireItem = new ToolStripMenuItem("Retire", null, Retire_Click, (Keys)Shortcut.CtrlR);
            ToolStripMenuItem QuitItem = new ToolStripMenuItem("Quit", null, Quit_Click, (Keys)Shortcut.CtrlQ);
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

            //View menu
            ToolStripMenuItem ViewMenu = new ToolStripMenuItem("View");
            MovePiecesItem = new Civ2ToolStripMenuItem("Move Pieces", MovePieces_Click, "v");
            ViewPiecesItem = new Civ2ToolStripMenuItem("View Pieces", ViewPieces_Click, "v");
            ZoomInItem = new Civ2ToolStripMenuItem("Zoom In", null, "z");
            ZoomOutItem = new Civ2ToolStripMenuItem("Zoom Out", null, "x");
            MaxZoomInItem = new ToolStripMenuItem("Max Zoom In", null, null, (Keys)Shortcut.CtrlZ);
            StandardZoomItem = new Civ2ToolStripMenuItem("Standard Zoom", null, "Shift+Z");
            MediumZoomOutItem = new Civ2ToolStripMenuItem("Medium Zoom Out", null, "Shift+X");
            StandardZoomItem = new Civ2ToolStripMenuItem("Standard Zoom", null, "Shift+Z");
            MediumZoomOutItem = new Civ2ToolStripMenuItem("Medium Zoom Out", null, "Shift+X");
            MaxZoomOutItem = new ToolStripMenuItem("Max Zoom Out", null, null, (Keys)Shortcut.CtrlX);
            ShowMapGridItem = new ToolStripMenuItem("Show Map Grid", null, ShowMapGrid_Click, (Keys)Shortcut.CtrlG);
            ArrangeWindowsItem = new Civ2ToolStripMenuItem("Arrange Windows", ArrangeWindows_Click, "");
            ShowHiddenTerrainItem = new Civ2ToolStripMenuItem("Show Hidden Terrain", ShowHiddenTerrain_Click, "t");
            CenterViewItem = new Civ2ToolStripMenuItem("Center View", CenterView_Click, "c");
            MainMenuStrip.Items.Add(ViewMenu);
            ViewMenu.DropDownItems.Add(MovePiecesItem);
            ViewMenu.DropDownItems.Add(ViewPiecesItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(ZoomInItem);
            ViewMenu.DropDownItems.Add(ZoomOutItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(MaxZoomInItem);
            ViewMenu.DropDownItems.Add(StandardZoomItem);
            ViewMenu.DropDownItems.Add(MediumZoomOutItem);
            ViewMenu.DropDownItems.Add(MaxZoomOutItem);
            ViewMenu.DropDownItems.Add(new ToolStripSeparator());
            ViewMenu.DropDownItems.Add(ShowMapGridItem);
            ViewMenu.DropDownItems.Add(ArrangeWindowsItem);
            ViewMenu.DropDownItems.Add(ShowHiddenTerrainItem);
            ViewMenu.DropDownItems.Add(CenterViewItem);

            //Kingdom menu
            ToolStripMenuItem KingdomMenu = new ToolStripMenuItem("Kingdom");
            TaxRateItem = new Civ2ToolStripMenuItem("Tax Rate", TaxRate_Click, "Shift+T");
            ViewThroneRoomItem = new Civ2ToolStripMenuItem("View Throne Room", ViewThroneRoom_Click, "Shift+H");
            FindCityItem = new Civ2ToolStripMenuItem("Find City", FindCity_Click, "Shift+C");
            RevolutionItem = new Civ2ToolStripMenuItem("REVOLUTION", Revolution_Click, "Shift+R");
            MainMenuStrip.Items.Add(KingdomMenu);
            KingdomMenu.DropDownItems.Add(TaxRateItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(ViewThroneRoomItem);
            KingdomMenu.DropDownItems.Add(FindCityItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(RevolutionItem);
                        
            //Orders
            OrdersMenu = new ToolStripMenuItem("Orders");
            BuildRoadItem = new Civ2ToolStripMenuItem("Build Road", BuildRoad_Click, "r");
            BuildIrrigationItem = new Civ2ToolStripMenuItem("Build Irrigation", BuildIrrigation_Click, "i");
            BuildMinesChangeForestItem = new ToolStripMenuItem("Build Mines", null, BuildMinesChangeForest_Click);
            CleanUpPollutionItem = new ToolStripMenuItem("Clean Up Pollution", null, CleanUpPollution_Click);
            PillageItem = new ToolStripMenuItem("Pillage", null, Pillage_Click);
            UnloadItem = new ToolStripMenuItem("Unload", null, Unload_Click);
            GoToItem = new ToolStripMenuItem("Go To", null, GoTo_Click);
            GoHomeToNearestCityItem = new ToolStripMenuItem("Go Home To Nearest City", null, GoHomeToNearestCity_Click);
            FortifyItem = new ToolStripMenuItem("Fortify", null, Fortify_Click);
            SleepItem = new ToolStripMenuItem("Sleep", null, Sleep_Click);
            DisbandItem = new ToolStripMenuItem("Disband", null, Disband_Click);
            ActivateUnitItem = new ToolStripMenuItem("Activate Unit", null, ActivateUnit_Click);
            WaitItem = new Civ2ToolStripMenuItem("Wait", Wait_Click, "w");
            SkipTurnItem = new Civ2ToolStripMenuItem("Skip Turn", SkipTurn_Click, "SPACE");
            EndPlayerTurnItem = new ToolStripMenuItem("End Player Turn", null, EndPlayerTurn_Click, (Keys)Shortcut.CtrlN);
            BuildNewCityItem = new ToolStripMenuItem("Build New City", null, BuildNewCity_Click);   //Settlers only items
            AutomateSettlerItem = new ToolStripMenuItem("Automate Settler", null, AutomateSettler_Click);
            ParadropItem = new ToolStripMenuItem("Paradrop", null, Paradrop_Click);   //Paratroopers only item
            MainMenuStrip.Items.Add(OrdersMenu);
                        
            SettlerItems = new List<ToolStripItem> { BuildNewCityItem, BuildRoadItem, BuildIrrigationItem, BuildMinesChangeForestItem, new ToolStripSeparator(), AutomateSettlerItem, CleanUpPollutionItem, new ToolStripSeparator(), GoToItem, GoHomeToNearestCityItem, new ToolStripSeparator(), SleepItem, new ToolStripSeparator(), DisbandItem, ActivateUnitItem, WaitItem, SkipTurnItem, new ToolStripSeparator(), EndPlayerTurnItem };

            NoSettlerItems = new List<ToolStripItem> { BuildRoadItem, BuildIrrigationItem, BuildMinesChangeForestItem, new ToolStripSeparator(), CleanUpPollutionItem, PillageItem, new ToolStripSeparator(), UnloadItem, GoToItem, ParadropItem, GoHomeToNearestCityItem, new ToolStripSeparator(), FortifyItem, SleepItem, new ToolStripSeparator(), DisbandItem, ActivateUnitItem, WaitItem, SkipTurnItem, new ToolStripSeparator(), EndPlayerTurnItem };

            OrdersMenu.DropDownItems.Add(BuildNewCityItem);
            OrdersMenu.DropDownItems.Add(BuildRoadItem);
            OrdersMenu.DropDownItems.Add(BuildIrrigationItem);
            OrdersMenu.DropDownItems.Add(BuildMinesChangeForestItem);
            OrdersMenu.DropDownItems.Add(new ToolStripSeparator());
            OrdersMenu.DropDownItems.Add(AutomateSettlerItem);
            OrdersMenu.DropDownItems.Add(CleanUpPollutionItem);
            OrdersMenu.DropDownItems.Add(PillageItem);
            OrdersMenu.DropDownItems.Add(new ToolStripSeparator());
            OrdersMenu.DropDownItems.Add(UnloadItem);
            OrdersMenu.DropDownItems.Add(GoToItem);
            OrdersMenu.DropDownItems.Add(ParadropItem);
            OrdersMenu.DropDownItems.Add(GoHomeToNearestCityItem);
            OrdersMenu.DropDownItems.Add(new ToolStripSeparator());
            OrdersMenu.DropDownItems.Add(FortifyItem);
            OrdersMenu.DropDownItems.Add(SleepItem);
            OrdersMenu.DropDownItems.Add(new ToolStripSeparator());
            OrdersMenu.DropDownItems.Add(DisbandItem);
            OrdersMenu.DropDownItems.Add(ActivateUnitItem);
            OrdersMenu.DropDownItems.Add(WaitItem);
            OrdersMenu.DropDownItems.Add(SkipTurnItem);
            OrdersMenu.DropDownItems.Add(new ToolStripSeparator());
            OrdersMenu.DropDownItems.Add(EndPlayerTurnItem);

            //Advisors menu
            ToolStripMenuItem AdvisorsMenu = new ToolStripMenuItem("Advisors");
            ToolStripMenuItem ChatWithKingsItem = new ToolStripMenuItem("Chat with Kings", null, ChatWithKings_Click, (Keys)Shortcut.CtrlC);
            ToolStripMenuItem ConsultHighCouncilItem = new ToolStripMenuItem("Consult High Council", null, ConsultHighCouncil_Click);
            ToolStripMenuItem CityStatusItem = new ToolStripMenuItem("City Status", null, CityStatus_Click, (Keys)Shortcut.F1);
            ToolStripMenuItem DefenseMinisterItem = new ToolStripMenuItem("Defense Minister", null, DefenseMinister_Click, (Keys)Shortcut.F2);
            ToolStripMenuItem ForeignMinisterItem = new ToolStripMenuItem("Foreign Minister", null, ForeignMinister_Click, (Keys)Shortcut.F3);
            ToolStripMenuItem AttitudeAdvisorItem = new ToolStripMenuItem("Attitude Advisor", null, AttitudeAdvisor_Click, (Keys)Shortcut.F4);
            ToolStripMenuItem TradeAdvisorItem = new ToolStripMenuItem("Trade Advisor", null, TradeAdvisor_Click, (Keys)Shortcut.F5);
            ToolStripMenuItem ScienecAdvisorItem = new ToolStripMenuItem("Science Advisor", null, ScienceAdvisor_Click, (Keys)Shortcut.F6);
            ToolStripMenuItem CasualtyTimelineItem = new ToolStripMenuItem("Casualty Timeline", null, CasualtyTimeline_Click, (Keys)Shortcut.CtrlD);
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

            //World menu
            ToolStripMenuItem WorldMenu = new ToolStripMenuItem("World");
            ToolStripMenuItem WondersOfWorldItem = new ToolStripMenuItem("Wonders of the World", null, WondersOfWorld_Click, (Keys)Shortcut.F7);
            ToolStripMenuItem Top5CitiesItem = new ToolStripMenuItem("Top 5 Cities", null, Top5Cities_Click, (Keys)Shortcut.F8);
            ToolStripMenuItem CivScoreItem = new ToolStripMenuItem("Civilization Score", null, CivScore_Click, (Keys)Shortcut.F9);
            ToolStripMenuItem DemographicsItem = new ToolStripMenuItem("Demographics", null, Demographics_Click, (Keys)Shortcut.F11);
            ToolStripMenuItem SpaceshipsItem = new ToolStripMenuItem("Spaceships", null, Spaceships_Click, (Keys)Shortcut.F12);
            MainMenuStrip.Items.Add(WorldMenu);
            WorldMenu.DropDownItems.Add(WondersOfWorldItem);
            WorldMenu.DropDownItems.Add(Top5CitiesItem);
            WorldMenu.DropDownItems.Add(CivScoreItem);
            WorldMenu.DropDownItems.Add(new ToolStripSeparator());
            WorldMenu.DropDownItems.Add(DemographicsItem);
            WorldMenu.DropDownItems.Add(SpaceshipsItem);

            //Cheat menu
            ToolStripMenuItem CheatMenu = new ToolStripMenuItem("Cheat");
            ToolStripMenuItem ToggleCheatModeItem = new ToolStripMenuItem("Toggle Cheat Mode", null, ToggleCheatMode_Click, (Keys)Shortcut.CtrlK);
            ToolStripMenuItem CreateUnitItem = new ToolStripMenuItem("Create Unit", null, CreateUnit_Click, (Keys)Shortcut.ShiftF1);
            ToolStripMenuItem RevealMapItem = new ToolStripMenuItem("Reveal Map", null, RevealMap_Click, (Keys)Shortcut.ShiftF2);
            ToolStripMenuItem SetHumanPlayerItem = new ToolStripMenuItem("Set Human Player", null, SetHumanPlayer_Click, (Keys)Shortcut.ShiftF3);
            ToolStripMenuItem SetGameYearItem = new ToolStripMenuItem("Set Game Year", null, SetGameYear_Click, (Keys)Shortcut.ShiftF4);
            ToolStripMenuItem KillCivilizationItem = new ToolStripMenuItem("Kill Civilization", null, KillCivilization_Click, (Keys)Shortcut.ShiftF5);
            ToolStripMenuItem TechnologyAdvanceItem = new ToolStripMenuItem("Technology Advance", null, TechnologyAdvance_Click, (Keys)Shortcut.ShiftF6);
            ToolStripMenuItem EditTechnologiesItem = new ToolStripMenuItem("Edit Technologies", null, EditTechnologies_Click, (Keys)Shortcut.CtrlShiftF6);
            ToolStripMenuItem ForceGovernmentItem = new ToolStripMenuItem("Force Government", null, ForceGovernment_Click, (Keys)Shortcut.ShiftF7);
            ToolStripMenuItem ChangeTerrainAtCursorItem = new ToolStripMenuItem("Change Terrain At Cursor", null, ChangeTerrainAtCursor_Click, (Keys)Shortcut.ShiftF8);
            ToolStripMenuItem DestroyAllUnitsAtCursorItem = new ToolStripMenuItem("Destroy All Units At Cursor", null, DestroyAllUnitsAtCursor_Click, (Keys)Shortcut.CtrlShiftD);
            ToolStripMenuItem ChangeMoneyItem = new ToolStripMenuItem("Change Money", null, ChangeMoney_Click, (Keys)Shortcut.ShiftF9);
            ToolStripMenuItem EditUnitItem = new ToolStripMenuItem("Edit Unit", null, EditUnit_Click, (Keys)Shortcut.CtrlShiftU);
            ToolStripMenuItem EditCityItem = new ToolStripMenuItem("Edit City", null, EditCity_Click, (Keys)Shortcut.CtrlShiftC);
            ToolStripMenuItem EditKingItem = new ToolStripMenuItem("Edit King", null, EditKing_Click, (Keys)Shortcut.CtrlShiftK);
            ToolStripMenuItem ScenarioParametersItem = new ToolStripMenuItem("Scenario Parameters", null, ScenarioParameters_Click, (Keys)Shortcut.CtrlShiftP);
            ToolStripMenuItem SaveAsScenarioItem = new ToolStripMenuItem("Save As Scenario", null, SaveAsScenario_Click, (Keys)Shortcut.CtrlShiftS);
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

            //Disable some item
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
            GameOptionsPanel GameOptionsPanel = new GameOptionsPanel(this, 746, 440);
            Controls.Add(GameOptionsPanel);
            GameOptionsPanel.Location = new Point(this.ClientSize.Width / 2 - GameOptionsPanel.Size.Width / 2, this.ClientSize.Height / 2 - GameOptionsPanel.Size.Height / 2);
            GameOptionsPanel.Show();
            GameOptionsPanel.BringToFront();
        }

        private void GraphicOptions_Click(object sender, EventArgs e) 
        {
            GraphicOptionsPanel GraphicOptionsPanel = new GraphicOptionsPanel(this, 746, 280);
            Controls.Add(GraphicOptionsPanel);
            GraphicOptionsPanel.Location = new Point(this.ClientSize.Width / 2 - GraphicOptionsPanel.Size.Width / 2, this.ClientSize.Height / 2 - GraphicOptionsPanel.Size.Height / 2);
            GraphicOptionsPanel.Show();
            GraphicOptionsPanel.BringToFront();
        }

        private void CityReportOptions_Click(object sender, EventArgs e) 
        {
            CityreportOptionsPanel CityreportOptionsPanel = new CityreportOptionsPanel(this, 746, 440);
            Controls.Add(CityreportOptionsPanel);
            CityreportOptionsPanel.Location = new Point(this.ClientSize.Width / 2 - CityreportOptionsPanel.Size.Width / 2, this.ClientSize.Height / 2 - CityreportOptionsPanel.Size.Height / 2);
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
            ViewPiecesItem.Enabled = true;
            MovePiecesItem.Enabled = false;
        }

        private void ViewPieces_Click(object sender, EventArgs e) 
        {
            ViewPieceMode = true;
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
            MovePiecesItem.Enabled = true;
            ViewPiecesItem.Enabled = false;
        }

        private void ShowMapGrid_Click(object sender, EventArgs e) 
        { 
            //int var = MapPanel.ToggleMapGrid();
            //if (var != 0) ShowMapGridItem.Checked = true;
            //else ShowMapGridItem.Checked = false;
        }
        private void ArrangeWindows_Click(object sender, EventArgs e) { }
        private void ShowHiddenTerrain_Click(object sender, EventArgs e) { }
        private void CenterView_Click(object sender, EventArgs e) { }
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
            if (BuildIrrigationItem.Enabled) Game.IssueUnitOrder(OrderType.BuildIrrigation); 
        }

        private void BuildMinesChangeForest_Click(object sender, EventArgs e) 
        {
            if (BuildMinesChangeForestItem.Enabled) Game.IssueUnitOrder(OrderType.BuildMine); 
        }

        private void CleanUpPollution_Click(object sender, EventArgs e) { }
        private void Pillage_Click(object sender, EventArgs e) { }
        private void Unload_Click(object sender, EventArgs e) { }

        private void GoTo_Click(object sender, EventArgs e) 
        {
            if (GoToItem.Enabled) Game.IssueUnitOrder(OrderType.GoTo);   //TODO: implement goto
        }

        private void GoHomeToNearestCity_Click(object sender, EventArgs e) 
        {
            if (GoHomeToNearestCityItem.Enabled) Game.IssueUnitOrder(OrderType.GoHome); 
        }

        private void Fortify_Click(object sender, EventArgs e) 
        {
            if (FortifyItem.Enabled) Game.IssueUnitOrder(OrderType.Fortify); 
        }

        private void Sleep_Click(object sender, EventArgs e) 
        {
            if (SleepItem.Enabled) Game.IssueUnitOrder(OrderType.Sleep); 
        }

        private void Disband_Click(object sender, EventArgs e) { }

        private void ActivateUnit_Click(object sender, EventArgs e) 
        {
            //if (ActivateUnitItem.Enabled) Actions.GiveCommand("Activate unit"); 
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
            if(BuildNewCityItem.Enabled) Game.IssueUnitOrder(OrderType.BuildCity); 
        }

        private void AutomateSettler_Click(object sender, EventArgs e) 
        {
            if (AutomateSettlerItem.Enabled) Game.IssueUnitOrder(OrderType.Automate); 
        }

        private void Paradrop_Click(object sender, EventArgs e) { }
        #endregion
        #region ADVISORS MENU EVENTS
        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }

        private void CityStatus_Click(object sender, EventArgs e) 
        {
            CityStatusPanel CityStatusPanel = new CityStatusPanel(this, 622, 421);
            Controls.Add(CityStatusPanel);
            CityStatusPanel.Location = new Point(this.ClientSize.Width / 2 - CityStatusPanel.Size.Width / 2, this.ClientSize.Height / 2 - CityStatusPanel.Size.Height / 2);
            CityStatusPanel.Show();
            CityStatusPanel.BringToFront();
        }

        private void DefenseMinister_Click(object sender, EventArgs e) 
        {
            DefenseMinisterPanel DefenseMinisterPanel = new DefenseMinisterPanel(this, 622, 421);
            Controls.Add(DefenseMinisterPanel);
            DefenseMinisterPanel.Location = new Point(this.ClientSize.Width / 2 - DefenseMinisterPanel.Size.Width / 2, this.ClientSize.Height / 2 - DefenseMinisterPanel.Size.Height / 2);
            DefenseMinisterPanel.Show();
            DefenseMinisterPanel.BringToFront();
        }

        private void ForeignMinister_Click(object sender, EventArgs e) { }

        private void AttitudeAdvisor_Click(object sender, EventArgs e) 
        {
            AttitudeAdvisorPanel AttitudeAdvisorPanel = new AttitudeAdvisorPanel(this, 622, 421);
            Controls.Add(AttitudeAdvisorPanel);
            AttitudeAdvisorPanel.Location = new Point(this.ClientSize.Width / 2 - AttitudeAdvisorPanel.Size.Width / 2, this.ClientSize.Height / 2 - AttitudeAdvisorPanel.Size.Height / 2);
            AttitudeAdvisorPanel.Show();
            AttitudeAdvisorPanel.BringToFront();
        }

        private void TradeAdvisor_Click(object sender, EventArgs e) 
        {
            TradeAdvisorPanel TradeAdvisorPanel = new TradeAdvisorPanel(this, 622, 421);
            Controls.Add(TradeAdvisorPanel);
            TradeAdvisorPanel.Location = new Point(this.ClientSize.Width / 2 - TradeAdvisorPanel.Size.Width / 2, this.ClientSize.Height / 2 - TradeAdvisorPanel.Size.Height / 2);
            TradeAdvisorPanel.Show();
            TradeAdvisorPanel.BringToFront();
        }

        private void ScienceAdvisor_Click(object sender, EventArgs e) 
        {
            ScienceAdvisorPanel ScienceAdvisorPanel = new ScienceAdvisorPanel(this, 622, 421);
            Controls.Add(ScienceAdvisorPanel);
            ScienceAdvisorPanel.Location = new Point(this.ClientSize.Width / 2 - ScienceAdvisorPanel.Size.Width / 2, this.ClientSize.Height / 2 - ScienceAdvisorPanel.Size.Height / 2);
            ScienceAdvisorPanel.Show();
            ScienceAdvisorPanel.BringToFront();
        }

        private void CasualtyTimeline_Click(object sender, EventArgs e) { }
        #endregion
        #region WORLD MENU EVENTS
        private void WondersOfWorld_Click(object sender, EventArgs e) 
        {
            //WondersOfWorldForm WondersOfWorldForm = new WondersOfWorldForm();
            //WondersOfWorldForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            //WondersOfWorldForm.ShowDialog();
        }

        private void Top5Cities_Click(object sender, EventArgs e) { }
        private void CivScore_Click(object sender, EventArgs e) { }

        private void Demographics_Click(object sender, EventArgs e)
        {
            //DemographicsForm DemographicsForm = new DemographicsForm();
            //DemographicsForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            //DemographicsForm.ShowDialog();
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
        private void AdvisorsForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Location = new Point(330, 250);
            frm.Width = 622;
            frm.Height = 421; }

        #region If a new unit or no unit is active, update orders menu accordingly
        public void UpdateOrdersMenu()
        {
            //if (MapForm.ViewingPiecesMode)  //disable all menus except disband & activate unit
                if (1 == 1) //disable all menus except disband & activate unit
                {
                    foreach (ToolStripItem item in OrdersMenu.DropDownItems) item.Enabled = false;
                DisbandItem.Enabled = true;
                ActivateUnitItem.Enabled = true;
            }
            else if (Game.Instance.ActiveUnit.Type == UnitType.Settlers)
            {
                OrdersMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in SettlerItems)
                {
                    OrdersMenu.DropDownItems.Add(item);
                    item.Enabled = true;
                }
            }
            else
            {
                OrdersMenu.DropDownItems.Clear();
                foreach (ToolStripItem item in NoSettlerItems)
                {
                    OrdersMenu.DropDownItems.Add(item);
                    item.Enabled = true;
                }
                if (Game.Instance.ActiveUnit.Type != UnitType.Paratroopers) OrdersMenu.DropDownItems.Remove(ParadropItem);
                //if (Game.Instance.ActiveUnit.GAS == (UnitGAS.Air | UnitGAS.Sea)) PillageItem.Enabled = false;
                BuildRoadItem.Enabled = false;
                BuildIrrigationItem.Enabled = false;
                BuildMinesChangeForestItem.Enabled = false;
                CleanUpPollutionItem.Enabled = false;
                UnloadItem.Enabled = false;
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
                            MovePiecesItem.Enabled = true;
                            ViewPiecesItem.Enabled = false;
                        }
                        else
                        {
                            MovePiecesItem.Enabled = false;
                            ViewPiecesItem.Enabled = true;
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
