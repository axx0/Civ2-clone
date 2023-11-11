using System.Drawing;
using System.Security.AccessControl;
using Model.Images;
using Raylib_cs;
using System.Numerics;
using Civ2engine;
using Model;
using Color = Raylib_cs.Color;
using Font = Raylib_cs.Font;
using Image = Raylib_cs.Image;
using Rectangle = Raylib_cs.Rectangle;

namespace RaylibUI;

public class ImageUtils
{
    private static Image _innerWallpaper;
    private static Image _outerWallpaper;

    public static void PaintPanelBorders(ref Image image, int width, int height, int paddingTop, int paddingBtm)
    {
        DrawBorderImage(OuterWallpaper, ref image, height, width, paddingTop, paddingBtm);
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

    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source">The source image to tile onto the border</param>
    /// <param name="destination">the Image we're rendering too</param>
    /// <param name="height">final image height</param>
    /// <param name="width">final image width</param>
    /// <param name="topWidth">width of top border</param>
    /// <param name="footerWidth">width of the footer</param>
    public static void DrawBorderImage(Image source,ref Image destination, int height, int width, int topWidth, int footerWidth)
    {  
        int rows = height / source.height + 1;
        var columns = width / source.width + 1;
        var headerSourceRec = new Rectangle { height = topWidth, width = source.width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, source, headerSourceRec,
                new Rectangle(col * source.width, 0, source.width, topWidth),
                Color.WHITE);
        }
        var leftSide = new Rectangle { height = source.height, width = 11 };

        var rightEdge = width - 11;
        var rightOffset = width % source.width;
        var rightSide = new Rectangle { x = rightOffset, height = source.height, width = 11 };
        
        for (int row = 0; row < rows; row++)
        {
            Raylib.ImageDraw(ref destination, source, leftSide,
                new Rectangle(0, row * source.height, 11, source.height),
                Color.WHITE);
            Raylib.ImageDraw(ref destination, source, rightSide,
                new Rectangle(rightEdge, row * source.height, 11, source.height),
                Color.WHITE);
        }

        var bottomEdge = height - footerWidth;
        var bottomOffset = height % source.height;
        var bottomSource = new Rectangle { y = bottomOffset, height = footerWidth, width = source.width };
        for (int col = 0; col < columns; col++)
        {
            Raylib.ImageDraw(ref destination, source, bottomSource,
                new Rectangle(col * source.width, bottomEdge, source.width, footerWidth),
                Color.WHITE);
        }
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
        var totalColumns = (width - 2 * paddingSide) / source.width;
        var totalRows = (height - paddingTop - paddingBtm) / source.height;
        
        var rightEdge = paddingSide + (source.width * totalColumns);
        var rightWidth = width - paddingSide - rightEdge;
        var rightSrcRect = new Rectangle { height = source.height, width = rightWidth };

        var srcRec = new Rectangle { height = source.height, width = source.width };
        
        for (int row = 0; row < totalRows; row++)
        {
            var rowPos = source.height * row + paddingTop;
            for (int col = 0; col < totalColumns ; col++)
            {
                Raylib.ImageDraw(ref destination, source, srcRec,
                    new Rectangle(col * source.width + paddingSide, rowPos, source.width, source.height),
                    Color.WHITE);
            }
            Raylib.ImageDraw(ref destination, source, rightSrcRect,
                new Rectangle(rightEdge, rowPos, rightWidth, source.height),
                Color.WHITE);
        }

        var bottomEdge = paddingTop + totalRows * source.height;
        var bottomWidth = height - paddingBtm - bottomEdge;
        if (bottomWidth > 0)
        {
            var bottomSourceRect = new Rectangle { height = bottomWidth, width = source.width };
            for (int col = 0; col < totalColumns; col++)
            {
                Raylib.ImageDraw(ref destination, source, bottomSourceRect,
                    new Rectangle(col * source.width + paddingSide, bottomEdge, source.width, bottomWidth),
                    Color.WHITE);
            }
            
            Raylib.ImageDraw(ref destination, source, new Rectangle { height = bottomWidth, width = rightWidth},
                new Rectangle(rightEdge, bottomEdge, rightWidth, bottomWidth),
                Color.WHITE);
        }
    }

    public static Image NewImage(int width, int h)
    {
        var image = Raylib.LoadImage("blank.png");
        
        Raylib.ImageResize(ref image, width, h);
        return image;
    }

