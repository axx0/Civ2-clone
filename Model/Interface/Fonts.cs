using Raylib_cs;

namespace Model.Interface;

public static class Fonts
{
    /// <summary>
    /// Times new roman
    /// </summary>
    public static Font TNR { get; set; } = Raylib.GetFontDefault();

    /// <summary>
    /// Bold times new roman font
    /// </summary>
    public static Font TNRbold { get; set; } = Raylib.GetFontDefault();

    /// <summary>
    /// Alternative font
    /// </summary>
    public static Font Arial { get; set; } = Raylib.GetFontDefault();

    public const int FontSize = 20;

    public static void SetTNR(Font font)
    {
        TNR = font;
    }

    public static void SetArial(Font font)
    {
        Arial = font;
    }

    public static void SetBold(Font font)
    {
        TNRbold = font;
    }
}

