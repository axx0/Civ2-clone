namespace Civ2engine;

public class ResourceValues
{
    public ResourceValues()
    {
    }

    public ResourceValues(int consumption, int surplus, int loss)
    {
        Consumption = consumption;
        Surplus = surplus;
        Loss = loss;
        Total = consumption + surplus + loss;
    }

    public int Total { get; set; }

    public int Consumption { get; set; }
    public int Surplus { get; set; }
    public int Loss { get; set; }
}