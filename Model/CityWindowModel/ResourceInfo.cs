using Civ2engine;
using Model.Images;

namespace Model;

public class ResourceInfo
{
    public string Name { get; set; }
    public Func<int, City, string> GetResourceLabel { get; set; }
    public IImageSource Icon { get; set; }
}