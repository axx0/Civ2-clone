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
    public class DefenseMinisterWindow : Civ2form
    {
        private Game Game => Game.Instance;
        private readonly VScrollBar bar;
        private int barVal0;
        private readonly List<UnitDefinition> unitStatDefs, unitCasDefs;
        private bool showCasualties;

        public DefenseMinisterWindow(Main main) : base(main.InterfaceStyle,622, 421, 11, 11)
        {
            showCasualties = false;

            // Unit definitions for stats view
            var sortedTypes = Game.Rules.UnitTypes.Select(c => c.Type).ToList();
            unitStatDefs = Game.GetActiveCiv.Units.Where(u => !u.Dead).Select(u => u.TypeDefinition).Distinct().ToList().
                OrderBy(x => sortedTypes.IndexOf(x.Type)).ToList();

            // Unit definitions for casualties view
            unitCasDefs = Game.GetActiveCiv.Units.Where(u => u.Dead).Select(u => u.TypeDefinition).Distinct().ToList().
                OrderBy(x => sortedTypes.IndexOf(x.Type)).ToList();
            var ok = Game.GetActiveCiv.Units.Where(u => u.Dead);

            if (unitStatDefs.Count > 12)
            {
                bar = new VScrollBar() { Height = 305, Value = 0, Maximum = unitStatDefs.Count };
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

            var btn2 = new Civ2button("Casualties", 297, 24, new Font("Times new roman", 11));
            btn2.Click += (_, _) =>
            {
                showCasualties = !showCasualties;
                if (showCasualties)
                {
                    btn2.Text = "Statistics";
                }
                else
                {
                    btn2.Text = "Casualties";
                }
                Surface.Invalidate();
            };
            Layout.Add(btn2, 13, 385);

            Surface.Paint += Surface_Paint;
            Content = Layout;
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Inner wallpaper
            e.Graphics.DrawImage(Images.ExtractBitmap(DLLs.Tiles, "defenseMinWallpaper").CropImage(new Rectangle(0, 0, 600, 400)), 
                new Rectangle(11, 11, 600, 400));
            
            // Text
            var font1 = new Font("Times New Roman", 14);
            var cycleText = showCasualties ? "Casualties" : "Statistics";
            Draw.Text(e.Graphics, $"DEFENSE MINISTER: {cycleText}", font1, Color.FromArgb(223, 223, 223), 
                new Point(300, 24), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"Holy Empire of the {Game.GetActiveCiv.Adjective}", font1, Color.FromArgb(223, 223, 223), 
                new Point(300, 45), true, true, Color.FromArgb(67, 67, 67), 2, 1);
            Draw.Text(e.Graphics, $"{Game.GetActiveCiv.LeaderTitle} {Game.GetActiveCiv.LeaderName}: {Game.GetGameYearString}", 
                font1, Color.FromArgb(223, 223, 223), new Point(300, 66), true, true, Color.FromArgb(67, 67, 67), 2, 1);

            if (!showCasualties)
            {
                // Unit types
                var drawnUnits = unitStatDefs.Skip(barVal0).Take(12).ToList();
                for (int i = 0; i < drawnUnits.Count; i++)
                {
                    var unitDef = drawnUnits[i];
                    var font = new Font("Times New Roman", 11, FontStyle.Bold);

                    // Unit image
                    Draw.UnitShield(e.Graphics, unitDef.Type, Game.GetActiveCiv.Id, OrderType.NoOrders, false, 100, 100, 0,
                        new Point(13 + 64 * ((barVal0 + i + 1) % 2), 78 + 24 * i));
                    Draw.UnitSprite(e.Graphics, unitDef.Type, false, false, 0, new Point(13 + 64 * ((barVal0 + i + 1) % 2), 78 + 24 * i));

                    // Unit name
                    Draw.Text(e.Graphics, unitDef.Name, font, Color.FromArgb(223, 223, 223),
                        new Point(149, 95 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);

                    // Stats
                    Draw.Text(e.Graphics, $"{unitDef.Attack}/{unitDef.Defense}/{unitDef.Move / 3}", font, Color.FromArgb(223, 223, 223),
                        new Point(245, 95 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);
                    Draw.Text(e.Graphics, $"{unitDef.Hitp / 10}/{unitDef.Firepwr}", font, Color.FromArgb(223, 223, 223),
                        new Point(300, 95 + 24 * i), false, false, Color.FromArgb(67, 67, 67), 1, 1);

                    var unitNo = Game.GetActiveCiv.Units.Where(u => u.Type == unitDef.Type).Count();
                    Draw.Text(e.Graphics, $"{unitNo} active", font, Color.FromArgb(255, 223, 79),
                        new Point(332, 95 + 24 * i), false, false, Colors.Black, 1, 1);

                    var unitInProdNo = Game.GetActiveCiv.Cities.Where(c => c.ItemInProduction.Type == ItemType.Unit &&
                        Game.Rules.UnitTypes[c.ItemInProduction.ImageIndex].Type == unitDef.Type).Count();
                    if (unitInProdNo > 0)
                    {
                        Draw.Text(e.Graphics, $"{unitInProdNo} in prod", font, Color.FromArgb(63, 187, 199),
                            new Point(393, 95 + 24 * i), false, false, Colors.Black, 1, 1);
                    }

                    // TODO: Add extra flags description
                }
            }
            else
            {

            }
        }
    }
}
