using Model.Images;
using System.Numerics;

namespace Model.Controls;

public class CityViewTiles
{
    public int Id { get; set; }

    /// <summary>
    /// Id corresponding to improvement/wonder's row in rules file.
    /// </summary>
    public int RulesId { get; set; }

    /// <summary>
    /// Source image.
    /// </summary>
    public IImageSource Source { get; set; }
    
    /// <summary>
    /// Image position.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Id of tiles to be drawn if improvement does not exist in city (forest, etc.)
    /// </summary>
    public int AlternativeTileId { get; set; }

    public CityViewTiles(int id, int rulesId, IImageSource source, Vector2 position, int altId)
    {
        Id = id;
        RulesId = rulesId;
        Source = source;
        Position = position;
        AlternativeTileId = altId;
    }
}
