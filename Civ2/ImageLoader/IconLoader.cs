using Civ2engine;
using Civ2engine.IO;
using Model;
using RaylibUtils;

namespace Civ2.ImageLoader;

public class IconLoader
{
    public static void LoadIcons(Ruleset ruleset, Civ2Interface active)
    {
        //var path = Utils.GetFilePath("ICONS", ruleset.Paths, "gif", "bmp");
        //Images.LoadPropertiesFromPic(path, active.IconsPicProps);
        //var iconProps = active.IconsPicProps;


        //var transparentLightPink = new Color(255, 159, 163,255);
        //var transparentPink = new Color(255, 0, 255,255);

        //Raylib.ImageColorReplace(ref iconsImage, transparentLightPink, new Color(0,0,0,0));
        //Raylib.ImageColorReplace(ref iconsImage, transparentPink, new Color(0,0,0,0));

        // City improvements
        // var improvements = new Bitmap[67];
        // int count = 1;  //start at 1. 0 is for no improvement.
        // for (int row = 0; row < 5; row++)
        // {
        //     for (int col = 0; col < 8; col++)
        //     {
        //         improvements[count] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 1 + row, 36, 20));
        //         count++;
        //         if (count == 39) break;
        //     }
        // }
        // // WondersIcons
        // for (int row = 0; row < 4; row++)
        // {
        //     for (int col = 0; col < 7; col++)
        //     {
        //         improvements[count] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 106 + row, 36, 20));
        //         count++;
        //     }
        // }
        // CityImages.Improvements = improvements;

        // Research icons
        // var researchIcons = new Bitmap[5, 4];
        // for (int row = 0; row < 4; row++)
        // {
        //     for (int col = 0; col < 5; col++)
        //     {
        //         researchIcons[col, row] = iconsImage.Clone(new Rectangle((36 * col) + 343 + col, (20 * row) + 211 + row, 36, 20));
        //     }
        // }
        // CityImages.ResearchIcons = researchIcons;
        //
        // CityImages.SellIcon = iconsImage.Clone(new Rectangle(16, 320, 14, 14));
        // CityImages.SellIcon.SetTransparent(new Color[] { transparentLightPink });

        active.MapImages.ViewPiece = active.PicSources["viewPiece"][0];
        active.MapImages.GridLines = active.PicSources["gridlines"][0];
        active.MapImages.GridLinesVisible = active.PicSources["gridlines,visible"][0];

        // Big icons in city resources
        // CityImages.HungerBig = iconsImage.Clone(new Rectangle(1, 290, 14, 14));
        // CityImages.HungerBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.ShortageBig = iconsImage.Clone(new Rectangle(16, 290, 14, 14));
        // CityImages.ShortageBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.CorruptBig = iconsImage.Clone(new Rectangle(31, 290, 14, 14));
        // CityImages.CorruptBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.FoodBig = iconsImage.Clone(new Rectangle(1, 305, 14, 14));
        // CityImages.FoodBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.SupportBig = iconsImage.Clone(new Rectangle(16, 305, 14, 14));
        // CityImages.SupportBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.TradeBig = iconsImage.Clone(new Rectangle(31, 305, 14, 14));
        // CityImages.TradeBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.LuxBig = iconsImage.Clone(new Rectangle(1, 320, 14, 14));
        // CityImages.LuxBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.TaxBig = iconsImage.Clone(new Rectangle(16, 320, 14, 14));
        // CityImages.TaxBig.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.SciBig = iconsImage.Clone(new Rectangle(31, 320, 14, 14));
        // CityImages.SciBig.SetTransparent(new Color[] { transparentLightPink });
        //
        // // Small icons in city resources
        // CityImages.FoodSmall = iconsImage.Clone(new Rectangle(49, 334, 10, 10));
        // CityImages.FoodSmall.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.SupportSmall = iconsImage.Clone(new Rectangle(60, 334, 10, 10));
        // CityImages.SupportSmall.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.TradeSmall = iconsImage.Clone(new Rectangle(71, 334, 10, 10));
        // CityImages.TradeSmall.SetTransparent(new Color[] { transparentLightPink });
        //
        // // Icon for next/previous city (black arrow)
        // CityImages.NextCity = iconsImage.Clone(new Rectangle(227, 389, 18, 24));
        // CityImages.NextCity.SetTransparent(new Color[] { transparentLightPink });
        // CityImages.PrevCity = iconsImage.Clone(new Rectangle(246, 389, 18, 24));
        // CityImages.PrevCity.SetTransparent(new Color[] { transparentLightPink });
        //
        // // City window icons
        // CityImages.Exit = iconsImage.Clone(new Rectangle(1, 389, 16, 16));
        // CityImages.ZoomOUT = iconsImage.Clone(new Rectangle(18, 389, 16, 16));
        // CityImages.ZoomIN = iconsImage.Clone(new Rectangle(35, 389, 16, 16));

        // Battle sprites
        active.UnitImages.BattleAnim = active.PicSources["battleAnim"];
    }
}