using System;
using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Model.ImageSets;
using Raylib_cs;
using RaylibUI;
using RayLibUtils;

namespace Civ2.ImageLoader;

public static class CityLoader
{
    public static void LoadCities(Ruleset ruleset, CityImageSet cities, Civ2Interface active)
    {
        var path = Utils.GetFilePath("CITIES", ruleset.Paths, "gif", "bmp");
        Images.LoadPropertiesFromPIC(path, active.CitiesPICprops);
        var cityProps = active.CitiesPICprops;

        cities.CityRectangle = new Rectangle(0, 0,
            cityProps["city"][0].Rect.Width, cityProps["city"][0].Rect.Height);

        // Cities images
        for (int row = 0; row < 7; row++)
        {
            var sets = new CityImage[8];
            for (int col = 0; col < 8; col++)
            {
                sets[col] = new CityImage()
                {
                    Image = cityProps["city"][8 * row + col].Image,
                    Texture = Raylib.LoadTextureFromImage(cityProps["city"][8 * row + col].Image),
                    FlagLoc = new Vector2(cityProps["city"][8 * row + col].Flag1x,
                                          cityProps["city"][8 * row + col].Flag1y),
                    SizeLoc = new Vector2(cityProps["city"][8 * row + col].Flag2x,
                                          cityProps["city"][8 * row + col].Flag2y)
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

        active.UnitImages.Fortify = Raylib.LoadTextureFromImage(cityProps["fortify"][0].Image);

        foreach (var terrain in active.TileSets)
        {

            terrain.ImprovementsMap[ImprovementTypes.Fortress] = new ImprovementGraphic
            { Levels = new[,] { { cityProps["fortress"][0].Image } } };

            // airbase
            terrain.ImprovementsMap[ImprovementTypes.Airbase] = new ImprovementGraphic
            {
                Levels = new[,] { { cityProps["airbase,empty"][0].Image } },
                UnitLevels = new[,] { { cityProps["airbase,full"][0].Image } }
            };
        }
    }
}