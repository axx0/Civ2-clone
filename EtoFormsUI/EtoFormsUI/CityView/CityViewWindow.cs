using System.Collections.Generic;
using Civ2engine;
using Civ2engine.Enums;
using Eto.Forms;
using Eto.Drawing;

namespace EtoFormsUI
{
    public class CityViewWindow : Form
    {
        private Game Game => Game.Instance;
        private readonly Drawable drawPanel;
        private readonly City _city;
        private readonly (int Id, int X, int Y, int tileId, ImprovementType improvement)[] drawData;
        private readonly Dictionary<ImprovementType, int> improvements = new Dictionary<ImprovementType, int>   // Lookup improvement id in CityViewImprovements bitmap
        {
            { ImprovementType.Palace, 23 },
            { ImprovementType.Barracks, 17 },
            { ImprovementType.Granary, 3 },
            { ImprovementType.Temple, 20 },
            { ImprovementType.Marketplace, 6 },
            { ImprovementType.Library, 4 },
            { ImprovementType.Courthouse, 1 },
            { ImprovementType.CityWalls, 27 },
            { ImprovementType.Aqueduct, 0 },
            { ImprovementType.Bank, 11 },
            { ImprovementType.Cathedral, 18 },
            { ImprovementType.University, 13 },
            { ImprovementType.MassTransit, 32 },
            { ImprovementType.Colosseum, 12 },
            { ImprovementType.Factory, 2 },
            { ImprovementType.MfgPlant, 5 },
            { ImprovementType.SDIdefense, 14 },
            { ImprovementType.RecyclCentre, 25 },
            { ImprovementType.PowerPlant, 21 },
            { ImprovementType.HydroPlant, 22 },
            { ImprovementType.NuclearPlant, 7 },
            { ImprovementType.StockExch, 26 },
            { ImprovementType.SewerSystem, 19 },
            { ImprovementType.Supermarket, 9 },
            { ImprovementType.ResearchLab, 8 },
            { ImprovementType.SAMbattery, 16 },
            { ImprovementType.CoastalFort, 15 },
            { ImprovementType.SolarPlant, 31 },
            { ImprovementType.Harbour, 28 },
            { ImprovementType.OffshorePlat, 30 },
            { ImprovementType.Airport, 10 },
            //{ ImprovementType.PoliceStat, 10 },   // ???
            { ImprovementType.PortFacil, 29 },
            { ImprovementType.Pyramids, 53 },
            { ImprovementType.HangingGardens, 37 },
            { ImprovementType.Colossus, 63 },
            { ImprovementType.Lighthouse, 61 },
            { ImprovementType.GreatLibrary, 36 },
            { ImprovementType.Oracle, 40 },
            { ImprovementType.GreatWall, 57 },
            { ImprovementType.WarAcademy, 50 },
            { ImprovementType.KR_Crusade, 52 },
            { ImprovementType.MP_Embassy, 51 },
            { ImprovementType.MichChapel, 39 },
            { ImprovementType.CoperObserv, 44 },
            { ImprovementType.MagellExped, 48 },
            { ImprovementType.ShakespTheat, 41 },
            { ImprovementType.DV_Workshop, 49 },
            { ImprovementType.JSB_Cathedral, 47 },
            { ImprovementType.IN_College, 46 },
            { ImprovementType.TradingCompany, 54 },
            { ImprovementType.DarwinVoyage, 45 },
            { ImprovementType.StatueLiberty, 59 },
            { ImprovementType.EiffelTower, 55 },
            { ImprovementType.WomenSuffrage, 34 },
            { ImprovementType.HooverDam, 35 },
            { ImprovementType.ManhattanProj, 38 },
            { ImprovementType.UnitedNations, 42 },
            { ImprovementType.ApolloProgr, 33 },
            { ImprovementType.SETIProgr, 43 },
            { ImprovementType.CureCancer, 56 },
        };

