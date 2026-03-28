using Model.Controls;

namespace Model.Core;

public class ThroneRoom
{
    public int WallBack { get; set; } = 1;
    public int Floor { get; set; } = 1;
    public int Rug { get; set; } = 0;
    public int WallFront { get; set; } = 1;
    public int ThroneDecor { get; set; } = 0;
    public int ColumnsBack { get; set; } = 0;
    public int Throne { get; set; } = 1;
    public int ColumnsFront { get; set; } = 0;
    public bool DecorRugs { get; set; }
    public bool DecorPaintings { get; set; }
    public bool DecorBushes { get; set; }
    public bool DecorThroneBushes { get; set; }
    public bool DecorPots { get; set; }
    public bool DecorTreasures { get; set; }
    public bool DecorStatues { get; set; }
    
    public ThroneRoom Clone()
    {
        return new ThroneRoom
        {
            WallBack = WallBack,
            Floor = Floor,
            Rug = Rug,
            WallFront = WallFront,
            ThroneDecor = ThroneDecor,
            ColumnsBack = ColumnsBack,
            Throne = Throne,
            ColumnsFront = ColumnsFront,
            DecorRugs = DecorRugs,
            DecorPaintings = DecorPaintings,
            DecorBushes = DecorBushes,
            DecorThroneBushes = DecorThroneBushes,
            DecorPots = DecorPots,
            DecorTreasures = DecorTreasures,
            DecorStatues = DecorStatues,
        };
    }
}
