using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Enums;
using RTciv2.Imagery;

namespace RTciv2.Forms
{
    public partial class MainCiv2Window : Form
    {
        MenuStrip MainMenuStrip;
        PictureBox MainIcon, SinaiIcon;
        ChoiceMenuPanel ChoiceMenu;
        //public MapForm MapForm;
        //public StatusForm statusForm;
        //public WorldMapForm WorldMapForm;
        public MapPanel MapPanel;
        public CityForm cityForm;
        ToolStripMenuItem OrdersMenu;
        ToolStripMenuItem BuildMinesChangeForestItem, CleanUpPollutionItem, PillageItem, UnloadItem, GoToItem, GoHomeToNearestCityItem, FortifyItem, SleepItem, DisbandItem, MaxZoomInItem, MaxZoomOutItem, ActivateUnitItem, WaitItem, SkipTurnItem, EndPlayerTurnItem, BuildNewCityItem, AutomateSettlerItem, ParadropItem;
        List<ToolStripItem> SettlerItems, NoSettlerItems;
        Civ2ToolStripMenuItem TaxRateItem, ViewThroneRoomItem, FindCityItem, RevolutionItem, BuildRoadItem, BuildIrrigationItem, MovePiecesItem, ViewPiecesItem, ZoomInItem, ZoomOutItem, StandardZoomItem, MediumZoomOutItem, ArrangeWindowsItem, ShowHiddenTerrainItem, CenterViewItem;
        public bool AreWeInIntroScreen, LoadGameCalled;
        ToolStripMenuItem ShowMapGridItem;
        MinimapPanel MinimapPanel;

        public MainCiv2Window(Resolution resol, string civ2Path, string SAVfile)
        {
            #region INITIAL SETTINGS
            InitializeComponent();
            Text = "Civilization II Multiplayer Gold (OpenCIV2)";
            BackColor = Color.FromArgb(143, 123, 99);
            this.Icon = Properties.Resources.civ2alt;   //Load the icon
            #endregion

            #region RESOLUTION
            if (resol.Name == "Fullscreen") WindowState = FormWindowState.Maximized;
            else { this.Size = new Size(resol.Width, resol.Height); CenterToScreen(); }
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
            WaitItem = new ToolStripMenuItem("Wait", null, Wait_Click);
            SkipTurnItem = new ToolStripMenuItem("Skip Turn", null, SkipTurn_Click);
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

            //Set some variables
            //AreWeInIntroScreen = true;
            //LoadGameCalled = false;
        }

        private void MainCiv2Window_Load(object sender, EventArgs e)
        {
            //Symbol image in the center of screen
            //Bitmap backimage = Images.MainScreenSymbol;
            //MainIcon = new PictureBox {
            //    Image = backimage,
            //    SizeMode = PictureBoxSizeMode.AutoSize,
            //    Anchor = AnchorStyles.None,
            //    Location = new Point((ClientSize.Width / 2) - (backimage.Width / 2), (ClientSize.Height / 2) - (backimage.Height / 2)) };
            //Controls.Add(MainIcon);
            //MainIcon.SendToBack();

            #region If starting game through intro screen
            ////Sinai image in the intro screen
            //Bitmap sinaiimage = Images.MainScreenSinai;
            //SinaiIcon = new PictureBox {
            //    Image = sinaiimage,
            //    BackgroundImage = Images.WallpaperMapForm,
            //    Width = sinaiimage.Width + 2 * 11,
            //    Height = sinaiimage.Height + 2 * 11,
            //    Location = new Point((int)(ClientSize.Width * 0.08333), (int)(ClientSize.Height * 0.0933)),
            //    SizeMode = PictureBoxSizeMode.CenterImage };
            //Controls.Add(SinaiIcon);
            //SinaiIcon.Paint += new PaintEventHandler(SinaiBorder_Paint);
            //SinaiIcon.Show();
            //SinaiIcon.BringToFront();

            ////Choice menu panel
            //ChoiceMenu = new ChoiceMenuPanel(this);
            //ChoiceMenu.Location = new Point((int)(ClientSize.Width * 0.745), (int)(ClientSize.Height * 0.570));
            //Controls.Add(ChoiceMenu);

            ////cityForm = new CityForm(this);

            ////If quickload is enabled skip intro screen & load game immediately
            //if (Program.QuickLoad) ChoiceMenuResult(2, String.Concat(String.Concat(Program.Path, Program.SAVName), ".SAV"));
            //else ShowIntroScreen();
            #endregion

            // MapPanel = new MapPanel(ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height);
            MapPanel = new MapPanel();
            MapPanel.CreateMapPanel(ClientSize.Width - 262, ClientSize.Height - MainMenuStrip.Height);
            MapPanel.Location = new Point(0, MainMenuStrip.Height);
            Controls.Add(MapPanel);
            MapPanel.SendCoordsEvent += MapPanel_SendCoordsEvent;
            ZoomInItem.Click += MapPanel.ZoomINclicked;
            ZoomOutItem.Click += MapPanel.ZoomOUTclicked;
            MaxZoomInItem.Click += MapPanel.MaxZoomINclicked;
            MaxZoomOutItem.Click += MapPanel.MaxZoomOUTclicked;
            StandardZoomItem.Click += MapPanel.StandardZOOMclicked;
            MediumZoomOutItem.Click += MapPanel.MediumZoomOUTclicked;


            MinimapPanel = new MinimapPanel(262, 149);
            MinimapPanel.Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height);
            Controls.Add(MinimapPanel);

