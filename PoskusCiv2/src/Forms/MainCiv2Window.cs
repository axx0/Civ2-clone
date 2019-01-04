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
        public MapForm mapForm;
        public StatusForm statusForm;
        public WorldMapForm worldMapForm;
        public CityForm cityForm;

        public MainCiv2Window()
        {
            InitializeComponent();

            mapForm = new MapForm(this);
            statusForm = new StatusForm(this);
            worldMapForm = new WorldMapForm(this);
            cityForm = new CityForm(this);
            
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
            menuStrip1.Items.Add(AdvisorsMenu);
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
        }

        private void MainCiv2Window_Load(object sender, EventArgs e)
        {
            //Load the icon
            Icon ico = Properties.Resources.civ2;
            this.Icon = ico;

            //Load forms
            mapForm.MdiParent = this;
            mapForm.Show();

            statusForm.MdiParent = this;
            statusForm.Show();

            worldMapForm.MdiParent = this;
            worldMapForm.Show();

            cityForm.MdiParent = this;
            cityForm.StartPosition = FormStartPosition.Manual;
            cityForm.Location = new Point(1260, 0);

            mapForm.Focus();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void joinGameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {

        }

        private void drawMapGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        //when grid is checked, pass value to MapForm to draw
        private void drawMapGridToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapForm.GridIsChecked = drawMapGridToolStripMenuItem.Checked;
            mapForm.Invalidate();
        }

        private void drawMapGridWithNumbersToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            mapForm.GridIsChecked = drawMapGridWithNumbersToolStripMenuItem.Checked;
            mapForm.DrawXYnumbers = drawMapGridWithNumbersToolStripMenuItem.Checked;
            mapForm.Invalidate();
        }

        private void toggleCheatModeToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //Create unit from cheat menu!
        private void createUnitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Load unit create form
            CreateUnitForm newMDIChild = new CreateUnitForm();
            newMDIChild.MdiParent = this;
            //newMDIChild1.Size = new System.Drawing.Size(1260, 770);
            //newMDIChild1.BackColor = Color.Black;
            newMDIChild.Show();

        }

        private void drawMapGridWithNumbersToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

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

        private void AdvisorsForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Location = new Point(330, 250);
            frm.Width = 622;
            frm.Height = 421;
        }

        private void ChatWithKings_Click(object sender, EventArgs e) { }
        private void ConsultHighCouncil_Click(object sender, EventArgs e) { }
        private void CasualtyTimeline_Click(object sender, EventArgs e) { }
    }
}
;