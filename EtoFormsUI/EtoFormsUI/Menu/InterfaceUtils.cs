using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using EtoFormsUI.Bitmaps.ImageLoader;

namespace EtoFormsUI.Menu;

public static class InterfaceUtils
{
    public static void DrawTiledImage(Bitmap image, Graphics graphics, int height, int width)
    {  
        var imgSize = image.Size;
        for (int row = 0; row < height / imgSize.Height + 1; row++)
        {
            for (int col = 0; col < width / imgSize.Width + 1; col++)
            {
                graphics.DrawImage(image, col * imgSize.Width, row * imgSize.Height);
            }
        }
    }
    
    public static void DrawTiledImage(Bitmap image, Graphics eGraphics, int height, int width, int paddingTop, int paddingBtm, int paddingSide)
    {
        var imgSize = image.Size;
        var totalColumns = (width - 2 * paddingSide) / imgSize.Width + 1;
            var totalRows = (height - paddingTop - paddingBtm) / imgSize.Height + 1;
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    var rectS = new Rectangle(0, 0, Math.Min(width - 2 * paddingSide - col * imgSize.Width, imgSize.Width),
                        Math.Min(height - paddingBtm - paddingTop - row * imgSize.Height, imgSize.Height));
                    eGraphics.DrawImage(image, rectS,
                        new Point(col * imgSize.Width + paddingSide, row * imgSize.Height + paddingTop));
                }
            }
    }
    
    // Import wallpapers for intro screen
    public static void ImportWallpapersFromIconsFile(string path)
    {
        using var icons = Common.LoadBitmapFrom("ICONS", path);
        PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
        PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
    }

    private static Bitmap PanelInnerWallpaper { get; set; }

    private static Bitmap PanelOuterWallpaper { get; set; }

    public static void DrawInnerWallpaper(Graphics graphics, int height, int width)
    {  
        var imgSize = PanelInnerWallpaper.Size;
        for (int row = 0; row < height / imgSize.Height + 1; row++)
        {
            for (int col = 0; col < width / imgSize.Width + 1; col++)
            {
                graphics.DrawImage(PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
            }
        }
    }

    public static void DrawOuterWallpaper(Graphics eGraphics, int height, int width)
    {
        var imgSize = PanelOuterWallpaper.Size;
        for (int row = 0; row < height / imgSize.Height + 1; row++)
        {
            for (int col = 0; col < width / imgSize.Width + 1; col++)
            {
                eGraphics.DrawImage(PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
            }
        }
    }

    public static void DrawInnerWallpaper(Graphics eGraphics, int height, int width, int paddingTop, int paddingBtm, int paddingSide)
    {
        var imgSize = PanelInnerWallpaper.Size;
        var totalColumns = (width - 2 * paddingSide) / imgSize.Width + 1;
            var totalRows = (height - paddingTop - paddingBtm) / imgSize.Height + 1;
            for (int row = 0; row < totalRows; row++)
            {
                for (int col = 0; col < totalColumns; col++)
                {
                    var rectS = new Rectangle(0, 0, Math.Min(width - 2 * paddingSide - col * imgSize.Width, imgSize.Width),
                        Math.Min(height - paddingBtm - paddingTop - row * imgSize.Height, imgSize.Height));
                    eGraphics.DrawImage(PanelInnerWallpaper, rectS,
                        new Point(col * imgSize.Width + paddingSide, row * imgSize.Height + paddingTop));
                }
            }
    }
}