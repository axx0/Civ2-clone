using System;
using System.Drawing;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class CityChangeForm : Civ2form
    {
        public event Action RefreshCityForm;
        DoubleBufferedPanel BackgroundPanel, ChoicePanel;
        City ThisCity;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of vertical bar
        int totalNoUnits;
        int totalNoImprov;

        public CityChangeForm(City thisCity)
        {
            InitializeComponent();

            ThisCity = thisCity;

            //Initial states
            //Calculate here total number of choices
            //TO-DO
            totalNoUnits = 62;
            totalNoImprov = 66;
            //BarValue should always be so that the chosen item is in the center. But the BarValue should be corrected once you are at the edges.
            BarValue = Math.Max(0, ThisCity.ItemInProduction - 8);  //correction for the lower value
            BarValue = Math.Min(totalNoUnits + totalNoImprov - 16, BarValue);   //correction for the upper value

            //StartPosition = FormStartPosition.CenterParent;
            Paint += new PaintEventHandler(CityChangeForm_Paint);

            //Panel in the middle
            BackgroundPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(668, 378),
                BackgroundImage = Images.WallpaperStatusForm,
                BorderStyle = BorderStyle.None
            };
            Controls.Add(BackgroundPanel);
            BackgroundPanel.Paint += new PaintEventHandler(BackgroundPanel_Paint);
            
            //Panel of choices
            ChoicePanel = new DoubleBufferedPanel
            {
                Location = new Point(4, 4),
                Size = new Size(BackgroundPanel.Width - 8, BackgroundPanel.Height - 8),
                BackColor = Color.FromArgb(207, 207, 207),
                BorderStyle = BorderStyle.None
            };
            BackgroundPanel.Controls.Add(ChoicePanel);
            ChoicePanel.Paint += new PaintEventHandler(ChoicePanel_Paint);
            ChoicePanel.MouseDown += new MouseEventHandler(ChoicePanel_MouseDown);

            //Auto button
            Civ2button AutoButton = new Civ2button
            {
                Location = new Point(9, 416),
                Size = new Size(165, 36),
                Font = new Font("Times New Roman", 12.0f),
                Text = "Auto"
            };
            Controls.Add(AutoButton);
            AutoButton.Click += new EventHandler(AutoButton_Click);

            //Help button
            Civ2button HelpButton = new Civ2button
            {
                Location = new Point(177, 416),
                Size = new Size(165, 36),
                Font = new Font("Times New Roman", 12.0f),
                Text = "Help"
            };
            Controls.Add(HelpButton);
            HelpButton.Click += new EventHandler(HelpButton_Click);

            //Cheat! button
            Civ2button CheatButton = new Civ2button
            {
                Location = new Point(345, 416),
                Size = new Size(165, 36),
                Font = new Font("Times New Roman", 12.0f),
                Text = "Cheat!"
            };
            Controls.Add(CheatButton);
            CheatButton.Click += new EventHandler(CheatButton_Click);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(513, 416),
                Size = new Size(164, 36),
                Font = new Font("Times New Roman", 12.0f),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Vertical bar for choosing production
            VerticalBar = new VScrollBar()
            {
                Location = new Point(643, 0),
                Size = new Size(17, 370),
                Maximum = totalNoUnits + totalNoImprov - 7    //16 can be shown
            };
            ChoicePanel.Controls.Add(VerticalBar);
            VerticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);
        }

        private void CityChangeForm_Load(object sender, EventArgs e)
        {
        }

        private void CityChangeForm_Paint(object sender, PaintEventArgs e)
        {
            //Add text on top
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("What shall we build in " + ThisCity.Name + "?", new Font("Times New Roman", 16), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 10), sf);
            e.Graphics.DrawString("What shall we build in " + ThisCity.Name + "?", new Font("Times New Roman", 16), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 9), sf);
            sf.Dispose();
        }

        //This is so that arrow keys are detected
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Down:
                    if (ThisCity.ItemInProduction < totalNoUnits + totalNoImprov - 1) ThisCity.ItemInProduction++;
                    break;
                case Keys.Up:
                    if (ThisCity.ItemInProduction > 0) ThisCity.ItemInProduction--;
                    break;
                case Keys.PageDown:
                    ThisCity.ItemInProduction = Math.Min(ThisCity.ItemInProduction + 16, totalNoUnits + totalNoImprov - 1);
                    break;
                case Keys.PageUp:
                    ThisCity.ItemInProduction = Math.Max(ThisCity.ItemInProduction - 16, 0);
                    break;
            }

            //Update relations between chosen value & bar value
            if (ThisCity.ItemInProduction > BarValue + 15) BarValue = ThisCity.ItemInProduction - 15;
            else if (ThisCity.ItemInProduction < BarValue) BarValue = ThisCity.ItemInProduction;
            VerticalBar.Value = BarValue;   //also update the bar value of control

            ChoicePanel.Refresh();  //refresh the panel
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ChoicePanel_MouseDown(object sender, MouseEventArgs e)
        {
            ThisCity.ItemInProduction = BarValue + e.Location.Y / 23;
            ChoicePanel.Refresh();  //refresh the panel
        }

        private void BackgroundPanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, BackgroundPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, BackgroundPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), BackgroundPanel.Width - 1, 0, BackgroundPanel.Width - 1, BackgroundPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, BackgroundPanel.Height - 1, BackgroundPanel.Width - 1, BackgroundPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, BackgroundPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, BackgroundPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), BackgroundPanel.Width - 2, 1, BackgroundPanel.Width - 2, BackgroundPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, BackgroundPanel.Height - 2, BackgroundPanel.Width - 2, BackgroundPanel.Height - 2);
        }

        private void ChoicePanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, ChoicePanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, ChoicePanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), ChoicePanel.Width - 1, 0, ChoicePanel.Width - 1, ChoicePanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, ChoicePanel.Height - 1, ChoicePanel.Width - 1, ChoicePanel.Height - 1);

            //Entries
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Far;
            Color textColor;
            FontStyle fontstyle;
            for (int row = 0; row < 16; row++)
            {
                //Draw selection rectangle & determine font of text in it
                if (BarValue + row == ThisCity.ItemInProduction)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(107, 107, 107)), new Rectangle(85, 2 + row * 23, 556, 21));
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 84, 1 + row * 23, 84 + 556 + 1, 1 + row * 23);  //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 84, 1 + row * 23, 84, 1 + row * 23 + 21);       //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 84, 1 + row * 23 + 22, 84 + 556 + 1, 1 + row * 23 + 22);  //border line
                    e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 641, 1 + row * 23, 641, 1 + row * 23 + 21);       //border line
                    textColor = Color.White;
                    fontstyle = FontStyle.Bold;
                }
                else
                {
                    textColor = Color.Black;
                    fontstyle = FontStyle.Regular;
                }

                if (BarValue + row < totalNoUnits)   //draw units
                {
                    if (BarValue + row == ThisCity.ItemInProduction)   //draw shadow of text for chosen line
                    {
                        e.Graphics.DrawString(ReadFiles.UnitName[BarValue + row], new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(85 + 1, row * 23 + 1));
                        e.Graphics.DrawString("(20 Turns, ADM: " + ReadFiles.UnitAttack[BarValue + row] + "/" + ReadFiles.UnitDefense[BarValue + row] + "/" + ReadFiles.UnitMove[BarValue + row] + " HP: " + ReadFiles.UnitHitp[BarValue + row] + "/" + ReadFiles.UnitFirepwr[BarValue + row] + ")", new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(ChoicePanel.Width - VerticalBar.Width, row * 23 + 1), sf);
                    }
                    e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Units[BarValue + row], 48, 36), new Point(1 + ((BarValue + row) % 2) * 38, 3 + row * 23 - 8));  //0.75-times the normal size of units
                    e.Graphics.DrawString(ReadFiles.UnitName[BarValue + row], new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(85, row * 23));
                    e.Graphics.DrawString("(20 Turns, ADM: " + ReadFiles.UnitAttack[BarValue + row] + "/" + ReadFiles.UnitDefense[BarValue + row] + "/" + ReadFiles.UnitMove[BarValue + row] + " HP: " + ReadFiles.UnitHitp[BarValue + row] + "/" + ReadFiles.UnitFirepwr[BarValue + row] + ")", new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(ChoicePanel.Width - VerticalBar.Width - 1, row * 23), sf);
                }
                else    //draw improvements
                {
                    int improvNo = BarValue + row - totalNoUnits + 1;
                    if (BarValue + row == ThisCity.ItemInProduction)   //draw shadow of text for chosen line
                    {
                        e.Graphics.DrawString(ReadFiles.ImprovementName[improvNo], new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(85 + 1, row * 23 + 1));
                        e.Graphics.DrawString("(20 Turns)", new Font("Times New Roman", 16, FontStyle.Bold), new SolidBrush(Color.Black), new Point(ChoicePanel.Width - VerticalBar.Width, row * 23 + 1), sf);
                    }
                    e.Graphics.DrawImage(Images.Improvements[improvNo], new Point(1 + ((BarValue + row) % 2) * 38, 3 + row * 23));
                    e.Graphics.DrawString(ReadFiles.ImprovementName[improvNo], new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(85, row * 23));
                    e.Graphics.DrawString("(20 Turns)", new Font("Times New Roman", 16, fontstyle), new SolidBrush(textColor), new Point(ChoicePanel.Width - VerticalBar.Width - 1, row * 23), sf);
                }
            }
            sf.Dispose();
            e.Dispose();
        }

        private void AutoButton_Click(object sender, EventArgs e) { }

        private void HelpButton_Click(object sender, EventArgs e) { }

        private void CheatButton_Click(object sender, EventArgs e) { }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = VerticalBar.Value;
            ChoicePanel.Refresh();
        }
    }
}
