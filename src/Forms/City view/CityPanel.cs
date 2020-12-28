using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Improvements;

namespace civ2.Forms
{
    public class CityPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private readonly Main _main;
        private City _thisCity;
        private readonly DoubleBufferedPanel _resourceMap, _cityResources, _foodStorage, _unitsFromCity, _unitsInCity, _productionPanel, _improvListPanel;
        private readonly VScrollBar _improvementsBar;

        public CityPanel(Main parent, City city, int _width, int _height) : base(_width, _height, "", 27, 11)   // TODO: correct padding for max/min zoom
        {
            _main = parent;
            _thisCity = city;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = Images.CityWallpaper;
            DrawPanel.Paint += DrawPanel_Paint;

            this.Paint += CityPanel_Paint;

            // Faces panel
            var _faces = new DoubleBufferedPanel
            {
                Location = new Point(3, 2),
                Size = new Size(433, 44),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_faces);
            _faces.Paint += Faces_Paint;

            // Resource map panel
            _resourceMap = new DoubleBufferedPanel
            {
                Location = new Point(3, 61),
                Size = new Size(196, 145),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_resourceMap);
            _resourceMap.Paint += ResourceMap_Paint;

            // City resources panel
            _cityResources = new DoubleBufferedPanel
            {
                Location = new Point(205, 61),
                Size = new Size(226, 151),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_cityResources);
            _cityResources.Paint += CityResources_Paint;

            // Units from city panel
            _unitsFromCity = new DoubleBufferedPanel
            {
                Location = new Point(7, 216),
                Size = new Size(181, 69),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_unitsFromCity);
            _unitsFromCity.Paint += UnitsFromCity_Paint;

            // Units in city panel
            _unitsInCity = new DoubleBufferedPanel
            {
                Location = new Point(193, 212),
                Size = new Size(242, 206),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_unitsInCity);
            _unitsInCity.Paint += UnitsInCity_Paint;

            // Food storage panel
            _foodStorage = new DoubleBufferedPanel
            {
                Location = new Point(437, 15),
                Size = new Size(195, 146),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_foodStorage);
            _foodStorage.Paint += FoodStorage_Paint;

            // Production panel
            _productionPanel = new DoubleBufferedPanel
            {
                Location = new Point(437, 165),
                Size = new Size(195, 191),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_productionPanel);
            _productionPanel.Paint += ProductionPanel_Paint;

            // Buy button
            var _buyButton = new Civ2button
            {
                Location = new Point(5, 16),
                Size = new Size(68, 24),
                Font = new Font("Arial", 9),
                Text = "Buy"
            };
            _productionPanel.Controls.Add(_buyButton);
            _buyButton.Click += BuyButton_Click;

            // Change button
            var _changeButton = new Civ2button
            {
                Location = new Point(120, 16),
                Size = new Size(68, 24),
                Font = new Font("Arial", 9),
                Text = "Change"
            };
            _productionPanel.Controls.Add(_changeButton);
            _changeButton.Click += ChangeButton_Click;

            // Improvements list panel
            _improvListPanel = new DoubleBufferedPanel()
            {
                Location = new Point(6, 306),
                Size = new Size(166, 108),
                BackColor = Color.Transparent
            };
            DrawPanel.Controls.Add(_improvListPanel);
            _improvListPanel.Paint += ImprovementsList_Paint;

            // Improvements vertical scrollbar
            _improvementsBar = new VScrollBar()
            {
                Location = new Point(174, 291),
                Size = new Size(17, 128),
                Maximum = 66 - 9 + 9    // Max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            };
            DrawPanel.Controls.Add(_improvementsBar);
            _improvementsBar.ValueChanged += ImprovementsBarValueChanged;

            // Info button
            var _infoButton = new Civ2button
            {
                Location = new Point(461, 364), // norm=(461,364), big=(692,549)
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "Info"
            };
            DrawPanel.Controls.Add(_infoButton);
            _infoButton.Click += InfoButton_Click;

            // Map button
            var _mapButton = new Civ2button
            {
                Location = new Point(_infoButton.Location.X + 58, _infoButton.Location.Y),
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "Map"
            };
            DrawPanel.Controls.Add(_mapButton);
            _mapButton.Click += MapButton_Click;

            // Rename button
            var _renameButton = new Civ2button
            {
                Location = new Point(_infoButton.Location.X + 116, _infoButton.Location.Y),
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "Rename"
            };
            DrawPanel.Controls.Add(_renameButton);
            _renameButton.Click += RenameButton_Click;

            // Happy button
            var _happyButton = new Civ2button
            {
                Location = new Point(_infoButton.Location.X, _infoButton.Location.Y + 25),
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "Happy"
            };
            DrawPanel.Controls.Add(_happyButton);
            _happyButton.Click += HappyButton_Click;

            // View button
            var _viewButton = new Civ2button
            {
                Location = new Point(_infoButton.Location.X + 58, _infoButton.Location.Y + 25),
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "View"
            };
            DrawPanel.Controls.Add(_viewButton);
            _viewButton.Click += ViewButton_Click;

            // Exit button
            var _exitButton = new Civ2button
            {
                Location = new Point(_infoButton.Location.X + 116, _infoButton.Location.Y + 25),
                Size = new Size(57, 24),  // norm=(57,24), big=(86,36)
                Font = new Font("Arial", 9),
                Text = "Exit"
            };
            DrawPanel.Controls.Add(_exitButton);
            _exitButton.Click += ExitButton_Click;

            // Next city (UP) button
            var _nextCityButton = new NoSelectButton
            {
                Location = new Point(440, 364), // norm=(440,364), big=(660,550)
                Size = new Size(21, 24),  // norm=(21,24), big=(32,36)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            _nextCityButton.FlatStyle = FlatStyle.Flat;
            DrawPanel.Controls.Add(_nextCityButton);
            _nextCityButton.Click += NextCityButton_Click;
            _nextCityButton.Paint += NextCityButton_Paint;

            // Previous city (DOWN) button
            var _prevCityButton = new NoSelectButton
            {
                Location = new Point(440, 389), // norm=(440,389), big=(660,550)
                Size = new Size(21, 24),  // norm=(21,24), big=(32,36)
                BackColor = Color.FromArgb(107, 107, 107)
            };
            _prevCityButton.FlatStyle = FlatStyle.Flat;
            DrawPanel.Controls.Add(_prevCityButton);
            _prevCityButton.Click += PrevCityButton_Click;
            _prevCityButton.Paint += PrevCityButton_Paint;
        }

        private void CityPanel_Paint(object sender, PaintEventArgs e)
        {
            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            string bcad = (Game.GameYear < 0) ? "B.C." : "A.D.";
            string text = String.Format($"City of {_thisCity.Name}, {Math.Abs(Game.GameYear)} {bcad}, Population {_thisCity.Population:n0} (Treasury: {_thisCity.Owner.Money} Gold)");
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString(text, new Font("Times New Roman", 14), new SolidBrush(Color.Black), new Point((Width / 2) + 1, 15), sf);
            e.Graphics.DrawString(text, new Font("Times New Roman", 14), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(Width / 2, 15), sf);
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Texts
            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString("Citizens", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(101 + 1, 53 + 1), sf);
            e.Graphics.DrawString("Citizens", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(101, 53), sf);
            e.Graphics.DrawString("City Resources", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(317 + 1, 52 + 1), sf);
            e.Graphics.DrawString("City Resources", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(317, 52), sf);
            e.Graphics.DrawString("Food Storage", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(535 + 1, 7 + 1), sf);
            e.Graphics.DrawString("Food Storage", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(75, 155, 35)), new Point(535, 7), sf);
            e.Graphics.DrawString("City Improvements", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(96 + 1, 296 + 1), sf);
            e.Graphics.DrawString("City Improvements", new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(223, 187, 63)), new Point(96, 296), sf);
        }