        public CityViewWindow(City city)
        {
            _city = city;

            WindowState = WindowState.Maximized;
            ShowInTaskbar = false;
            WindowStyle = WindowStyle.None;
            this.Shown += (sender, _) => drawPanel.Size = this.Size;

            // Define locations where and what is to be drawn
            // tileId ... id of Images.CityViewTiles (normal tiles)
            // improvement ... type of improvement to be drawn
            // X, Y ... tile offset on the map
            drawData = new (int Id, int X, int Y, int tileId, ImprovementType improvement)[]
            {
                (1, 4, 0, 2, ImprovementType.ManhattanProj),
                (2, 165, 47, 15, ImprovementType.Supermarket),
                (3, 267, 10, 2, ImprovementType.Aqueduct),
                (4, 401, 10, 5, ImprovementType.MichChapel),
                (5, 556, 5, 15, ImprovementType.Marketplace),
                (6, 653, 1, 14, ImprovementType.ResearchLab),
                (7, 728, 9, 2, ImprovementType.CoperObserv),
                (8, 850, 9, 2, ImprovementType.JSB_Cathedral),
                (9, 980, 7, 1, ImprovementType.Courthouse),
                (10, 1116, 7, 0, ImprovementType.Airport),
                (11, 60, 110, 13, ImprovementType.MfgPlant),
                (12, 170, 100, 12, ImprovementType.RecyclCentre),
                (13, 268, 100, 15, ImprovementType.SDIdefense),
                (14, 370, 100, 13, ImprovementType.SAMbattery),
                (15, 514, 63, 18, ImprovementType.Granary),
                (16, 620, 67, 15, ImprovementType.Temple),
                (17, 460, 120, 20, ImprovementType.Pyramids),
                (18, 586, 100, 15, ImprovementType.TradingCompany),
                (19, 676, 114, 14, ImprovementType.Palace),
                (20, 775, 105, 12, ImprovementType.Cathedral),
                (21, 870, 100, 13, ImprovementType.Colosseum),
                (22, 960, 114, 15, ImprovementType.NuclearPlant),
                (23, 1062, 105, 14, ImprovementType.CureCancer),
                (24, 1162, 100, 12, ImprovementType.HangingGardens),
                (25, 95, 150, 15, ImprovementType.DV_Workshop),
                (26, 192, 150, 15, ImprovementType.DarwinVoyage),
                (27, 290, 150, 14, ImprovementType.Bank),
                (28, 537, 153, 19, ImprovementType.Barracks),
                (29, 533, 200, 19, ImprovementType.University),
                (30, 658, 156, 4, ImprovementType.KR_Crusade),
                (31, 780, 156, 2, ImprovementType.HooverDam),
                (32, 916, 167, 15, ImprovementType.Factory),
                (33, 1036, 175, 0, ImprovementType.WomenSuffrage),
                (34, 1160, 213, 14, ImprovementType.CoastalFort),
                (35, 0, 213, 2, ImprovementType.StockExch),
                (36, 110, 260, 14, ImprovementType.SewerSystem),
                (37, 210, 226, 3, ImprovementType.SolarPlant),
                (38, 332, 226, 2, ImprovementType.GreatLibrary),
                (39, 450, 256, 7, ImprovementType.Oracle),
                (40, 572, 256, 7, ImprovementType.ShakespTheat),
                (41, 735, 250, 20, ImprovementType.Library),
                (42, 845, 259, 15, ImprovementType.MassTransit),
                (43, 10, 295, 3, ImprovementType.EiffelTower),
                (44, 167, 287, 3, ImprovementType.PowerPlant),
                (45, 293, 287, 1, ImprovementType.HydroPlant),
                (46, 0, 364, 2, ImprovementType.IN_College),
                (47, 129, 356, 1, ImprovementType.UnitedNations),
                (48, 250, 356, 2, ImprovementType.MagellExped),
                (49, 411, 324, 4, ImprovementType.MP_Embassy),
                (50, 533, 324, 1, ImprovementType.ApolloProgr),
                (51, 680, 284, 3, ImprovementType.SETIProgr),
                (52, 928, 273, 12, ImprovementType.Nothing), // Only for continental
                (53, 1020, 256, 0, ImprovementType.Nothing), // Only for continental
                (54, 1156, 286, 15, ImprovementType.Nothing),    // Only for continental
                (55, 1043, 328, 2, ImprovementType.Nothing),    // Only for continental
                (56, 1155, 396, 13, ImprovementType.Nothing),    // Only for continental & river
            };

            var Layout = new PixelLayout();
            drawPanel = new Drawable() { BackgroundColor = Colors.Black };
            drawPanel.MouseUp += (sender, e) => Close();
            drawPanel.Paint += PaintPanel;
            Layout.Add(drawPanel, new Point(0, 0));
            Content = Layout;
        }

        private void PaintPanel(object sender, PaintEventArgs e)
        {
            // Road type (0=no road / 1=dirt road / 2=asphalt road / 3=highway)
            int roadId = 1;
            // TODO: If city owner has Invention+Phylosophy (=Renaissance) then it has dirt roads in city view window
            if (_city.ImprovementExists(ImprovementType.Superhighways))
                roadId = 3;

            // Terrain type (continental/next to river/next to ocean)
            if (_city.IsNextToOcean)
                e.Graphics.DrawImage(Images.CityViewOcean[roadId], new Point(0, 0));
            else if (_city.IsNextToRiver)
                e.Graphics.DrawImage(Images.CityViewRiver[roadId], new Point(0, 0));
            else
                e.Graphics.DrawImage(Images.CityViewLand[roadId], new Point(0, 0));

            // Tiles
            for (int no = 0; no < drawData.Length; no++)
            {
                if (_city.IsNextToOcean && no > 49) continue;
                if (_city.IsNextToRiver && no > 49 && no < 54) continue;

                if (_city.ImprovementExists(drawData[no].improvement))
                    e.Graphics.DrawImage(Images.CityViewImprovements[improvements[drawData[no].improvement]], new Point(drawData[no].X, drawData[no].Y));
                else
                    e.Graphics.DrawImage(Images.CityViewTiles[drawData[no].tileId], new Point(drawData[no].X, drawData[no].Y));
            }

            // City walls
            if (_city.ImprovementExists(ImprovementType.CityWalls))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.CityWalls]], new Point(368, 390));

            // Offshore platrofrm
            if (_city.ImprovementExists(ImprovementType.OffshorePlat))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.OffshorePlat]], new Point(926, 366));

            // Harbor/port fac.
            if (_city.ImprovementExists(ImprovementType.PortFacil))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.PortFacil]], new Point(907, 296));
            else if (_city.ImprovementExists(ImprovementType.Harbour))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.Harbour]], new Point(907, 274));

            // Colossus
            if (_city.ImprovementExists(ImprovementType.Colossus))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.Colossus]], new Point(1070, 319));

            // Lighthouse
            if (_city.ImprovementExists(ImprovementType.Lighthouse))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.Lighthouse]], new Point(1184, 305));

            // Great wall
            if (_city.ImprovementExists(ImprovementType.GreatWall))
                e.Graphics.DrawImage(Images.CityViewImprovements[improvements[ImprovementType.GreatWall]], new Point(0, 0));

            Draw.Text(e.Graphics, $"{_city.Name}: {Game.GetGameYearString}", new Font("Times new roman", 26, FontStyle.Bold), Color.FromArgb(132, 132, 132), new Point(790, 10), false, false, Colors.Black, 1, 1);
        }
    }
}
