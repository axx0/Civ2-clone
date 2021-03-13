using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Units;
using Civ2engine.Improvements;
using Civ2engine.Enums;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public class CityPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private readonly Main _main;
        private City _thisCity;
        //private readonly DoubleBufferedPanel _resourceMap, _cityResources, _foodStorage, _unitsFromCity, _unitsInCity, _productionPanel, _improvListPanel;
        //private readonly VScrollBar _improvementsBar;

        public CityPanel(Main parent, City city, int width, int height) : base(width, height, 27, 11, "")   // TODO: correct padding for max/min zoom
        {
            _main = parent;
            _thisCity = city;

            MainPanel.Paint += MainPanel_Paint;
            InnerPanel.Paint += InnerPanel_Paint;

            //this.Paint += CityPanel_Paint;

            // Faces panel
            var faces = new Drawable();
            faces.Size = new Size(433, 44);
            faces.Paint += Faces_Paint;
            InnerPanelLayout.Add(faces, 3, 2);

            // Resource map panel
            var resourceMap = new Drawable();
            resourceMap.Size = new Size(196, 145);
            resourceMap.Paint += ResourceMap_Paint;
            InnerPanelLayout.Add(resourceMap, 3, 61);

            // City resources panel
            var cityResources = new Drawable();
            cityResources.Size = new Size(226, 151);
            cityResources.Paint += CityResources_Paint;
            InnerPanelLayout.Add(cityResources, 205, 61);

            // Units from city panel
            var unitsFromCity = new Drawable();
            unitsFromCity.Size = new Size(181, 69);
            unitsFromCity.Paint += UnitsFromCity_Paint;
            InnerPanelLayout.Add(unitsFromCity, 7, 216);

            // Units in city panel
            var unitsInCity = new Drawable();
            unitsInCity.Size = new Size(242, 206);
            unitsInCity.Paint += UnitsInCity_Paint;
            InnerPanelLayout.Add(unitsInCity, 193, 212);

            // Food storage panel
            var foodStorage = new Drawable();
            foodStorage.Size = new Size(195, 146);
            foodStorage.Paint += FoodStorage_Paint;
            InnerPanelLayout.Add(foodStorage, 437, 15);

            // Production panel
            var productionPanel = new Drawable();
            productionPanel.Size = new Size(195, 191);
            productionPanel.Paint += ProductionPanel_Paint;
            InnerPanelLayout.Add(productionPanel, 437, 165);

            // Buy button
            var buyButton = new Civ2button("Buy", 68, 24, new Font("Arial", 9));
            buyButton.Click += BuyButton_Click;
            
            // Change button
            var changeButton = new Civ2button("Change", 68, 24, new Font("Arial", 9));
            changeButton.Click += ChangeButton_Click;

            // Add buy/change buttons to production panel
            var ProductionPanelLayout = new PixelLayout();
            ProductionPanelLayout.Size = new Size(productionPanel.Width, productionPanel.Height);
            ProductionPanelLayout.Add(buyButton, 5, 16);
            ProductionPanelLayout.Add(changeButton, 120, 16);
            productionPanel.Content = ProductionPanelLayout;

            // Improvements list panel
            var improvListPanel = new Drawable();
            improvListPanel.Size = new Size(166, 108);
            improvListPanel.Paint += ImprovementsList_Paint;
            InnerPanelLayout.Add(improvListPanel, 6, 306);

            //// Improvements vertical scrollbar
            //_improvementsBar = new VScrollBar()
            //{
            //    Location = new Point(174, 291),
            //    Size = new Size(17, 128),
            //    Maximum = 66 - 9 + 9    // Max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            //};
            //DrawPanel.Controls.Add(_improvementsBar);
            //_improvementsBar.ValueChanged += ImprovementsBarValueChanged;

            // Info button
            var infoButton = new Civ2button("Info", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            infoButton.Click += InfoButton_Click;
            InnerPanelLayout.Add(infoButton, 461, 364); // norm=(461,364), big=(692,549)

            // Map button
            var mapButton = new Civ2button("Map", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            mapButton.Click += MapButton_Click;
            InnerPanelLayout.Add(mapButton, infoButton.Location.X + 58, infoButton.Location.Y);

            // Rename button
            var renameButton = new Civ2button("Rename", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            renameButton.Click += RenameButton_Click;
            InnerPanelLayout.Add(renameButton, infoButton.Location.X + 116, infoButton.Location.Y);

            // Happy button
            var happyButton = new Civ2button("Happy", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            happyButton.Click += HappyButton_Click;
            InnerPanelLayout.Add(happyButton, infoButton.Location.X, infoButton.Location.Y + 25);

            // View button
            var viewButton = new Civ2button("View", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            viewButton.Click += ViewButton_Click;
            InnerPanelLayout.Add(viewButton, infoButton.Location.X + 58, infoButton.Location.Y + 25);

            // Exit button
            var exitButton = new Civ2button("Exit", 57, 24, new Font("Arial", 9));  // norm=(57,24), big=(86,36)
            exitButton.Click += ExitButton_Click;
            InnerPanelLayout.Add(exitButton, infoButton.Location.X + 116, infoButton.Location.Y + 25);

            //// Next city (UP) button
            //var _nextCityButton = new NoSelectButton
            //{
            //    Location = new Point(440, 364), // norm=(440,364), big=(660,550)
            //    Size = new Size(21, 24),  // norm=(21,24), big=(32,36)
            //    BackColor = Color.FromArgb(107, 107, 107)
            //};
            //_nextCityButton.FlatStyle = FlatStyle.Flat;
            //DrawPanel.Controls.Add(_nextCityButton);
            //_nextCityButton.Click += NextCityButton_Click;
            //_nextCityButton.Paint += NextCityButton_Paint;

            //// Previous city (DOWN) button
            //var _prevCityButton = new NoSelectButton
            //{
            //    Location = new Point(440, 389), // norm=(440,389), big=(660,550)
            //    Size = new Size(21, 24),  // norm=(21,24), big=(32,36)
            //    BackColor = Color.FromArgb(107, 107, 107)
            //};
            //_prevCityButton.FlatStyle = FlatStyle.Flat;
            //DrawPanel.Controls.Add(_prevCityButton);
            //_prevCityButton.Click += PrevCityButton_Click;
            //_prevCityButton.Paint += PrevCityButton_Paint;

            InnerPanel.Content = InnerPanelLayout;
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            string bcad = (Game.GetGameYear < 0) ? "B.C." : "A.D.";
            string text = String.Format($"City of {_thisCity.Name}, {Math.Abs(Game.GetGameYear)} {bcad}, Population {_thisCity.Population:n0} (Treasury: {_thisCity.Owner.Money} Gold)");
            using var font = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, text, font, Color.FromArgb(135, 135, 135), new Point(Width / 2, 15), true, true, Colors.Black, 1, 0);
        }

        private void InnerPanel_Paint(object sender, PaintEventArgs e)
        {
            // Background wallpaper
            e.Graphics.DrawImage(Images.CityWallpaper, 0, 0);
            // Texts
            using var font = new Font("Arial", 9, FontStyle.Bold);
            Draw.Text(e.Graphics, "Citizens", font, Color.FromArgb(223, 187, 63), new Point(101, 53), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "City Resources", font, Color.FromArgb(223, 187, 63), new Point(317, 52), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "Food Storage", font, Color.FromArgb(75, 155, 35), new Point(535, 7), true, true, Colors.Black, 1, 1);
            Draw.Text(e.Graphics, "City Improvements", font, Color.FromArgb(223, 187, 63), new Point(96, 296), true, true, Color.FromArgb(67, 67, 67), 1, 1);
        }

        // Draw faces
        private void Faces_Paint(object sender, PaintEventArgs e)
        {
            using var citizensPic = Draw.Citizens(_thisCity, 0);
            e.Graphics.DrawImage(citizensPic, 2, 7);
        }

        // Draw resource map
        private void ResourceMap_Paint(object sender, PaintEventArgs e)
        {
            // Map around city
            using var resMapPic = Draw.CityResourcesMap(_thisCity, -2);
            e.Graphics.DrawImage(resMapPic, 0, 21);
        }

        // Draw city resources
        private void CityResources_Paint(object sender, PaintEventArgs e)
        {
            using var resPic = Draw.CityResources(_thisCity);
            e.Graphics.DrawImage(resPic, new Point(0, 0));
        }

        private void FoodStorage_Paint(object sender, PaintEventArgs e)
        {
            using var storagePic = Draw.FoodStorage(_thisCity);
            e.Graphics.DrawImage(storagePic, new Point(0, 0));
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
                Draw.Unit(e.Graphics, unit, false, zoom, new Point(8 * (8 + zoom) * col, (4 * (8 + zoom + 3)) * row));
                count++;

                if (count >= 8) { break; }
            }
        }

        private void UnitsInCity_Paint(object sender, PaintEventArgs e)
        {
            int count = 0;
            int row;
            int col;
            int zoom = -2;  // zoom=-2(norm), 1(big)
            using var font = new Font("Arial", 9, FontStyle.Bold);
            foreach (IUnit unit in _thisCity.UnitsInCity)
            {
                col = count % 5;
                row = count / 5;
                Draw.Unit(e.Graphics, unit, false, zoom, new Point(8 * (8 + zoom) * col, 6 * (8 + zoom) * row + 5 * row));
                Draw.Text(e.Graphics, unit.HomeCity.Name.Substring(0, 3), font, Colors.Black, new Point(8 * (8 + zoom) * col + 8 * (8 + zoom) / 2, 6 * (8 + zoom) * row + 5 * row + 6 * (8 + zoom)), true, true, Color.FromArgb(135, 135, 135), 1, 1);  // TODO: doesn't work for <3 characters in city name
                count++;
            }

            // Trade text
            Draw.Text(e.Graphics, $"Supplies: {_thisCity.CommoditySupplied[0]}, {_thisCity.CommoditySupplied[2]}, {_thisCity.CommoditySupplied[2]}", font, Color.FromArgb(227, 83, 15), new Point(6, 135), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, $"Demands: {_thisCity.CommodityDemanded[0]}, {_thisCity.CommodityDemanded[2]}, {_thisCity.CommodityDemanded[2]}", font, Color.FromArgb(227, 83, 15), new Point(6, 148), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, $"{Game.GetCities[_thisCity.TradeRoutePartnerCity[0]].Name} {_thisCity.CommodityInRoute[0]}: +1", font, Color.FromArgb(227, 83, 15), new Point(6, 163), false, false, Color.FromArgb(67, 67, 67), 1, 1);
        }

        private void ProductionPanel_Paint(object sender, PaintEventArgs e)
        {
            // Show item currently in production (ProductionItem=0...61 are units, 62...127 are improvements)
            // zoom: Units=-1(norm), Improvements=0(norm)
            using var font = new Font("Arial", 9, FontStyle.Bold);
            // Units
            int zoom;
            if (_thisCity.ItemInProduction < 62)
            {
                zoom = -1;
                Draw.UnitSprite(e.Graphics, (UnitType)_thisCity.ItemInProduction, false, false, zoom, new Point(64, 0));
            }
            // Improvements
            else
            {
                Draw.Text(e.Graphics, Game.Rules.ImprovementName[_thisCity.ItemInProduction - 62 + 1], font, Color.FromArgb(63, 79, 167), new Point(98, 5), true, true, Colors.Black, 1, 1);
                zoom = 0;
                using var improvPic = Images.Improvements[_thisCity.ItemInProduction - 62 + 1].Resize(zoom);
                e.Graphics.DrawImage(improvPic, new Point(79, 18));
            }

            using var cityProdPic = Draw.CityProduction(_thisCity);
            e.Graphics.DrawImage(cityProdPic, new Point(0, 0));  // Draw production shields and sqare around them
        }

        private void ImprovementsList_Paint(object sender, PaintEventArgs e)
        {
            // Draw city improvements
            int x = 2;
            int y = 1;
            //int starting = _improvementsBar.Value;   // Starting improvement to draw (changes with slider)
            int starting = 0;   // Starting improvement to draw (changes with slider)
            int zoom;
            using var font = new Font("Arial", 9, FontStyle.Bold);
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= _thisCity.Improvements.Length) break;  // Break if no of improvements+wonders

                // Draw improvements
                zoom = -4;  // For normal
                using var improvPic = Images.Improvements[(int)_thisCity.Improvements[i + starting].Type].Resize(zoom);
                e.Graphics.DrawImage(improvPic, new Point(2, 1 + 12 * i));
                // Sell icons
                zoom = -1;  // For normal
                if ((int)_thisCity.Improvements[i + starting].Type < 39) // Wonders don't have a sell icon
                {
                    using var iconPic = Images.SellIcon.Resize(zoom);
                    e.Graphics.DrawImage(iconPic, new Point(148, 1 + 12 * i));
                }
                // Improvements text
                Draw.Text(e.Graphics, _thisCity.Improvements[i + starting].Name, font, Colors.White, new Point(x + 26, 2 + 12 * i), false, false, Colors.Black, 1, 0);
            }
        }

        // Once slider value changes --> redraw improvements list
        private void ImprovementsBarValueChanged(object sender, EventArgs e)
        {
            //DrawPanel.Invalidate();
        }

        private void BuyButton_Click(object sender, EventArgs e)
        {
            // Use this so the panel returns a chosen value (what it has chosen to produce)
            //var _cityBuyPanel = new CityBuyPanel(this, _thisCity);
            //_main.Controls.Add(_cityBuyPanel);
            //_cityBuyPanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityBuyPanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityBuyPanel.Height / 2));
            //_cityBuyPanel.Show();
            //_cityBuyPanel.BringToFront();
            //this.Enabled = false;   // Freze this panel while rename panel is shown
        }

        // Panel that returns a chosen value (what it has chosen to produce)
        private void ChangeButton_Click(object sender, EventArgs e)
        {
            //var _cityChangePanel = new CityChangePanel(this, _thisCity);
            //_main.Controls.Add(_cityChangePanel);
            //_cityChangePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityChangePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityChangePanel.Height / 2));
            //_cityChangePanel.Show();
            //_cityChangePanel.BringToFront();
            //this.Enabled = false;   // Freze this panel while rename panel is shown
        }

        private void InfoButton_Click(object sender, EventArgs e) { }

        private void MapButton_Click(object sender, EventArgs e) { }

        private void RenameButton_Click(object sender, EventArgs e)
        {
            //var _cityRenamePanel = new CityRenamePanel(this, _thisCity);
            //_main.Controls.Add(_cityRenamePanel);
            //_cityRenamePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityRenamePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityRenamePanel.Height / 2));
            //_cityRenamePanel.Show();
            //_cityRenamePanel.BringToFront();
            //this.Enabled = false;   // Freze this panel while rename panel is shown
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
            //DrawPanel.Invalidate();
        }

        private void PrevCityButton_Click(object sender, EventArgs e) { }

        private void NextCityButton_Paint(object sender, PaintEventArgs e)
        {
            // Draw lines in button
            using var _pen1 = new Pen(Color.FromArgb(175, 175, 175));
            using var _pen2 = new Pen(Color.FromArgb(43, 43, 43));
            e.Graphics.DrawLine(_pen1, 1, 1, 30, 1);
            e.Graphics.DrawLine(_pen1, 1, 2, 29, 2);
            e.Graphics.DrawLine(_pen1, 1, 1, 1, 33);
            e.Graphics.DrawLine(_pen1, 2, 1, 2, 32);
            e.Graphics.DrawLine(_pen2, 1, 34, 30, 34);
            e.Graphics.DrawLine(_pen2, 2, 33, 30, 33);
            e.Graphics.DrawLine(_pen2, 29, 2, 29, 33);
            e.Graphics.DrawLine(_pen2, 30, 1, 30, 33);
            // Draw the arrow icon
            e.Graphics.DrawImage(Images.NextCityLarge, 2, 1);
        }

        private void PrevCityButton_Paint(object sender, PaintEventArgs e)
        {
            // Draw lines in button
            using var _pen1 = new Pen(Color.FromArgb(175, 175, 175));
            using var _pen2 = new Pen(Color.FromArgb(43, 43, 43));
            e.Graphics.DrawLine(_pen1, 1, 1, 30, 1);
            e.Graphics.DrawLine(_pen1, 1, 2, 29, 2);
            e.Graphics.DrawLine(_pen1, 1, 1, 1, 33);
            e.Graphics.DrawLine(_pen1, 2, 1, 2, 32);
            e.Graphics.DrawLine(_pen2, 1, 34, 30, 34);
            e.Graphics.DrawLine(_pen2, 2, 33, 30, 33);
            e.Graphics.DrawLine(_pen2, 29, 2, 29, 33);
            e.Graphics.DrawLine(_pen2, 30, 1, 30, 33);
            // Draw the arrow icon
            e.Graphics.DrawImage(Images.PrevCityLarge, 2, 1);
        }
    }
}
