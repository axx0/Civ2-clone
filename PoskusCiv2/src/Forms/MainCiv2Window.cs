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
            statusForm.StartPosition = FormStartPosition.Manual;
            statusForm.Location = new Point(1260, 200);
            statusForm.Show();

            worldMapForm.MdiParent = this;
            worldMapForm.StartPosition = FormStartPosition.Manual;
            worldMapForm.Location = new Point(1260, 0);
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
    }
}
;