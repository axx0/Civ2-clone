using System;
using System.Drawing;
using System.Windows.Forms;
using civ2.Bitmaps;

namespace civ2.Forms
{
    public class WondersOfWorldPanel : Civ2panel
    {
        private Game _game => Game.Instance;

        private readonly Main _main;

        private readonly VScrollBar _verticalBar;
        private int _barValue;       // Starting value of view of horizontal bar
        private readonly bool[] _wonderBuilt;  // 28 wonders
        private readonly int[] _whoOwnsWonder;  // 0...barbarians, 1...1st civ, ...
        private readonly string[] _cityWonder;

        public WondersOfWorldPanel(Main parent, int _width, int _height) : base(_width, _height, null, 11, 10)
        {
            _main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.WondersOfWorldWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            // Close button
            var _closeButton = new Civ2button
            {
                Location = new Point(2, 373),
                Size = new Size(596, 24),
                Font = new Font("Times New Roman", 11),
                Text = "Close"
            };
            DrawPanel.Controls.Add(_closeButton);
            _closeButton.Click += CloseButton_Click;

            // Vertical bar
            _verticalBar = new VScrollBar()
            {
                Location = new Point(581, 4),
                Size = new Size(17, 370),
                LargeChange = 1
                //Maximum = TO-DO...
            };
            DrawPanel.Controls.Add(_verticalBar);
            _verticalBar.ValueChanged += VerticalBarValueChanged;

            // Find if wonders are built and who owns them
            _wonderBuilt = new bool[28];
            _whoOwnsWonder = new int[28];
            _cityWonder = new string[28];
            foreach (City city in _game.GetCities)
            {
                for (int i = 0; i < city.Improvements.Length; i++) // Check for each improvement
                {
                    int Id = city.Improvements[i].Id;
                    if (Id > 38)    // Improvements Id>38 are wonders
                    {
                        _wonderBuilt[Id - 39] = true;
                        _whoOwnsWonder[Id - 39] = city.Owner.Id;
                        _cityWonder[Id - 39] = city.Name;
                    }
                }
            }

            //for (int i = 0; i < 28; i++)
            //{
            //    if (WonderBuilt[i]) Console.Write("\n Wonder {0}, {1}, owner {2}", Rules.ImprovementName[i + 39], CityWonder[i], Game.Civs[WhoOwnsWonder[i]].TribeName);
            //}

            _barValue = 0;
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Text
            var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString("WONDERS OF THE ANCIENT WORLD", new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(302 + 2, 22 + 1), sf);
            e.Graphics.DrawString("WONDERS OF THE ANCIENT WORLD", new Font("Times New Roman", 14), new SolidBrush(Color.White), new Point(302, 22), sf);
            // Display wonders
            int count = 0;
            SizeF stringSize;
            for (int i = 0; i < 28; i++)
            {
                if (_wonderBuilt[i])
                {
                    string wonderText = $"{_game.Rules.ImprovementName[i + 39]}  of {_cityWonder[i]} ( {_game.GetCivs[_whoOwnsWonder[i]].TribeName} )";

                    e.Graphics.DrawString(wonderText, new Font("Times New Roman", 13), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(292 + 1, 65 + 40 * count + 1), sf);
                    e.Graphics.DrawString(wonderText, new Font("Times New Roman", 13), new SolidBrush(CivColors.Light[_whoOwnsWonder[i]]), new Point(292, 65 + 40 * count), sf);
                    e.Graphics.DrawRectangle(new Pen(CivColors.Light[_whoOwnsWonder[i]]), new Rectangle(4, 46 + count * 40, 576, 36));

                    // Measure string size so you can position the image of wonder
                    stringSize = e.Graphics.MeasureString(wonderText, new Font("Times New Roman", 13));

                    e.Graphics.DrawImage(Images.Improvements[i + 39], new Point(235 - (int)(stringSize.Width / 2), 55 + 40 * count));

                    count++;
                }
            }
            sf.Dispose();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        // Once slider value changes --> redraw list
        private void VerticalBarValueChanged(object sender, EventArgs e)
        {
            _barValue = _verticalBar.Value;
            Refresh();
        }
    }
}
