namespace Model;

public struct Padding
{
    public int Left { get; set; }
    public  int Right { get; set; }
    public int Top { get; set; }
    public int Bottom { get; set; }

    public Padding(int top, int left = int.MaxValue, int bottom = int.MaxValue, int right = int.MaxValue)
    {
        Top = top;
        Left = left == int.MaxValue ? top : left;
        Right = right == int.MaxValue ? Left : right;
        Bottom = bottom == int.MaxValue ? top : bottom;
    }

    public Padding(Padding previous, int top, int left = 0, int bottom = 0, int right = 0)
    {
        Top = previous.Top + top;
        Left = previous.Left + left;
        Bottom = previous.Bottom + bottom;
        Right = previous.Right + right;
    }

    public static readonly Padding None = new(0);
}
