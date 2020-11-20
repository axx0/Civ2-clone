using System;
using System.Drawing;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class CityBuyForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        RadioButton CompleteitButton, NevermindButton;
        City ThisCity;
        int itemNo;

        public CityBuyForm(City thisCity)
        {
            InitializeComponent();
            Paint += new PaintEventHandler(CityBuyForm_Paint);

            ThisCity = thisCity;
            itemNo = ThisCity.ItemInProduction;

            //Panel in the middle
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(740, 132),
                BackgroundImage = Images.WallpaperStatusForm,
                BorderStyle = BorderStyle.None
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, 169),
                Size = new Size(740, 36),
                Font = new Font("Times New Roman", 12.0f),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Radio button 1
            CompleteitButton = new RadioButton
            {
                Text = "Complete it.",
                Location = new Point(85, 67),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 18.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(CompleteitButton);

            //Radio button 2
            NevermindButton = new RadioButton
            {
                Text = "Never mind.",
                Location = new Point(85, 97),
                BackColor = Color.Transparent,
                Font = new Font("Times New Roman", 18.0f),
                ForeColor = Color.FromArgb(51, 51, 51),
                AutoSize = true
            };
            MainPanel.Controls.Add(NevermindButton);
        }

        private void CityBuyForm_Load(object sender, EventArgs e)
        {
            NevermindButton.Checked = true;
        }

        private void CityBuyForm_Paint(object sender, PaintEventArgs e)
        {
            //String
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            string itemName;
            if (itemNo < 62) itemName = ReadFiles.UnitName[itemNo];
            else itemName = ReadFiles.ImprovementName[itemNo - 62 + 1];
            e.Graphics.DrawString("Buy " + itemName, new Font("Times New Roman", 18), new SolidBrush(Color.Black), new Point(379 + 1, 7 + 1), sf);
            e.Graphics.DrawString("Buy " + itemName, new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(379, 7), sf);
            sf.Dispose();
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Borders of panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 2, MainPanel.Width, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 0, MainPanel.Width - 2, MainPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 1, MainPanel.Width - 3, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 0, 1, MainPanel.Height - 3);

            //Draw icons and text
            if (itemNo < 62)    //it's a unit
            {
                CompleteitButton.Location = new Point(125, 67);
                NevermindButton.Location = new Point(125, 97);

                string itemName = ReadFiles.UnitName[itemNo];
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Units[itemNo], 128, 96), 4, 4); //2-times larger
                int costToComplete = 10 * ReadFiles.UnitCost[itemNo] - ThisCity.ShieldsProgress;
                e.Graphics.DrawString("Cost to complete " + itemName + ": " + costToComplete.ToString() + " gold.", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(120, 8));
                e.Graphics.DrawString("Treasury: " + Game.Civs[1].Money.ToString() + " gold.", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(120, 35));
            }
            else    //it's an improvement
            {
                CompleteitButton.Location = new Point(85, 67);
                NevermindButton.Location = new Point(85, 97);

                string itemName = ReadFiles.ImprovementName[itemNo - 62 + 1];
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Improvements[itemNo - 62 + 1], 72, 40), 4, 4);
                int costToComplete = 10 * ReadFiles.ImprovementCost[itemNo - 62 + 1] - ThisCity.ShieldsProgress;
                e.Graphics.DrawString("Cost to complete " + itemName + ": " + costToComplete.ToString() + " gold.", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(80, 8));
                e.Graphics.DrawString("Treasury: " + Game.Civs[1].Money.ToString() + " gold.", new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(80, 35));
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (CompleteitButton.Checked) DialogResult = DialogResult.OK;
            else DialogResult = DialogResult.No;
            Close();
        }
    }
}
