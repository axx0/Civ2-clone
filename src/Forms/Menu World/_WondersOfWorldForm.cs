using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public partial class _WondersOfWorldForm : Civ2form
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        DoubleBufferedPanel MainPanel;
        VScrollBar VerticalBar;
        public int BarValue { get; set; }       //starting value of view of horizontal bar
        bool[] WonderBuilt = new bool[28];  //28 wonders
        int[] WhoOwnsWonder = new int[28];  //0...barbarians, 1...1st civ, ...
        string[] CityWonder = new string[28];

        public _WondersOfWorldForm()
        {
            InitializeComponent();

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 9),
                Size = new Size(604, 404)
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //Close button
            Civ2button CloseButton = new Civ2button
            {
                Location = new Point(4, 376),
                Size = new Size(596, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            MainPanel.Controls.Add(CloseButton);
            CloseButton.Click += new EventHandler(CloseButton_Click);

            //Vertical bar
            VerticalBar = new VScrollBar()
            {
                Location = new Point(583, 4),
                Size = new Size(17, 370),
                LargeChange = 1
                //Maximum = TO-DO...
            };
            MainPanel.Controls.Add(VerticalBar);
            VerticalBar.ValueChanged += new EventHandler(VerticalBarValueChanged);

            //Find if wonders are built and who owns them
            foreach (City city in Game.GetCities)
            {
                for (int i = 0; i < city.Improvements.Count(); i++) //check for each improvement
                {
                    int Id = city.Improvements[i].Id;
                    if (Id > 38)    //improvements Id>38 are wonders
                    {
                        WonderBuilt[Id - 39] = true;
                        WhoOwnsWonder[Id - 39] = city.Owner.Id;
                        CityWonder[Id - 39] = city.Name;
                    }
                }
            }

            //for (int i = 0; i < 28; i++)
            //{
            //    if (WonderBuilt[i]) Console.Write("\n Wonder {0}, {1}, owner {2}", Rules.ImprovementName[i + 39], CityWonder[i], Game.Civs[WhoOwnsWonder[i]].TribeName);
            //}
        }

        private void WondersOfWorldForm_Load(object sender, EventArgs e) { }

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
            //Draw background
            e.Graphics.DrawImage(Images.WondersOfWorldWallpaper, new Rectangle(2, 2, 600, 400));
            //Text
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("WONDERS OF THE ANCIENT WORLD", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 22 + 1), sf);
            e.Graphics.DrawString("WONDERS OF THE ANCIENT WORLD", new Font("Times New Roman", 14), new SolidBrush(Color.White), new Point(302, 22), sf);
            //Display wonders
            int count = 0;
            for (int i = 0; i < 28; i++)
            {
                if (WonderBuilt[i])
                {
                    string wonderText = Game.Rules.ImprovementName[i + 39] + " of " + CityWonder[i] + " (" + Game.GetCivs[WhoOwnsWonder[i]].TribeName + ")";

                    e.Graphics.DrawString(wonderText, new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(292 + 1, 65 + 40 * count + 1), sf);
                    e.Graphics.DrawString(wonderText, new Font("Times New Roman", 13), new SolidBrush(CivColors.Light[WhoOwnsWonder[i]]), new Point(292, 65 + 40 * count), sf);
                    e.Graphics.DrawRectangle(new Pen(CivColors.Light[WhoOwnsWonder[i]]), new Rectangle(4, 46 + count * 40, 576, 36));

                    //measure string size so you can position the image of wonder
                    SizeF stringSize = e.Graphics.MeasureString(wonderText, new Font("Times New Roman", 13));
                    
                    e.Graphics.DrawImage(Images.Improvements[i + 39], new Point(235 - (int)(stringSize.Width / 2), 55 + 40 * count));

                    count++;
                }
            }
            sf.Dispose();
        }

        private void CloseButton_Click(object sender, EventArgs e) { Close(); }

        //Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            BarValue = VerticalBar.Value;
            Refresh();
        }
    }
}
