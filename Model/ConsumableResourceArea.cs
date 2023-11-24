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
    public string Label { get; set; }
    public IImageSource Icon { get; set; }
}

public class ConsumableResourceArea : ResourceArea
{
    public ConsumableResourceArea(string name, Rectangle bounds, string consumptionLabel, string lossLabel,
        string surplusLabel = "", bool labelBelow = false) : base(bounds, labelBelow)
    {
        Name = name;
        ConsumptionLabel = consumptionLabel;
        LossLabel = lossLabel;
        SurplusLabel = surplusLabel;
    }

    public string Name { get; set; }
    public string ConsumptionLabel { get; set; }
    public string LossLabel { get; set; }
    public string SurplusLabel { get; }
}