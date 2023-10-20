using Model.Images;

namespace Model;

public class InterfaceStyle
{
    public IImageSource Outer { get; init; }
    public IImageSource Inner { get; init; }
    
    public IImageSource[] RadioButtons { get; init; }
    public string DefaultFont { get; init; }

    public string BoldFont { get; init; }
    public string AlternativeFont { get; init; }
    public IImageSource[] CheckBoxes { get; init; }
}