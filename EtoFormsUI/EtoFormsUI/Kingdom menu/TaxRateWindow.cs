using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUIExtensionMethods;
using Civ2engine;

namespace EtoFormsUI
{
    public class TaxRateWindow : Civ2customDialog
    {
        private Main main;
        private Game Game => Game.Instance;
        private readonly HScrollBar taxBar, sciBar, luxBar;
        private readonly CheckBox taxCheckbox, sciCheckbox, luxCheckbox;
        private int taxRate, sciRate, luxRate, maxRate;
        bool maxWarn, incTax, decTax, incSci, decSci, incLux, decLux;

        public TaxRateWindow(Main parent) : base(parent, 622, 432, paddingBtmInnerPanel: 10, title: "How Shall We Distribute The Wealth?")
        {
            main = parent;

            taxRate = Game.GetActiveCiv.TaxRate;
            sciRate = Game.GetActiveCiv.ScienceRate;
            luxRate = Game.GetActiveCiv.LuxRate;
            maxRate = 80;

            DefaultButton = new Civ2button(Labels.Ok, 596, 28, new Font("Times new roman", 11));

            taxCheckbox = new CheckBox() { Visible = false };
            taxCheckbox.CheckedChanged += (_, _) =>
            {
                if (taxCheckbox.Checked == true)
                {
                    sciCheckbox.Checked = false;
                    luxCheckbox.Checked = false;
                }
                Surface.Invalidate();
            };
            taxCheckbox.GotFocus += (_, _) => Surface.Invalidate();
            Layout.Add(taxCheckbox, 561, 207);

            sciCheckbox = new CheckBox() { Visible = false };
            sciCheckbox.CheckedChanged += (_, _) =>
            {
                if (sciCheckbox.Checked == true)
                {
                    taxCheckbox.Checked = false;
                    luxCheckbox.Checked = false;
                }
                Surface.Invalidate();
            };
            sciCheckbox.GotFocus += (_, _) => Surface.Invalidate();
            Layout.Add(sciCheckbox, 561, 268);

            luxCheckbox = new CheckBox() { Visible = false };
            luxCheckbox.CheckedChanged += (_, _) =>
            {
                if (luxCheckbox.Checked == true)
                {
                    taxCheckbox.Checked = false;
                    sciCheckbox.Checked = false;
                }
                Surface.Invalidate();
            };
            luxCheckbox.GotFocus += (_, _) => Surface.Invalidate();
            Layout.Add(luxCheckbox, 561, 329);

            taxBar = new HScrollBar() { Width = 505, Maximum = 10, Value = taxRate / 10 };
            sciBar = new HScrollBar() { Width = 505, Maximum = 10, Value = sciRate / 10 };
            luxBar = new HScrollBar() { Width = 505, Maximum = 10, Value = luxRate / 10 };
            taxBar.ValueChanged += (_, _) =>
            {
                incTax = (taxBar.Value * 10 > taxRate);
                decTax = (taxBar.Value * 10 < taxRate);
                if (decTax)
                    maxWarn = false;

                taxRate = taxBar.Value * 10;

                if (taxRate > maxRate)
                {
                    taxRate = maxRate;
                    maxWarn = true;
                }
                else
                {
                    if (incTax)
                    {
                        if (taxRate + sciRate + luxRate != 100) 
                        {
                            if (luxRate != 0 && luxCheckbox.Checked == false)
                            {
                                luxRate -= 10;
                            }
                            else if (sciRate != 0 && sciCheckbox.Checked == false)
                            {
                                sciRate -= 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                taxRate -= 10;
                        }
                    }
                    else
                    {
                        if (taxRate + sciRate + luxRate != 100)
                        {
                            if (luxRate != maxRate && luxCheckbox.Checked == false)
                            {
                                luxRate += 10;
                            }
                            else if (sciRate != maxRate && sciCheckbox.Checked == false)
                            {
                                sciRate += 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                taxRate += 10;
                        }
                    }
                }

                if ((sciBar.Value * 10 > sciRate) || (luxBar.Value * 10 > luxRate))
                    maxWarn = false;

                taxBar.Value = taxRate / 10;
                sciBar.Value = sciRate / 10;
                luxBar.Value = luxRate / 10;
                Invalidate();
            };
            sciBar.ValueChanged += (_, _) =>
            {
                incSci = (sciBar.Value * 10 > sciRate);
                decSci = (sciBar.Value * 10 < sciRate);
                if (decSci)
                    maxWarn = false;

                sciRate = sciBar.Value * 10;

                if (sciRate > maxRate)
                {
                    sciRate = maxRate;
                    maxWarn = true;
                }
                else
                {
                    if (incSci)
                    {
                        if (taxRate + sciRate + luxRate != 100)
                        {
                            if (luxRate != 0 && luxCheckbox.Checked == false)
                            {
                                luxRate -= 10;
                            }
                            else if (taxRate != 0 && taxCheckbox.Checked == false)
                            {
                                taxRate -= 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                sciRate -= 10;
                        }
                    }
                    else
                    {
                        if (taxRate + sciRate + luxRate != 100)
                        {
                            if (luxRate != maxRate && luxCheckbox.Checked == false)
                            {
                                luxRate += 10;
                            }
                            else if (taxRate != maxRate && taxCheckbox.Checked == false)
                            {
                                taxRate += 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                sciRate += 10;
                        }
                    }
                }

                if ((taxBar.Value * 10 > taxRate) || (luxBar.Value * 10 > luxRate))
                    maxWarn = false;

                taxBar.Value = taxRate / 10;
                sciBar.Value = sciRate / 10;
                luxBar.Value = luxRate / 10;
                Invalidate();
            };
            luxBar.ValueChanged += (_, _) =>
            {
                incLux = (luxBar.Value * 10 > luxRate);
                decLux = (luxBar.Value * 10 < luxRate);
                if (decLux)
                    maxWarn = false;

                luxRate = luxBar.Value * 10;

                if (luxRate > maxRate)
                {
                    luxRate = maxRate;
                    maxWarn = true;
                }
                else
                {
                    if (incLux)
                    {
                        if (taxRate + sciRate + luxRate != 100)
                        {
                            if (taxRate != 0 && taxCheckbox.Checked == false)
                            {
                                taxRate -= 10;
                            }
                            else if (sciRate != 0 && sciCheckbox.Checked == false)
                            {
                                sciRate -= 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                luxRate -= 10;
                        }
                    }
                    else
                    {
                        if (taxRate + sciRate + luxRate != 100)
                        {
                            if (taxRate != maxRate && taxCheckbox.Checked == false)
                            {
                                taxRate += 10;
                            }
                            else if (sciRate != maxRate && sciCheckbox.Checked == false)
                            {
                                sciRate += 10;
                            }

                            if (taxRate + sciRate + luxRate != 100)
                                luxRate += 10;
                        }
                    }
                }

                if ((taxBar.Value * 10 > taxRate) || (sciBar.Value * 10 > sciRate))
                    maxWarn = false;

                taxBar.Value = taxRate / 10;
                sciBar.Value = sciRate / 10;
                luxBar.Value = luxRate / 10;
                Invalidate();
            };
            Layout.Add(taxBar, 26, 209);
            Layout.Add(sciBar, 26, 270);
            Layout.Add(luxBar, 26, 331);

            Surface.Paint += Surface_Paint;

            Surface.MouseDown += (_, e) =>
            {
                if (e.Location.X > 560 && e.Location.X < 580 && e.Location.Y > 206 && e.Location.Y < 226)
                {
                    taxCheckbox.Checked = !taxCheckbox.Checked;
                    taxCheckbox.Focus();
                    taxCheckbox.Invalidate();
                }
                
                if (e.Location.X > 560 && e.Location.X < 580 && e.Location.Y > 267 && e.Location.Y < 287)
                {
                    sciCheckbox.Checked = !sciCheckbox.Checked;
                    sciCheckbox.Focus();
                    sciCheckbox.Invalidate();
                }

                if (e.Location.X > 560 && e.Location.X < 580 && e.Location.Y > 328 && e.Location.Y < 348)
                {
                    luxCheckbox.Checked = !luxCheckbox.Checked;
                    luxCheckbox.Focus();
                    luxCheckbox.Invalidate();
                }
            };

            DefaultButton.Click += (sender, e) => CloseWindow();

            KeyDown += (sender, args) =>
            {
                if (args.Key is Keys.Escape) CloseWindow();
            };

            Layout.Add(DefaultButton, 13, 392);
            Content = Layout;
        }

        private void CloseWindow()
        {
            foreach (var item in main.Menu.Items) item.Enabled = true;
            Game.GetActiveCiv.TaxRate = taxRate;
            Game.GetActiveCiv.ScienceRate = sciRate;
            Game.GetActiveCiv.LuxRate = luxRate;
            Close();
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            Color warningColor = maxWarn ? Color.FromArgb(243, 0, 0) : Color.FromArgb(223, 223, 223);
            Color warningColorShadow = maxWarn ? Colors.Black : Color.FromArgb(67, 67, 67);

            e.Graphics.DrawImage(Images.ExtractBitmap(DLLs.Tiles, "taxRateWallpaper").CropImage(new Rectangle(0, 0, 600, 385)), 
                new Rectangle(11, 38, 600, 385));

            var font1 = new Font("Times New Roman", 18);
            Draw.Text(e.Graphics, $"Government: {Game.GetActiveCiv.Government}", font1, warningColor, 
                new Point(17, 47), false, false, warningColorShadow, 1, 1);
            Draw.Text(e.Graphics, $"Maximum Rate: {maxRate}%", font1, warningColor, 
                new Point(327, 47), false, false, warningColorShadow, 1, 1);

            int totalIncome = Game.GetActiveCiv.Cities.Sum(c => c.Tax);
            Draw.Text(e.Graphics, $"Total Income: {totalIncome}", font1, Color.FromArgb(223, 223, 223), 
                new Point(91, 101), false, false, Color.FromArgb(67, 67, 67), 1, 1);

            int totalCost = Game.GetActiveCiv.Cities.Sum(c => c.Improvements.Sum(i => i.Upkeep));
            Draw.Text(e.Graphics, $"Total Cost: {totalCost}", font1, Color.FromArgb(223, 223, 223), 
                new Point(295, 101), false, false, Color.FromArgb(67, 67, 67), 1, 1);

            int discoveries = 0;    // TODO: determine discoveries
            Draw.Text(e.Graphics, $"Discoveries: {discoveries}", font1, Color.FromArgb(223, 223, 223), 
                new Point(278, 128), true, false, Color.FromArgb(67, 67, 67), 1, 1);

            Draw.Text(e.Graphics, "0%", font1, Color.FromArgb(223, 223, 223), 
                new Point(22, 182), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "0%", font1, Color.FromArgb(223, 223, 223), 
                new Point(22, 243), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "0%", font1, Color.FromArgb(223, 223, 223), 
                new Point(22, 304), false, false, Color.FromArgb(67, 67, 67), 1, 1);

            Draw.Text(e.Graphics, $"Taxes: {taxRate}%", font1, 
                Color.FromArgb(223, 223, 223), new Point(278, 182), true, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, $"Science: {sciRate}%", font1, 
                Color.FromArgb(223, 223, 223), new Point(278, 243), true, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, $"Luxuries: {luxRate}%", font1, 
                Color.FromArgb(223, 223, 223), new Point(278, 304), true, false, Color.FromArgb(67, 67, 67), 1, 1);

            Draw.Text(e.Graphics, "100%", font1, Color.FromArgb(223, 223, 223), 
                new Point(472, 182), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "100%", font1, Color.FromArgb(223, 223, 223), 
                new Point(472, 243), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Text(e.Graphics, "100%", font1, Color.FromArgb(223, 223, 223), 
                new Point(472, 304), false, false, Color.FromArgb(67, 67, 67), 1, 1);

            Draw.Text(e.Graphics, "Lock", font1, Color.FromArgb(223, 223, 223),
                new Point(573, 182), true, false, Color.FromArgb(67, 67, 67), 1, 1);
            Draw.Checkbox(e.Graphics, taxCheckbox.Checked == true, new Point(563, 209));
            Draw.Checkbox(e.Graphics, sciCheckbox.Checked == true, new Point(563, 270));
            Draw.Checkbox(e.Graphics, luxCheckbox.Checked == true, new Point(563, 331));
        }
    }
}
