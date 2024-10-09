using System;
using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.IO;
using Civ2engine.Terrains;
using Model;
using Model.ImageSets;
using Raylib_CSharp.Transformations;
using RaylibUI;
using RaylibUtils;

namespace Civ2.ImageLoader;

public static class CityLoader
{
    public static void LoadCities(Ruleset ruleset, CityImageSet cities, Civ2Interface active)
    {
        // Cities images
        for (int row = 0; row < 6; row++)
        {
            var sets = new CityImage[8];
            for (int col = 0; col < 8; col++)
            {
                var props = Images.ExtractBitmapData(active.PicSources["city"][8 * row + col], active); // put into cache
                cities.CityRectangle = new Rectangle(0, 0, props.Image.Width, props.Image.Height);

                sets[col] = new CityImage()
                {
                    Image = active.PicSources["city"][8 * row + col],
                    FlagLoc = props.Flag1,
                    SizeLoc = props.Flag2,
                };
            }

            cities.Sets.Add(sets);
        }

        // Colours
        active.LoadPlayerColours();

        if (active.TileSets.Count == 0)
        {
            active.TileSets.Add(new TerrainSet(64, 32));
        }

        active.UnitImages.Fortify = active.PicSources["fortify"][0];

        foreach (var terrain in active.TileSets)
        {
            terrain.ImprovementsMap[ImprovementTypes.Fortress] = new ImprovementGraphic
            { Levels = new[,] { { active.PicSources["fortress"][0] } } };

            // airbase
            terrain.ImprovementsMap[ImprovementTypes.Airbase] = new ImprovementGraphic
            {
                Levels = new[,] { { active.PicSources["airbase,empty"][0] } },
                UnitLevels = new[,] { { active.PicSources["airbase,full"][0] } }
            };
        }
    }
}