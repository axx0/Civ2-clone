using Model.Images;

namespace Model.Interface;

public class ListBoxEntry
{
    /// <summary>
    /// Left-aligned text
    /// </summary>
    public string? LeftText { get; set; }

    /// <summary>
    /// Right-aligned text
    /// </summary>
    public string? RightText { get; set; }
 
    /// <summary>
    /// Icons left of text
    /// </summary>
    public IImageSource? Icon { get; set; }  
}