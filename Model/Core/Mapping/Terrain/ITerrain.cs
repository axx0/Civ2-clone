namespace Model.Core.Mapping;

public interface ITerrain
{
    public string Name { get; set; }
    public int MoveCost { get; set; }
    public int Defense { get; set; }
    public int Food { get; set; }
    public int Shields { get; set; }
    public int Trade { get; set; }
}