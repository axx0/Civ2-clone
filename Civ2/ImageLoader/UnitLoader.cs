using System.Numerics;
using Civ2engine;
using Civ2engine.Units;
using Model.Images;
using Model.ImageSets;
using Raylib_cs;
using RayLibUtils;

namespace Civ2.ImageLoader
{
    public static class UnitLoader
    {
        private static readonly Color ShadowColour = new(51, 51, 51, 255);
        private static readonly Color ReplacementColour = new Color(255, 0, 0, 255);

        public static void LoadUnits(Ruleset ruleset, Civ2Interface active)
        {
            var path = Utils.GetFilePath("UNITS", ruleset.Paths, "gif", "bmp");
            Images.LoadPropertiesFromPIC(path, active.UnitPICprops);
            var unitProps = active.UnitPICprops;

            // Initialize objects
            var units = new UnitImage[9 * active.UnitsRows];

            for (int i = 0; i < units.Length; i++)
            {
                units[i] = new UnitImage
                {
                    Image = unitProps["unit"][i].Image,
                    Texture = Raylib.LoadTextureFromImage(unitProps["unit"][i].Image),
                    FlagLoc = new Vector2(unitProps["unit"][i].Flag1x, unitProps["unit"][i].Flag1y)
                };
            }

            active.UnitImages.UnitRectangle = new Rectangle(0, 0, 64, active.UnitsPxHeight);
            active.UnitImages.Units = units;

            var shield = unitProps["backShield1"][0].Image;
            var shield2 = unitProps["backShield2"][0].Image;

            var shieldFront = Raylib.ImageFromImage(shield, new Rectangle(0, 0, shield.width, shield.height));

            Raylib.ImageDrawRectangle(ref shieldFront,0,0,shieldFront.width, 7, Color.BLACK);

            var shadow = Raylib.ImageFromImage(shield2, new Rectangle(0, 0, shield.width, shield.height));
            Raylib.ImageColorReplace(ref shadow, ReplacementColour, ShadowColour);
            
            active.UnitImages.Shields = new MemoryStorage(shieldFront, "Unit-Shield", ReplacementColour);
            if (unitProps.ContainsKey("backShield1"))
            {
                active.UnitImages.ShieldBack = new MemoryStorage(shield, "Unit-Shield-Back", ReplacementColour, true);
                active.UnitImages.ShieldShadow = new MemoryStorage(shadow, "Unit-Shield-Shadow");
            }
        }
    }
}