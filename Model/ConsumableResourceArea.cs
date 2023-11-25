using Civ2engine;
using Model.Images;
using Raylib_cs;

namespace Model;

public class ResourceArea
{
    protected ResourceArea(Rectangle bounds, bool labelBelow)
    {
        Bounds = bounds;
        LabelBelow = labelBelow;
    }

    public Rectangle Bounds { get; }
    public bool LabelBelow { get; }
}

public class SharedResourceArea : ResourceArea
{
    public SharedResourceArea(Rectangle bounds, bool labelBelow) : base(bounds, labelBelow)
    {
    }

    public List<ResourceInfo> Resources { get; set; } = new();
}

public class ResourceInfo
{
    public string Name { get; set; }
    public Func<int, City, string> GetResourceLabel { get; set; }
    public IImageSource Icon { get; set; }
}

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

public enum OutputType
{
    Consumption,
    Loss,
    Surplus
}