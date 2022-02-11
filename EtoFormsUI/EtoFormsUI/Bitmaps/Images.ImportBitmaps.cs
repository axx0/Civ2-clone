using Eto.Drawing;
using Civ2engine;
using Civ2engine.Terrains;
using EtoFormsUI.ImageLoader;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Images
    {
        public static Bitmap[,] MapTileGraphic;

        public static void LoadGraphicsAssetsFromFiles(Ruleset ruleset, Rules rules)
        {
            TerrainLoader.LoadTerrain(ruleset, rules);
            UnitLoader.LoadUnits(ruleset);
            LoadIcons(ruleset);
            LoadPeopleIcons(ruleset);
            LoadCityWallpaper(ruleset);
        }

        public static void LoadGraphicsAssetsAtIntroScreen()
        {
            DLLs.Tiles = File.ReadAllBytes(Settings.Civ2Path + "Tiles.dll");
            DLLs.Intro = File.ReadAllBytes(Settings.Civ2Path + "Intro.dll");
            ImportWallpapersFromIconsFile();
            //ImportCiv2Icon();
        }

        public static void RedrawTile(Tile tile)
        {
            var col = (tile.X - tile.Odd) / 2;
            Images.MapTileGraphic[col, tile.Y] = Draw.MakeTileGraphic(tile, col, tile.Y, Game.Instance.Options.FlatEarth, MapImages.Terrains[Game.Instance.CurrentMap.MapIndex]);
        }

        public static Bitmap MapTileGraphicC2(int xC2, int yC2) => MapTileGraphic[(((xC2 + 2 * Game.Instance.CurrentMap.XDim) % (2 * Game.Instance.CurrentMap.XDim)) - yC2 % 2) / 2, yC2];  // Return tile graphics for civ2-coords input

        // Extract icon from civ2.exe file
        //public static void ImportCiv2Icon()
        //{
        //    try
        //    {
        //        Civ2Icon = Icon.ExtractAssociatedIcon(Settings.Civ2Path + "civ2.exe");
        //    }
        //    catch
        //    {
        //        Debug.WriteLine("Civ2.exe not found!");
        //    }
        //}

        public static void LoadIcons(Ruleset path)
        {
            using var iconsImage = Common.LoadBitmapFrom("ICONS", path.Paths);

            var transparentLightPink = Color.FromArgb(255, 159, 163);
            var transparentPink = Color.FromArgb(255, 0, 255);

            // City improvements
            var improvements = new Bitmap[67];
            int count = 1;  //start at 1. 0 is for no improvement.
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    improvements[count] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 1 + row, 36, 20));
                    count++;
                    if (count == 39) break;
                }
            }
            // WondersIcons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    improvements[count] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 106 + row, 36, 20));
                    count++;
                }
            }
            CityImages.Improvements = improvements;

            // Research icons
            var researchIcons = new Bitmap[5, 4];
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    researchIcons[col, row] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 211 + row, 36, 20));
                }
            }
            CityImages.ResearchIcons = researchIcons;

            CityImages.SellIcon = iconsImage.Clone(new Rectangle(16, 320, 14, 14));
            CityImages.SellIcon.SetTransparent(new Color[] { transparentLightPink });

            MapImages.ViewPiece = iconsImage.Clone(new Rectangle(199, 256, 64, 32));
            MapImages.ViewPiece.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            MapImages.GridLines = iconsImage.Clone(new Rectangle(183, 430, 64, 32));
            MapImages.GridLines.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            MapImages.GridLinesVisible = iconsImage.Clone(new Rectangle(248, 430, 64, 32));
            MapImages.GridLinesVisible.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            // Big icons in city resources
            CityImages.HungerBig = iconsImage.Clone(new Rectangle(1, 290, 14, 14));
            CityImages.HungerBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.ShortageBig = iconsImage.Clone(new Rectangle(16, 290, 14, 14));
            CityImages.ShortageBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.CorruptBig = iconsImage.Clone(new Rectangle(31, 290, 14, 14));
            CityImages.CorruptBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.FoodBig = iconsImage.Clone(new Rectangle(1, 305, 14, 14));
            CityImages.FoodBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.SupportBig = iconsImage.Clone(new Rectangle(16, 305, 14, 14));
            CityImages.SupportBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.TradeBig = iconsImage.Clone(new Rectangle(31, 305, 14, 14));
            CityImages.TradeBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.LuxBig = iconsImage.Clone(new Rectangle(1, 320, 14, 14));
            CityImages.LuxBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.TaxBig = iconsImage.Clone(new Rectangle(16, 320, 14, 14));
            CityImages.TaxBig.SetTransparent(new Color[] { transparentLightPink });
            CityImages.SciBig = iconsImage.Clone(new Rectangle(31, 320, 14, 14));
            CityImages.SciBig.SetTransparent(new Color[] { transparentLightPink });

            // Small icons in city resources
            CityImages.FoodSmall = iconsImage.Clone(new Rectangle(49, 334, 10, 10));
            CityImages.FoodSmall.SetTransparent(new Color[] { transparentLightPink });
            CityImages.SupportSmall = iconsImage.Clone(new Rectangle(60, 334, 10, 10));
            CityImages.SupportSmall.SetTransparent(new Color[] { transparentLightPink });
            CityImages.TradeSmall = iconsImage.Clone(new Rectangle(71, 334, 10, 10));
            CityImages.TradeSmall.SetTransparent(new Color[] { transparentLightPink });

            // Icon for next/previous city (black arrow)
            CityImages.NextCity = iconsImage.Clone(new Rectangle(227, 389, 18, 24));
            CityImages.NextCity.SetTransparent(new Color[] { transparentLightPink });
            CityImages.PrevCity = iconsImage.Clone(new Rectangle(246, 389, 18, 24));
            CityImages.PrevCity.SetTransparent(new Color[] { transparentLightPink });

            // City window icons
            CityImages.Exit = iconsImage.Clone(new Rectangle(1, 389, 16, 16));
            CityImages.ZoomOUT = iconsImage.Clone(new Rectangle(18, 389, 16, 16));
            CityImages.ZoomIN = iconsImage.Clone(new Rectangle(35, 389, 16, 16));

            // Battle sprites
            var battleAnim = new Bitmap[8];
            for (int i = 0; i < 8; i++)
            {
                battleAnim[i] = iconsImage.Clone(new Rectangle(1 + 33 * i, 356, 32, 32));
                battleAnim[i].SetTransparent(new Color[] { transparentLightPink });
            }
            MapImages.BattleAnim = battleAnim;
        }

        public static void LoadPeopleIcons(Ruleset ruleset)
        {
            using var iconsImage = Common.LoadBitmapFrom("PEOPLE", ruleset.Paths);

            var peopleL = new Bitmap[11, 4];
            var peopleLshadow = new Bitmap[11, 4];

            var transparentPink = Color.FromArgb(255, 0, 255);

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 11; col++)
                {
                    peopleL[col, row] = iconsImage.Clone(new Rectangle((27 * col) + 2 + col, (30 * row) + 6 + row, 27, 30));
                    peopleL[col, row].SetTransparent(new Color[] { transparentPink });

                    peopleLshadow[col, row] = peopleL[col, row].Clone();
                    peopleLshadow[col, row].ToSingleColor(Colors.Black);
                }
            }

            CityImages.PeopleLarge = peopleL;
            CityImages.PeopleShadowLarge = peopleLshadow;
        }

        public static void LoadCityWallpaper(Ruleset ruleset)
        {
            var wallpaper = Common.LoadBitmapFrom("CITY", ruleset.Paths);
            CityImages.Wallpaper = wallpaper.CropImage(new Rectangle(0, 0, 636, 421));
        }

        // Import wallpapers for intro screen
        public static void ImportWallpapersFromIconsFile()
        {
            using var icons = Common.LoadBitmapFrom("ICONS", Settings.SearchPaths);
            MapImages.PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
            MapImages.PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
        }

        /// <summary>
        /// Convert indexed to non-indexed images (required for making transparent pixels, etc.)
        /// </summary>
        /// <param name="src">Source indexed image</param>
        /// <returns>Non-indexed image</returns>
        public static Bitmap CreateNonIndexedImage(Image src)
        {
            var newBmp = new Bitmap(src.Width, src.Height, PixelFormat.Format32bppRgba);

            using var g = new Graphics(newBmp);
            g.DrawImage(src, 0, 0);

            return newBmp;
        }
    }
}
