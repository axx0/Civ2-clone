using Raylib_CSharp.Images;
using Raylib_CSharp.Textures;

namespace Model.ImageSets;

public class Wallpaper
{
    public Image Outer { get; set; }
    public Image[] Inner { get; set; }
    public Image InnerAlt { get; set; }
    public Image[] OuterTitleTop { get; set; }
    public Image[] OuterThinTop { get; set; }
    public Image[] OuterBottom { get; set; }
    public Image[] OuterMiddle { get; set; }
    public Image[] OuterLeft { get; set; }
    public Image[] OuterRight { get; set; }
    public Image OuterTitleTopLeft { get; set; }
    public Image OuterTitleTopRight { get; set; }
    public Image OuterThinTopLeft { get; set; }
    public Image OuterThinTopRight { get; set; }
    public Image OuterMiddleLeft { get; set; }
    public Image OuterMiddleRight { get; set; }
    public Image OuterBottomLeft { get; set; }
    public Image OuterBottomRight { get; set; }
    public Image[] Button { get; set; }
    public Image[] ButtonClicked { get; set; }
    public Texture2D OuterTexture { get; set; }
    public Texture2D InnerTexture { get; set; }
}