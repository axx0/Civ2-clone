using Raylib_CSharp.Transformations;

namespace Model.Controls;

public class SharedResourceArea : ResourceArea
{
    public SharedResourceArea(Rectangle bounds, bool labelBelow) : base(bounds, labelBelow)
    {
    }

    public List<ResourceInfo> Resources { get; set; } = new();
}