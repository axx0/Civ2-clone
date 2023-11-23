using System.Numerics;
using Civ2engine;
using Civ2engine.Terrains;
using Model.ImageSets;
using Raylib_cs;
using RaylibUI;
using RayLibUtils;

namespace Civ2.ImageLoader
{
    public static class CityLoader
    {
        public static unsafe void LoadCities(Ruleset ruleset,
            CityImageSet cities, Civ2Interface activeInterface)
        {
            // Read file in local directory. If it doesn't exist there, read it in root civ2 directory.
            var citiesImage = Images.LoadImage("CITIES", ruleset.Paths, "gif", "bmp");


            // Load special colours
            var colours = new List<Color>();
            var first = new List<int>();
            var last = new List<int>();
            var frequency = new List<int>();
            var max = 0;


            var imageColours = Raylib.LoadImageColors(citiesImage);

            for (var i = 0; i < citiesImage.height; i++)
            {
                var colour = imageColours[i * citiesImage.width];
                var index = colours.IndexOf(colour);
                if (index == -1)
                {
                    colours.Add(colour);
                    first.Add(i);
                    last.Add(i);
                    frequency.Add(1);
                }
                else
                {
                    last[index] = i;
                    frequency[index]++;
                    if (index != max && frequency[index] > frequency[max])
                    {
                        max = index;
                    }
                }
            }
            
            var borderColour = colours[max];
            var firstRow = first[max];

            var indexRow = (firstRow + 1) * citiesImage.width;

            var firstTransparent = imageColours[0];
            var secondTransparent = imageColours[1 + indexRow];

            var borderColours = colours.Where((_, i) => first[i] >= firstRow).ToList();

            var height = 0;
            for (var i = firstRow + 1; i < citiesImage.height; i++)
            {
                if (borderColours.IndexOf(imageColours[1 + citiesImage.width * i]) == -1) continue;
                height = i - firstRow;
                break;
            }

            var width = 0;


            for (var i = 1; i < citiesImage.width; i++)
            {
                if (borderColours.IndexOf(imageColours[i + indexRow]) == -1) continue;
                width = i;
                break;
            }


            Raylib.UnloadImageColors(imageColours);

            cities.CityRectangle = new Rectangle(0, 0, width, height);

            var transparent = new Color(0, 0, 0, 0);
            Raylib.ImageColorReplace(ref citiesImage, firstTransparent, transparent);
            Raylib.ImageColorReplace(ref citiesImage, secondTransparent, transparent);

            imageColours = Raylib.LoadImageColors(citiesImage);

            for (var i = firstRow; i < citiesImage.height - firstRow; i++)
            {
                
                var set = new List<CityImage>();
                if (borderColours.IndexOf(imageColours[1 + i * citiesImage.width]) == -1 ||
                    borderColours.IndexOf(imageColours[1 + (i + height) * citiesImage.width]) == -1) continue;
                //We have a candidate row
                var rowOffset = (i + 1) * citiesImage.width;
                for (var j = 0; j < citiesImage.width - width; j++)
                {
                    if (!imageColours[j + rowOffset].Equals(borderColour) ||
                        !imageColours[j + width + rowOffset].Equals(borderColour)) continue;
                    //This looks like a city image
                    var cityImage = MakeCityImage(citiesImage, imageColours, i, j, width, height);
                    if (cityImage != null)
                    {
                        set.Add(cityImage);
                    }
                }

                cities.Sets.Add(set.ToArray());
            }

            var lastRow = last[max];
            var flagHeight = 0;
            for (var i = lastRow - 1; i > 0; i--)
            {
                if (!imageColours[1 + i * citiesImage.width].Equals(borderColour)) continue;
                flagHeight = lastRow - i;
                break;
            }

            var flagWidth = 0;
            var rowOffest = (lastRow - 1) * citiesImage.width;
            for (var i = 1; i < flagHeight; i++)
            {

                if (!imageColours[i + rowOffest].Equals(borderColour)) continue;
                flagWidth = i;
                break;
            }

            var playerColours = new List<PlayerColour>();
            var topLeft = lastRow - 2 * flagHeight + 1;
            var col = 1;
            for (; col < citiesImage.width; col += flagWidth)
            {
                if (imageColours[col + flagWidth - 1 + topLeft * citiesImage.width].Equals(borderColour))
                {
                    var lightCandidate = imageColours[col + (topLeft - 4) * citiesImage.width];
                    var darkCandidate = imageColours[col + (topLeft - 3) * citiesImage.width];
                    playerColours.Add(new PlayerColour
                    {
                        Normal = Raylib.ImageFromImage(citiesImage,
                            new Rectangle(col, topLeft, flagWidth - 1, flagHeight - 1)),
                        TextColour = imageColours[col + (topLeft - 2) * citiesImage.width],
                        DarkColour = darkCandidate.a == 0
                            ? imageColours[col + 6 + (topLeft + 5) * citiesImage.width]
                            : darkCandidate,
                        LightColour = lightCandidate.a == 0 || lightCandidate.Equals(borderColour)
                            ? imageColours[col + 5 + (topLeft + 6) * citiesImage.width]
                            : lightCandidate

                    });
                }
                else
                {
                    break;
                }
            }

            activeInterface.PlayerColours = playerColours.ToArray();

            var specials = new List<Image>();
            var specialStart = col + 1;
            while (imageColours[specialStart + topLeft * citiesImage.width].a == 0)
            {
                specialStart++;
            }

            var specialTop = topLeft;
            while (imageColours[specialStart + specialTop * citiesImage.width].Equals(borderColour))
            {
                specialTop--;
            }

            specialTop += 2;

            for (var i = specialStart; i < citiesImage.width - width; i += width)
            {
                if (imageColours[i + width + specialTop * citiesImage.width].Equals(borderColour))
                {
                    specials.Add(Raylib.ImageFromImage(citiesImage,
                        new Rectangle(i + 1, specialTop, width - 1, height - 1)));
                }
            }

            if (activeInterface.TileSets.Count == 0)
            {
                activeInterface.TileSets.Add(new TerrainSet(64,32));
            }

            foreach (var terrain in activeInterface.TileSets)
            {

                terrain.ImprovementsMap[ImprovementTypes.Fortress] = new ImprovementGraphic
                    { Levels = new[,] { { specials[1] } } };

                // Airbase
                terrain.ImprovementsMap[ImprovementTypes.Airbase] = new ImprovementGraphic
                {
                    Levels = new[,] { { specials[2] } },
                    UnitLevels = new[,] { { specials[3] } }
                };
            }

            Raylib.UnloadImageColors(imageColours);
        }


