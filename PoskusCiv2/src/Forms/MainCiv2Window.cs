using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoskusCiv2.Forms
{
    public partial class MainCiv2Window : Form
    {
        MenuStrip MainMenuStrip;
        public MapForm mapForm;
        public StatusForm statusForm;
        public WorldMapForm worldMapForm;
        public CityForm cityForm;

        public MainCiv2Window()
        {
            InitializeComponent();
            IsMdiContainer = true;
            WindowState = FormWindowState.Maximized;

            //Load the icon
            Icon ico = Properties.Resources.civ2;
            this.Icon = ico;

            //Menustrip
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

            //Kingdom menu
            ToolStripMenuItem KingdomMenu = new ToolStripMenuItem("Kingdom");
            ToolStripMenuItem TaxRateItem = new ToolStripMenuItem("Tax Rate", null, TaxRate_Click);
            ToolStripMenuItem ViewThroneRoomItem = new ToolStripMenuItem("View Throne Room", null, ViewThroneRoom_Click);
            ToolStripMenuItem FindCityItem = new ToolStripMenuItem("Find City", null, FindCity_Click);
            ToolStripMenuItem RevolutionItem = new ToolStripMenuItem("REVOLUTION", null, Revolution_Click);
            MainMenuStrip.Items.Add(KingdomMenu);
            KingdomMenu.DropDownItems.Add(TaxRateItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(ViewThroneRoomItem);
            KingdomMenu.DropDownItems.Add(FindCityItem);
            KingdomMenu.DropDownItems.Add(new ToolStripSeparator());
            KingdomMenu.DropDownItems.Add(RevolutionItem);

            //View menu
            ToolStripMenuItem ViewMenu = new ToolStripMenuItem("View");
            MainMenuStrip.Items.Add(ViewMenu);

            //Orders
            ToolStripMenuItem OrdersMenu = new ToolStripMenuItem("Orders");
            MainMenuStrip.Items.Add(OrdersMenu);

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
            AdvisorsMenu.DropDownItems.Add(ConsultHighCouncilItem);
            AdvisorsMenu.DropDownItems.Add(ChatWithKingsItem);
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
        }

        private void MainCiv2Window_Load(object sender, EventArgs e)
        {
            //Forms
            mapForm = new MapForm(this);
            mapForm.MdiParent = this;
            mapForm.Location = new Point(0, 0);
            mapForm.Show();

            worldMapForm = new WorldMapForm(this);
            worldMapForm.MdiParent = this;
            worldMapForm.StartPosition = FormStartPosition.Manual;
            worldMapForm.Location = new Point((int)(ClientSize.Width * 0.8625) - 6 + 1, 0);
            worldMapForm.Show();

            statusForm = new StatusForm(this);
            statusForm.MdiParent = this;
            statusForm.StartPosition = FormStartPosition.Manual;
            statusForm.Location = new Point((int)(ClientSize.Width * 0.8625) - 6 + 1, worldMapForm.Height + 1);
            statusForm.Show();

            cityForm = new CityForm(this);

            //cityForm.MdiParent = this;
            //cityForm.StartPosition = FormStartPosition.Manual;
            //cityForm.Location = new Point(1260, 0);
        }

        // GAME MENU
        private void GameOptions_Click(object sender, EventArgs e) { }
        private void GraphicOptions_Click(object sender, EventArgs e) { }
        private void CityReportOptions_Click(object sender, EventArgs e) { }
        private void MultiplayerOptions_Click(object sender, EventArgs e) { }
        private void GameProfile_Click(object sender, EventArgs e) { }
        private void PickMusic_Click(object sender, EventArgs e) { }
        private void SaveGame_Click(object sender, EventArgs e) { }
        private void LoadGame_Click(object sender, EventArgs e) { }
        private void JoinGame_Click(object sender, EventArgs e) { }
        private void SetPassword_Click(object sender, EventArgs e) { }
        private void ChangeTimer_Click(object sender, EventArgs e) { }
        private void Retire_Click(object sender, EventArgs e) { }
        private void Quit_Click(object sender, EventArgs e) { Close(); }

        // KINGDOM MENU
        private void TaxRate_Click(object sender, EventArgs e) { }
        private void ViewThroneRoom_Click(object sender, EventArgs e) { }
        private void FindCity_Click(object sender, EventArgs e) { }
        private void Revolution_Click(object sender, EventArgs e) { }

        // ADVISORS MENU
        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }

        private void CityStatus_Click(object sender, EventArgs e)
        {
            CityStatusForm CityStatusForm = new CityStatusForm();
            CityStatusForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            CityStatusForm.ShowDialog();
        }

        private void DefenseMinister_Click(object sender, EventArgs e)
        {
            DefenseMinisterForm DefenseMinisterForm = new DefenseMinisterForm();
            DefenseMinisterForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DefenseMinisterForm.ShowDialog();
        }

        private void ForeignMinister_Click(object sender, EventArgs e) { }

        private void AttitudeAdvisor_Click(object sender, EventArgs e)
        {
            AttitudeAdvisorForm AttitudeAdvisorForm = new AttitudeAdvisorForm();
            AttitudeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            AttitudeAdvisorForm.ShowDialog();
        }

        private void TradeAdvisor_Click(object sender, EventArgs e)
        {
            TradeAdvisorForm TradeAdvisorForm = new TradeAdvisorForm();
            TradeAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            TradeAdvisorForm.ShowDialog();
        }

        private void ScienceAdvisor_Click(object sender, EventArgs e)
        {
            ScienceAdvisorForm ScienceAdvisorForm = new ScienceAdvisorForm();
            ScienceAdvisorForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            ScienceAdvisorForm.ShowDialog();
        }

        private void CasualtyTimeline_Click(object sender, EventArgs e) { }

        // WORLD MENU
        private void WondersOfWorld_Click(object sender, EventArgs e)
        {
            WondersOfWorldForm WondersOfWorldForm = new WondersOfWorldForm();
            WondersOfWorldForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            WondersOfWorldForm.ShowDialog();
        }

        private void Top5Cities_Click(object sender, EventArgs e) { }
        private void CivScore_Click(object sender, EventArgs e) { }

        private void Demographics_Click(object sender, EventArgs e)
        {
            DemographicsForm DemographicsForm = new DemographicsForm();
            DemographicsForm.Load += new EventHandler(AdvisorsForm_Load);   //so you set the correct size of form
            DemographicsForm.ShowDialog();
        }

        private void Spaceships_Click(object sender, EventArgs e) { }

        private void AdvisorsForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Location = new Point(330, 250);
            frm.Width = 622;
            frm.Height = 421;
        }
    }
}
