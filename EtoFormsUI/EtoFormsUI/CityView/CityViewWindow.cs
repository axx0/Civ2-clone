using System.Collections.Generic;
using System.IO;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public class CityViewWindow : Form
    {
        private Game Game => Game.Instance;
        private readonly Drawable drawPanel;
        private readonly City _city;
        private readonly Bitmap baseTile, improvementsTiles, wondersTiles, alternTiles;
        private readonly List<CityViewTiles> improvements;
        private readonly List<Rectangle> alternativeTileSrcRect;

        public CityViewWindow(City city)
        {
            _city = city;
            byte[] bytes = File.ReadAllBytes(Settings.Civ2Path + "cv.dll");

            // TODO: also determine if road/ashphalt/highway exists
            // Invention+Phylosophy (=Renaissance) then it has dirt roads
            // if highways exist --> highway roads
            string background;
            if (_city.IsNextToOcean)
                background = "cityviewBaseoceanempty";
            else if (_city.IsNextToRiver)
                background = "cityviewBaseriverempty";
            else
                background = "cityviewBasecontinentempty";

            // Get tiles
            baseTile = Images.ExtractBitmap(bytes, background);
            improvementsTiles = Images.ExtractBitmap(bytes, "cityviewImprovements");
            improvementsTiles = Images.CreateNonIndexedImage(improvementsTiles);
            improvementsTiles.SetTransparent(new Color[] { Color.FromArgb(255, 0, 255), Color.FromArgb(135, 135, 135) });
            wondersTiles = Images.ExtractBitmap(bytes, "cityviewWonders");
            wondersTiles = Images.CreateNonIndexedImage(wondersTiles);
            wondersTiles.SetTransparent(new Color[] { Color.FromArgb(255, 0, 255), Color.FromArgb(135, 135, 135) });
            alternTiles = Images.ExtractBitmap(bytes, "cityviewAlternative");
            alternTiles = Images.CreateNonIndexedImage(alternTiles);
            alternTiles.SetTransparent(new Color[] { Color.FromArgb(255, 0, 255), Color.FromArgb(135, 135, 135) });

            // Define improvements in order in which they are drawn (left to right, in consequtive rows)
            improvements = new ()
            {
                new CityViewTiles(0, ImprovementType.ManhattanProj, wondersTiles, new Rectangle(160, 116, 158, 114), new Point(4, 0), 2),
                new CityViewTiles(1, ImprovementType.Supermarket, improvementsTiles, new Rectangle(497, 84, 123, 82), new Point(165, 47), 15),
                new CityViewTiles(2, ImprovementType.Aqueduct, improvementsTiles, new Rectangle(1, 1, 123, 82), new Point(267, 10), 2),
                new CityViewTiles(3, ImprovementType.MichChapel, wondersTiles, new Rectangle(319, 116, 158, 114), new Point(401, 10), 5),
                new CityViewTiles(4, ImprovementType.Marketplace, improvementsTiles, new Rectangle(125, 84, 123, 82), new Point(556, 5), 15),
                new CityViewTiles(5, ImprovementType.ResearchLab, improvementsTiles, new Rectangle(373, 84, 123, 82), new Point(653, 1), 14),
                new CityViewTiles(6, ImprovementType.CoperObserv, wondersTiles, new Rectangle(478, 231, 158, 114), new Point(728, 9), 2),
                new CityViewTiles(7, ImprovementType.JSB_Cathedral, wondersTiles, new Rectangle(319, 346, 158, 114), new Point(850, 9), 2),
                new CityViewTiles(8, ImprovementType.Courthouse, improvementsTiles, new Rectangle(125, 1, 123, 82), new Point(980, 7), 1),
                new CityViewTiles(9, ImprovementType.Airport, improvementsTiles, new Rectangle(1, 167, 123, 82), new Point(1116, 7), 0),
                new CityViewTiles(10, ImprovementType.MfgPlant, improvementsTiles, new Rectangle(1, 84, 123, 82), new Point(60, 110), 13),
                new CityViewTiles(11, ImprovementType.RecyclCentre, improvementsTiles, new Rectangle(1, 416, 123, 82), new Point(170, 100), 12),
                new CityViewTiles(12, ImprovementType.SDIdefense, improvementsTiles, new Rectangle(497, 167, 123, 82), new Point(268, 100), 15),
                new CityViewTiles(13, ImprovementType.SAMbattery, improvementsTiles, new Rectangle(125, 250, 123, 82), new Point(370, 100), 13),
                new CityViewTiles(14, ImprovementType.Granary, improvementsTiles, new Rectangle(373, 1, 123, 82), new Point(514, 63), 18),
                new CityViewTiles(15, ImprovementType.Temple, improvementsTiles, new Rectangle(1, 333, 123, 82), new Point(620, 67), 15),
                new CityViewTiles(16, ImprovementType.Pyramids, wondersTiles, new Rectangle(1, 576, 158, 114), new Point(460, 120), 20),
                new CityViewTiles(17, ImprovementType.TradingCompany, wondersTiles, new Rectangle(160, 576, 158, 114), new Point(586, 100), 15),
                new CityViewTiles(18, ImprovementType.Palace, improvementsTiles, new Rectangle(373, 333, 123, 82), new Point(676, 114), 14),
                new CityViewTiles(19, ImprovementType.Cathedral, improvementsTiles, new Rectangle(373, 250, 123, 82), new Point(775, 105), 12),
                new CityViewTiles(20, ImprovementType.Colosseum, improvementsTiles, new Rectangle(249, 167, 123, 82), new Point(870, 100), 13),
                new CityViewTiles(21, ImprovementType.NuclearPlant, improvementsTiles, new Rectangle(249, 84, 123, 82), new Point(960, 114), 15),
                new CityViewTiles(22, ImprovementType.CureCancer, wondersTiles, new Rectangle(478, 576, 158, 114), new Point(1062, 105), 14),
                new CityViewTiles(23, ImprovementType.HangingGardens, wondersTiles, new Rectangle(1, 116, 158, 114), new Point(1162, 100), 12),
                new CityViewTiles(24, ImprovementType.DV_Workshop, wondersTiles, new Rectangle(1, 461, 158, 114), new Point(95, 150), 15),
                new CityViewTiles(25, ImprovementType.DarwinVoyage, wondersTiles, new Rectangle(1, 346, 158, 114), new Point(192, 150), 15),
                new CityViewTiles(26, ImprovementType.Bank, improvementsTiles, new Rectangle(125, 167, 123, 82), new Point(290, 150), 14),
                new CityViewTiles(27, ImprovementType.Barracks, improvementsTiles, new Rectangle(249, 250, 123, 82), new Point(537, 153), 19),
                new CityViewTiles(28, ImprovementType.University, improvementsTiles, new Rectangle(373, 167, 123, 82), new Point(533, 200), 19),
                new CityViewTiles(29, ImprovementType.KR_Crusade, wondersTiles, new Rectangle(478, 461, 158, 114), new Point(658, 156), 4),
                new CityViewTiles(30, ImprovementType.HooverDam, wondersTiles, new Rectangle(319, 1, 158, 114), new Point(780, 156), 2),
                new CityViewTiles(31, ImprovementType.Factory, improvementsTiles, new Rectangle(249, 1, 123, 82), new Point(916, 167), 15),
                new CityViewTiles(32, ImprovementType.WomenSuffrage, wondersTiles, new Rectangle(160, 1, 158, 114), new Point(1036, 175), 0),
                new CityViewTiles(33, ImprovementType.CoastalFort, improvementsTiles, new Rectangle(1, 250, 123, 82), new Point(1160, 213), 14),
                new CityViewTiles(34, ImprovementType.StockExch, improvementsTiles, new Rectangle(125, 416, 123, 82), new Point(0, 213), 2),
                new CityViewTiles(35, ImprovementType.SewerSystem, improvementsTiles, new Rectangle(497, 250, 123, 82), new Point(110, 260), 14),
                new CityViewTiles(36, ImprovementType.SolarPlant, improvementsTiles, new Rectangle(1, 618, 123, 82), new Point(210, 226), 3),
                new CityViewTiles(37, ImprovementType.GreatLibrary, wondersTiles, new Rectangle(478, 1, 158, 114), new Point(332, 226), 2),
                new CityViewTiles(38, ImprovementType.Oracle, wondersTiles, new Rectangle(478, 116, 158, 114), new Point(450, 256), 7),
                new CityViewTiles(39, ImprovementType.ShakespTheat, wondersTiles, new Rectangle(1, 231, 158, 114), new Point(572, 256), 7),
                new CityViewTiles(40, ImprovementType.Library, improvementsTiles, new Rectangle(497, 1, 123, 82), new Point(735, 250), 20),
                new CityViewTiles(41, ImprovementType.MassTransit, improvementsTiles, new Rectangle(125, 618, 123, 82), new Point(845, 259), 15),
                new CityViewTiles(42, ImprovementType.EiffelTower, wondersTiles, new Rectangle(319, 576, 158, 114), new Point(10, 295), 3),
                new CityViewTiles(43, ImprovementType.PowerPlant, improvementsTiles, new Rectangle(125, 333, 123, 82), new Point(167, 287), 3),
                new CityViewTiles(44, ImprovementType.HydroPlant, improvementsTiles, new Rectangle(249, 333, 123, 82), new Point(293, 287), 1),
                new CityViewTiles(45, ImprovementType.IN_College, wondersTiles, new Rectangle(160, 346, 158, 114), new Point(0, 364), 2),
                new CityViewTiles(46, ImprovementType.UnitedNations, wondersTiles, new Rectangle(160, 231, 158, 114), new Point(129, 356), 1),
                new CityViewTiles(47, ImprovementType.MagellExped, wondersTiles, new Rectangle(478, 346, 158, 114), new Point(250, 356), 2),
                new CityViewTiles(48, ImprovementType.MP_Embassy, wondersTiles, new Rectangle(319, 461, 158, 114), new Point(411, 324), 4),
                new CityViewTiles(49, ImprovementType.ApolloProgr, wondersTiles, new Rectangle(1, 1, 158, 114), new Point(533, 324), 1),
                new CityViewTiles(50, ImprovementType.SETIProgr, wondersTiles, new Rectangle(319, 231, 158, 114), new Point(680, 284), 3),
                new CityViewTiles(51, ImprovementType.Nothing, wondersTiles, new Rectangle(0, 0, 0, 0), new Point(928, 273), 12),   // Draw altern. tile where ocean/river is (continental only)
                new CityViewTiles(52, ImprovementType.Nothing, wondersTiles, new Rectangle(0, 0, 0, 0), new Point(1020, 256), 0),   // Draw altern. tile where ocean/river is (continental only)
                new CityViewTiles(53, ImprovementType.Nothing, wondersTiles, new Rectangle(0, 0, 0, 0), new Point(1156, 286), 15),   // Draw altern. tile where ocean/river is (continental only)
                new CityViewTiles(54, ImprovementType.Nothing, wondersTiles, new Rectangle(0, 0, 0, 0), new Point(1043, 328), 2),   // Draw altern. tile where ocean/river is (continental only)
                new CityViewTiles(55, ImprovementType.Nothing, wondersTiles, new Rectangle(0, 0, 0, 0), new Point(1155, 396), 13),   // Draw altern. tile where ocean is (continental & river)
                new CityViewTiles(56, ImprovementType.CityWalls, improvementsTiles, new Rectangle(249, 416, 357, 78), new Point(368, 390), 0),
                new CityViewTiles(57, ImprovementType.OffshorePlat, improvementsTiles, new Rectangle(590, 499, 105, 105), new Point(926, 366), 0),
                new CityViewTiles(58, ImprovementType.Harbour, improvementsTiles, new Rectangle(1, 499, 220, 100), new Point(907, 274), 0),
                new CityViewTiles(59, ImprovementType.PortFacil, improvementsTiles, new Rectangle(222, 499, 367, 118), new Point(907, 296), 0),
                new CityViewTiles(60, ImprovementType.Colossus, wondersTiles, new Rectangle(407, 852, 94, 160), new Point(1070, 319), 0),   // on water
                new CityViewTiles(61, ImprovementType.Colossus, wondersTiles, new Rectangle(502, 852, 94, 160), new Point(1070, 319), 0),   // on land
                new CityViewTiles(62, ImprovementType.Lighthouse, wondersTiles, new Rectangle(253, 852, 76, 133), new Point(1184, 305), 0),   // on water
                new CityViewTiles(63, ImprovementType.Lighthouse, wondersTiles, new Rectangle(330, 852, 76, 133), new Point(1184, 305), 0),   // on land
                new CityViewTiles(64, ImprovementType.GreatWall, wondersTiles, new Rectangle(1, 691, 304, 160), new Point(0, 0), 0),   // v1
                new CityViewTiles(65, ImprovementType.GreatWall, wondersTiles, new Rectangle(306, 691, 304, 160), new Point(0, 0), 0),   // v2
                new CityViewTiles(66, ImprovementType.StatueLiberty, wondersTiles, new Rectangle(1, 852, 125, 253), new Point(0, 0), 0),   // on sea
                new CityViewTiles(67, ImprovementType.StatueLiberty, wondersTiles, new Rectangle(127, 852, 125, 253), new Point(0, 0), 0),   // on land
            };

            // Locations of alternative tiles (villages, forest) in bitmap
            alternativeTileSrcRect = new List<Rectangle>()
            {
                new Rectangle(1, 1, 158, 114),
                new Rectangle(160, 1, 158, 114),
                new Rectangle(319, 1, 158, 114),
                new Rectangle(478, 1, 158, 114),
                new Rectangle(1, 116, 158, 114),
                new Rectangle(160, 116, 158, 114),
                new Rectangle(319, 116, 158, 114),
                new Rectangle(478, 116, 158, 114),
                new Rectangle(1, 231, 158, 114),
                new Rectangle(160, 231, 158, 114),
                new Rectangle(319, 231, 158, 114),
                new Rectangle(478, 231, 158, 114),
                new Rectangle(1, 346, 123, 82),
                new Rectangle(125, 346, 123, 82),
                new Rectangle(249, 346, 123, 82),
                new Rectangle(373, 346, 123, 82),
                new Rectangle(497, 346, 123, 82),
                new Rectangle(1, 429, 123, 82),
                new Rectangle(125, 429, 123, 82),
                new Rectangle(249, 429, 123, 82),
                new Rectangle(373, 429, 123, 82),
                new Rectangle(497, 429, 123, 82),
                new Rectangle(1, 512, 123, 82),
                new Rectangle(125, 512, 123, 82),
                new Rectangle(249, 512, 123, 82),
                new Rectangle(373, 512, 123, 82),
                new Rectangle(497, 512, 123, 82),
            };

            WindowState = WindowState.Maximized;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            this.Shown += (_, _) => drawPanel.Size = this.Size;

            var Layout = new PixelLayout();
            drawPanel = new Drawable() { BackgroundColor = Colors.Black };
            drawPanel.MouseUp += (_, _) => Close();
            drawPanel.Paint += PaintPanel;
            Layout.Add(drawPanel, new Point(0, 0));
            Content = Layout;
        }

        private void PaintPanel(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(baseTile, new Point(0, 0));
            
            // Tiles
            for (int id = 0; id < 56; id++)
            {
                if (_city.IsNextToOcean && id > 49) continue;
                if (_city.IsNextToRiver && id > 49 && id < 54) continue;

                if (_city.ImprovementExists(improvements[id].Type))
                    DrawImprovement(e.Graphics, id);
                else
                    DrawAlternativeTile(e.Graphics, improvements[id].AlternativeTileId);
            }

            // City walls
            if (_city.ImprovementExists(ImprovementType.CityWalls))
                DrawImprovement(e.Graphics, 56);

            // Offshore platrofrm
            if (_city.ImprovementExists(ImprovementType.OffshorePlat)) 
                DrawImprovement(e.Graphics, 57);

            // Harbor/port fac.
            if (_city.ImprovementExists(ImprovementType.PortFacil))
                DrawImprovement(e.Graphics, 59);
            else if (_city.ImprovementExists(ImprovementType.Harbour))
                DrawImprovement(e.Graphics, 58);

            // Colossus
            if (_city.ImprovementExists(ImprovementType.Colossus))
                if (_city.IsNextToRiver)
                    DrawImprovement(e.Graphics, 61);
                else
                    DrawImprovement(e.Graphics, 60);

            // Lighthouse
            if (_city.ImprovementExists(ImprovementType.Lighthouse))
                if (_city.IsNextToRiver)
                    DrawImprovement(e.Graphics, 63);
                else
                    DrawImprovement(e.Graphics, 62);

            // Statue liberty
            // TODO: find out where to draw statue of liberty in city view

            // Great wall
            if (_city.ImprovementExists(ImprovementType.GreatWall))
                DrawImprovement(e.Graphics, 64);

            Draw.Text(e.Graphics, $"{_city.Name}: {Game.GetGameYearString}", new Font("Times new roman", 26, FontStyle.Bold), Color.FromArgb(132, 132, 132), new Point(790, 10), false, false, Colors.Black, 1, 1);
        }

        private void DrawImprovement(Graphics g, int id)
        {
            g.DrawImage(improvements[id].SourceBmp.Clone(improvements[id].SourceRect), improvements[id].DrawOffset);
        }

        private void DrawAlternativeTile(Graphics g, int id)
        {
            g.DrawImage(alternTiles.Clone(alternativeTileSrcRect[id]), improvements[id].DrawOffset);
        }
    }
}
