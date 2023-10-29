namespace RaylibUI.RunGame.GameControls.Mapping;

public record MapDimensions
{
    public int TotalWidth { get; set; }
    public int TotalHeight { get; set; }
    public int HalfHeight { get; set; }
    public int TileHeight { get; set; }
    public int TileWidth { get; set; }
    public int HalfWidth { get; set; }
    public int DiagonalCut { get; set; }
}   