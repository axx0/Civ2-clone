using System.Linq;
using System.Collections.Generic;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUIExtensionMethods;
using Civ2engine;
using Civ2engine.Production;
using Civ2engine.Enums;
using Civ2engine.Units;

namespace EtoFormsUI
{
    public class AttitudeAdvisorWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private readonly VScrollBar bar;
        private int barVal0;

        public AttitudeAdvisorWindow() : base(622, 421, 11, 11)
        {
            if (Game.GetActiveCiv.Cities.Count > 9)
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
            e.Graphics.DrawImage(Images.ExtractBitmap(DLLs.Tiles, "attitudeAdvWallpaper").CropImage(new Rectangle(0, 0, 600, 400)), new Rectangle(11, 11, 600, 400));

            // Text
            var font1 = new Font("Times New Roman", 14);
            Draw.Text(e.Graphics, "ATTITUDE ADVISOR", font1, Color.FromArgb(223, 223, 223), new Point(300, 24), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Holy Empire of the {Game.GetActiveCiv.Adjective}", font1, Color.FromArgb(223, 223, 223),
                new Point(300, 45), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"{Game.GetActiveCiv.LeaderTitle} {Game.GetActiveCiv.LeaderName}: {Game.GetGameYearString}", font1, Color.FromArgb(223, 223, 223),
                new Point(300, 66), true, true, Color.FromArgb(67, 67, 67), 2, 1);


            // Cities
            var drawnCities = Game.GetActiveCiv.Cities.Skip(barVal0).Take(9).ToList();
            for (int i = 0; i < drawnCities.Count; i++)
            {
                var city = drawnCities[i];
                var font = new Font("Times New Roman", 11, FontStyle.Bold);

                // City image
                Draw.City(e.Graphics, city, true, 0, new Point(13 + 64 * ((barVal0 + i + 1) % 2), 78 + 32 * i));

                // City name
                Draw.Text(e.Graphics, city.Name, font, Color.FromArgb(223, 223, 223),
                    new Point(149, 87 + 32 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);

                // People
                var spacing = city.Size switch
                {
                    int n when (n <= 10) => 28,
                    int n when (n == 11) => 27,
                    int n when (n == 12) => 25,
                    int n when (n == 13) => 23,
                    int n when (n == 14) => 21,
                    int n when (n == 15) => 19,
                    int n when (n == 16) => 18,
                    int n when (n == 17) => 17,
                    int n when (n == 18 || n == 19) => 15,
                    int n when (n == 20) => 14,
                    int n when (n == 21 || n == 22) => 13,
                    int n when (n == 23 || n == 24) => 12,
                    int n when (n == 25 || n == 26) => 11,
                    int n when (n == 27 || n == 28) => 10,
                    int n when (n >= 29 && n <= 31) => 9,
                    int n when (n >= 32 && n <= 35) => 8,
                    int n when (n >= 36 && n <= 40) => 7,
                    int n when (n >= 41 && n <= 47) => 6,
                    int n when (n >= 48 && n <= 56) => 5,
                    int n when (n >= 57 && n <= 70) => 4,
                    int n when (n >= 71 && n <= 93) => 3,
                    int n when (n >= 94) => 2,
                    _ => 28,
                };

                PeopleType[] peoples = city.People;
                var offsetX = 254;
                int drawIndex;
                for (int p = 0; p < city.Size; p++)
                {
                    drawIndex = (int)peoples[p];
                    if (p % 2 == 1 && (drawIndex == 0 || drawIndex == 2 || drawIndex == 4 || drawIndex == 6))
                    {
                        drawIndex++;  // Change men/woman appearance
                    }
                    var plpPic = CityImages.PeopleLarge[drawIndex, (int)city.Owner.Epoch];
                    e.Graphics.DrawImage(plpPic, offsetX + 5 + p * spacing, 81 + 32 * i);
                }
            }
        }
    }
}
