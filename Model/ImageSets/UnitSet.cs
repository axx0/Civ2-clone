using Raylib_cs;

namespace Model.ImageSets;

public class UnitSet
{
    public Rectangle UnitRectangle { get; set; }
    public UnitImage[] Units { get; set; }
    public Image[] Shields { get; set; }
    public Image[] ShieldBack { get; set; }
    public Image ShieldShadow { get; set; }
    public Image[] BattleAnim { get; set; }
}