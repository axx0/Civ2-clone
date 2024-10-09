using Raylib_CSharp.Fonts;
using Raylib_CSharp.Textures;

namespace Model.Interface;

public static class Fonts
{
    /// <summary>
    /// Times new roman
    /// </summary>
    public static Font Tnr { get; set; } = Font.GetDefault();

    /// <summary>
    /// Bold times new roman font
    /// </summary>
    public static Font TnRbold { get; set; } = Font.GetDefault();

    /// <summary>
    /// Alternative font
    /// </summary>
    public static Font Arial { get; set; } = Font.GetDefault();

    public const int FontSize = 20;

    public static void SetTnr(Font font)
    {
        Tnr = font;
        Tnr.Texture.SetFilter(TextureFilter.Bilinear);
    }

    public static void SetArial(Font font)
    {
        Arial = font;
        //Raylib.SetTextureFilter(Arial.Texture, TextureFilter.Point);
    }

    public static void SetBold(Font font)
    {
        TnRbold = font;
        TnRbold.Texture.SetFilter( TextureFilter.Bilinear);
    }
}

