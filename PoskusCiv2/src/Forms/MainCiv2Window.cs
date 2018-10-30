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

        Label label1 = new Label(); //for testing

        public MainCiv2Window()
        {
            InitializeComponent();

            //Load background icons
            Bitmap iconsGIF = new Bitmap(@"C:\DOS\CIV 2\Civ2\ICONS.GIF");
            Rectangle srcRect1 = new Rectangle(199, 322, 64, 32);
            Bitmap wallpaperMapForm = (Bitmap)iconsGIF.Clone(srcRect1, iconsGIF.PixelFormat);
            Rectangle srcRect2 = new Rectangle(299, 190, 31, 31);
            Bitmap wallpaperStatusForm = (Bitmap)iconsGIF.Clone(srcRect2, iconsGIF.PixelFormat);

            mapForm = new MapForm(this);
            statusForm = new StatusForm(this);
            worldMapForm = new WorldMapForm(this);
            
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(1100, 30);
            label1.ForeColor = Color.Black;
            label1.Text = "WAITING...";
            this.Controls.Add(label1);
        }

        private void MainCiv2Window_Load(object sender, EventArgs e)
        {
            //Load the icon
            System.Drawing.Icon ico = Properties.Resources.civ2;
            this.Icon = ico;

            //Load forms
            mapForm.MdiParent = this;
            mapForm.Size = new System.Drawing.Size(1260, 770);
            mapForm.BackColor = Color.Black;
            mapForm.Show();

            statusForm.MdiParent = this;
            statusForm.StartPosition = FormStartPosition.Manual;
            statusForm.Location = new Point(1260, 200);
            statusForm.Show();

            worldMapForm.MdiParent = this;
            worldMapForm.StartPosition = FormStartPosition.Manual;
            worldMapForm.Location = new Point(1260, 0);
            worldMapForm.Show();

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