            StatusPanel StatusPanel = new StatusPanel(262, ClientSize.Height - MainMenuStrip.Height - 148);
            StatusPanel.Location = new Point(ClientSize.Width - 262, MainMenuStrip.Height + 148);
            Controls.Add(StatusPanel);
        }

        #region What to show on itro screen
        //public void ShowIntroScreen() {
        //    AreWeInIntroScreen = true;
        //    SinaiIcon.Show();
        //    ChoiceMenu.Visible = true;
        //    if (MapForm != null) MapForm.Close();
        //    if (statusForm != null) statusForm.Close();
        //    if (WorldMapForm != null) WorldMapForm.Close();
        //    MainMenuStrip.Enabled = false; }
        #endregion

        #region Make actions based on choice menu result
        //public void ChoiceMenuResult(int choiceNo, string SAVpath) {
        //    if (choiceNo == 2) LoadGame(SAVpath);
        //    ChoiceMenu.Visible = false;
        //    MainMenuStrip.Enabled = true; }
        #endregion

        #region GAME MENU EVENTS
        private void GameOptions_Click(object sender, EventArgs e) {
            GameOptionsForm GameOptionsForm = new GameOptionsForm();
            GameOptionsForm.Load += new EventHandler(GameOptionsForm_Load);   //so you set the correct size of form
            GameOptionsForm.ShowDialog(); }

