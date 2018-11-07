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
    public partial class WorldMapForm : Form
    {
        public MainCiv2Window mainCiv2Window;

        public WorldMapForm(MainCiv2Window _mainCiv2Window)
        {
            InitializeComponent();
            mainCiv2Window = _mainCiv2Window;
        }

        private void WorldMapForm_Load(object sender, EventArgs e)
        {
            //Load background icons
            Bitmap iconsGIF = new Bitmap(@"C:\DOS\CIV 2\Civ2\ICONS.GIF");
            Rectangle srcRect1 = new Rectangle(199, 322, 64, 32);
            Bitmap wallpaperMapForm = (Bitmap)iconsGIF.Clone(srcRect1, iconsGIF.PixelFormat);
            Rectangle srcRect2 = new Rectangle(299, 190, 31, 31);
            Bitmap wallpaperStatusForm = (Bitmap)iconsGIF.Clone(srcRect2, iconsGIF.PixelFormat);
            tableLayoutPanel1.BackgroundImage = wallpaperMapForm;
            panel1.BackgroundImage = wallpaperStatusForm;   //Panel background image
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
