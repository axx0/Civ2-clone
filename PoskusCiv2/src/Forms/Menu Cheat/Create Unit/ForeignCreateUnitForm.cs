using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Imagery;

namespace PoskusCiv2.Forms
{
    public partial class ForeignCreateUnitForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;

        public ForeignCreateUnitForm()
        {
            InitializeComponent();

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(668, Game.Data.CivsInPlay.Sum() * 32 + 4),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, Game.Data.CivsInPlay.Sum() * 32 + 42),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(344, Game.Data.CivsInPlay.Sum() * 32 + 42),
                Size = new Size(333, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);
        }

        private void ForeignCreateUnitForm_Load(object sender, EventArgs e) { }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);

            //Civs
            int count = 0;
            for (int civ = 0; civ < 8; civ++)
            {
                if (Game.Data.CivsInPlay[civ] == 0)
                {
                    e.Graphics.DrawString(Game.Civs[civ].TribeName, new Font("Times New Roman", 16), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(40, 2 + count * 32));
                    count++;
                }
            }            
        }

        private void OKButton_Click(object sender, EventArgs e)
        {

        }

        //if cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e) { Close(); }
    }
}
