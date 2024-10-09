using System.Numerics;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Images;
using Model.ImageSets;
using Raylib_CSharp.Transformations;
using RaylibUtils;

namespace Civ2.ImageLoader
{
    public static class UnitLoader
    {
        public static void LoadUnits(Ruleset ruleset, Civ2Interface active)
        {
            // Initialize objects
            var units = new UnitImage[9 * active.UnitsRows];

            for (int i = 0; i < units.Length; i++)
            {
                var props = Images.ExtractBitmapData(active.PicSources["unit"][i], active); // put into cache

                units[i] = new UnitImage
                {
                    Image = active.PicSources["unit"][i],
                    FlagLoc = props.Flag1,
                };
            }

            active.UnitImages.UnitRectangle = new Rectangle(0, 0, 64, active.UnitsPxHeight);
            active.UnitImages.Units = units;

            active.GetShieldImages();
        }
    }
}