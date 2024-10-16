using Model.Images;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Colors;

namespace Model;

public class InterfaceStyle
{
    public IImageSource? Outer { get; init; }
    public IImageSource[]? Inner { get; init; }
    public IImageSource? InnerAlt { get; init; }    // only used in TOT for status panel wallpaper
    public IImageSource[]? OuterTitleTop { get; init; }  // TOT
    public IImageSource[]? OuterThinTop { get; init; }   // TOT
    public IImageSource[]? OuterBottom { get; init; }   // TOT
    public IImageSource[]? OuterMiddle { get; init; }   // TOT
    public IImageSource[]? OuterLeft { get; init; }   // TOT
    public IImageSource[]? OuterRight { get; init; }   // TOT
    public IImageSource? OuterTitleTopLeft { get; init; }    // TOT
    public IImageSource? OuterTitleTopRight { get; init; }   // TOT
    public IImageSource? OuterThinTopLeft { get; init; } // TOT
    public IImageSource? OuterThinTopRight { get; init; }    // TOT
    public IImageSource? OuterMiddleLeft { get; init; }  // TOT
    public IImageSource? OuterMiddleRight { get; init; } // TOT
    public IImageSource? OuterBottomLeft { get; init; }  // TOT
    public IImageSource? OuterBottomRight { get; init; }    // TOT

    public IImageSource[]? Button { get; init; }   // TOT
    public IImageSource[]? ButtonClicked { get; init; }   // TOT

    public IImageSource[] RadioButtons { get; init; }
    public IImageSource[] CheckBoxes { get; init; }

    public Font DefaultFont { get; init; }
    public Font ButtonFont { get; init; }
    public int ButtonFontSize { get; init; }
    public Color ButtonColour { get; init; }
    public Font HeaderLabelFont { get; init; }
    public int HeaderLabelFontSizeNormal { get; init; }
    public int HeaderLabelFontSizeLarge { get; init; }
    public int CityHeaderLabelFontSizeLarge { get; init; }
    public int CityHeaderLabelFontSizeNormal { get; init; }
    public int CityHeaderLabelFontSizeSmall { get; init; }
    public bool HeaderLabelShadow { get; init; } 
    public Color HeaderLabelColour { get; init; }
    public Font LabelFont { get; init; }
    public int LabelFontSize { get; init; }
    public Color LabelColour { get; init; }
    public Color LabelShadowColour { get; init; }
    public Font CityWindowFont { get; init; }
    public int CityWindowFontSize { get; init; }
    public Font MenuFont { get; init; }
    public int MenuFontSize { get; init; }
    public Font StatusPanelLabelFont { get; init; }
    public Color StatusPanelLabelColor { get; init; }
    public Color StatusPanelLabelColorShadow { get; init; }
    public Color MovingUnitsViewingPiecesLabelColor { get; init; }
    public Color MovingUnitsViewingPiecesLabelColorShadow { get; init; }
    public Color[] EndOfTurnColors { get; init; }
}