    /// <summary>
    /// Paint base screen of a dialog
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="top"></param>
    /// <param name="paddingBtm"></param>
    /// <param name="paddingSide"></param>
    /// <param name="image1"></param>
    /// Dialog width
    /// Dialog height
    public static Texture2D? PaintDialogBase(int width, int height, int top, int paddingBtm, int paddingSide, Image? centerImage = null)
    {
        // Outer wallpaper
        var image = NewImage(width, height);
        PaintPanelBorders(ref image,width,height,top, paddingBtm);
        if (centerImage != null)
        {
            var innerWidth =Math.Min(width - 2*paddingSide, centerImage.Value.width);
            var innerHeight = Math.Min(height - top - paddingBtm, centerImage.Value.height);
            Raylib.ImageDraw( ref image, centerImage.Value, new Rectangle(0,0,innerWidth,innerHeight), new Rectangle(paddingSide, top, innerWidth,innerHeight), Color.WHITE);
        }
        else
        {
            DrawTiledImage(InnerWallpaper, ref image, height, width, top, paddingBtm, paddingSide);
        }

        return Raylib.LoadTextureFromImage(image);
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

    public static void PaintCheckbox(int x, int y, bool isChecked)
    {
        Raylib.DrawRectangle(x + 3, y + 2, 15, 17, Color.WHITE);
        Raylib.DrawRectangle(x + 2, y + 3, 17, 15, Color.WHITE);
        Raylib.DrawRectangle(x + 4, y + 3, 13, 15, new Color(128, 128, 128, 255));
        Raylib.DrawRectangle(x + 3, y + 4, 15, 13, new Color(128, 128, 128, 255));
        Raylib.DrawLine(x + 4, y + 3, x + 16, y + 3, Color.BLACK);
        Raylib.DrawLine(x + 3, y + 4, x + 3, y + 16, Color.BLACK);
        Raylib.DrawLine(x + 3, y + 4, x + 4, y + 4, Color.BLACK);
        Raylib.DrawLine(x + 4, y + 19, x + 18, y + 19, Color.BLACK);
        Raylib.DrawLine(x + 18, y + 18, x + 19, y + 18, Color.BLACK);
        Raylib.DrawLine(x + 19, y + 4, x + 19, y + 17, Color.BLACK);

        if (isChecked)
        {
            Raylib.DrawLine(x + 21, y + 3, x + 25, y + 3, Color.BLACK);
            Raylib.DrawLine(x + 20, y + 4, x + 23, y + 4, Color.BLACK);
            Raylib.DrawLine(x + 19, y + 5, x + 21, y + 5, Color.BLACK);
            Raylib.DrawLine(x + 18, y + 6, x + 20, y + 6, Color.BLACK);
            Raylib.DrawLine(x + 17, y + 7, x + 19, y + 7, Color.BLACK);
            Raylib.DrawLine(x + 16, y + 8, x + 18, y + 8, Color.BLACK);
            Raylib.DrawLine(x + 15, y + 9, x + 17, y + 9, Color.BLACK);
            Raylib.DrawLine(x + 5, y + 10, x + 6, y + 10, Color.BLACK);
            Raylib.DrawLine(x + 14, y + 10, x + 16, y + 10, Color.BLACK);
            Raylib.DrawLine(x + 6, y + 11, x + 7, y + 11, Color.BLACK);
            Raylib.DrawLine(x + 14, y + 11, x + 16, y + 11, Color.BLACK);
            Raylib.DrawLine(x + 7, y + 12, x + 8, y + 12, Color.BLACK);
            Raylib.DrawLine(x + 13, y + 12, x + 15, y + 12, Color.BLACK);
            Raylib.DrawLine(x + 8, y + 13, x + 14, y + 13, Color.BLACK);
            Raylib.DrawLine(x + 12, y + 13, x + 15, y + 13, Color.BLACK);
            Raylib.DrawLine(x + 12, y + 14, x + 14, y + 14, Color.BLACK);
            Raylib.DrawLine(x + 9, y + 15, x + 12, y + 15, Color.BLACK);
            Raylib.DrawLine(x + 10, y + 16, x + 12, y + 16, Color.BLACK);
            Raylib.DrawLine(x + 11, y + 16, x + 11, y + 17, Color.BLACK);
            Raylib.DrawLine(x + 20, y + 1, x + 22, y + 1, Color.WHITE);
            Raylib.DrawLine(x + 19, y + 2, x + 20, y + 2, Color.WHITE);
            Raylib.DrawLine(x + 20, y + 2, x + 22, y + 2, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 18, y + 3, x + 19, y + 3, Color.WHITE);
            Raylib.DrawLine(x + 19, y + 3, x + 20, y + 3, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 17, y + 4, x + 18, y + 4, Color.WHITE);
            Raylib.DrawLine(x + 18, y + 4, x + 19, y + 4, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 16, y + 5, x + 17, y + 5, Color.WHITE);
            Raylib.DrawLine(x + 17, y + 5, x + 18, y + 5, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 15, y + 6, x + 16, y + 6, Color.WHITE);
            Raylib.DrawLine(x + 16, y + 6, x + 17, y + 6, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 14, y + 7, x + 15, y + 7, Color.WHITE);
            Raylib.DrawLine(x + 15, y + 7, x + 16, y + 7, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 4, y + 8, x + 5, y + 8, Color.WHITE);
            Raylib.DrawLine(x + 5, y + 8, x + 5, y + 9, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 13, y + 8, x + 14, y + 8, Color.WHITE);
            Raylib.DrawLine(x + 14, y + 8, x + 15, y + 8, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 6, y + 9, x + 6, y + 10, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 13, y + 9, x + 14, y + 9, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 7, y + 10, x + 7, y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 12, y + 10, x + 13, y + 10, Color.WHITE);
            Raylib.DrawLine(x + 13, y + 10, x + 13, y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 8, y + 11, x + 8, y + 12, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 11, y + 11, x + 12, y + 11, Color.WHITE);
            Raylib.DrawLine(x + 12, y + 11, x + 13, y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 9, y + 12, x + 9, y + 13, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 11, y + 12, x + 12, y + 12, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 9, y + 13, x + 11, y + 13, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 9, y + 14, x + 11, y + 14, new Color(192, 192, 192, 255));
            Raylib.DrawLine(x + 10, y + 14, x + 10, y + 15, new Color(192, 192, 192, 255));
        }
    }

    public static void SetInner(IImageSource inner)
    {
        InnerWallpaper = Images.ExtractBitmap(inner);
    }

    public static Image InnerWallpaper
    {
        get => _innerWallpaper;
        set
        {
            _innerWallpaper = value;
            InnerWallpaperTexture = Raylib.LoadTextureFromImage(value);
        }
    }

    public static void SetOuter(IImageSource outer)
    {
        OuterWallpaper = Images.ExtractBitmap(outer);
    }

    public static Image OuterWallpaper
    {
        get => _outerWallpaper;
        set
        {
            _outerWallpaper = value;
            OuterWallpaperTexture = Raylib.LoadTextureFromImage(value);
        }
    }

    public static Texture2D InnerWallpaperTexture { get; private set; }
    public static Texture2D OuterWallpaperTexture { get; private set; }

    public static Texture2D[] GetOptionImages(bool checkbox)
    {
        var images = checkbox ? Look.CheckBoxes : Look.RadioButtons;
        return images.Select(Images.ExtractBitmap).Select(Raylib.LoadTextureFromImage).ToArray();
        // unsafe
        // {
        //     var image = NewImage(64, 64);
        //     var x = 16;
        //     var y = 14;
        //     Raylib.ImageDrawCircle(ref image, x + 16, y + 16, 16, new Color(128, 128, 128, 255));
        //     Raylib.ImageDrawCircleLines(&image, x + 18, y + 18, 16, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 2, y + 8, 4, 6, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 6, y + 4, 4, 4, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 12, y + 2, 2, 2, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 22, y + 30, 6, 4, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 28, y + 26, 4, 4, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, x + 32, y + 22, 2, 2, Color.BLACK);
        //     Raylib.ImageDrawCircleLines(&image, x + 16, y + 16, 16, Color.WHITE);
        //
        //     var unselected = Raylib.ImageCopy(image);
        //     
        //         Raylib.ImageDrawRectangle(ref image,x + 6, y + 4, 5, 9, new Color( 192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref image, x + 4, y + 6, 9, 5, new Color(192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref image, x + 5, y + 11, 1, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 4, y + 6, 1, 5, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 5, y + 5, 1, 2, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 6, y + 4, 1, 2, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 7, y + 4, 4, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 11, y + 5, 1, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image,x + 11, y + 11, 1, 1, new Color( 192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref image, x + 7, y + 13, 4, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 11, y + 12, 1, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 12, y + 11, 1, 1, Color.WHITE);
        //         Raylib.ImageDrawRectangle(ref image, x + 13, y + 7, 1, 4, Color.WHITE);
        //
        //         Raylib.ImageDrawRectangle(ref unselected, x + 7, y + 4, 4, 10, new Color( 192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref unselected,x + 4, y + 7, 10, 4, new Color( 192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref unselected,x + 6, y + 5, 6, 8, new Color(192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref unselected,x + 5, y + 6, 8, 6, new Color(192, 192, 192, 255));
        //         Raylib.ImageDrawRectangle(ref unselected, x + 7, y + 6, 4, 6, Color.BLACK);
        //         Raylib.ImageDrawRectangle(ref unselected, x + 6, y + 7, 6, 4, Color.BLACK);
        //         return new[] { new TextureDetail(0.5f,Raylib.LoadTextureFromImage(unselected)), new TextureDetail(0.5f,Raylib.LoadTextureFromImage(image)) };
        // }

        // unsafe
        // {
        //     int y = 0;
        //
        //     var image = Raylib.LoadImage("blank.png");
        //     Raylib.ImageResize(ref image, 38, 38);
        //     Raylib.ImageDrawCircle(ref image, 18, 8, 8, new Color(128, 128, 128, 255));
        //     Raylib.ImageDrawCircleLines(&image, 8, 8 + y + 8 + 1, 8, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 1, 4, 2, 3, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 3, 2, 2, 2, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 6, 1, 1, 1, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 11, 15, 3, 2, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 14, 13, 2, 2, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref image, 16, 11, 1, 1, Color.BLACK);
        //     //Raylib.DrawCircleLines(8, 8, 8.0f, Color.WHITE);
        //
        //     var unselected = Raylib.ImageCopy(image);
        //
        //     Raylib.ImageDrawRectangle(ref image, 6, 4, 5, 9, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref image, 4, 6, 9, 5, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref image, 5, 11, 1, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 4, 6, 1, 5, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 5, 5, 1, 2, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 6, 4, 1, 2, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 7, 4, 4, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 11, 5, 1, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 11, 11, 1, 1, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref image, 7, 13, 4, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 11, 12, 1, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 12, 11, 1, 1, Color.WHITE);
        //     Raylib.ImageDrawRectangle(ref image, 13, 7, 1, 4, Color.WHITE);
        //
        //     Raylib.ImageDrawRectangle(ref unselected, 7, 4, 4, 10, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref unselected, 4, 7, 10, 4, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref unselected, 6, 5, 6, 8, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref unselected, 5, 6, 8, 6, new Color(192, 192, 192, 255));
        //     Raylib.ImageDrawRectangle(ref unselected, 7, 6, 4, 6, Color.BLACK);
        //     Raylib.ImageDrawRectangle(ref unselected, 6, 7, 6, 4, Color.BLACK);
        //
        //
        //     
        // }
    }
    
    

    public static void SetLook(InterfaceStyle look)
    {
        ImageUtils.SetInner(look.Inner);
        ImageUtils.SetOuter(look.Outer);

        Look = look;
        var fontPath = Utils.GetFilePath(look.DefaultFont);
        Fonts.SetFont(Raylib.LoadFont(fontPath));
        var bold = Utils.GetFilePath(look.BoldFont);
        Fonts.SetBold(Raylib.LoadFontEx(bold, 104, null, 0));
        var alternative = Utils.GetFilePath(look.AlternativeFont);
        Fonts.SetAlt(Raylib.LoadFont(alternative));
    }

    private static InterfaceStyle Look;

    public static Image[] GetScrollImages(int dim)
    {
        var image = NewImage(dim, dim);
        var color1 = new Color(227, 227, 227, 255);
        Raylib.ImageDrawLine(ref image, 0,0, image.width -1, 0, color1);
        Raylib.ImageDrawLine(ref image, 0,0, 0, image.height -1, color1);
        
        var color2 = Color.WHITE;
        Raylib.ImageDrawLine(ref image, 1,1, image.width -2, 1, color2);
        Raylib.ImageDrawLine(ref image, 1,1, 1, image.height -2, color2);

        var color3 = new Color(240, 240, 240, 255);
        Raylib.ImageDrawRectangle(ref image, 3,3,image.width -4, image.height - 4, color3);

        var color4 = new Color(160, 160, 160, 255);
        Raylib.ImageDrawLine(ref image, image.width -2,1, image.width -2, image.height -2, color4);
        Raylib.ImageDrawLine(ref image, 1, image.height -2, image.width-2, image.height -2, color4);
        
        var color5 =new Color(105,105,105,255);
        Raylib.ImageDrawLine(ref image, 0, image.height -1, image.width -1, image.height -1, color5);
        Raylib.ImageDrawLine(ref image, image.width -1, 0, image.width -1, image.height -1, color5);

        var left = Raylib.ImageFromImage(image, new Rectangle(0,0,dim, dim));
        Raylib.ImageDrawPixel(ref left, 6,9,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 7,8,7,10,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 8,7,8,11,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 9,6,9,12,Color.BLACK);
        var right = Raylib.ImageFromImage(image, new Rectangle(0,0,dim, dim));
        Raylib.ImageDrawPixel(ref right, dim - 6,9,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-7,8,dim-7,10,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-8,7,dim-8,11,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-9,6,dim-9,12,Color.BLACK);
        return new[] { left, image, right };
    }
}

public static class Fonts
{
    public static Font DefaultFont = Raylib.GetFontDefault();
    public static Font BoldFont = Raylib.GetFontDefault();
    public static Font AlternativeFont = Raylib.GetFontDefault();

    public const int FontSize = 20;
    public static void SetFont(Font font)
    {
        DefaultFont = font;
    }

    public static void SetAlt(Font font)
    {
        AlternativeFont = font;
    }

    public static void SetBold(Font font)
    {
        BoldFont = font;
    }

}