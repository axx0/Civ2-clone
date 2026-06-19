using System.Numerics;
using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Core.GameRules;
using Model.Images;
using Model.ImageSets;
using Raylib_CSharp.Transformations;
using RaylibUtils;

namespace Civ2.ImageLoader
{
    public static class UnitLoader
    {
        private const int UnitColumns = 9;
        private const int OriginalUnitWidth = 64;
        private const int ShieldWidth = 12;

        public static void LoadUnits(Ruleset ruleset, Civ2Interface active)
        {
            var logicalSize = new Vector2(OriginalUnitWidth, active.UnitsPxHeight);

            // Initialize objects
            var units = new UnitImage[UnitColumns * active.UnitsRows];

            for (var i = 0; i < units.Length; i++)
            {
                var originalSource = GetClassicUnitImage(active, i);
                var mapSource = active.GetFossArtUnitImage(i) ?? originalSource;

                // Keep the classic 64px unit sheet as the UI icon source. City
                // windows, status panels, listboxes and production pickers all
                // expect small Civ2 sprites. The high-resolution FOSS art belongs
                // only to zoomable map rendering.
                var uiProps = Images.ExtractBitmapData(originalSource, active);
                var mapProps = Images.ExtractBitmapData(mapSource, active);

                var drawScale = GetDrawScale(mapProps.Image.Width, mapProps.Image.Height, logicalSize);
                var drawSize = new Vector2(mapProps.Image.Width * drawScale, mapProps.Image.Height * drawScale);
                var drawOffset = new Vector2(
                    MathF.Max(0, (logicalSize.X - drawSize.X) / 2f),
                    MathF.Max(0, logicalSize.Y - drawSize.Y));

                units[i] = new UnitImage
                {
                    Image = originalSource,
                    MapImage = ReferenceEquals(mapSource, originalSource) ? null : mapSource,
                    FlagLoc = uiProps.Flag1,
                    LogicalSize = logicalSize,
                    DrawOffset = drawOffset,
                    DrawScale = drawScale,
                };
            }

            active.UnitImages.UnitRectangle = new Rectangle(0, 0, logicalSize.X, logicalSize.Y);
            active.UnitImages.Units = units;

            active.GetShieldImages();
        }

        private static IImageSource GetClassicUnitImage(Civ2Interface active, int index)
        {
            if (active.PicSources.TryGetValue("unit", out var unitIcons) &&
                index >= 0 && index < unitIcons.Length &&
                unitIcons[index] is BitmapStorage bitmap &&
                string.Equals(Path.GetFileNameWithoutExtension(bitmap.Filename), "UNITS", StringComparison.OrdinalIgnoreCase))
            {
                return unitIcons[index];
            }

            return new BitmapStorage("UNITS",
                new Rectangle(1 + 65 * (index % UnitColumns), 1 + (active.UnitsPxHeight + 1) * (index / UnitColumns),
                    OriginalUnitWidth, active.UnitsPxHeight),
                searchFlagLoc: true);
        }

        private static float GetDrawScale(int sourceWidth, int sourceHeight, Vector2 logicalSize)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0)
            {
                return 1f;
            }

            return MathF.Min(logicalSize.X / sourceWidth, logicalSize.Y / sourceHeight);
        }
    }
}
