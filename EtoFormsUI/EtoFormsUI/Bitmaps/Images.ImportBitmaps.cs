using Eto.Drawing;
using System.IO;
using System.Diagnostics;
using Civ2engine;
using Civ2engine.Terrains;
using EtoFormsUI.ImageLoader;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static partial class Images
    {
        public static Bitmap CityHungerBig, CityShortageBig, CityCorruptBig, CityFoodBig, CitySupportBig, CityTradeBig, CityLuxBig, CityTaxBig, CitySciBig, CityFoodSmall, CitySupportSmall, CityTradeSmall, NextCity, CityWallpaper, PanelOuterWallpaper, PanelInnerWallpaper, ViewPiece, GridLines, GridLinesVisible, SellIcon, NextCityLarge, PrevCity, PrevCityLarge, CityExit, CityZoomIN, CityZoomOUT;
        public static Bitmap[] Improvements, BattleAnim;
        public static Bitmap[,] PeopleL, PeopleLshadow, ResearchIcons;
        
        
        public static Bitmap[,] MapTileGraphic;

        public static void LoadGraphicsAssetsFromFiles(Ruleset ruleset, Rules rules)
        {
            TerrainLoader.LoadTerrain(ruleset, rules);
            UnitLoader.LoadUnits(ruleset);
            PeopleIconsBitmapsImportFromFile(ruleset.Root);
            IconsBitmapsImportFromFile(ruleset.Root);
            CityWallpaperBitmapImportFromFile();
        }

        public static void LoadGraphicsAssetsAtIntroScreen()
        {
            ImportDLLimages();
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

        public static void PeopleIconsBitmapsImportFromFile(string path)
        {
            var pplIcons = LoadBitmapFrom("PEOPLE.GIF", path);

            // Initialize objects
            PeopleL = new Bitmap[11, 4];
            PeopleLshadow = new Bitmap[11, 4];

            // Define transparent colors
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)

            // Make shadows of faces
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 11; col++)
                {
                    PeopleL[col, row] = pplIcons.Clone(new Rectangle((27 * col) + 2 + col, (30 * row) + 6 + row, 27, 30));

                    PeopleLshadow[col, row] = new Bitmap(PeopleL[col, row].Width, PeopleL[col, row].Height, PixelFormat.Format32bppRgba);

                    // If color is non-pink, replace it with black to get shadow (otherwise make transparent)
                    for (int x = 0; x < 27; x++)
                    {
                        for (int y = 0; y < 30; y++)
                        {
                            if (PeopleL[col, row].GetPixel(x, y) != transparentPink)
                                PeopleLshadow[col, row].SetPixel(x, y, Colors.Black);
                            else
                            {
                                Color transparent = PeopleLshadow[col, row].GetPixel(x, y);
                                transparent.A = 0;
                                PeopleLshadow[col, row].SetPixel(x, y, transparent);
                            }

                        }
                    }
                    PeopleL[col, row].SetTransparent(new Color[] { transparentPink });
                }
            }

            pplIcons.Dispose();
        }

        // Import wallpapers for intro screen
        public static void ImportWallpapersFromIconsFile()
        {
            using (var icons = LoadBitmapFrom("ICONS.GIF"))
            {
                PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
                PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
            }
        }

        /// <summary>
        //  Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
        /// </summary>
        /// <param name="name">the filename to load</param>
        /// <param name="path">the local directory to load from</param>
        /// <returns></returns>
        private static Bitmap LoadBitmapFrom(string name, string path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                string FilePath_local = path + Path.DirectorySeparatorChar + name;
                if (File.Exists(FilePath_local))
                {
                    return new Bitmap(FilePath_local);
                }
            }

            string FilePath_root = Settings.Civ2Path + name;
            if (File.Exists(FilePath_root))
            {
                return new Bitmap(FilePath_root);
            }
            Debug.WriteLine(name + " not found!");
            return new Bitmap(640, 480, PixelFormat.Format32bppRgba);
        }

        public static void IconsBitmapsImportFromFile(string path)
        {
            var icons = LoadBitmapFrom("ICONS.GIF",path);

            // Initialize objects
            Improvements = new Bitmap[67];
            ResearchIcons = new Bitmap[5, 4];

            // Define transparent colors
            Color transparentLightPink = Color.FromArgb(255, 159, 163);//define transparent back color (light pink)
            Color transparentPink = Color.FromArgb(255, 0, 255);    //define transparent back color (pink)
            //Color transparentGray = Color.FromArgb(135, 83, 135);    //define transparent back color (gray)

            // Improvement icons
            int count = 1;  //start at 1. 0 is for no improvement.
            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 1 + row, 36, 20));
                    count++;
                    if (count == 39) break;
                }
            }

            // WondersIcons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    Improvements[count] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 106 + row, 36, 20));
                    count++;
                }
            }

            // Research icons
            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < 5; col++)
                {
                    ResearchIcons[col, row] = icons.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 211 + row, 36, 20));
                }
            }

            SellIcon = icons.Clone(new Rectangle(16, 320, 14, 14));
            SellIcon.SetTransparent(new Color[] { transparentLightPink });

            ViewPiece = icons.Clone(new Rectangle(199, 256, 64, 32));
            ViewPiece.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            GridLines = icons.Clone(new Rectangle(183, 430, 64, 32));
            GridLines.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            GridLinesVisible = icons.Clone(new Rectangle(248, 430, 64, 32));
            GridLinesVisible.SetTransparent(new Color[] { transparentLightPink, transparentPink });

            // Big icons in city resources
            CityHungerBig = icons.Clone(new Rectangle(1, 290, 14, 14));
            CityHungerBig.SetTransparent(new Color[] { transparentLightPink });
            CityShortageBig = icons.Clone(new Rectangle(16, 290, 14, 14));
            CityShortageBig.SetTransparent(new Color[] { transparentLightPink });
            CityCorruptBig = icons.Clone(new Rectangle(31, 290, 14, 14));
            CityCorruptBig.SetTransparent(new Color[] { transparentLightPink });
            CityFoodBig = icons.Clone(new Rectangle(1, 305, 14, 14));
            CityFoodBig.SetTransparent(new Color[] { transparentLightPink });
            CitySupportBig = icons.Clone(new Rectangle(16, 305, 14, 14));
            CitySupportBig.SetTransparent(new Color[] { transparentLightPink });
            CityTradeBig = icons.Clone(new Rectangle(31, 305, 14, 14));
            CityTradeBig.SetTransparent(new Color[] { transparentLightPink });
            CityLuxBig = icons.Clone(new Rectangle(1, 320, 14, 14));
            CityLuxBig.SetTransparent(new Color[] { transparentLightPink });
            CityTaxBig = icons.Clone(new Rectangle(16, 320, 14, 14));
            CityTaxBig.SetTransparent(new Color[] { transparentLightPink });
            CitySciBig = icons.Clone(new Rectangle(31, 320, 14, 14));
            CitySciBig.SetTransparent(new Color[] { transparentLightPink });

            // Small icons in city resources
            CityFoodSmall = icons.Clone(new Rectangle(49, 334, 10, 10));
            CityFoodSmall.SetTransparent(new Color[] { transparentLightPink });
            CitySupportSmall = icons.Clone(new Rectangle(60, 334, 10, 10));
            CitySupportSmall.SetTransparent(new Color[] { transparentLightPink });
            CityTradeSmall = icons.Clone(new Rectangle(71, 334, 10, 10));
            CityTradeSmall.SetTransparent(new Color[] { transparentLightPink });

            // Wallpaper icons
            //PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
            //PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));

            // Icon for next/previous city (black arrow)
            NextCity = icons.Clone(new Rectangle(227, 389, 18, 24));
            NextCity.SetTransparent(new Color[] { transparentLightPink });
            PrevCity = icons.Clone(new Rectangle(246, 389, 18, 24));
            PrevCity.SetTransparent(new Color[] { transparentLightPink });
            //NextCityLarge = ModifyImage.Resize(NextCity, 27, 36);    //50% larger
            NextCityLarge = NextCity;
            //PrevCityLarge = ModifyImage.Resize(PrevCity, 27, 36);    //50% larger
            PrevCityLarge = PrevCity;

            // City window icons
            CityExit = icons.Clone(new Rectangle(1, 389, 16, 16));
            CityZoomOUT = icons.Clone(new Rectangle(18, 389, 16, 16));
            CityZoomIN = icons.Clone(new Rectangle(35, 389, 16, 16));

            // Battle sprites
            BattleAnim = new Bitmap[8];
            for (int i = 0; i < 8; i++)
            {
                BattleAnim[i] = icons.Clone(new Rectangle(1  + 33 * i, 356, 32, 32));
                BattleAnim[i].SetTransparent(new Color[] { transparentLightPink });
            }

            icons.Dispose();
        }

        public static void CityWallpaperBitmapImportFromFile()
        {
            var cityWallpaper = LoadBitmapFrom("CITY.GIF");
            CityWallpaper = cityWallpaper.CropImage(new Rectangle(0, 0, 636, 421));
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
