using System.Linq;
using Civ2engine.Enums;
using System.Numerics;
using Raylib_cs;

namespace RaylibUI
{
    public partial class Main
    {
        int startX, startY = 0;
        float animationSkip = 0.2f; // in seconds
        float elapsedTime = 0;
        bool drawActiveUnit = true;

        private void DrawStuff()
        {
            Raylib.ClearBackground(Color.WHITE);

            // DRAW MAP
            int Width = Raylib.GetScreenWidth();// MapTileTextureC2(0, 0).Width;
            int Height = 2;// * Raylib.GetScreenHeight() / MapTileTextureC2(0, 0).Height;

            // Center map around active unit in start
            if (Raylib.GetFrameTime() == 0)
            {
                startX = activeUnit.X - Width;
                startY = activeUnit.Y - Height / 2;
            }

            // startX += (int)mouseMapViewIncrement.X;
            // startY += (int)mouseMapViewIncrement.Y;

            for (int _row = -2; _row < Height + 2; _row++)
            {
                for (int _col = -2; _col < Width; _col++)
                {
                    // Determine column index in civ2-style coords
                    int col = 2 * _col + (_row % 2);
                    int row = _row;

                    // Don't draw beyond borders
                    var xC2 = startX + col;
                    var yC2 = startY + row;
                    if (xC2 < 0 || yC2 < 0 || xC2 >= map.XDimMax || yC2 >= map.YDim) continue;

                    //Raylib.DrawTexture(MapTileTextureC2(xC2, yC2), 32 * col, 16 * row, Color.WHITE);

                    // Units
                    // var unitsHere = map.TileC2(xC2, yC2).UnitsHere;
                    // if (unitsHere.Count > 0)
                    // {
                    //     if (unitsHere.Contains(activeUnit))
                    //     {
                    //         elapsedTime += Raylib.GetFrameTime();
                    //         if (elapsedTime > animationSkip)
                    //         {
                    //             elapsedTime = 0;    // reset
                    //             drawActiveUnit = !drawActiveUnit;
                    //         }
                    //
                    //         if (drawActiveUnit)
                    //             Raylib.DrawTexture(MapImages.Units[(int)activeUnit.Type].Texture, 32 * col, 16 * (row - 1), Color.WHITE);
                    //     }
                    //     else 
                    //     {
                    //         var unit = unitsHere.Last();
                    //         if (unitsHere.Any(u => u.Domain == UnitGAS.Sea)) unit = unitsHere.Last(u => u.Domain == UnitGAS.Sea);   // Show naval unit
                    //         if (!unit.IsInCity)
                    //             Raylib.DrawTexture(MapImages.Units[(int)unit.Type].Texture, 32 * col, 16 * (row - 1), Color.WHITE);
                    //     }
                    // }

                    // Cities
                    var city = map.TileC2(xC2, yC2).CityHere;
                    if (city != null)
                    {
                        // Determine city style
                        // ANCIENT OR RENAISSANCE EPOCH => 4 city size styles (0=sizes 1...3, 1=sizes 4...5, 2=sizes 6...7, 3=sizes >= 8)
                        // If city is capital => 3 size styles (1=sizes 1...3, 2=sizes 4...5, 3=sizes >= 6)
                        var style = city.Owner.CityStyle;
                        int sizeStyle;
                        if (city.Owner.Epoch == EpochType.Ancient || city.Owner.Epoch == EpochType.Renaissance)
                        {
                            sizeStyle = city.Size switch
                            {
                                <= 3 => 0,
                                > 3 and <= 5 => 1,
                                > 5 and <= 7 => 2,
                                _ => 3
                            };

                        }
                        // INDUSTRIAL EPOCH => 4 city size styles (0=sizes 1...4, 1=sizes 5...7, 2=sizes 8...10, 3=sizes >= 11)
                        // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...7, 3=sizes >= 8)
                        else if (city.Owner.Epoch == EpochType.Industrial)
                        {
                            sizeStyle = city.Size switch
                            {
                                <= 4 => 0,
                                > 4 and <= 7 => 1,
                                > 7 and <= 10 => 2,
                                _ => 3
                            };
                        }
                        // MODERN EPOCH => 4 city size styles (0=sizes 1...4, 1=sizes 5...10, 2=sizes 11...18, 3=sizes >= 19)
                        // If city is capital => 3 size styles (1=sizes 1...4, 2=sizes 5...10, 3=sizes >= 11)
                        else
                        {
                            sizeStyle = city.Size switch
                            {
                                <= 4 => 0,
                                > 4 and <= 10 => 1,
                                > 10 and <= 18 => 2,
                                _ => 3
                            };
                        }

                        if (sizeStyle < 3)
                        {
                            sizeStyle++;
                        }

                        int cityIndex;
                        if (city.Owner.Epoch == EpochType.Industrial)
                        {
                            cityIndex = 4 * 8 + sizeStyle;
                        }
                        else if (city.Owner.Epoch == EpochType.Modern)
                        {
                            cityIndex = 5 * 8 + sizeStyle;
                        }
                        else
                        {
                            cityIndex = (int)style * 8 + sizeStyle;
                        }

                        // Raylib.DrawTexture(MapImages.Cities[cityIndex].Texture, 32 * col, 16 * (row - 1), Color.WHITE);
                    }
                }
            }

            // City names
            for (int _row = -2; _row < Height + 2; _row++)
            {
                for (int _col = -2; _col < Width; _col++)
                {
                    // Determine column index in civ2-style coords
                    int col = 2 * _col + (_row % 2);
                    int row = _row;

                    // Don't draw beyond borders
                    var xC2 = startX + col;
                    var yC2 = startY + row;
                    if (xC2 < 0 || yC2 < 0 || xC2 >= map.XDimMax || yC2 >= map.YDim) continue;

                    var city = map.TileC2(xC2, yC2).CityHere;
                    if (city != null)
                    {
                        //var txtMeasure = Raylib.MeasureTextEx(fonts[0], cityName, fontSize, spacing);

                        Raylib.DrawText(city.Name, 32 * col + 1, 16 * row + 20 + 1, 26, Color.BLACK);    // shadow
                        Raylib.DrawText(city.Name, 32 * col, 16 * row + 20, 26, Color.WHITE);
                    }
                }
            }
        }
    }
}

