using Model.Images;

namespace Model.Dialog;

public class DialogImageElements
{
    public IImageSource[]? Image { get; }
    public float Scale { get; }
    public int[,] Coords { get; }

    public DialogImageElements(IImageSource[]? image, float scale = 1f, int[,]? coords = null)
    {
        Image = image;
        Scale = scale;
        Coords = coords ?? new int[image?.GetLength(0) ?? 1, 2];
    }

    public DialogImageElements(IImageSource? image, float scale = 1f) : 
        this(new[] { image }, scale: scale, coords: new int[,] { { 0, 0 } })
    {
    }
}
