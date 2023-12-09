using System.Numerics;
using Civ2engine;
using Civ2engine.Units;
using Model.ImageSets;
using Raylib_cs;
using RayLibUtils;

namespace Civ2.ImageLoader
{
    public static class UnitLoader
    {
        private static readonly Color ShadowColour = new(51, 51, 51, 255);

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
                    FlagLoc = new Vector2(unitProps["unit"][i].Flag1x, unitProps["unit"][i].Flag1y)
                };
            }

            active.UnitImages.UnitRectangle = new Rectangle(0, 0, 64, active.UnitsPxHeight);
            active.UnitImages.Units = units;

            Image MakeShield(Color colour)
            {
                var shield = Raylib.ImageFromImage(unitProps["HPshield"][0].Image, 
                    new Rectangle(0, 0, unitProps["HPshield"][0].Rect.width, unitProps["HPshield"][0].Rect.height));
                Raylib.ImageColorReplace(ref shield, new Color(255, 0, 255, 0), colour);
                return shield;
            }

            Image MakeBackShield(Color colour)
            {
                var shield = Raylib.ImageFromImage(unitProps["backShield1"][0].Image,
                    new Rectangle(0, 0, unitProps["HPshield"][0].Rect.width, unitProps["HPshield"][0].Rect.height));
                Raylib.ImageColorReplace(ref shield, new Color(255, 0, 0, 255), colour);
                return shield;
            }

            active.UnitImages.Shields = active.PlayerColours.Select(c => c.LightColour).Select((Func<Color, Image>)MakeShield).ToArray();
            if (unitProps.ContainsKey("backShield1"))
            {
                active.UnitImages.ShieldBack = active.PlayerColours.Select(c => c.DarkColour).Select((Func<Color, Image>)MakeBackShield).ToArray();
                active.UnitImages.ShieldShadow = MakeBackShield(ShadowColour);
            }
        }
    }
}