using System;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI.Bitmaps.ImageLoader;

namespace EtoFormsUI.Menu;

public abstract class InterfaceStyle
{
    private readonly string _path;

    protected InterfaceStyle(string title, string path)
    {
        _path = path;
        this.Title = title;
        
        ImportWallpapersFromIconsFile(_path);
    }

    public string Title { get; init; }

    public static InterfaceStyle? GetMenuImageSet(string path)
    {
        var gameTxt = Path.Combine(path, "Game.txt");
        if (!File.Exists(gameTxt)) return null;
        
        var title = File.ReadLines(gameTxt).Where(l => l.StartsWith("@title")).Select(l => l.Split("=", 2)[1])
            .FirstOrDefault();
        return title switch
        {
            "Civilization II Multiplayer Gold" => new Civ2GoldMenu(path),
            "Test of Time" => new TestOfTimeMenu(path),
            _ => null
        };
    }
    
    // Import wallpapers for intro screen
    private void ImportWallpapersFromIconsFile(string path)
    {
        using var icons = Common.LoadBitmapFrom("ICONS", path);
        this.PanelOuterWallpaper = icons.Clone(new Rectangle(199, 322, 64, 32));
        this.PanelInnerWallpaper = icons.Clone(new Rectangle(298, 190, 32, 32));
    }

    private Bitmap PanelInnerWallpaper { get; set; }

    private Bitmap PanelOuterWallpaper { get; set; }

    public void DrawInnerWallpaper(Graphics graphics, int height, int width)
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

    public void DrawOuterWallpaper(Graphics eGraphics, int height, int width)
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

    public void DrawInnerWallpaper(Graphics eGraphics, int height, int width, int paddingTop, int paddingBtm, int paddingSide)
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

    public abstract void DrawIntroScreen(PixelLayout pixelLayout);
    public abstract void ShowMainMenuDecoration(PixelLayout layout);
    public abstract void ClearMainMenuDecoration();
}