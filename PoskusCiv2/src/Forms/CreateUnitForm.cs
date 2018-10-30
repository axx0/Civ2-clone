using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Forms
{
    public partial class CreateUnitForm : Form
    {
        public int selectedIndex;
        public bool isCreateUnitFormActive;

        public CreateUnitForm()
        {
            InitializeComponent();

            //Load background icons
            Bitmap iconsGIF = new Bitmap(@"C:\DOS\CIV 2\Civ2\ICONS.GIF");
            Bitmap wallpaperMapForm = (Bitmap)iconsGIF.Clone(new Rectangle(199, 322, 64, 32), iconsGIF.PixelFormat);
            tableLayoutPanel1.BackgroundImage = wallpaperMapForm;
            label1.BackColor = Color.Transparent;

            //Add units list
            foreach(string unitname in Enum.GetNames(typeof(UnitType)))
            {
                listBox1.Items.Add(unitname);
            }

            selectedIndex = 0;
            isCreateUnitFormActive = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CreateUnitForm_Load(object sender, EventArgs e)
        {
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            //AddUnit(int type, bool veteran, int locX, int locY, string city, int civ)
            //Units.AddUnit(listBox1.SelectedIndex, true, rnd.Next(10, 20), rnd.Next(10, 20), "London", 1);  //add another unit to stack

            //Units_.AddUnit(listBox1.SelectedIndex, true, MapForm.ClickedBoxX, MapForm.ClickedBoxY, "London", 1);  //add another unit to stack

            //Game.CreateUnit((UnitType)Enum.Parse(typeof(UnitType), listBox1.SelectedItem.ToString()), MapForm.ClickedBoxX, MapForm.ClickedBoxY);

            //draw units in Map Form
            Application.OpenForms.OfType<MapForm>().First().Invalidate();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
