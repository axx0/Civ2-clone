using Raylib_CSharp.Transformations;

namespace Model;

public class ConsumableResourceArea : ResourceArea
{
    public ConsumableResourceArea(string name, Rectangle bounds, Func<int, OutputType, string> getDisplayDetails,
        bool noSurplus = false, bool labelBelow = false) : base(bounds, labelBelow)
    {
        Name = name;
        GetDisplayDetails = getDisplayDetails;
        NoSurplus = noSurplus;
    }

    public string Name { get; set; }
    public Func<int, OutputType, string> GetDisplayDetails { get; }
    public bool NoSurplus { get; }
}