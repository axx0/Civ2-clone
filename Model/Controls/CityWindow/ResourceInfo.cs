using Civ2engine;
using Model.Core.Cities;
using Model.Images;

namespace Model.Controls;

public class ResourceInfo
{
    public string Name { get; set; } = string.Empty;
    public Func<int, City, string> GetResourceLabel { get; set; } = (_, _) => string.Empty;
    public IImageSource Icon { get; set; } = null!;
}
