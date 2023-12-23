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
                    FlagLoc = new Vector2(unitProps["unit"][i].Flag1x, unitProps["unit"][i].Flag1y),
                };
            }

            active.UnitImages.UnitRectangle = new Rectangle(0, 0, 64, active.UnitsPxHeight);
            active.UnitImages.Units = units;

            active.GetShieldImages();
        }
    }
}