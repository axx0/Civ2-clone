using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Eto.Drawing;
using EtoFormsUI.ImageLoader;
using EtoFormsUIExtensionMethods;

namespace EtoFormsUI
{
    public static class MapImages
    {
        public static CityImage[] Cities { get; set; }

        public static Bitmap[] Specials { get; set; }
        
        public static PlayerColour[] PlayerColours { get; set; }
        
        public static UnitImage[] Units { get; set; }
        public static Bitmap[] Shields { get; set; }
        public static Bitmap[] ShieldBack { get; set; }
        public static Bitmap ShieldShadow { get; set; }
        public static TerrainSet[] Terrains { get; set; }
    }
}