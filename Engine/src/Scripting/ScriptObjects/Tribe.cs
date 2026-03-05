namespace Civ2engine.Scripting;

public class Tribe  
{
    public Civilization Civ { get; }

    public Tribe(Civilization civ)
    {
        Civ = civ;
        Id = civ.Id;
    }

    public int Id { get; set; }
}