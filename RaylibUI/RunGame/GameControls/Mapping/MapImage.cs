using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.ImageSets;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.Mapping;

public static class MapImage
{
    
    private static readonly (int, int)[][] _coastMap = {
        new[]{ (0,4), (3,1) }, 
        new[]{ (3,2) }, 
        new[]{ (3,4), (1,1) }, 
        new[]{ (1,2) }, 
        new[]{ (1,4), (2,1) },
        new[]{ (2,2) },
        new[]{ (2,4), (0,1) },
        new[]{ (0,2) }
    };

    public static Rectangle TileRec = new (0, 0, 64, 32);

    internal static Image MakeTileGraphic(Tile tile, Map map,
        TerrainSet terrainSet, Game game)
    {
        // Define base bitmap for drawing
        var tilePic = Raylib.ImageCopy(terrainSet.BaseTiles[(int)tile.Type]);

        var directNeighbours = map.DirectNeighbours(tile, true).ToArray();

        var neighbours = map.Neighbours(tile, true).ToArray();

        // Dither
        if (tile.Type != TerrainType.Ocean)
        {
            for (var index = 0; index < directNeighbours.Length; index++)
            {
                var neighbour = directNeighbours[index];
                if (neighbour != null)
                {
                    ApplyDither(tilePic, neighbour.Type, tile.Type, terrainSet.DitherMaps[index]);
                }
            }
        }
        else
        {
            //drawCoasts
            var coastIndex = new[] { 0, 0, 0, 0 };
            foreach (var (neighbour, ind) in neighbours.Zip(_coastMap))
            {
                if (neighbour != null && neighbour.Type != TerrainType.Ocean)
                {
                    foreach (var (index, valueVARIABLE) in ind)
                    {
                        coastIndex[index] += valueVARIABLE;
                    }
                }
            }

            // NW+N+NE tiles
            Raylib.ImageDraw(ref tilePic, terrainSet.Coast[coastIndex[0], 0], new Rectangle(0, 0, 32, 16),
                new Rectangle(16, 0, 32, 16), Color.WHITE);

            // SW+S+SE tiles
            Raylib.ImageDraw(ref tilePic, terrainSet.Coast[coastIndex[1], 1], new Rectangle(0, 0, 32, 16),
                new Rectangle(16, 16, 32, 16), Color.WHITE);

            // SW+W+NW tiles
            Raylib.ImageDraw(ref tilePic, terrainSet.Coast[coastIndex[2], 2], new Rectangle(0, 0, 32, 16),
                new Rectangle(0, 8, 32, 16), Color.WHITE);

            // NE+E+SE tiles
            Raylib.ImageDraw(ref tilePic, terrainSet.Coast[coastIndex[3], 3], new Rectangle(0, 0, 32, 16),
                new Rectangle(32, 8, 32, 16), Color.WHITE);

            // River mouth
            // If river is next to ocean, draw river mouth on this tile.
            for (var index = 0; index < directNeighbours.Length; index++)
            {
                var neighbour = directNeighbours[index];
                if (neighbour is { River: true })
                {
                    Raylib.ImageDraw(ref tilePic, terrainSet.RiverMouth[index], TileRec, TileRec, Color.WHITE);
                }
            }
        }

        if (tile.Type is TerrainType.Forest or TerrainType.Hills or TerrainType.Mountains)
        {
            var index = 0;
            var increment = 1;
            foreach (var neighbour in directNeighbours)
            {
                if (neighbour != null && neighbour.Type == tile.Type)
                {
                    index += increment;
                }

                increment *= 2;
            }

            Raylib.ImageDraw(ref tilePic, terrainSet.ImagesFor(tile.Type)[index], TileRec,
                TileRec, Color.WHITE);
        }

        // Draw rivers
        if (tile.River)
        {
            var index = 0;
            var increment = 1;
            foreach (var neighbour in directNeighbours)
            {
                if (neighbour != null && (neighbour.River || neighbour.Type == TerrainType.Ocean))
                {
                    index += increment;
                }

                increment *= 2;
            }

            Raylib.ImageDraw(ref tilePic, terrainSet.River[index], TileRec, TileRec, Color.WHITE);
        }

        // Draw shield for grasslands
        if (tile.Type == TerrainType.Grassland)
        {
            if (tile.HasShield)
            {
                Raylib.ImageDraw(ref tilePic, terrainSet.GrasslandShield, TileRec, TileRec, Color.WHITE);
            }
        }
        else if (tile.Special != -1)
        {
            // Draw special resources if they exist
            Raylib.ImageDraw(ref tilePic, terrainSet.Specials[tile.Special][(int)tile.Type], TileRec, TileRec, Color.WHITE);
        }


        var improvements = tile.Improvements
            .Where(ci => game.TerrainImprovements.ContainsKey(ci.Improvement))
            .OrderBy(ci => game.TerrainImprovements[ci.Improvement].Layer).ToList();

        foreach (var construct in improvements)
        {
            var improvement = game.TerrainImprovements[construct.Improvement];
            var graphics = terrainSet.ImprovementsMap[construct.Improvement];

            if (improvement.HasMultiTile)
            {
                bool hasNeighbours = false;

                for (int i = 0; i < neighbours.Length; i++)
                {
                    var neighbour = neighbours[i];

                    var neighboringImprovement =
                        neighbour?.Improvements.FirstOrDefault(i =>
                            i.Improvement == construct.Improvement);
                    if (neighboringImprovement != null)
                    {
                        var index = i + 1;
                        if (index != -1)
                        {
                            if (neighboringImprovement.Level < construct.Level)
                            {
                                Raylib.ImageDraw(ref tilePic, graphics.Levels[neighboringImprovement.Level, index], TileRec,
                                    TileRec, Color.WHITE);
                            }
                            else
                            {
                                hasNeighbours = true;
                                Raylib.ImageDraw(ref tilePic, graphics.Levels[construct.Level, index], TileRec, TileRec,
                                    Color.WHITE);
                            }
                        }
                    }
                }

                if (!hasNeighbours)
                {
                    if (tile.CityHere is null)
                    {
                        Raylib.ImageDraw(ref tilePic, graphics.Levels[construct.Level, 0], TileRec, TileRec, Color.WHITE);
                    }
                }
            }
            else if (tile.CityHere is not null)
            {
                if (tile.Map.DirectNeighbours(tile)
                    .Any(t => t.Improvements.Any(i => i.Improvement == construct.Improvement)))
                {
                    Raylib.ImageDraw(ref tilePic, graphics.Levels[construct.Level, 0], TileRec, TileRec, Color.WHITE);
                }
            }
            else
            {
                if (tile.IsUnitPresent && graphics.UnitLevels != null)
                {
                    Raylib.ImageDraw(ref tilePic, graphics.UnitLevels[construct.Level, 0], TileRec, TileRec, Color.WHITE);
                }
                else
                {
                    Raylib.ImageDraw(ref tilePic, graphics.Levels[construct.Level, 0], TileRec, TileRec, Color.WHITE);
                }
            }
        }

        return tilePic;
    }

    private static void ApplyDither(Image orig_img, TerrainType neighbourType, TerrainType tileType,
        DitherMap ditherMap)
    {
        if (neighbourType == TerrainType.Ocean)
        {
            neighbourType = TerrainType.Grassland;
        }

        if (neighbourType == tileType) return;
        Raylib.ImageDraw(ref orig_img, ditherMap.Images[(int)neighbourType], new Rectangle(0, 0, 32, 16),
            new Rectangle(ditherMap.x, ditherMap.y, 32, 16), Color.WHITE);
    }
}