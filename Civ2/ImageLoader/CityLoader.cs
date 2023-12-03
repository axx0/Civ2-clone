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
            cityProps["city"][0].Rect.width, cityProps["city"][0].Rect.height);

        // Cities images
        for (int row = 0; row < 7; row++)
        {
            var sets = new CityImage[8];
            for (int col = 0; col < 8; col++)
            {
                sets[col] = new CityImage()
                {
                    Image = cityProps["city"][8 * row + col].Image,
                    FlagLoc = new Vector2(cityProps["city"][8 * row + col].Flag1x,
                                          cityProps["city"][8 * row + col].Flag1y),
                    SizeLoc = new Vector2(cityProps["city"][8 * row + col].Flag2x,
                                          cityProps["city"][8 * row + col].Flag2y)
                };
            }

            cities.Sets.Add(sets);
        }

        // Colours
        var playerColours = new PlayerColour[9];
        for (int col = 0; col < 9; col++)
        {
            unsafe
            {
                var imageColours = Raylib.LoadImageColors(cityProps["textColors"][col].Image);
                var textColour = imageColours[0];

                imageColours = Raylib.LoadImageColors(cityProps["flags"][col].Image);
                var lightColour = imageColours[3 * cityProps["flags"][col].Image.width + 8];

                imageColours = Raylib.LoadImageColors(cityProps["flags"][9 + col].Image);
                var darkColour = imageColours[3 * cityProps["flags"][9 + col].Image.width + 5];
                Raylib.UnloadImageColors(imageColours);

                playerColours[col] = new PlayerColour
                {
                    Normal = cityProps["flags"][col].Image,
                    TextColour = textColour,
                    LightColour = lightColour,
                    DarkColour = darkColour
                };
            }
        }
        active.PlayerColours = playerColours;

        if (active.TileSets.Count == 0)
        {
            active.TileSets.Add(new TerrainSet(64, 32));
        }

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