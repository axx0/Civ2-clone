using System.Linq;
using Civ2engine;
using Eto.Drawing;
using EtoFormsUI.ImageLoader;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static class CityImages
    {
        public static Bitmap[] Improvements { get; set; }
        public static Bitmap[,] ResearchIcons { get; set; }
        public static Bitmap SellIcon { get; set; }
        public static Bitmap HungerBig { get; set; }
        public static Bitmap ShortageBig { get; set; }
        public static Bitmap CorruptBig { get; set; }
        public static Bitmap FoodBig { get; set; }
        public static Bitmap SupportBig { get; set; }
        public static Bitmap TradeBig { get; set; }
        public static Bitmap LuxBig { get; set; }
        public static Bitmap TaxBig { get; set; }
        public static Bitmap SciBig { get; set; }
        public static Bitmap FoodSmall { get; set; }
        public static Bitmap SupportSmall { get; set; }
        public static Bitmap TradeSmall { get; set; }
        public static Bitmap NextCity { get; set; }
        public static Bitmap PrevCity { get; set; }
        public static Bitmap Exit { get; set; }
        public static Bitmap ZoomOUT { get; set; }
        public static Bitmap ZoomIN { get; set; }
        public static Bitmap[,] PeopleLarge { get; set; }
        public static Bitmap[,] PeopleShadowLarge { get; set; }
        public static Bitmap Wallpaper { get; set; }
    }
}