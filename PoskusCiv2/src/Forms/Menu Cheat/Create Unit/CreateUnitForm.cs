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
using PoskusCiv2.Units;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Forms
{
    public partial class CreateUnitForm : Civ2form
    {
        DoubleBufferedPanel MainPanel, ChoicePanel;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of vertical bar
        public int ChosenUnit { get; set; }
        public bool IsVeteran { get; set; }
        int totalNoUnits;

        public CreateUnitForm()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(CreateUnitForm_Paint);

            //Initial states
            //Calculate here total number of choices
            //TO-DO
            totalNoUnits = 62;
            ChosenUnit = 0; //beginning choice
            IsVeteran = false;

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(728, 378),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //Panel of choices
            ChoicePanel = new DoubleBufferedPanel
            {
                Location = new Point(4, 4),
                Size = new Size(MainPanel.Width - 8, MainPanel.Height - 8),
                BackColor = Color.FromArgb(207, 207, 207),
                BorderStyle = BorderStyle.None
            };
            MainPanel.Controls.Add(ChoicePanel);
            ChoicePanel.Paint += new PaintEventHandler(ChoicePanel_Paint);
            ChoicePanel.MouseDown += new MouseEventHandler(ChoicePanel_MouseDown);

            //Foreign button
            Civ2button ForeignButton = new Civ2button
            {
                Location = new Point(9, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Foreign"
            };
            Controls.Add(ForeignButton);
            ForeignButton.Click += new EventHandler(ForeignButton_Click);

            //Veteran button
            Civ2button VeteranButton = new Civ2button
            {
                Location = new Point(131, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Veteran"
            };
            Controls.Add(VeteranButton);
            VeteranButton.Click += new EventHandler(VeteranButton_Click);

            //Obs. button
            Civ2button ObsButton = new Civ2button
            {
                Location = new Point(253, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Obs."
            };
            Controls.Add(ObsButton);
            ObsButton.Click += new EventHandler(ObsButton_Click);

            //Adv. button
            Civ2button AdvButton = new Civ2button
            {
                Location = new Point(374, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Adv."
            };
            Controls.Add(AdvButton);
            AdvButton.Click += new EventHandler(AdvButton_Click);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(496, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(618, 416),
                Size = new Size(119, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            //Vertical bar
            VerticalBar = new VScrollBar()
            {
                Location = new Point(ChoicePanel.Width - 18, 1),
                Size = new Size(17, 368),
                Maximum = totalNoUnits - 7    //16 can be shown
            };
            ChoicePanel.Controls.Add(VerticalBar);
            VerticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);

            if (totalNoUnits < 17) VerticalBar.Visible = false; //don't show it if there are less than 17 units
        }

        private void CreateUnitForm_Load(object sender, EventArgs e) { }

        private void CreateUnitForm_Paint(object sender, PaintEventArgs e)
        {
            //Text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Select Units To Create", new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Select Units To Create", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        //This is so that arrow keys are detected
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                    if (ChosenUnit < totalNoUnits - 1) ChosenUnit++;
                    break;
                case Keys.Up:
                    if (ChosenUnit > 0) ChosenUnit--;
                    break;
                case Keys.PageDown:
                    ChosenUnit = Math.Min(ChosenUnit + 16, totalNoUnits - 1);
                    break;
                case Keys.PageUp:
                    ChosenUnit = Math.Max(ChosenUnit - 16, 0);
                    break;
            }

            //Update relations between chosen value & bar value
            if (ChosenUnit > BarValue + 15) BarValue = ChosenUnit - 15;
            else if (ChosenUnit < BarValue) BarValue = ChosenUnit;
            VerticalBar.Value = BarValue;   //also update the bar value of control

            ChoicePanel.Refresh();  //refresh the panel

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChoicePanel_MouseDown(object sender, MouseEventArgs e)
        {
            ChosenUnit = BarValue + e.Location.Y / 23;
            ChoicePanel.Refresh();  //refresh the panel
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
        }

        private void ChoicePanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, ChoicePanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, ChoicePanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), ChoicePanel.Width - 1, 0, ChoicePanel.Width - 1, ChoicePanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, ChoicePanel.Height - 1, ChoicePanel.Width - 1, ChoicePanel.Height - 1);

            //Entries
            Color textColor;
            FontStyle fontstyle;
            for (int row = 0; row < 16; row++)
            {
                //Draw selection rectangle & determine font of text in it
                if (BarValue + row == ChosenUnit)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(107, 107, 107)), new Rectangle(1, 1 + row * 23, 718, 21));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, 1 + row * 23, 718 + 1, 1 + row * 23);  //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, 1 + row * 23, 1, 1 + row * 23 + 21);       //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, row * 23, 718, row * 23);  //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, (row + 1) * 23 - 1, 718, (row + 1) * 23 - 1);  //border line
                    textColor = Color.White;
                    fontstyle = FontStyle.Bold;
                }
                else
                {
                    textColor = Color.Black;
                    fontstyle = FontStyle.Regular;
                }

                string unitText;
                if (IsVeteran) unitText = "Veteran " + ReadFiles.UnitName[BarValue + row];
                else unitText = ReadFiles.UnitName[BarValue + row];
                if (BarValue + row == ChosenUnit)   //draw shadow of text for chosen line
                {                    
                    e.Graphics.DrawString(unitText, new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(1, row * 23 + 1));
                }
                e.Graphics.DrawString(unitText, new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(1, row * 23));
            }
            e.Dispose();
        }

        private void ForeignButton_Click(object sender, EventArgs e)
        {
            Close();
            ForeignCreateUnitForm ForeignCreateUnitForm = new ForeignCreateUnitForm();
            ForeignCreateUnitForm.Load += new EventHandler(ForeignCreateUnitForm_Load);   //so you set the correct size of form
            ForeignCreateUnitForm.ShowDialog();
        }

        private void ForeignCreateUnitForm_Load(object sender, EventArgs e)
        {
            Form frm = sender as Form;
            frm.Width = 686;
            frm.Height = Game.Data.CivsInPlay.Sum() * 32 + 84;   //dependent on the number of civs in play * 32 + the height of frames
            frm.Location = new Point(330, 250);
        }

        private void VeteranButton_Click(object sender, EventArgs e)
        {
            IsVeteran = !IsVeteran;
            ChoicePanel.Refresh();
        }

        private void ObsButton_Click(object sender, EventArgs e) { }
        private void AdvButton_Click(object sender, EventArgs e) { }

        private void OKButton_Click(object sender, EventArgs e)
        {
            Game.CreateUnit((UnitType)ChosenUnit, 5, 5, false, true, false, IsVeteran, 1, 0, 0, 0, 0, OrderType.NoOrders, 0, 0, 0, 0, 0);
            Application.OpenForms.OfType<StatusForm>().First().Update();
            Close();
        }

        //if cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e) { Close(); }

        //Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = VerticalBar.Value;
            ChoicePanel.Refresh();
        }
    }
}
