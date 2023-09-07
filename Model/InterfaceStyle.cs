using Model.Images;

namespace Model;

public class InterfaceStyle
{
    public IImageSource Outer { get; init; }
    public IImageSource Inner { get; init; }
    
    public IImageSource[] RadioButtons { get; init; }
    public string Font { get; set; }
}