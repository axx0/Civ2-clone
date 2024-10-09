using Raylib_CSharp.Transformations;

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