        private void GameOptionsForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Width = 746;
            frm.Height = 440;
            frm.Location = new Point(330, 250); }

        private void GraphicOptions_Click(object sender, EventArgs e) {
            GraphicOptionsForm GraphicOptionsForm = new GraphicOptionsForm();
            GraphicOptionsForm.Load += new EventHandler(GraphicOptionsForm_Load);   //so you set the correct size of form
            GraphicOptionsForm.ShowDialog(); }

        private void GraphicOptionsForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Width = 746;
            frm.Height = 280;
            frm.Location = new Point(330, 250); }

        private void CityReportOptions_Click(object sender, EventArgs e) {
            CityReportOptionsForm CityReportOptionsForm = new CityReportOptionsForm();
            CityReportOptionsForm.Load += new EventHandler(CityReportOptionsForm_Load);   //so you set the correct size of form
            CityReportOptionsForm.ShowDialog(); }

        private void CityReportOptionsForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Width = 746;
            frm.Height = 440;
            frm.Location = new Point(330, 250); }

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
        private void MovePieces_Click(object sender, EventArgs e) { }
        private void ViewPieces_Click(object sender, EventArgs e) { }
        private void ShowMapGrid_Click(object sender, EventArgs e) 
        { 
            int var = MapPanel.ToggleMapGrid();
            if (var != 0) ShowMapGridItem.Checked = true;
            else ShowMapGridItem.Checked = false;
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

        private void BuildIrrigation_Click(object sender, EventArgs e) {
            if (BuildIrrigationItem.Enabled) Actions.GiveCommand("Build irrigation"); }

        private void BuildMinesChangeForest_Click(object sender, EventArgs e) {
            if (BuildMinesChangeForestItem.Enabled) Actions.GiveCommand("Build mines/Change forest"); }

        private void CleanUpPollution_Click(object sender, EventArgs e) { }
        private void Pillage_Click(object sender, EventArgs e) { }
        private void Unload_Click(object sender, EventArgs e) { }

        private void GoTo_Click(object sender, EventArgs e) {
            if (GoToItem.Enabled) Actions.GiveCommand("Go To"); }

        private void GoHomeToNearestCity_Click(object sender, EventArgs e) {
            if (GoHomeToNearestCityItem.Enabled) Actions.GiveCommand("Go Home"); }

        private void Fortify_Click(object sender, EventArgs e) {
            if (FortifyItem.Enabled) Actions.GiveCommand("Fortify"); }

        private void Sleep_Click(object sender, EventArgs e) {
            if (SleepItem.Enabled) Actions.GiveCommand("Sleep"); }

        private void Disband_Click(object sender, EventArgs e) { }

        private void ActivateUnit_Click(object sender, EventArgs e) {
            if (ActivateUnitItem.Enabled) Actions.GiveCommand("Activate unit"); }

        private void Wait_Click(object sender, EventArgs e) { }

        private void SkipTurn_Click(object sender, EventArgs e) {
            Actions.GiveCommand("Skip turn"); }

        private void EndPlayerTurn_Click(object sender, EventArgs e) { }

        private void BuildNewCity_Click(object sender, EventArgs e) {
            if(BuildNewCityItem.Enabled) Actions.GiveCommand("Build city"); }

        private void AutomateSettler_Click(object sender, EventArgs e) {
            if (AutomateSettlerItem.Enabled) Actions.GiveCommand("Automate"); }

        private void Paradrop_Click(object sender, EventArgs e) { }
        #endregion
        #region ADVISORS MENU EVENTS
        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }

        private void CityStatus_Click(object sender, EventArgs e) {
            CityStatusForm CityStatusForm = new CityStatusForm();
            CityStatusForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            CityStatusForm.ShowDialog(); }

        private void DefenseMinister_Click(object sender, EventArgs e) {
            DefenseMinisterForm DefenseMinisterForm = new DefenseMinisterForm();
            DefenseMinisterForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DefenseMinisterForm.ShowDialog(); }

        private void ForeignMinister_Click(object sender, EventArgs e) { }

        private void AttitudeAdvisor_Click(object sender, EventArgs e) {
            AttitudeAdvisorForm AttitudeAdvisorForm = new AttitudeAdvisorForm();
            AttitudeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            AttitudeAdvisorForm.ShowDialog(); }

        private void TradeAdvisor_Click(object sender, EventArgs e) {
            TradeAdvisorForm TradeAdvisorForm = new TradeAdvisorForm();
            TradeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            TradeAdvisorForm.ShowDialog(); }

        private void ScienceAdvisor_Click(object sender, EventArgs e) {
            ScienceAdvisorForm ScienceAdvisorForm = new ScienceAdvisorForm();
            ScienceAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            ScienceAdvisorForm.ShowDialog(); }

        private void CasualtyTimeline_Click(object sender, EventArgs e) { }
        #endregion
        #region WORLD MENU EVENTS
        private void WondersOfWorld_Click(object sender, EventArgs e) {
            WondersOfWorldForm WondersOfWorldForm = new WondersOfWorldForm();
            WondersOfWorldForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            WondersOfWorldForm.ShowDialog(); }

        private void Top5Cities_Click(object sender, EventArgs e) { }
        private void CivScore_Click(object sender, EventArgs e) { }

        private void Demographics_Click(object sender, EventArgs e) {
            DemographicsForm DemographicsForm = new DemographicsForm();
            DemographicsForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DemographicsForm.ShowDialog(); }

        private void Spaceships_Click(object sender, EventArgs e) { }
        #endregion
        #region CHEAT MENU EVENTS
        private void ToggleCheatMode_Click(object sender, EventArgs e) { }

        private void CreateUnit_Click(object sender, EventArgs e) {
            CreateUnitForm CreateUnitForm = new CreateUnitForm();
            CreateUnitForm.Load += new EventHandler(CreateUnitForm_Load);   //so you set the correct size of form
            CreateUnitForm.ShowDialog(); }

        private void CreateUnitForm_Load(object sender, EventArgs e) {
            Form frm = sender as Form;
            frm.Location = new Point(300, 200);
            frm.Width = 746;
            frm.Height = 459; }

        private void RevealMap_Click(object sender, EventArgs e) { }
        private void SetHumanPlayer_Click(object sender, EventArgs e) { }

        private void SetGameYear_Click(object sender, EventArgs e) {
            SetGameYearForm SetGameYearForm = new SetGameYearForm();
            SetGameYearForm.Load += new EventHandler(SetGameYearForm_Load);   //so you set the correct size of form
            SetGameYearForm.ShowDialog(); }

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
                if (Game.Instance.ActiveUnit.GAS == (UnitGAS.Air | UnitGAS.Sea)) PillageItem.Enabled = false;
                BuildRoadItem.Enabled = false;
                BuildIrrigationItem.Enabled = false;
                BuildMinesChangeForestItem.Enabled = false;
                CleanUpPollutionItem.Enabled = false;
                UnloadItem.Enabled = false;
            }
        }
        #endregion

        #region Some shortcuts keys are not supported. Grab them with this method.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!AreWeInIntroScreen)
            {
                switch (keyData)
                {
                    case Keys.NumPad1: Actions.GiveCommand("Move SW"); break;
                    case Keys.NumPad2: Actions.GiveCommand("Move S"); break;
                    case Keys.NumPad3: Actions.GiveCommand("Move SE"); break;
                    case Keys.NumPad4: Actions.GiveCommand("Move W"); break;
                    case Keys.NumPad6: Actions.GiveCommand("Move E"); break;
                    case Keys.NumPad7: Actions.GiveCommand("Move NW"); break;
                    case Keys.NumPad8: Actions.GiveCommand("Move N"); break;
                    case Keys.NumPad9: Actions.GiveCommand("Move NE"); break;
                    case Keys.Down: Actions.GiveCommand("Move S"); break;
                    case Keys.Left: Actions.GiveCommand("Move W"); break;
                    case Keys.Right: Actions.GiveCommand("Move E"); break;
                    case Keys.Up: Actions.GiveCommand("Move N"); break;
                    case Keys.A: ActivateUnit_Click(null, null); break;
                    case Keys.B: BuildNewCity_Click(null, null); break;
                    case Keys.F: Fortify_Click(null, null); break;
                    case Keys.G: GoTo_Click(null, null); break;
                    case Keys.H: GoHomeToNearestCity_Click(null, null); break;
                    case Keys.I: BuildIrrigation_Click(null, null); break;
                    case Keys.K: AutomateSettler_Click(null, null); break;
                    case Keys.M: BuildMinesChangeForest_Click(null, null); break;
                    case Keys.O: Actions.GiveCommand("Terraform"); break;
                    case Keys.P: CleanUpPollution_Click(null, null); break; //paradrop!!!
                    case Keys.R: BuildRoad_Click(null, null); break;
                    case Keys.S: Sleep_Click(null, null); break;
                    case Keys.U: Unload_Click(null, null); break;
                    case Keys.W: Wait_Click(null, null); break;
                    case Keys.X: MapPanel.ZoomLvl--; break;
                    case Keys.Z: MapPanel.ZoomLvl++; break;
                    case Keys.Space: SkipTurn_Click(null, null); break;
                    case Keys.Enter: Actions.GiveCommand("ENTER"); break;
                    case (Keys.Control | Keys.N): EndPlayerTurn_Click(null, null); break;
                    case (Keys.Shift | Keys.C): FindCity_Click(null, null); break;
                    case (Keys.Shift | Keys.D): Disband_Click(null, null); break;
                    case (Keys.Shift | Keys.H): ViewThroneRoom_Click(null, null); break;
                    case (Keys.Shift | Keys.P): Pillage_Click(null, null); break;
                    case (Keys.Shift | Keys.R): Revolution_Click(null, null); break;
                    case (Keys.Shift | Keys.T): TaxRate_Click(null, null); break;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Draw border around Sinai image
        private void SinaiBorder_Paint(object sender, PaintEventArgs e) {            
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, SinaiIcon.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, SinaiIcon.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), SinaiIcon.Width - 1, 0, SinaiIcon.Width - 1, SinaiIcon.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, SinaiIcon.Height - 1, SinaiIcon.Width - 1, SinaiIcon.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, SinaiIcon.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, SinaiIcon.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), SinaiIcon.Width - 2, 1, SinaiIcon.Width - 2, SinaiIcon.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, SinaiIcon.Height - 2, SinaiIcon.Width - 2, SinaiIcon.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, SinaiIcon.Width - 4, 2);   //3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, SinaiIcon.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), SinaiIcon.Width - 3, 2, SinaiIcon.Width - 3, SinaiIcon.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, SinaiIcon.Height - 3, SinaiIcon.Width - 3, SinaiIcon.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, SinaiIcon.Width - 5, 3);   //4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, SinaiIcon.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), SinaiIcon.Width - 4, 3, SinaiIcon.Width - 4, SinaiIcon.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, SinaiIcon.Height - 4, SinaiIcon.Width - 4, SinaiIcon.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, SinaiIcon.Width - 6, 4);   //5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, SinaiIcon.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), SinaiIcon.Width - 5, 4, SinaiIcon.Width - 5, SinaiIcon.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, SinaiIcon.Height - 5, SinaiIcon.Width - 5, SinaiIcon.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, SinaiIcon.Width - 11, 9);   //1st layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, 9, SinaiIcon.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), SinaiIcon.Width - 10, 9, SinaiIcon.Width - 10, SinaiIcon.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, SinaiIcon.Height - 10, SinaiIcon.Width - 10, SinaiIcon.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, SinaiIcon.Width - 12, 10);   //2nd layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, 10, SinaiIcon.Height - 12);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), SinaiIcon.Width - 11, 10, SinaiIcon.Width - 11, SinaiIcon.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, SinaiIcon.Height - 11, SinaiIcon.Width - 11, SinaiIcon.Height - 11); }
        #endregion

        private void MapPanel_SendCoordsEvent(int[] rectCoords, int[] rectSize)
        {
            MinimapPanel.UpdateMinimap(rectCoords, rectSize);
        }
    }
}
