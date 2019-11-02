using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RTciv2.Imagery;

namespace RTciv2.Forms
{
    public partial class SetGameYearForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        TextBox ChangeTextBox;

        public SetGameYearForm()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(SetGameYearForm_Paint);

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(458, 78),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, 116),
                Size = new Size(228, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(239, 116),
                Size = new Size(228, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            //Textbox for changeing turn number
            ChangeTextBox = new TextBox
            {
                Location = new Point(134, 36),
                Size = new Size(95, 30),
                Text = Game.Data.TurnNumber.ToString(),
                Font = new Font("Times New Roman", 14)
            };
            MainPanel.Controls.Add(ChangeTextBox);
        }

        private void SetGameYearForm_Load(object sender, EventArgs e) { }

        private void SetGameYearForm_Paint(object sender, PaintEventArgs e)
        {
            //Text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Change # Of Turns Elapsed", new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Change # Of Turns Elapsed", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

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
            //Text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("# Turns Elapsed: " + Game.Data.TurnNumber + ".", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(191, 191, 191)), new Point(3 + 1, 17 + 1), sf);
            e.Graphics.DrawString("# Turns Elapsed: " + Game.Data.TurnNumber + ".", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(3, 17), sf);
            e.Graphics.DrawString("New # Turns:", new Font("Times New Roman", 12), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(3, 50), sf);
            sf.Dispose();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            int newGameYear;
            try
            {
                newGameYear = Convert.ToInt32(ChangeTextBox.Text);
            }            
            catch (System.FormatException)  //If format is invalid (input is string instead of number), then set the turn to 0
            {
                newGameYear = 0;
            }
            
            Game.Data.TurnNumber = newGameYear;
            Application.OpenForms.OfType<StatusForm>().First().RefreshStatusForm();
            Close();
        }

        //if cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e) { Close(); }
    }
}
