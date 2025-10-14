using Raylib_CSharp.Transformations;
using System.Numerics;

namespace Model;

public class ResourceProduction
{
    public Rectangle TitlePosition { get; set; }
    public IList<ResourceArea> Resources { get; set; }
}