        private static readonly Color FlagColour = new(0, 0, 255, 255);

        private static unsafe CityImage? MakeCityImage(Image citiesImage, Color* colorArray, int y, int x, int width,
            int height)
        {
            var flagX = 0;
            var flagY = 0;
            var sizeX = 0;
            var sizeY = 0;
            var borderColour = colorArray[x + y * citiesImage.width];
            for (var i = x; i < x + width; i++)
            {
                var colour = colorArray[i + y * citiesImage.width];
                if (colour.Equals(borderColour)) continue;

                if (!colour.Equals(FlagColour))
                {
                    sizeX = i - x;
                    if (flagX == 0) flagX = sizeX;
                }
                else
                {
                    flagX = i - x;
                    if (sizeX == 0) sizeX = flagX;
                }
            }

            for (var i = y; i < y + height; i++)
            {
                var colour = colorArray[x + i * citiesImage.width];
                if (colour.Equals(borderColour)) continue;

                if (!colour.Equals(FlagColour))
                {
                    sizeY = i - y;
                    if (flagY == 0) flagY = sizeY;
                }
                else
                {
                    flagY = i - y;
                    if (sizeY == 0) sizeY = flagY;
                }
            }


            if (flagX == 0 || flagY == 0 || sizeX == 0 || sizeY == 0)
            {
                return null;
            }

            return new CityImage
            {
                Image = Raylib.ImageFromImage(citiesImage, new Rectangle(x + 1, y + 1, width - 1, height - 1)),
                FlagLoc = new Vector2(flagX, flagY),
                SizeLoc = new Vector2(sizeX, sizeY)
            };
        }
    }
}