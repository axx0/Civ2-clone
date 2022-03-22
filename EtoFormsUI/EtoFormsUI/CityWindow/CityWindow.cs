using System;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Units;
using Civ2engine.Production;
using Civ2engine.Improvements;
using Civ2engine.Enums;
using EtoFormsUIExtensionMethods;
using System.Diagnostics;
using System.Collections.Generic;
using Civ2engine.MapObjects;

namespace EtoFormsUI
{
    public class CityWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private Map Map => Game.CurrentMap;
        private readonly MapPanel _parent;
        private readonly Civ2button closeIcon, zoomInIcon, zoomOutIcon, infoButton, mapButton, renameButton, happyButton, viewButton, exitButton, buyButton, changeButton;
        private Point BuyButtonLoc => new(11 + 442.ZoomScale(_cityZoom * 4), PaddingTop + 181.ZoomScale(_cityZoom * 4));
        private Point ChangeButtonLoc => new(11 + 557.ZoomScale(_cityZoom * 4), PaddingTop + 181.ZoomScale(_cityZoom * 4));
        private Point InfoButtonLoc => new(11 + 459.ZoomScale(_cityZoom * 4), PaddingTop + 364.ZoomScale(_cityZoom * 4));
        private Point MapButtonLoc => new(InfoButtonLoc.X + 58.ZoomScale(_cityZoom * 4), InfoButtonLoc.Y);
        private Point RenameButtonLoc => new(InfoButtonLoc.X + 116.ZoomScale(_cityZoom * 4), InfoButtonLoc.Y);
        private Point HappyButtonLoc => new(InfoButtonLoc.X, InfoButtonLoc.Y + 25.ZoomScale(_cityZoom * 4));
        private Point ViewButtonLoc => new(InfoButtonLoc.X + 58.ZoomScale(_cityZoom * 4), InfoButtonLoc.Y + 25.ZoomScale(_cityZoom * 4));
        private Point ExitButtonLoc => new(InfoButtonLoc.X + 116.ZoomScale(_cityZoom * 4), InfoButtonLoc.Y + 25.ZoomScale(_cityZoom * 4));
        private int BuyButtonWidth => 68.ZoomScale(_cityZoom * 4);
        private int InfoButtonWidth => 57.ZoomScale(_cityZoom * 4);
        private int ButtonHeight => 24.ZoomScale(_cityZoom * 4);
        private City _thisCity;
        private readonly Main _main;
        private int _cityZoom;
        private WhatToDraw whatToDraw = WhatToDraw.Info;
        private VScrollBar _improvementsBar;
        private Point ImprovementsBarLoc => new(_cityZoom == -1 ? 89 : (_cityZoom == 0 ? 185 : 281), _cityZoom == -1 ? 167 : (_cityZoom == 0 ? 318 : 475));
        private int ImprovementsBarHeight => _cityZoom == -1 ? 63 : (_cityZoom == 0 ? 129 : 194);
        private int PaddingTop => 11 + 16.ZoomScale(Zoom) + Zoom;
        private int ButtonFontSize => 9.ZoomScale(_cityZoom * 4);
        private int Zoom => _cityZoom == -1 ? -2 : (_cityZoom == 0 ? 0 : 4);

