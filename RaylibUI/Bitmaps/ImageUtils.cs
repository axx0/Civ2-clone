using Model.Images;
using Raylib_cs;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibUI;

public class ImageUtils
{

    public static void PaintPanelBorders(ref Image image, int width, int height, int paddingTop, int paddingBtm)
    {
        DrawTiledImage(OuterWallpaper, ref image, height, width);
        // Paint panel borders
        // Outer border
        var pen1 = new Color(227, 227, 227, 255);
        var pen2 = new Color(105, 105, 105, 255);
        var pen3 = new Color(255, 255, 255, 255);
        var pen4 = new Color(160, 160, 160, 255);
        var pen5 = new Color(240, 240, 240, 255);
        var pen6 = new Color(223, 223, 223, 255);
        var pen7 = new Color(67, 67, 67, 255);
        Raylib.ImageDrawLine(ref image,0,0,width -2,0,pen1); // 1st layer of border
        Raylib.ImageDrawLine(ref image,0, 0, width - 2, 0,pen1);
        Raylib.ImageDrawLine(ref image,0, 0, 0, height - 2,pen1);
        Raylib.ImageDrawLine(ref image,width - 1, 0, width - 1, height - 1,pen2);
        Raylib.ImageDrawLine(ref image,0, height - 1, width - 1, height - 1,pen2);
        Raylib.ImageDrawLine(ref image,1, 1, width - 3, 1,pen3); // 2nd layer of border
        Raylib.ImageDrawLine(ref image,1, 1, 1, height - 3,pen3);
        Raylib.ImageDrawLine(ref image,width - 2, 1, width - 2, height - 2,pen4);
        Raylib.ImageDrawLine(ref image,1, height - 2, width - 2, height - 2,pen4);
        Raylib.ImageDrawLine(ref image,2, 2, width - 4, 2,pen5); // 3rd layer of border
        Raylib.ImageDrawLine(ref image,2, 2, 2, height - 4,pen5);
        Raylib.ImageDrawLine(ref image,width - 3, 2, width - 3, height - 3,pen5);
        Raylib.ImageDrawLine(ref image,2, height - 3, width - 3, height - 3,pen5);
        Raylib.ImageDrawLine(ref image,3, 3, width - 5, 3,pen6); // 4th layer of border
        Raylib.ImageDrawLine(ref image,3, 3, 3, height - 5,pen6);
        Raylib.ImageDrawLine(ref image,width - 4, 3, width - 4, height - 4,pen7);
        Raylib.ImageDrawLine(ref image,3, height - 4, width - 4, height - 4,pen7);
        Raylib.ImageDrawLine(ref image,4, 4, width - 6, 4,pen6); // 5th layer of border
        Raylib.ImageDrawLine(ref image,4, 4, 4, height - 6,pen6);
        Raylib.ImageDrawLine(ref image,width - 5, 4, width - 5, height - 5,pen7);
        Raylib.ImageDrawLine(ref image,4, height - 5, width - 5, height - 5,pen7);

        // Inner panel
        Raylib.ImageDrawLine(ref image,9, paddingTop - 1, 9 + (width - 18 - 1), paddingTop - 1,pen7); // 1st layer of border
        Raylib.ImageDrawLine(ref image,10, paddingTop - 1, 10, height - paddingBtm - 1,pen7);
        Raylib.ImageDrawLine(ref image,width - 11, paddingTop - 1, width - 11, height - paddingBtm - 1,pen6);
        Raylib.ImageDrawLine(ref image,9, height - paddingBtm, width - 9 - 1, height - paddingBtm,pen6);
        Raylib.ImageDrawLine(ref image,10, paddingTop - 2, 9 + (width - 18 - 2), paddingTop - 2,pen7); // 2nd layer of border
        Raylib.ImageDrawLine(ref image,9, paddingTop - 2, 9, height - paddingBtm,pen7);
        Raylib.ImageDrawLine(ref image,width - 10, paddingTop - 2, width - 10, height - paddingBtm,pen6);
        Raylib.ImageDrawLine(ref image,9, height - paddingBtm + 1, width - 9 - 1, height - paddingBtm + 1,pen6);
        
        
        //DrawTiledImage(InnerWalpaper, ref image, height, width);
    }

