using Model.Images;

namespace Model.Interface;

/// <summary>
/// Texts and images that are formed into groups of elements in listbox.
/// </summary>
public class ListboxGroupElement
{
    public string Text { get; set; } = string.Empty;
    public IImageSource? Icon { get; set; }
    public float ScaleIcon { get; set; } = 1.0f;

    /// <summary>
    /// Custom width of control.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Custom x-offset of control. Otherwise it's positioned after previous control.
    /// </summary>
    public int? Xoffset { get; set; }

    /// <summary>
    /// Text/image is right-aligned to width of control.
    /// </summary>
    public bool RightAligned { get; set; } = false;
}