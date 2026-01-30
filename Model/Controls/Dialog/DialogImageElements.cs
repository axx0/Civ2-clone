using Model.Core.Units;
using Model.Images;

namespace Model.Controls;

public class DialogImageElements
{
    public IImageSource[]? Image { get; }
    public float Scale { get; } = 1f;
    public int[,] Coords { get; }
    
    public DialogImageElements(IImageSource[]? image, float scale = 1f, int[,]? coords = null)
    {
        Image = image;
        Scale = scale;
        Coords = coords ?? new int[image?.GetLength(0) ?? 1, 2];
    }

    public DialogImageElements(IImageSource? image, float scale = 1f) : 
        this([image], scale: scale, coords: new int[,] { { 0, 0 } })
    {
    }

    public DialogImageElements(IUnit unit, IUserInterface active)
    {
        Image = [active.PicSources["unit"][unit.Type]];
        Coords = new int[,] { { 0, 0 } };
    }
}
