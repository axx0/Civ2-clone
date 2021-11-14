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
    public class CityWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private Map Map => Game.CurrentMap;
        private readonly MapPanel _parent;
        private readonly Civ2button closeIcon, zoomInIcon, zoomOutIcon, infoButton, mapButton, renameButton, happyButton, viewButton, exitButton, buyButton, changeButton;
        private City _thisCity;
        private int _cityZoom;
        private WhatToDraw whatToDraw = WhatToDraw.Info;

        public CityWindow(Main main, MapPanel parent, City city, int cityZoom) : base(636 * (2 + cityZoom) / 2 + 2 * 11, 421 * (2 + cityZoom) / 2 + 11 + (cityZoom == -1 ? 21 : (cityZoom == 0 ? 27 : 39)), cityZoom == -1 ? 21 : (cityZoom == 0 ? 27 : 39), 11, "")
        {
            _cityZoom = cityZoom;
            _parent = parent;
            _thisCity = city;

            Location = _parent.CityWindowLocation;
            Surface.Paint += Surface_Paint;

            // Buy button
            buyButton = new Civ2button("Buy", 68.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            buyButton.Click += BuyButton_Click;
            Layout.Add(buyButton, 11 + 442.ZoomScale(_cityZoom * 4), _paddingTop + 181.ZoomScale(_cityZoom * 4));

            // Change button
            changeButton = new Civ2button("Change", 68.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            changeButton.Click += ChangeButton_Click;
            Layout.Add(changeButton, 11 + 557.ZoomScale(_cityZoom * 4), _paddingTop + 181.ZoomScale(_cityZoom * 4));

            ////// Improvements vertical scrollbar
            ////_improvementsBar = new VScrollBar()
            ////{
            ////    Location = new Point(174, 291),
            ////    Size = new Size(17, 128),
            ////    Maximum = 66 - 9 + 9    // Max improvements=66, 9 can be shown, because of slider size it's 9 elements smaller
            ////};
            ////DrawPanel.Controls.Add(_improvementsBar);
            ////_improvementsBar.ValueChanged += ImprovementsBarValueChanged;

            // Info button
            infoButton = new Civ2button("Info", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            infoButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.Info;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(infoButton, 11 + 459.ZoomScale(_cityZoom * 4), _paddingTop + 364.ZoomScale(_cityZoom * 4));

            // Map button
            mapButton = new Civ2button("Map", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            mapButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.SupportMap;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(mapButton, infoButton.Location.X + 58.ZoomScale(_cityZoom * 4), infoButton.Location.Y);

            // Rename button
            renameButton = new Civ2button("Rename", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            renameButton.Click += RenameButton_Click;
            Layout.Add(renameButton, infoButton.Location.X + 116.ZoomScale(_cityZoom * 4), infoButton.Location.Y);

            // Happy button
            happyButton = new Civ2button("Happy", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            happyButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.HappinessAnalysis;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(happyButton, infoButton.Location.X, infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));

            // View button
            viewButton = new Civ2button("View", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            viewButton.Click += (sender, _) =>
            {
                var cityView = new CityViewWindow(_thisCity);
                cityView.Show();
            };
            Layout.Add(viewButton, infoButton.Location.X + 58.ZoomScale(_cityZoom * 4), infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));

            // Exit button
            exitButton = new Civ2button("Exit", 57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4), new Font("Arial", 9.ZoomScale(_cityZoom * 4)));
            exitButton.Click += (sender, _) =>
            {
                parent.CityWindowZoom = _cityZoom;
                parent.CityWindowLocation = this.Location;
                this.Visible = false;
                this.Close();
            };
            Layout.Add(exitButton, infoButton.Location.X + 116.ZoomScale(_cityZoom * 4), infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));

            //// Next city (UP) button
            //nextCityButton = new NoSelectButton
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

            closeIcon = new Civ2button("", 16, 16, null, Images.CityExit);
            closeIcon.MouseDown += (sender, e) =>
            {
                parent.CityWindowZoom = _cityZoom;
                parent.CityWindowLocation = this.Location;
                this.Visible = false;
                this.Close();
            };
            Layout.Add(closeIcon, 11, 7);

            zoomOutIcon = new Civ2button("", 16, 16, null, Images.CityZoomOUT);
            zoomOutIcon.MouseDown += (sender, e) =>
            {
                if (_cityZoom > -1)
                {
                    _cityZoom--;
                    parent.CityWindowZoom = _cityZoom;
                    RedrawWindowAndControls();
                }
            };
            Layout.Add(zoomOutIcon, 11 + 16 + 2, 7);

            zoomInIcon = new Civ2button("", 16, 16, null, Images.CityZoomIN);
            zoomInIcon.MouseDown += (sender, e) =>
            {
                if (_cityZoom < 1)
                {
                    _cityZoom++;
                    parent.CityWindowZoom = _cityZoom;
                    RedrawWindowAndControls();
                }
            };
            Layout.Add(zoomInIcon, 11 + 2 * 16 + 2 * 2, 7);

            Content = Layout;
        }

        private void RedrawWindowAndControls()
        {
            // New size & location of city window
            var center = new Point(Location.X + this.Width / 2, Location.Y + this.Height / 2);
            _paddingTop = _cityZoom == -1 ? 21 : (_cityZoom == 0 ? 27 : 39);
            Size = new Size(636 * (2 + _cityZoom) / 2 + 2 * 11, 421 * (2 + _cityZoom) / 2 + 11 + _paddingTop);
            Layout.Size = Size;
            Surface.Size = Size;
            Location = new Point(center.X - this.Width / 2, center.Y - this.Height / 2);

            // New size of close/zoom icons
            int zoom = _cityZoom;
            if (_cityZoom == -1) zoom = -2;
            if (_cityZoom == 1) zoom = 4;
            var closeButtonImg = Images.CityExit.Resize(zoom);
            var zoomOutButtonImg = Images.CityZoomOUT.Resize(zoom);
            var zoomInButtonImg = Images.CityZoomIN.Resize(zoom);
            closeIcon.BackgroundImage = closeButtonImg;
            zoomOutIcon.BackgroundImage = zoomOutButtonImg;
            zoomInIcon.BackgroundImage = zoomInButtonImg;
            closeIcon.Size = new Size(closeButtonImg.Width, closeButtonImg.Height);
            zoomOutIcon.Size = new Size(zoomOutButtonImg.Width, zoomOutButtonImg.Height);
            zoomInIcon.Size = new Size(zoomInButtonImg.Width, zoomInButtonImg.Height);
            int paddtop = 7 + _cityZoom;
            if (_cityZoom == 1) paddtop++;
            Layout.Add(closeIcon, 11, paddtop);
            Layout.Add(zoomOutIcon, 11 + closeButtonImg.Width + 2, paddtop);
            Layout.Add(zoomInIcon, 11 + closeButtonImg.Width + zoomOutButtonImg.Width + 2 * 2, paddtop);

            // New size of buttons
            infoButton.Size = new Size(57.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4));
            infoButton.Font = new Font("Arial", 9.ZoomScale(zoom));
            Layout.Add(infoButton, 11 + 459.ZoomScale(_cityZoom * 4), _paddingTop + 364.ZoomScale(_cityZoom * 4));
            mapButton.Size = infoButton.Size;
            mapButton.Font = infoButton.Font;
            Layout.Add(mapButton, infoButton.Location.X + 58.ZoomScale(_cityZoom * 4), infoButton.Location.Y);
            renameButton.Size = infoButton.Size;
            renameButton.Font = infoButton.Font;
            Layout.Add(renameButton, infoButton.Location.X + 116.ZoomScale(_cityZoom * 4), infoButton.Location.Y);
            happyButton.Size = infoButton.Size;
            happyButton.Font = infoButton.Font;
            Layout.Add(happyButton, infoButton.Location.X, infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));
            viewButton.Size = infoButton.Size;
            viewButton.Font = infoButton.Font;
            Layout.Add(viewButton, infoButton.Location.X + 58.ZoomScale(_cityZoom * 4), infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));
            exitButton.Size = infoButton.Size;
            exitButton.Font = infoButton.Font;
            Layout.Add(exitButton, infoButton.Location.X + 116.ZoomScale(_cityZoom * 4), infoButton.Location.Y + 25.ZoomScale(_cityZoom * 4));
            buyButton.Size = new Size(68.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4));
            buyButton.Font = infoButton.Font;
            Layout.Add(buyButton, 11 + 442.ZoomScale(_cityZoom * 4), _paddingTop + 181.ZoomScale(_cityZoom * 4));
            changeButton.Size = new Size(68.ZoomScale(_cityZoom * 4), 24.ZoomScale(_cityZoom * 4));
            changeButton.Font = infoButton.Font;
            Layout.Add(changeButton, 11 + 557.ZoomScale(_cityZoom * 4), _paddingTop + 181.ZoomScale(_cityZoom * 4));

            Content = Layout;

            Invalidate();
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            _paddingTop = _cityZoom == -1 ? 21 : (_cityZoom == 0 ? 27 : 39);

            // TITLE
            string text = "";
            int fontSize = 12;
            string bcad = (Game.GetGameYear < 0) ? "B.C." : "A.D.";
            switch (_cityZoom)
            {
                case -1:
                    text = String.Format($"City of {_thisCity.Name}");
                    fontSize = 10;
                    break;
                case 0:
                    text = String.Format($"City of {_thisCity.Name}, {Math.Abs(Game.GetGameYear)} {bcad}, Population {_thisCity.Population:n0} (Treasury: {_thisCity.Owner.Money} Gold)");
                    fontSize = 12;
                    break;
                case 1:
                    text = String.Format($"City of {_thisCity.Name}, {Math.Abs(Game.GetGameYear)} {bcad}, Population {_thisCity.Population:n0} (Treasury: {_thisCity.Owner.Money} Gold)");
                    fontSize = 17;
                    break;
                default:
                    break;
            }
            var font = new Font("Times New Roman", fontSize);
            Draw.Text(e.Graphics, text, font, Color.FromArgb(135, 135, 135), new Point(Width / 2, _paddingTop / 2), true, true, Colors.Black, 1, 0);

            // WALLPAPER
            e.Graphics.DrawImage(Images.CityWallpaper.Resize(_cityZoom * 4), 11, _paddingTop);

            // TEXTS
            fontSize = _cityZoom == -1 ? 4 : (_cityZoom == 0 ? 9 : 13);
            font = new Font("Arial", fontSize, FontStyle.Bold);
            Draw.Text(e.Graphics, "Citizens", font, Color.FromArgb(223, 187, 63), new Point(101 * (2 + _cityZoom) / 2 + 11, 53 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "City Resources", font, Color.FromArgb(223, 187, 63), new Point(317 * (2 + _cityZoom) / 2 + 11, 52 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "Food Storage", font, Color.FromArgb(75, 155, 35), new Point(535 * (2 + _cityZoom) / 2 + 11, 7 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Colors.Black, 1, 1);
            Draw.Text(e.Graphics, "City Improvements", font, Color.FromArgb(223, 187, 63), new Point(96 * (2 + _cityZoom) / 2 + 11, 296 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

            // CITIZEN FACES
            Draw.CityCitizens(e.Graphics, _thisCity, _cityZoom, new Point(11, _paddingTop));

            // RESOURCES
            Draw.CityResources(e.Graphics, _thisCity, _cityZoom, new Point(11, _paddingTop));

            // FOOD STORAGE
            Draw.CityFoodStorage(e.Graphics, _thisCity, _cityZoom, new Point(11, _paddingTop));

            // RESOURCE MAP
            Draw.CityResourcesMap(e.Graphics, _thisCity, _cityZoom, new Point(11, _paddingTop));

            // PRODUCTION
            Draw.CityProduction(e.Graphics, _thisCity, _cityZoom, new Point(11, _paddingTop));

            // UNITS FROM CITY
            int count = 0;
            int row, col;
            int zoom = 4 * _cityZoom - 3;
            foreach (IUnit unit in _thisCity.SupportedUnits)
            {
                col = count % 4;
                row = count / 4;

                Draw.Unit(e.Graphics, unit, false, zoom, new Point(11 + (7 + 40 * col).ZoomScale(4 * _cityZoom), _paddingTop + (216 + 32 * row).ZoomScale(4 * _cityZoom)));
                count++;

                if (count >= 8) break;
            }

            // UNITS IN CITY / SUPPORT MAP / HAPPINESS
            switch (whatToDraw)
            {
                case WhatToDraw.Info:
                    {
                        // UNITS IN CITY
                        count = 0;
                        zoom = 4 * _cityZoom - 2;
                        font = new Font("Arial", 9.ZoomScale(4 * _cityZoom), FontStyle.Bold);
                        foreach (IUnit unit in _thisCity.UnitsInCity)
                        {
                            col = count % 5;
                            row = count / 5;
                            Draw.Unit(e.Graphics, unit, false, zoom, new Point(11 + (197 + 48 * col).ZoomScale(4 * _cityZoom), _paddingTop + (216 + 41 * row).ZoomScale(4 * _cityZoom)));
                            Draw.Text(e.Graphics, unit.HomeCity == null ? "NON" : unit.HomeCity.Name.Substring(0, 3),
                                font, Colors.Black,
                                new Point(11 + (221 + 48 * col).ZoomScale(4 * _cityZoom),
                                    _paddingTop + (252 + 41 * row).ZoomScale(4 * _cityZoom)), true, true,
                                Color.FromArgb(135, 135, 135), 1, 1);
                            count++;
                        }

                        // TRADE TEXT
                        Draw.Text(e.Graphics,
                            $"Supplies: {string.Join(", ", _thisCity.CommoditySupplied?.Select(c => Game.Rules.CaravanCommoditie[(int) c]) ?? Array.Empty<string>())}",
                            font, Color.FromArgb(227, 83, 15),
                            new Point(11 + 203.ZoomScale(4 * _cityZoom), _paddingTop + 351.ZoomScale(4 * _cityZoom)),
                            false, false, Color.FromArgb(67, 67, 67), 1, 1);
                        Draw.Text(e.Graphics,
                            $"Demands: {string.Join(", ", _thisCity.CommodityDemanded?.Select(c => Game.Rules.CaravanCommoditie[(int) c]) ?? Array.Empty<string>())}",
                            font, Color.FromArgb(227, 83, 15),
                            new Point(11 + 203.ZoomScale(4 * _cityZoom), _paddingTop + 364.ZoomScale(4 * _cityZoom)),
                            false, false, Color.FromArgb(67, 67, 67), 1, 1);
                        if (_thisCity.TradeRoutePartnerCity != null)
                        {
                            Draw.Text(e.Graphics,
                                $"{Game.GetCities[_thisCity.TradeRoutePartnerCity[0]].Name} {_thisCity.CommodityInRoute[0]}: +1",
                                font, Color.FromArgb(227, 83, 15),
                                new Point(11 + 203.ZoomScale(4 * _cityZoom),
                                    _paddingTop + 379.ZoomScale(4 * _cityZoom)), false, false,
                                Color.FromArgb(67, 67, 67), 1, 1);
                        }

                        break;
                    }
                case WhatToDraw.SupportMap:
                    {
                        Color _color;
                        int _drawingOffsetX = 11 + 107 * (2 + _cityZoom);
                        int _drawingOffsetY = _paddingTop + 118 * (2 + _cityZoom);
                        int sqW = 2 * (2 + _cityZoom);
                        int sqH = 1 * (2 + _cityZoom);
                        e.Graphics.AntiAlias = false;
                        for (int _row = 0; _row < Map.YDim; _row++)
                        {
                            for (int _col = 0; _col < Map.XDim; _col++)
                            {
                                _color = Map.Tile[_col, _row].Type == TerrainType.Ocean ? Color.FromArgb(0, 0, 95) : Color.FromArgb(55, 123, 23);
                                e.Graphics.FillRectangle(_color, _drawingOffsetX + sqW * _col, _drawingOffsetY + sqH * _row, sqW, sqH);
                                // Mark this city
                                if (_col == (_thisCity.X - _thisCity.Y % 2) / 2 && _row == _thisCity.Y)
                                    e.Graphics.FillRectangle(Colors.White, _drawingOffsetX + sqW * _col, _drawingOffsetY + sqH * _row, sqH, sqH);
                            }
                        }
                        // Mark supported units (omit those in the city)
                        foreach(IUnit unit in _thisCity.SupportedUnits.Where(u => u.X != _thisCity.X && u.Y != _thisCity.Y))
                            e.Graphics.FillRectangle(Color.FromArgb(159, 159, 159), _drawingOffsetX + sqW * unit.Xreal, _drawingOffsetY + sqH * unit.Y, sqH, sqH);
                        // Text
                        Draw.Text(e.Graphics, "Support Map", font, Color.FromArgb(223, 187, 63), new Point(310 * (2 + _cityZoom) / 2 + 11, 226 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

                        break;
                    }
                case WhatToDraw.HappinessAnalysis:
                    {
                        // Text
                        Draw.Text(e.Graphics, "Happiness Analysis", font, Color.FromArgb(223, 187, 63), new Point(310 * (2 + _cityZoom) / 2 + 11, 226 * (2 + _cityZoom) / 2 + _paddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

                        break;
                    }
            }

            // CITY IMPROVEMENTS LIST
            //int starting = _improvementsBar.Value;   // Starting improvement to draw (changes with slider)
            int starting = 0;   // Starting improvement to draw (changes with slider)
            font = new Font("Arial", 9.ZoomScale(4 * _cityZoom), FontStyle.Bold);
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= _thisCity.Improvements.Length) break;  // Break if no of improvements+wonders

                // Draw improvements
                zoom = 2 * _cityZoom - 4;
                Draw.CityImprovement(e.Graphics, _thisCity.Improvements[i + starting].Type, zoom, new Point(11 + 8.ZoomScale(4 * _cityZoom), _paddingTop + (307 + 12 * i).ZoomScale(4 * _cityZoom)));
                // Sell icons
                zoom = 2 * _cityZoom - 1;
                if ((int)_thisCity.Improvements[i + starting].Type < 39) // Wonders don't have a sell icon
                {
                    using var iconPic = Images.SellIcon.Resize(zoom);
                    e.Graphics.DrawImage(iconPic, new Point(11 + 155.ZoomScale(4 * _cityZoom), _paddingTop + (306 + 12 * i).ZoomScale(4 * _cityZoom)));
                }
                // Improvements text
                Draw.Text(e.Graphics, _thisCity.Improvements[i + starting].Name, font, Colors.White, new Point(11 + 32.ZoomScale(4 * _cityZoom), _paddingTop + (308 + 12 * i).ZoomScale(4 * _cityZoom)), false, false, Colors.Black, 1, 0);
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

        private void RenameButton_Click(object sender, EventArgs e)
        {
            //var _cityRenamePanel = new CityRenamePanel(this, _thisCity);
            //_main.Controls.Add(_cityRenamePanel);
            //_cityRenamePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityRenamePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityRenamePanel.Height / 2));
            //_cityRenamePanel.Show();
            //_cityRenamePanel.BringToFront();
            //this.Enabled = false;   // Freze this panel while rename panel is shown
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

        private enum WhatToDraw
        {
            Info,
            SupportMap,
            HappinessAnalysis
        }
    }
}