        public CityWindow(Main main, MapPanel parent, City city, int cityZoom) : base(636 * (2 + cityZoom) / 2 + 2 * 11, 421 * (2 + cityZoom) / 2 + 11 + (cityZoom == -1 ? 21 : (cityZoom == 0 ? 27 : 39)), cityZoom == -1 ? 21 : (cityZoom == 0 ? 27 : 39), 11, "")
        {
            _main = main;
            _cityZoom = cityZoom;
            _parent = parent;
            _thisCity = city;

            Location = _parent.CityWindowLocation;
            Surface.Paint += Surface_Paint;

            // Buy button
            buyButton = new Civ2button("Buy", BuyButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            buyButton.Click += BuyButton_Click;
            Layout.Add(buyButton, BuyButtonLoc);

            // Change button
            changeButton = new Civ2button("Change", BuyButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            changeButton.Click += ChangeButton_Click;
            Layout.Add(changeButton, ChangeButtonLoc);

            // Info button
            infoButton = new Civ2button("Info", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            infoButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.Info;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(infoButton, InfoButtonLoc);

            // Map button
            mapButton = new Civ2button("Map", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            mapButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.SupportMap;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(mapButton, MapButtonLoc);

            // Rename button
            renameButton = new Civ2button("Rename", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            renameButton.Click += RenameButton_Click;
            Layout.Add(renameButton, RenameButtonLoc);

            // Happy button
            happyButton = new Civ2button("Happy", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            happyButton.Click += (sender, e) =>
            {
                whatToDraw = WhatToDraw.HappinessAnalysis;
                main.Sounds.PlaySound(GameSounds.Move);
                Invalidate();
            };
            Layout.Add(happyButton, HappyButtonLoc);

            // View button
            viewButton = new Civ2button("View", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            viewButton.Click += (sender, _) =>
            {
                var cityView = new CityViewWindow(_thisCity);
                cityView.Show();
            };
            Layout.Add(viewButton, ViewButtonLoc);

            // Exit button
            exitButton = new Civ2button("Exit", InfoButtonWidth, ButtonHeight, new Font("Arial", ButtonFontSize));
            exitButton.Click += (sender, _) =>
            {
                parent.CityWindowZoom = _cityZoom;
                parent.CityWindowLocation = this.Location;
                this.Visible = false;
                this.Close();
            };
            Layout.Add(exitButton, ExitButtonLoc);

            // Improvements scrollbar
            _improvementsBar = new VScrollBar()
            {
                Height = ImprovementsBarHeight,
                Maximum = 16,
                Value = 0
            };
            Layout.Add(_improvementsBar, ImprovementsBarLoc);
            _improvementsBar.ValueChanged += (sender, e) => Surface.Invalidate();

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

            closeIcon = new Civ2button("", 16.ZoomScale(Zoom), 16.ZoomScale(Zoom), null, CityImages.Exit);
            closeIcon.MouseDown += (_, _) =>
            {
                parent.CityWindowZoom = _cityZoom;
                parent.CityWindowLocation = this.Location;
                this.Visible = false;
                this.Close();
            };
            Layout.Add(closeIcon, 11, 7 + Zoom / 2);

            zoomOutIcon = new Civ2button("", 16.ZoomScale(Zoom), 16.ZoomScale(Zoom), null, CityImages.ZoomOUT);
            zoomOutIcon.MouseDown += (_, _) =>
            {
                if (_cityZoom > -1)
                {
                    _cityZoom--;
                    parent.CityWindowZoom = _cityZoom;
                    RedrawWindowAndControls();
                }
            };
            Layout.Add(zoomOutIcon, 11 + 16.ZoomScale(Zoom) + 2, 7 + Zoom / 2);

            zoomInIcon = new Civ2button("", 16.ZoomScale(Zoom), 16.ZoomScale(Zoom), null, CityImages.ZoomIN);
            zoomInIcon.MouseDown += (_, _) =>
            {
                if (_cityZoom < 1)
                {
                    _cityZoom++;
                    parent.CityWindowZoom = _cityZoom;
                    RedrawWindowAndControls();
                }
            };
            Layout.Add(zoomInIcon, 11 + 2 * 16.ZoomScale(Zoom) + 2 * 2, 7 + Zoom / 2);

            Content = Layout;
        }

        private void RedrawWindowAndControls()
        {
            // New size & location of city window
            var center = new Point(Location.X + this.Width / 2, Location.Y + this.Height / 2);
            Size = new Size(636 * (2 + _cityZoom) / 2 + 2 * 11, 421 * (2 + _cityZoom) / 2 + 11 + PaddingTop);
            Layout.Size = Size;
            Surface.Size = Size;
            Location = new Point(center.X - this.Width / 2, center.Y - this.Height / 2);
            _paddingTop = PaddingTop;

            // New size of close/zoom icons
            closeIcon.BackgroundImage = CityImages.Exit.Resize(Zoom);
            zoomOutIcon.BackgroundImage = CityImages.ZoomOUT.Resize(Zoom);
            zoomInIcon.BackgroundImage = CityImages.ZoomIN.Resize(Zoom);
            closeIcon.Size = closeIcon.BackgroundImage.Size;
            zoomOutIcon.Size = zoomOutIcon.BackgroundImage.Size;
            zoomInIcon.Size = zoomInIcon.BackgroundImage.Size;
            Layout.Move(closeIcon, 11, 7 + Zoom / 2);
            Layout.Move(zoomOutIcon, 11 + closeIcon.Width + 2, 7 + Zoom / 2);
            Layout.Move(zoomInIcon, 11 + closeIcon.Width + zoomOutIcon.Width + 2 * 2, 7 + Zoom / 2);

            // New size of buttons
            infoButton.Size = new Size(InfoButtonWidth, ButtonHeight);
            infoButton.Font = new Font("Arial", ButtonFontSize);
            Layout.Move(infoButton, InfoButtonLoc);
            mapButton.Size = infoButton.Size;
            mapButton.Font = infoButton.Font;
            Layout.Move(mapButton, MapButtonLoc);
            renameButton.Size = infoButton.Size;
            renameButton.Font = infoButton.Font;
            Layout.Move(renameButton, RenameButtonLoc);
            happyButton.Size = infoButton.Size;
            happyButton.Font = infoButton.Font;
            Layout.Move(happyButton, HappyButtonLoc);
            viewButton.Size = infoButton.Size;
            viewButton.Font = infoButton.Font;
            Layout.Move(viewButton, ViewButtonLoc);
            exitButton.Size = infoButton.Size;
            exitButton.Font = infoButton.Font;
            Layout.Move(exitButton, ExitButtonLoc);
            buyButton.Size = new Size(BuyButtonWidth, ButtonHeight);
            buyButton.Font = infoButton.Font;
            Layout.Move(buyButton, BuyButtonLoc);
            changeButton.Size = new Size(BuyButtonWidth, ButtonHeight);
            changeButton.Font = infoButton.Font;
            Layout.Move(changeButton, ChangeButtonLoc);

            _improvementsBar.Height = ImprovementsBarHeight;
            Layout.Move(_improvementsBar, ImprovementsBarLoc);

            Invalidate();
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
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
            Draw.Text(e.Graphics, text, font, Color.FromArgb(135, 135, 135), new Point(Width / 2, PaddingTop / 2), true, true, Colors.Black, 1, 0);

            // WALLPAPER
            e.Graphics.DrawImage(CityImages.Wallpaper.Resize(_cityZoom * 4), 11, PaddingTop);

            // TEXTS
            fontSize = _cityZoom == -1 ? 4 : (_cityZoom == 0 ? 9 : 13);
            font = new Font("Arial", fontSize, FontStyle.Bold);
            Draw.Text(e.Graphics, "Citizens", font, Color.FromArgb(223, 187, 63), new Point(101.ZoomScale(4 * _cityZoom) + 11, 53.ZoomScale(4 * _cityZoom) + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "City Resources", font, Color.FromArgb(223, 187, 63), new Point(317.ZoomScale(4 * _cityZoom) + 11, 52.ZoomScale(4 * _cityZoom) + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "Food Storage", font, Color.FromArgb(75, 155, 35), new Point(535.ZoomScale(4 * _cityZoom) + 11, 7.ZoomScale(4 * _cityZoom) + PaddingTop), true, true, Colors.Black, 1, 1);
            Draw.Text(e.Graphics, "City Improvements", font, Color.FromArgb(223, 187, 63), new Point(96.ZoomScale(4 * _cityZoom) + 11, 296.ZoomScale(4 * _cityZoom) + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "Resource Map", font, Color.FromArgb(223, 187, 63), new Point(101.ZoomScale(4 * _cityZoom) + 11, 195.ZoomScale(4 * _cityZoom) + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

            // CITIZEN FACES
            Draw.CityCitizens(e.Graphics, _thisCity, _cityZoom, new Point(11, PaddingTop));

            // RESOURCES
            Draw.CityResources(e.Graphics, _thisCity, _cityZoom, new Point(11, PaddingTop));

            // FOOD STORAGE
            Draw.CityFoodStorage(e.Graphics, _thisCity, _cityZoom, new Point(11, PaddingTop));

            // RESOURCE MAP
            Draw.CityResourcesMap(e.Graphics, _thisCity, _cityZoom, new Point(11, PaddingTop));

            // PRODUCTION
            Draw.CityProduction(e.Graphics, _thisCity, _cityZoom, new Point(11, PaddingTop));

            // UNITS FROM CITY
            var supUnitPanelPos = new Point(3.ZoomScale(4 * _cityZoom), 212.ZoomScale(4 * _cityZoom));
            if (_thisCity.SupportedUnits.Count < 5)
            {
                fontSize = _cityZoom == -1 ? 4 : (_cityZoom == 0 ? 9 : 13);
                font = new Font("Arial", fontSize, FontStyle.Bold);
                Draw.Text(e.Graphics, "Units Supported", font, Color.FromArgb(223, 187, 63),
                    new Point(11 + supUnitPanelPos.X + (189 / 2).ZoomScale(4 * _cityZoom), PaddingTop + supUnitPanelPos.Y + 12.ZoomScale(4 * _cityZoom)),
                    true, true, Color.FromArgb(67, 67, 67), 1, 1);
            }
            int offsetX, offsetY;
            for (int i = 0; i < 8; i++)
            {
                if (_thisCity.SupportedUnits.Count < i + 1) break;

                offsetX = 11 + supUnitPanelPos.X + (8 + (40 + 3) * (i % 4)).ZoomScale(4 * _cityZoom);
                offsetY = PaddingTop + supUnitPanelPos.Y + (8 + 32 * (i / 4)).ZoomScale(4 * _cityZoom);

                if (_thisCity.SupportedUnits.Count <= 4)
                {
                    offsetY = PaddingTop + supUnitPanelPos.Y + 24.ZoomScale(4 * _cityZoom);
                }

                Draw.Unit(e.Graphics, _thisCity.SupportedUnits[i], false, 3 * _cityZoom - 3, new Point(offsetX, offsetY));
            }

            // UNITS IN CITY / SUPPORT MAP / HAPPINESS
            var unitPanelPos = new Point(193.ZoomScale(4 * _cityZoom), 212.ZoomScale(4 * _cityZoom));
            switch (whatToDraw)
            {
                case WhatToDraw.Info:
                    {
                        if (_thisCity.UnitsInCity.Count < 6)
                        {
                            fontSize = _cityZoom == -1 ? 4 : (_cityZoom == 0 ? 9 : 13);
                            font = new Font("Arial", fontSize, FontStyle.Bold);
                            Draw.Text(e.Graphics, "Units Present", font, Color.FromArgb(223, 187, 63), 
                                new Point(11 + unitPanelPos.X + (242 / 2).ZoomScale(4 * _cityZoom), PaddingTop + unitPanelPos.Y + 12.ZoomScale(4 * _cityZoom)), 
                                true, true, Color.FromArgb(67, 67, 67), 1, 1);
                        }

                        // UNITS IN CITY
                        font = new Font("Arial", 9.ZoomScale(4 * _cityZoom), FontStyle.Bold);
                        for (int i = 0; i < 18; i++)
                        {
                            if (_thisCity.UnitsInCity.Count < i + 1) break;

                            if (i < 10)
                            {
                                offsetX = 11 + unitPanelPos.X + (1 + 48 * (i % 5)).ZoomScale(4 * _cityZoom);
                                offsetY = PaddingTop + unitPanelPos.Y + (3 + 39 * (i / 5)).ZoomScale(4 * _cityZoom);
                            }
                            else
                            {
                                offsetX = 11 + unitPanelPos.X + (25 + 48 * ((i - 10) % 4)).ZoomScale(4 * _cityZoom);
                                offsetY = PaddingTop + unitPanelPos.Y + (22 + 39 * ((i - 10) / 4)).ZoomScale(4 * _cityZoom);
                            }

                            if (_thisCity.UnitsInCity.Count <= 5)
                            {
                                offsetY = PaddingTop + unitPanelPos.Y + 22.ZoomScale(4 * _cityZoom);
                            }

                            Draw.Unit(e.Graphics, _thisCity.UnitsInCity[i], false, 3 * _cityZoom - 2, new Point(offsetX, offsetY));
                            if (i < 10)
                            {
                                Draw.Text(e.Graphics, ShortCityName(_thisCity.UnitsInCity[i].HomeCity),
                                    font, Colors.Black,
                                    new Point(offsetX + 24.ZoomScale(4 * _cityZoom), offsetY + 36.ZoomScale(4 * _cityZoom)), true, false,
                                    Color.FromArgb(135, 135, 135), 1, 1);
                            }
                        }

                        // TRADE TEXT
                        Draw.Text(e.Graphics,
                            $"Supplies: {string.Join(", ", _thisCity.CommoditySupplied?.Select(c => Game.Rules.CaravanCommoditie[(int) c]) ?? Array.Empty<string>())}",
                            font, Color.FromArgb(227, 83, 15),
                            new Point(11 + 203.ZoomScale(4 * _cityZoom), PaddingTop + 351.ZoomScale(4 * _cityZoom)),
                            false, false, Color.FromArgb(67, 67, 67), 1, 1);
                        Draw.Text(e.Graphics,
                            $"Demands: {string.Join(", ", _thisCity.CommodityDemanded?.Select(c => Game.Rules.CaravanCommoditie[(int) c]) ?? Array.Empty<string>())}",
                            font, Color.FromArgb(227, 83, 15),
                            new Point(11 + 203.ZoomScale(4 * _cityZoom), PaddingTop + 364.ZoomScale(4 * _cityZoom)),
                            false, false, Color.FromArgb(67, 67, 67), 1, 1);
                        if (_thisCity.TradeRoutePartnerCity != null)
                        {
                            Draw.Text(e.Graphics,
                                $"{Game.AllCities[_thisCity.TradeRoutePartnerCity[0]].Name} {_thisCity.CommodityInRoute[0]}: +1",
                                font, Color.FromArgb(227, 83, 15),
                                new Point(11 + 203.ZoomScale(4 * _cityZoom),
                                    PaddingTop + 379.ZoomScale(4 * _cityZoom)), false, false,
                                Color.FromArgb(67, 67, 67), 1, 1);
                        }

                        break;
                    }
                case WhatToDraw.SupportMap:
                    {
                        Color _color;
                        int _drawingOffsetX = 11 + 107 * (2 + _cityZoom);
                        int _drawingOffsetY = PaddingTop + 118 * (2 + _cityZoom);
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
                        foreach(Unit unit in _thisCity.SupportedUnits.Where(u => u.X != _thisCity.X && u.Y != _thisCity.Y))
                            e.Graphics.FillRectangle(Color.FromArgb(159, 159, 159), _drawingOffsetX + sqW * unit.Xreal, _drawingOffsetY + sqH * unit.Y, sqH, sqH);
                        // Text
                        Draw.Text(e.Graphics, "Support Map", font, Color.FromArgb(223, 187, 63), new Point(310 * (2 + _cityZoom) / 2 + 11, 226 * (2 + _cityZoom) / 2 + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

                        break;
                    }
                case WhatToDraw.HappinessAnalysis:
                    {
                        // Text
                        Draw.Text(e.Graphics, "Happiness Analysis", font, Color.FromArgb(223, 187, 63), new Point(310 * (2 + _cityZoom) / 2 + 11, 226 * (2 + _cityZoom) / 2 + PaddingTop), true, true, Color.FromArgb(67, 67, 67), 1, 1);

                        break;
                    }
            }

            // CITY IMPROVEMENTS LIST
            _improvementsBar.Maximum = _thisCity.Improvements.Count + 8;
            int starting = _improvementsBar.Value;   // Starting improvement to draw (changes with slider)
            font = new Font("Arial", 9.ZoomScale(4 * _cityZoom), FontStyle.Bold);
            for (int i = 0; i < 9; i++)
            {
                if ((i + starting) >= _thisCity.Improvements.Count) break;  // Break if no of improvements+wonders

                // Draw improvements
                Draw.CityImprovement(e.Graphics, _thisCity.Improvements[i + starting].Type, 2 * _cityZoom - 4, 
                    new Point(11 + 8.ZoomScale(4 * _cityZoom), PaddingTop + (307 + 12 * i).ZoomScale(4 * _cityZoom)));
                // Sell icons
                if ((int)_thisCity.Improvements[i + starting].Type < 39) // Wonders don't have a sell icon
                {
                    e.Graphics.DrawImage(CityImages.SellIcon.Resize(3 * _cityZoom - 1), 
                        new Point(11 + 156.ZoomScale(4 * _cityZoom), PaddingTop + (306 + 12 * i).ZoomScale(4 * _cityZoom)));
                }
                // Improvements text
                Draw.Text(e.Graphics, _thisCity.Improvements[i + starting].Name, font, Colors.White, 
                    new Point(11 + 30.ZoomScale(4 * _cityZoom), PaddingTop + (305 + 12 * i).ZoomScale(4 * _cityZoom)), false, false, Colors.Black, 1, 0);
            }
        }

        /// <summary>
        /// Fixes a crash with city names shorter than 3 characters
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        private static string ShortCityName(City city)
        {
            return city == null ? "NON" : city.Name.Length < 3 ? city.Name : city.Name[..3];
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
            var canProduce = ProductionPossibilities.GetAllowedProductionOrders(_thisCity);
            var dialog = new Civ2dialog(_main, _main.popupBoxList["PRODUCTION"],
                new List<string> { _thisCity.Name },
                listbox: new ListboxDefinition { LeftText = canProduce.Select(p => p.GetDescription()).ToList() });
            
            dialog.ShowModal(this);
            if (dialog.SelectedButton == "OK")
            {
                _thisCity.ItemInProduction = canProduce[dialog.SelectedIndex];
                this.Invalidate();
            }
            // var _cityChangePanel = new CityChangePanel(this, _thisCity);
            // _main.Controls.Add(_cityChangePanel);
            // _cityChangePanel.Location = new Point(this.Location.X + (this.Width / 2) - (_cityChangePanel.Width / 2), this.Location.Y + (this.Height / 2) - (_cityChangePanel.Height / 2));
            // _cityChangePanel.Show();
            // _cityChangePanel.BringToFront();
            // this.Enabled = false;   // Freze this panel while rename panel is shown
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
            _thisCity = Game.AllCities[1];  // TODO: search only in your civ's cities
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
            e.Graphics.DrawImage(CityImages.NextCity.Resize(1.5, 1.5), 2, 1);
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
            e.Graphics.DrawImage(CityImages.PrevCity.Resize(1.5, 1.5), 2, 1);
        }

        private enum WhatToDraw
        {
            Info,
            SupportMap,
            HappinessAnalysis
        }
    }
}
