using Model.Images;
using Raylib_cs;

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

    public static void SetInner(IImageSource inner)
    {
        InnerWalpaper = Images.ExtractBitmap(inner);
    }

    public static Image InnerWalpaper { get; set; }

    public static void SetOuter(IImageSource outer)
    {
        
        OuterWallpaper = Images.ExtractBitmap(outer);
    }

    public static Image OuterWallpaper { get; set; }
}