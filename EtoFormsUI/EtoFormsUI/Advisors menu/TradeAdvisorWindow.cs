using System.Linq;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUIExtensionMethods;
using Civ2engine;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public class TradeAdvisorWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private readonly VScrollBar bar;
        private int barVal0;
        private readonly List<(ImprovementType, int, int)> improvCosts = new();

        public TradeAdvisorWindow(Main main) : base(main.InterfaceStyle,622, 421, 11, 11)
        {
            // Get costs of improvements
            var sortedTypes = Game.Rules.Improvements.Select(c => c.Type).ToList();
            foreach (var type in sortedTypes)
            {
                var cities = Game.GetActiveCiv.Cities.Where(c => c.Improvements.Any(impr => impr.Type == type));
                var upkeep = cities.Select(c => c.Improvements.First(impr => impr.Type == type).Upkeep).Sum();

                if (cities.Any())
                    improvCosts.Add((type, cities.Count(), upkeep));
            }

            if (Game.GetActiveCiv.Cities.Count + 5 > 9)
            {
                bar = new VScrollBar() { Height = 305, Value = 0, Maximum = Game.GetActiveCiv.Cities.Count + 5 };
                bar.ValueChanged += (_, _) =>
                {
                    barVal0 = bar.Value;
                    Surface.Invalidate();
                };
                Layout.Add(bar, 592, 79);
            }

            barVal0 = bar is null ? 0 : bar.Value;

            KeyDown += (sender, args) =>
            {
                if (args.Key is Keys.Escape) Close();
            };

            var btn1 = new Civ2button("Close", 297, 24, new Font("Times new roman", 11));
            btn1.Click += (_, _) => Close();
            Layout.Add(btn1, 312, 385);

            var btn2 = new Civ2button("Supply and Demand", 297, 24, new Font("Times new roman", 11));
            btn2.Click += (_, _) =>
            {

            };
            Layout.Add(btn2, 13, 385);

            Surface.Paint += Surface_Paint;
            Content = Layout;
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Inner wallpaper
            e.Graphics.DrawImage(Images.ExtractBitmap(DLLs.Tiles, "tradeAdvWallpaper").CropImage(new Rectangle(0, 0, 600, 400)), new Rectangle(11, 11, 600, 400));

            // Text
            var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "TRADE ADVISOR", font1, Color.FromArgb(223, 223, 223), new Point(300, 24), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Holy Empire of the {Game.GetActiveCiv.Adjective}", font1, Color.FromArgb(223, 223, 223),
                new Point(300, 45), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"{Game.GetActiveCiv.LeaderTitle} {Game.GetActiveCiv.LeaderName}: {Game.GetGameYearString}", font1, Color.FromArgb(223, 223, 223),
                new Point(300, 66), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, "City Trade", font1, Colors.White,
                new Point(148, 89), false, false, Colors.Black, 1, 1);

            // Cities
            var font = new Font("Times New Roman", 11, FontStyle.Bold);
            var drawnCities = Game.GetActiveCiv.Cities.Skip(barVal0).Take(11).ToList();
            for (int i = 0; i < drawnCities.Count; i++)
            {
                var city = drawnCities[i];

                // City image
                Draw.City(e.Graphics, city, true, 0, new Point(13 + 64 * ((barVal0 + i + 1) % 2), 104 + 24 * i));

                // City name
                Draw.Text(e.Graphics, city.Name, font, Color.FromArgb(223, 223, 223),
                    new Point(149, 113 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);

                // Tax
                var offsetX = 254;
                var tax = new FormattedText()
                {
                    Font = font,
                    Text = city.Tax.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(223, 223, 223))
                };
                var taxShadow = new FormattedText()
                {
                    Font = font,
                    Text = city.Tax.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(67, 67, 67))
                };
                e.Graphics.DrawText(taxShadow, new Point(offsetX + 1, 113 + 24 * i + 1));
                e.Graphics.DrawText(tax, new Point(offsetX, 113 + 24 * i));
                offsetX += (int)tax.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.TaxBig, new Point(offsetX + 1, 115 + 24 * i));

                // Science
                var sci = new FormattedText()
                {
                    Font = font,
                    Text = city.Science.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(223, 223, 223))
                };
                var sciShadow = new FormattedText()
                {
                    Font = font,
                    Text = city.Science.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(67, 67, 67))
                };
                offsetX += 16;
                e.Graphics.DrawText(sciShadow, new Point(offsetX + 1, 113 + 24 * i + 1));
                e.Graphics.DrawText(sci, new Point(offsetX, 113 + 24 * i));
                offsetX += (int)sci.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.SciBig, new Point(offsetX, 115 + 24 * i));
            }

            // Total cost
            if (drawnCities.Count <= 10)
            {
                int totalCost = Game.GetActiveCiv.Cities.Sum(c => c.Improvements.Sum(i => i.Upkeep));
                var totalCostText = new FormattedText()
                {
                    Font = font,
                    Text = "Total Cost: " + totalCost,
                    ForegroundBrush = new SolidBrush(Colors.White)
                };
                var totalCostTextShadow = new FormattedText()
                {
                    Font = font,
                    Text = "Total Cost: " + totalCost,
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(totalCostTextShadow, new Point(149 + 1, 113 + 24 * (drawnCities.Count + 1) + 1));
                e.Graphics.DrawText(totalCostText, new Point(149, 113 + 24 * (drawnCities.Count + 1)));
                e.Graphics.DrawImage(CityImages.TaxBig, 
                    new Point(149 + (int)totalCostText.Measure().Width + 1, 115 + 24 * (drawnCities.Count + 1)));
            }

            // Total income
            if (drawnCities.Count <= 9)
            {
                int totalIncome = Game.GetActiveCiv.Cities.Sum(c => c.Tax);
                var totalIncomeText = new FormattedText()
                {
                    Font = font,
                    Text = "Total Income: " + totalIncome,
                    ForegroundBrush = new SolidBrush(Colors.White)
                };
                var totalIncomeTextShadow = new FormattedText()
                {
                    Font = font,
                    Text = "Total Income: " + totalIncome,
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(totalIncomeTextShadow, new Point(149 + 1, 113 + 24 * (drawnCities.Count + 2) + 1));
                e.Graphics.DrawText(totalIncomeText, new Point(149, 113 + 24 * (drawnCities.Count + 2)));
                e.Graphics.DrawImage(CityImages.TaxBig,
                    new Point(149 + (int)totalIncomeText.Measure().Width + 1, 115 + 24 * (drawnCities.Count + 2)));
            }

            // Total science
            if (drawnCities.Count <= 8)
            {
                int totalSci = Game.GetActiveCiv.Cities.Sum(c => c.Science);
                var totalSciText = new FormattedText()
                {
                    Font = font,
                    Text = "Total Science: " + totalSci,
                    ForegroundBrush = new SolidBrush(Colors.White)
                };
                var totalSciTextShadow = new FormattedText()
                {
                    Font = font,
                    Text = "Total Science: " + totalSci,
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(totalSciTextShadow, new Point(149 + 1, 113 + 24 * (drawnCities.Count + 3) + 1));
                e.Graphics.DrawText(totalSciText, new Point(149, 113 + 24 * (drawnCities.Count + 3)));
                e.Graphics.DrawImage(CityImages.SciBig,
                    new Point(149 + (int)totalSciText.Measure().Width + 1, 115 + 24 * (drawnCities.Count + 3)));
            }

            // Discoveries
            if (drawnCities.Count <= 7)
            {
                int discoveries = 0;    // TODO: determine discoveries
                var discoveriesText = new FormattedText()
                {
                    Font = font,
                    Text = "Discoveries: " + discoveries + " Turns",
                    ForegroundBrush = new SolidBrush(Colors.White)
                };
                var discoveriesTextShadow = new FormattedText()
                {
                    Font = font,
                    Text = "Discoveries: " + discoveries + " Turns",
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(discoveriesTextShadow, new Point(149 + 1, 113 + 24 * (drawnCities.Count + 4) + 1));
                e.Graphics.DrawText(discoveriesText, new Point(149, 113 + 24 * (drawnCities.Count + 4)));
            }

            // Maintenance costs
            Draw.Text(e.Graphics, "Maintenance Costs", font1, Colors.White,
                new Point(342, 89), false, false, Colors.Black, 1, 1);
            var drawnMainten = improvCosts.Skip(barVal0).Take(11).ToList();
            for (int i = 0; i < drawnMainten.Count; i++)
            {
                var offsetX = 342;
                var mainten = new FormattedText()
                {
                    Font = font,
                    Text = drawnMainten[i].Item2 + " " + Game.Rules.Improvements[(int)drawnMainten[i].Item1].Name
                        + " (Cost: " + drawnMainten[i].Item3,
                    ForegroundBrush = new SolidBrush(Color.FromArgb(255, 223, 79))
                };
                var maintenShadow = new FormattedText()
                {
                    Font = font,
                    Text = drawnMainten[i].Item2 + " " + Game.Rules.Improvements[(int)drawnMainten[i].Item1].Name
                        + " (Cost: " + drawnMainten[i].Item3,
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(maintenShadow, new Point(offsetX + 1, 113 + 24 * i + 1));
                e.Graphics.DrawText(mainten, new Point(offsetX, 113 + 24 * i));
                offsetX += (int)mainten.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.TaxBig, new Point(offsetX + 1, 115 + 24 * i));
                Draw.Text(e.Graphics, ")", font, Color.FromArgb(255, 223, 79), new Point(offsetX + 14, 113 + 24 * i),
                    false, false, Colors.Black, 1, 1);
            }

            // Total costs
            if (drawnMainten.Count < 11)
            {
                var offsetX = 342;
                var totTxt = new FormattedText()
                {
                    Font = font,
                    Text = "Total Cost: " + improvCosts.Sum(imp => imp.Item3),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(255, 223, 79))
                };
                var totTxtShadow = new FormattedText()
                {
                    Font = font,
                    Text = "Total Cost: " + improvCosts.Sum(imp => imp.Item3),
                    ForegroundBrush = new SolidBrush(Colors.Black)
                };
                e.Graphics.DrawText(totTxtShadow, new Point(offsetX + 1, 113 + 24 * (drawnMainten.Count + 1) + 1));
                e.Graphics.DrawText(totTxt, new Point(offsetX, 113 + 24 * (drawnMainten.Count + 1)));
                offsetX += (int)totTxt.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.TaxBig, new Point(offsetX + 1, 113 + 24 * (drawnMainten.Count + 1) + 1));
            }
        }
    }
}