    public static void DrawTiledImage(Image source,ref Image destination, int height, int width)
    {  
        int rows = height / source.height + 1;
        var columns = width / source.width + 1;
        var sourceRec = new Rectangle { height = source.height, width = source.width };
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Raylib.ImageDraw(ref destination, source, sourceRec, new Rectangle(col* source.width, row* source.height, source.width, source.height), Color.WHITE);
            }
        }
    }
    
    public static void DrawTiledImage(Image source,ref Image destination, int height, int width, int paddingTop, int paddingBtm, int paddingSide)
    {
        var totalColumns = (width - 2 * paddingSide) / source.width + 1;
        var totalRows = (height - paddingTop - paddingBtm) / source.height + 1;
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalColumns; col++)
            {
                var rectS = new Rectangle(0, 0, Math.Min(width - 2 * paddingSide - col * source.width, source.width),
                    Math.Min(height - paddingBtm - paddingTop - row * source.height, source.height));
                Raylib.ImageDraw(ref destination, source, rectS, 
                    new Rectangle(col* source.width, row* source.height, rectS.width, rectS.height), Color.WHITE);
            }
        }
    }

    /// <summary>
    /// Paint base screen of a dialog
    /// </summary>
    /// <param name="x"></param> X-location of dialog on game screen
    /// <param name="y"></param> Y-location of dialog on game screen
    /// <param name="w"></param> Dialog width
    /// <param name="h"></param> Dialog height
    public static void PaintDialogBase(int x, int y, int w, int h, Padding padding)
    {
        // Outer wallpaper
        Raylib.DrawTextureTiled(OuterWallpaperTexture, new Rectangle(0, 0, OuterWallpaperTexture.width, OuterWallpaperTexture.height), new Rectangle(x, y, w, h), new Vector2(0, 0), 0.0f, 1.0f, Color.WHITE);

        // Outer border
        var color1 = new Color(227, 227, 227, 255);
        var color2 = new Color(105, 105, 105, 255);
        var color3 = new Color(255, 255, 255, 255);
        var color4 = new Color(160, 160, 160, 255);
        var color5 = new Color(240, 240, 240, 255);
        var color6 = new Color(223, 223, 223, 255);
        var color7 = new Color(63, 63, 63, 255);
        Raylib.DrawLine(x, y, x + w - 1, y, color1);    // 1st layer of border
        Raylib.DrawLine(x + 1, y + 1, x + 1, y + h - 1, color1);
        Raylib.DrawLine(x + w, y, x + w, y + h - 1, color2);
        Raylib.DrawLine(x, y + h - 1, x + w, y + h - 1, color2);
        Raylib.DrawLine(x + 1, y + 1, x + w - 2, y + 1, color3);    // 2nd layer of border
        Raylib.DrawLine(x + 2, y + 1, x + 2, y + h - 2, color3);
        Raylib.DrawLine(x + w - 1, y + 1, x + w - 1, y + h - 1, color4);
        Raylib.DrawLine(x + 1, y + h - 2, x + w - 1, y + h - 2, color4);
        Raylib.DrawLine(x + 2, y + 2, x + w - 3, y + 2, color5);    // 3rd layer of border
        Raylib.DrawLine(x + 3, y + 2, x + 3, y + h - 3, color5);
        Raylib.DrawLine(x + w - 2, y + 2, x + w - 2, y + h - 2, color5);
        Raylib.DrawLine(x + 2, y + h - 3, x + w - 2, y + h - 3, color5);
        Raylib.DrawLine(x + 3, y + 3, x + w - 4, y + 3, color6);    // 4th layer of border
        Raylib.DrawLine(x + 4, y + 3, x + 4, y + h - 3, color6);
        Raylib.DrawLine(x + w - 3, y + 3, x + w - 3, y + h - 3, color7);
        Raylib.DrawLine(x + 4, y + h - 4, x + w - 4, y + h - 4, color7);
        Raylib.DrawLine(x + 4, y + 4, x + w - 6, y + 4, color6);    // 5th layer of border
        Raylib.DrawLine(x + 5, y + 4, x + 5, y + h - 4, color6);
        Raylib.DrawLine(x + w - 4, y + 4, x + w - 4, y + h - 4, color7);

        // Inner border
        Raylib.DrawLine(x + padding.L - 2, y + padding.T - 2, x + w - padding.R + 1, y + padding.T - 2, color7);    // 1st layer of border
        Raylib.DrawLine(x + padding.L - 1, y + padding.T - 1, x + padding.L - 1, y + h - padding.B + 1, color7);
        Raylib.DrawLine(x + w - padding.R + 2, y + padding.T - 2, x + w - padding.R + 2, y + h - padding.B + 1, color6);
        Raylib.DrawLine(x + padding.L - 2, y + h - padding.B + 1, x + w - padding.R + 2, y + h - padding.B + 1, color6);
        Raylib.DrawLine(x + padding.L - 2, y + padding.T - 1, x + w - padding.R, y + padding.T - 1, color7);    // 2nd layer of border
        Raylib.DrawLine(x + padding.L, y + padding.T - 1, x + padding.L, y + h - padding.B, color7);
        Raylib.DrawLine(x + w - padding.R + 1, y + padding.T - 1, x + w - padding.R + 1, y + h - padding.B + 1, color6);
        Raylib.DrawLine(x + padding.L - 1, y + h - padding.B, x + w - padding.R + 2, y + h - padding.B, color6);

        // Inner wallpaper
        Raylib.DrawTextureTiled(InnerWallpaperTexture, new Rectangle(0, 0, InnerWallpaperTexture.width, InnerWallpaperTexture.height), new Rectangle(x + padding.R, y + padding.T, w - padding.R - padding.L, h - padding.T - padding.B), new Vector2(0, 0), 0.0f, 1.0f, Color.WHITE);
    }

    public static void PaintRadioButton(int x, int y, bool isSelected)
    {
        Raylib.DrawCircle(x + 8, y + 8, 8.0f, new Color(128, 128, 128, 255));
        Raylib.DrawCircleLines(x + 8 + 1, y + 8 + 1, 8.0f, Color.BLACK);
        Raylib.DrawRectangle(x + 1, y + 4, 2, 3, Color.BLACK);
        Raylib.DrawRectangle(x + 3, y + 2, 2, 2, Color.BLACK);
        Raylib.DrawRectangle(x + 6, y + 1, 1, 1, Color.BLACK);
        Raylib.DrawRectangle(x + 11, y + 15, 3, 2, Color.BLACK);
        Raylib.DrawRectangle(x + 14, y + 13, 2, 2, Color.BLACK);
        Raylib.DrawRectangle(x + 16, y + 11, 1, 1, Color.BLACK);
        Raylib.DrawCircleLines(x + 8, y + 8, 8.0f, Color.WHITE);

        if (!isSelected)
        {
            Raylib.DrawRectangle(x + 6, y + 4, 5, 9, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 4, y + 6, 9, 5, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 5, y + 11, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 4, y + 6, 1, 5, Color.WHITE);
            Raylib.DrawRectangle(x + 5, y + 5, 1, 2, Color.WHITE);
            Raylib.DrawRectangle(x + 6, y + 4, 1, 2, Color.WHITE);
            Raylib.DrawRectangle(x + 7, y + 4, 4, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 11, y + 5, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 11, y + 11, 1, 1, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 7, y + 13, 4, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 11, y + 12, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 12, y + 11, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(x + 13, y + 7, 1, 4, Color.WHITE);
        }
        else
        {
            Raylib.DrawRectangle(x + 7, y + 4, 4, 10, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 4, y + 7, 10, 4, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 6, y + 5, 6, 8, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 5, y + 6, 8, 6, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(x + 7, y + 6, 4, 6, Color.BLACK);
            Raylib.DrawRectangle(x + 6, y + 7, 6, 4, Color.BLACK);
        }
    }

    public static void SetInner(IImageSource inner)
    {
        InnerWallpaper = Images.ExtractBitmap(inner);
    }

    public static Image InnerWallpaper { get; set; }

    public static void SetOuter(IImageSource outer)
    {
        
        OuterWallpaper = Images.ExtractBitmap(outer);
    }

    public static Image OuterWallpaper { get; set; }

    public static void SetInnerTexture()
    {
        InnerWallpaperTexture = Raylib.LoadTextureFromImage(InnerWallpaper);
    }

    public static void SetOuterTexture()
    {
        OuterWallpaperTexture = Raylib.LoadTextureFromImage(OuterWallpaper);
    }

    public static Texture2D InnerWallpaperTexture { get; set; }
    public static Texture2D OuterWallpaperTexture { get; set; }
}