using RaylibUI.ImageLoader;
using Raylib_cs;

namespace RaylibUI
{
    public static class MapImages
    {
        public static CityImage[] Cities { get; set; }

        public static Image[] Specials { get; set; }
        
        public static PlayerColour[] PlayerColours { get; set; }
        
        public static UnitImage[] Units { get; set; }
        
        public static Rectangle UnitRectangle { get; set; }
        public static Image[] Shields { get; set; }
        public static Image[] ShieldBack { get; set; }
        public static Image ShieldShadow { get; set; }
        public static TerrainSet[] Terrains { get; set; }
        public static Image ViewPiece { get; set; }
        public static Image GridLines { get; set; }
        public static Image GridLinesVisible { get; set; }
        public static Image[] BattleAnim { get; set; }
        public static Image PanelOuterWallpaper { get; set; }
        public static Image PanelInnerWallpaper { get; set; }
        public static Rectangle CityRectangle { get; set; }
    }
}