        // Draw faces
        private void Faces_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Draw.Citizens(_thisCity, 0), 2, 7);
        }

        // Draw resource map
        private void ResourceMap_Paint(object sender, PaintEventArgs e)
        {
            // Map around city
            e.Graphics.DrawImage(Draw.CityResourcesMap(_thisCity, -2), 0, 21);
            // Text
            using var sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
        }

        // Draw city resources
        private void CityResources_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Draw.CityResources(_thisCity), new Point(0, 0));
        }

        private void FoodStorage_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(Draw.FoodStorage(_thisCity), new Point(0, 0));
        }

        private void UnitsFromCity_Paint(object sender, PaintEventArgs e)
        {
            int count = 0;
            int row, col;
            int zoom = -3;  // norm=0.67, big=0.67*1.5=1
            foreach (IUnit unit in _thisCity.SupportedUnits)
            {
                col = count % 4;
                row = count / 4;
                e.Graphics.DrawImage(Draw.Unit(unit, false, zoom), new Point(8 * (8 + zoom) * col, (4 * (8 + zoom + 3)) * row));
                count++;

                if (count >= 8) { break; }
            }
        }

        private void UnitsInCity_Paint(object sender, PaintEventArgs e)
        {
            using var sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            
            int count = 0;
            int row = 0;
            int col = 0;
            int zoom = -2;  // zoom=-2(norm), 1(big)
            foreach (IUnit unit in _thisCity.UnitsInCity)
            {
                col = count % 5;
                row = count / 5;
                e.Graphics.DrawImage(Draw.Unit(unit, false, zoom), new Point(8 * (8 + zoom) * col, 6 * (8 + zoom) * row + 5 * row));
                e.Graphics.DrawString(unit.HomeCity.Name.Substring(0, 3), new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)),  // TODO: doesn't work for <3 characters in city name
                    new Point(8 * (8 + zoom) * col + 8 * (8 + zoom) / 2 + 1, 6 * (8 + zoom) * row + 5 * row + 6 * (8 + zoom) + 1), sf);
                e.Graphics.DrawString(unit.HomeCity.Name.Substring(0, 3), new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black),
                    new Point(8 * (8 + zoom) * col + 8 * (8 + zoom) / 2, 6 * (8 + zoom) * row + 5 * row + 6 * (8 + zoom)), sf);
                count++;
            }

            // Trade text
            e.Graphics.DrawString($"Supplies: {_thisCity.CommoditySupplied[0]}, {_thisCity.CommoditySupplied[2]}, {_thisCity.CommoditySupplied[2]}",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(6 + 1, 135 + 1));
            e.Graphics.DrawString($"Supplies: {_thisCity.CommoditySupplied[0]}, {_thisCity.CommoditySupplied[2]}, {_thisCity.CommoditySupplied[2]}",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(6, 135));
            e.Graphics.DrawString($"Demands: {_thisCity.CommodityDemanded[0]}, {_thisCity.CommodityDemanded[2]}, {_thisCity.CommodityDemanded[2]}",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(6 + 1, 148 + 1));
            e.Graphics.DrawString($"Demands: {_thisCity.CommodityDemanded[0]}, {_thisCity.CommodityDemanded[2]}, {_thisCity.CommodityDemanded[2]}",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(6, 148));
            e.Graphics.DrawString($"{Game.GetCities[_thisCity.TradeRoutePartnerCity[0]].Name} {_thisCity.CommodityInRoute[0]}: +1",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(67, 67, 67)), new Point(6 + 1, 163 + 1));
            e.Graphics.DrawString($"{Game.GetCities[_thisCity.TradeRoutePartnerCity[0]].Name} {_thisCity.CommodityInRoute[0]}: +1",
                new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(227, 83, 15)), new Point(6, 163));
        }

        private void ProductionPanel_Paint(object sender, PaintEventArgs e)
        {
            // Show item currently in production (ProductionItem=0...61 are units, 62...127 are improvements)
            // zoom: Units=-1(norm), Improvements=0(norm)
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            // Units
            int zoom;
            if (_thisCity.ItemInProduction < 62)
            {
                zoom = -1;
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Units[_thisCity.ItemInProduction], zoom), new Point(64, 0));
            }
            // Improvements
            else
            {
                using var sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(Game.Rules.ImprovementName[_thisCity.ItemInProduction - 62 + 1], new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), 98 + 1, 5 + 1, sf);
                e.Graphics.DrawString(Game.Rules.ImprovementName[_thisCity.ItemInProduction - 62 + 1], new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.FromArgb(63, 79, 167)), 98, 5, sf);
                zoom = 0;
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Improvements[_thisCity.ItemInProduction - 62 + 1], zoom), new Point(79, 18));
                sf.Dispose();
            }

            e.Graphics.DrawImage(Draw.CityProduction(_thisCity), new Point(0, 0));  // Draw production shields and sqare around them
        }

        private void ImprovementsList_Paint(object sender, PaintEventArgs e)
        {
            // Draw city improvements
            int x = 2;
            int y = 1;
            int starting = _improvementsBar.Value;   // Starting improvement to draw (changes with slider)
            int zoom;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= _thisCity.Improvements.Length) break;  // Break if no of improvements+wonders to small

                // Draw improvements
                zoom = -4;  // For normal
                e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.Improvements[(int)_thisCity.Improvements[i + starting].Type], zoom), new Point(2, 1 + 12 * i));
                // Sell icons
                zoom = -1;  // For normal
                if ((int)_thisCity.Improvements[i + starting].Type < 39) // Wonders don't have a sell icon
                {
                    e.Graphics.DrawImage(ModifyImage.ResizeImage(Images.SellIcon, zoom), new Point(148, 1 + 12 * i));
                }
                // Improvements text
                e.Graphics.DrawString(_thisCity.Improvements[i + starting].Name, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.Black), new Point(x + 26 + 1, 2 + 12 * i));
                e.Graphics.DrawString(_thisCity.Improvements[i + starting].Name, new Font("Arial", 9, FontStyle.Bold), new SolidBrush(Color.White), new Point(x + 26, 2 + 12 * i));
            }
        }

        // Once slider value changes --> redraw improvements list
        private void ImprovementsBarValueChanged(object sender, EventArgs e)
        {
            DrawPanel.Invalidate();
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
            // Use this so the panel returns a chosen value (what it has chosen to produce)
            var _cityBuyPanel = new CityBuyPanel(this, _thisCity);
            _main.Controls.Add(_cityBuyPanel);
            _cityBuyPanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityBuyPanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityBuyPanel.Height / 2));
            _cityBuyPanel.Show();
            _cityBuyPanel.BringToFront();
            this.Enabled = false;   // Freze this panel while rename panel is shown
        }

        // Panel that returns a chosen value (what it has chosen to produce)
        private void ChangeButton_Click(object sender, EventArgs e)
        {
            var _cityChangePanel = new CityChangePanel(this, _thisCity);
            _main.Controls.Add(_cityChangePanel);
            _cityChangePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityChangePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityChangePanel.Height / 2));
            _cityChangePanel.Show();
            _cityChangePanel.BringToFront();
            this.Enabled = false;   // Freze this panel while rename panel is shown
        }

        private void InfoButton_Click(object sender, EventArgs e) { }

        private void MapButton_Click(object sender, EventArgs e) { }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            var _cityRenamePanel = new CityRenamePanel(this, _thisCity);
            _main.Controls.Add(_cityRenamePanel);
            _cityRenamePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityRenamePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityRenamePanel.Height / 2));
            _cityRenamePanel.Show();
            _cityRenamePanel.BringToFront();
            this.Enabled = false;   // Freze this panel while rename panel is shown
        }

        private void HappyButton_Click(object sender, EventArgs e) { }

        private void ViewButton_Click(object sender, EventArgs e) { }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

        private void NextCityButton_Click(object sender, EventArgs e) 
        {
            _thisCity = Game.GetCities[1];  // TODO: search only in your civ's cities
            Invalidate();
            DrawPanel.Invalidate();
        }

        private void PrevCityButton_Click(object sender, EventArgs e) { }

        private void NextCityButton_Paint(object sender, PaintEventArgs e)
        {
            // Draw lines in button
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 30, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 2, 29, 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 1, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 2, 1, 2, 32);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 1, 34, 30, 34);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 2, 33, 30, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 29, 2, 29, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 30, 1, 30, 33);
            // Draw the arrow icon
            e.Graphics.DrawImage(Images.NextCityLarge, 2, 1);
        }

        private void PrevCityButton_Paint(object sender, PaintEventArgs e)
        {
            // Draw lines in button
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 30, 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 2, 29, 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 1, 1, 1, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(175, 175, 175)), 2, 1, 2, 32);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 1, 34, 30, 34);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 2, 33, 30, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 29, 2, 29, 33);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(43, 43, 43)), 30, 1, 30, 33);
            // Draw the arrow icon
            e.Graphics.DrawImage(Images.PrevCityLarge, 2, 1);
        }
    }
}
