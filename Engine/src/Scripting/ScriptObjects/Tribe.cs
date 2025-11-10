namespace Civ2engine.Scripting;

public class Tribe  
{
    public Civilization Civ { get; }

    public Tribe(Civilization civ)
    {
        Civ = civ;
    }

    public int Id { get; set; }
}