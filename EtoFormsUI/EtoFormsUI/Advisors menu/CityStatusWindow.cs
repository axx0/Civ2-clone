using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUIExtensionMethods;
using Civ2engine;
using Civ2engine.Production;

namespace EtoFormsUI
{
    public class CityStatusWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private readonly VScrollBar bar;
        private int barVal0;

        public CityStatusWindow(Main main) : base(main.ActiveInterface.Look, 622, 421, 11, 11)
        {
            if (Game.GetActiveCiv.Cities.Count > 12)
            {
                bar = new VScrollBar() { Height = 305, Value = 0, Maximum = Game.GetActiveCiv.Cities.Count };
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

            var btn = new Civ2button("Close", 596, 24, new Font("Times new roman", 11));
            btn.Click += (_, _) => Close();
            Layout.Add(btn, 13, 385);

            Surface.Paint += Surface_Paint;
            Content = Layout;
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Inner wallpaper
            e.Graphics.DrawImage(Images.ExtractBitmap(DLLs.Tiles, "cityStatusWallpaper").CropImage(new Rectangle(0, 0, 600, 400)), new Rectangle(11, 11, 600, 400));
            
            // Text
            var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "CITY STATUS", font1, Color.FromArgb(223, 223, 223), new Point(300, 24), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Holy Empire of the {Game.GetActiveCiv.Adjective}", font1, Color.FromArgb(223, 223, 223), 
                new Point(300, 45), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"{Game.GetActiveCiv.LeaderTitle} {Game.GetActiveCiv.LeaderName}: {Game.GetGameYearString}", font1, Color.FromArgb(223, 223, 223), 
                new Point(300, 66), true, true, Color.FromArgb(67, 67, 67), 2, 1);

            // Cities
            var drawnCities = Game.GetActiveCiv.Cities.Skip(barVal0).Take(12).ToList();
            for (int i = 0; i < drawnCities.Count; i++)
            {
                var city = drawnCities[i];
                var font = new Font("Times New Roman", 11, FontStyle.Bold);

                // City image
                Draw.City(e.Graphics, city, true, 0, new Point(13 + 64 * ((barVal0 + i + 1) % 2), 78 + 24 * i));

                // City name
                Draw.Text(e.Graphics, city.Name, font, Color.FromArgb(223, 223, 223), 
                    new Point(149, 87 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);

                // Food
                var offsetX = 254;
                var foodProd = new FormattedText()
                {
                    Font = font,
                    Text = city.FoodProduction.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(223, 223, 223))
                };
                var foodProdShadow = new FormattedText()
                {
                    Font = font,
                    Text = city.FoodProduction.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(67, 67, 67))
                };
                e.Graphics.DrawText(foodProdShadow, new Point(offsetX + 1, 87 + 24 * i + 1));
                e.Graphics.DrawText(foodProd, new Point(offsetX, 87 + 24 * i));
                offsetX += (int)foodProd.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.FoodBig, new Point(offsetX +  + 1, 87 + 24 * i));

                // Production
                var prod = new FormattedText()
                {
                    Font = font,
                    Text = city.TotalProduction.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(223, 223, 223))
                };
                var prodShadow = new FormattedText()
                {
                    Font = font,
                    Text = city.TotalProduction.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(67, 67, 67))
                };
                offsetX += 16;
                e.Graphics.DrawText(prodShadow, new Point(offsetX + 1, 87 + 24 * i + 1));
                e.Graphics.DrawText(prod, new Point(offsetX, 87 + 24 * i));
                offsetX += (int)prod.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.SupportBig, new Point(offsetX, 87 + 24 * i));

                // Trade
                var trade = new FormattedText()
                {
                    Font = font,
                    Text = city.Trade.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(223, 223, 223))
                };
                var tradeShadow = new FormattedText()
                {
                    Font = font,
                    Text = city.Trade.ToString(),
                    ForegroundBrush = new SolidBrush(Color.FromArgb(67, 67, 67))
                };
                offsetX += 16;
                e.Graphics.DrawText(tradeShadow, new Point(offsetX + 1, 87 + 24 * i + 1));
                e.Graphics.DrawText(trade, new Point(offsetX, 87 + 24 * i));
                offsetX += (int)trade.Measure().Width + 1;
                e.Graphics.DrawImage(CityImages.TradeBig, new Point(offsetX, 87 + 24 * i));

                // Item in production
                var item = drawnCities[i].ItemInProduction;
                var itemText = new FormattedText()
                {
                    Font = font,
                    Text = item.GetDescription(),
                    ForegroundBrush = new SolidBrush(item.Type == ItemType.Unit ? Color.FromArgb(255, 223, 79) : Color.FromArgb(223, 223, 223))
                };
                var itemShadowText = new FormattedText()
                {
                    Font = font,
                    Text = item.GetDescription(),
                    ForegroundBrush = new SolidBrush(item.Type == ItemType.Unit ? Colors.Black : Color.FromArgb(67, 67, 67))
                };
                offsetX = 374;
                e.Graphics.DrawText(itemShadowText, new Point(offsetX + 1, 87 + 24 * i + 1));
                e.Graphics.DrawText(itemText, new Point(offsetX, 87 + 24 * i));
                offsetX += (int)itemText.Measure().Width + 3;
                var progressText = "(" + city.ShieldsProgress.ToString() + "/" + (10 * item.Cost).ToString() + ")";
                Draw.Text(e.Graphics, progressText, font, Color.FromArgb(191, 191, 191), 
                    new Point(offsetX, 87 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);
            }
        }
    }
}
