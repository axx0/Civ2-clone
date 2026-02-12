using Model.Core;
using Model.Core.Units;
using Model.Images;
using Raylib_CSharp.Colors;

namespace Model.Controls;

/// <summary>
/// Texts and images that are formed into groups of elements in listbox.
/// </summary>
public class ListboxGroupElement
{
    public IUnit? Unit { get; set; }
    public IGame? Game { get; set; }
    public string Text { get; set; } = string.Empty;
    public int? TextSizeOverride { get; set; } = null;
    public Color? FrontColorOverride { get; set; } = null;
    public Color? ShadowColorOverride { get; set; } = null;
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;
    public IImageSource? Icon { get; set; }
    public float ScaleIcon { get; set; } = 1.0f;

    /// <summary>
    /// Custom width of control.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Custom height of control.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Custom x-offset of control. Otherwise it's positioned after previous control.
    /// </summary>
    public int? Xoffset { get; set; }
}