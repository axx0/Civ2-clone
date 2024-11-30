using Model.Images;
using System.Numerics;
using Civ2engine;
using Civ2engine.Units;
using Civ2engine.Enums;
using Raylib_CSharp.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Transformations;
using Raylib_CSharp.Textures;
using Raylib_CSharp.Rendering;
using Model;
using Model.Core;
using Model.ImageSets;
using Model.Interface;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;
using RaylibUtils;

namespace RaylibUI;

public static class ImageUtils
{
    private static Image _innerWallpaper;
    private static Image _outerWallpaper;
    private static Image _outerTitleTopWallpaper;

    public static void PaintPanelBorders(IUserInterface? active, ref Image image, int width, int height, Padding padding, bool statusPanel = false, bool toTStatusPanelLayout = false)
    {
        if (active == null)
        {
            image.DrawRectangle(0, 0, width, height, Color.Gray);
        }
        else
        {
            active.DrawBorderWallpaper(Wallpaper, ref image, height, width, padding, statusPanel && !toTStatusPanelLayout);
            active.DrawBorderLines(ref image, height, width, padding, statusPanel);
        }
    }

    /// <summary>
    /// Draw tiles within a rectangle (chose tiles randomly)
    /// </summary>
    /// <param name="tiles">Wallpaper tile images</param>
    /// <param name="dest">Destination image</param>
    /// <param name="rect">Rectangle within the image where tiles are to be drawn</param>
    public static void DrawTiledRectangle(Image[] tiles, ref Image dest, Rectangle rect)
    {
        var rnd = new Random();
        var len = tiles.Length;

        var totalColumns = Math.Ceiling(rect.Width / tiles[0].Width);
        var totalRows = Math.Ceiling(rect.Height / tiles[0].Height);

        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < totalColumns; col++)
            {
                var srcRec = new Rectangle { Height = tiles[0].Height, Width = tiles[0].Width };
                var destRec = new Rectangle(rect.X + col * tiles[0].Width, rect.Y + row * tiles[0].Height, tiles[0].Width, tiles[0].Height);

                if (col == totalColumns - 1)
                {
                    srcRec.Width = rect.Width - tiles[0].Width * col;
                    destRec.Width = srcRec.Width;
                }

                if (row == totalRows - 1)
                {
                    srcRec.Height = rect.Height - tiles[0].Height * row;
                    destRec.Height = srcRec.Height;
                }

                dest.Draw(tiles[rnd.Next(len)], srcRec, destRec, Color.White);
            }
        }
    }


    public static void DrawTiledImage(Wallpaper wp, ref Image destination, int height, int width, Padding padding, bool statusPanel = false, bool ToTStatusPanelLayout = false)
    {
        // MGE uses inner wallpaper from ICONS for all dialogs
        // TOT uses inner wallpaper from ICONS only for status panel, otherwise uses tiles from dialog image file
        var tiles = wp != null ? (statusPanel && wp.InnerAlt.Width > 0 ? new[] { wp.InnerAlt } : wp.Inner) : new[] { InnerWallpaper };

        if (!statusPanel)
        {
            DrawTiledRectangle(tiles, ref destination,
                new Rectangle(padding.Left, padding.Top, width - padding.Left - padding.Right, height - padding.Top - padding.Bottom));
        }
        else
        {
            if (ToTStatusPanelLayout)
            {
                DrawTiledRectangle(tiles, ref destination,
                    new Rectangle(padding.Left, padding.Top, 0.25f * width - padding.Left - padding.Right, height - padding.Top - padding.Bottom));
                DrawTiledRectangle(tiles, ref destination,
                    new Rectangle(padding.Left + (0.25f * width - padding.Left - padding.Right) + 8, padding.Top, width - 0.25f * width - 8, height - padding.Top - padding.Bottom));
            }
            else
            {
                DrawTiledRectangle(tiles, ref destination,
                    new Rectangle(padding.Left, padding.Top, width - padding.Left - padding.Right, 60));
                DrawTiledRectangle(tiles, ref destination,
                    new Rectangle(padding.Left, padding.Top + 68, width - padding.Left - padding.Right, height - padding.Top - padding.Bottom - 68));
            }
        }
    }

    /// <summary>
    /// Paint base screen of a dialog
    /// </summary>
    /// <param name="width">Width of dialog</param>
    /// <param name="height"></param>
    /// <param name="padding"></param>
    /// <param name="centerImage">Image to place in centre of dialog</param>
    /// <param name="noWallpaper">true to not draw inner wallpaper defaults to false</param>
    /// <param name="statusPanel">true to draw status panel-style background</param>
    public static Texture2D? PaintDialogBase(IUserInterface active, int width, int height, Padding padding, Image? centerImage = null, bool noWallpaper = false, bool statusPanel = false, bool ToTStatusPanelLayout = false)
    {
        // Outer wallpaper
        var image = Image.GenColor(width, height, new Color(0, 0, 0, 0));
        PaintPanelBorders(active, ref image, width, height, padding, statusPanel: statusPanel, toTStatusPanelLayout: ToTStatusPanelLayout);
        if (centerImage != null)
        {
            var innerWidth = Math.Min(width - padding.Left - padding.Right, centerImage.Value.Width);
            var innerHeight = Math.Min(height - padding.Top - padding.Bottom, centerImage.Value.Height);
            image.Draw(centerImage.Value, new Rectangle(0, 0, innerWidth, innerHeight), new Rectangle(padding.Left, padding.Top, innerWidth, innerHeight), Color.White);
        }
        else if (!noWallpaper)
        {
            DrawTiledImage(Wallpaper, ref image, height, width, padding, statusPanel: statusPanel, ToTStatusPanelLayout: ToTStatusPanelLayout);
        }

        return Texture2D.LoadFromImage(image);
    }

    public static Texture2D PaintButtonBase(int width, int height)
    {
        var rnd = new Random();
        var btn = Wallpaper?.Button ?? new[] { Image.GenColor(width, height, new Color(192, 192, 192, 255)) };
        var len = btn.Length - 2;  // variations of inner texture
        var cols = Math.Ceiling(width / (double)btn[0].Width);

        var image = Image.GenColor(width, height, new Color(0, 0, 0, 0));
        image.Draw(btn[0], new Rectangle(0, 0, btn[0].Width, btn[0].Height), new Rectangle(0, 0, btn[0].Width, btn[0].Height), Color.White);
        for (int col = 1; col < cols - 1; col++)
        {
            image.Draw(btn[rnd.Next(1, len + 1)], new Rectangle(0, 0, btn[0].Width, btn[0].Height), new Rectangle(btn[0].Width * col, 0, btn[0].Width, btn[0].Height), Color.White);
        }
        image.Draw(btn[^1], new Rectangle(0, 0, btn[0].Width, btn[0].Height), new Rectangle(width - btn[0].Width, 0, btn[0].Width, btn[0].Height), Color.White);
        return Texture2D.LoadFromImage(image);
    }

    public static void PaintRadioButton(int x, int y, bool isSelected)
    {
        Graphics.DrawCircle(x + 8, y + 8, 8.0f, new Color(128, 128, 128, 255));
        Graphics.DrawCircleLines(x + 8 + 1, y + 8 + 1, 8.0f, Color.Black);
        Graphics.DrawRectangle(x + 1, y + 4, 2, 3, Color.Black);
        Graphics.DrawRectangle(x + 3, y + 2, 2, 2, Color.Black);
        Graphics.DrawRectangle(x + 6, y + 1, 1, 1, Color.Black);
        Graphics.DrawRectangle(x + 11, y + 15, 3, 2, Color.Black);
        Graphics.DrawRectangle(x + 14, y + 13, 2, 2, Color.Black);
        Graphics.DrawRectangle(x + 16, y + 11, 1, 1, Color.Black);
        Graphics.DrawCircleLines(x + 8, y + 8, 8.0f, Color.White);

        if (!isSelected)
        {
            Graphics.DrawRectangle(x + 6, y + 4, 5, 9, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 4, y + 6, 9, 5, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 5, y + 11, 1, 1, Color.White);
            Graphics.DrawRectangle(x + 4, y + 6, 1, 5, Color.White);
            Graphics.DrawRectangle(x + 5, y + 5, 1, 2, Color.White);
            Graphics.DrawRectangle(x + 6, y + 4, 1, 2, Color.White);
            Graphics.DrawRectangle(x + 7, y + 4, 4, 1, Color.White);
            Graphics.DrawRectangle(x + 11, y + 5, 1, 1, Color.White);
            Graphics.DrawRectangle(x + 11, y + 11, 1, 1, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 7, y + 13, 4, 1, Color.White);
            Graphics.DrawRectangle(x + 11, y + 12, 1, 1, Color.White);
            Graphics.DrawRectangle(x + 12, y + 11, 1, 1, Color.White);
            Graphics.DrawRectangle(x + 13, y + 7, 1, 4, Color.White);
        }
        else
        {
            Graphics.DrawRectangle(x + 7, y + 4, 4, 10, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 4, y + 7, 10, 4, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 6, y + 5, 6, 8, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 5, y + 6, 8, 6, new Color(192, 192, 192, 255));
            Graphics.DrawRectangle(x + 7, y + 6, 4, 6, Color.Black);
            Graphics.DrawRectangle(x + 6, y + 7, 6, 4, Color.Black);
        }
    }

    public static void PaintCheckbox(int x, int y, bool isChecked)
    {
        Graphics.DrawRectangle(x + 3, y + 2, 15, 17, Color.White);
        Graphics.DrawRectangle(x + 2, y + 3, 17, 15, Color.White);
        Graphics.DrawRectangle(x + 4, y + 3, 13, 15, new Color(128, 128, 128, 255));
        Graphics.DrawRectangle(x + 3, y + 4, 15, 13, new Color(128, 128, 128, 255));
        Graphics.DrawLine(x + 4, y + 3, x + 16, y + 3, Color.Black);
        Graphics.DrawLine(x + 3, y + 4, x + 3, y + 16, Color.Black);
        Graphics.DrawLine(x + 3, y + 4, x + 4, y + 4, Color.Black);
        Graphics.DrawLine(x + 4, y + 19, x + 18, y + 19, Color.Black);
        Graphics.DrawLine(x + 18, y + 18, x + 19, y + 18, Color.Black);
        Graphics.DrawLine(x + 19, y + 4, x + 19, y + 17, Color.Black);

        if (isChecked)
        {
            Graphics.DrawLine(x + 21, y + 3, x + 25, y + 3, Color.Black);
            Graphics.DrawLine(x + 20, y + 4, x + 23, y + 4, Color.Black);
            Graphics.DrawLine(x + 19, y + 5, x + 21, y + 5, Color.Black);
            Graphics.DrawLine(x + 18, y + 6, x + 20, y + 6, Color.Black);
            Graphics.DrawLine(x + 17, y + 7, x + 19, y + 7, Color.Black);
            Graphics.DrawLine(x + 16, y + 8, x + 18, y + 8, Color.Black);
            Graphics.DrawLine(x + 15, y + 9, x + 17, y + 9, Color.Black);
            Graphics.DrawLine(x + 5, y + 10, x + 6, y + 10, Color.Black);
            Graphics.DrawLine(x + 14, y + 10, x + 16, y + 10, Color.Black);
            Graphics.DrawLine(x + 6, y + 11, x + 7, y + 11, Color.Black);
            Graphics.DrawLine(x + 14, y + 11, x + 16, y + 11, Color.Black);
            Graphics.DrawLine(x + 7, y + 12, x + 8, y + 12, Color.Black);
            Graphics.DrawLine(x + 13, y + 12, x + 15, y + 12, Color.Black);
            Graphics.DrawLine(x + 8, y + 13, x + 14, y + 13, Color.Black);
            Graphics.DrawLine(x + 12, y + 13, x + 15, y + 13, Color.Black);
            Graphics.DrawLine(x + 12, y + 14, x + 14, y + 14, Color.Black);
            Graphics.DrawLine(x + 9, y + 15, x + 12, y + 15, Color.Black);
            Graphics.DrawLine(x + 10, y + 16, x + 12, y + 16, Color.Black);
            Graphics.DrawLine(x + 11, y + 16, x + 11, y + 17, Color.Black);
            Graphics.DrawLine(x + 20, y + 1, x + 22, y + 1, Color.White);
            Graphics.DrawLine(x + 19, y + 2, x + 20, y + 2, Color.White);
            Graphics.DrawLine(x + 20, y + 2, x + 22, y + 2, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 18, y + 3, x + 19, y + 3, Color.White);
            Graphics.DrawLine(x + 19, y + 3, x + 20, y + 3, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 17, y + 4, x + 18, y + 4, Color.White);
            Graphics.DrawLine(x + 18, y + 4, x + 19, y + 4, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 16, y + 5, x + 17, y + 5, Color.White);
            Graphics.DrawLine(x + 17, y + 5, x + 18, y + 5, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 15, y + 6, x + 16, y + 6, Color.White);
            Graphics.DrawLine(x + 16, y + 6, x + 17, y + 6, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 14, y + 7, x + 15, y + 7, Color.White);
            Graphics.DrawLine(x + 15, y + 7, x + 16, y + 7, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 4, y + 8, x + 5, y + 8, Color.White);
            Graphics.DrawLine(x + 5, y + 8, x + 5, y + 9, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 13, y + 8, x + 14, y + 8, Color.White);
            Graphics.DrawLine(x + 14, y + 8, x + 15, y + 8, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 6, y + 9, x + 6, y + 10, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 13, y + 9, x + 14, y + 9, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 7, y + 10, x + 7, y + 11, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 12, y + 10, x + 13, y + 10, Color.White);
            Graphics.DrawLine(x + 13, y + 10, x + 13, y + 11, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 8, y + 11, x + 8, y + 12, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 11, y + 11, x + 12, y + 11, Color.White);
            Graphics.DrawLine(x + 12, y + 11, x + 13, y + 11, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 9, y + 12, x + 9, y + 13, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 11, y + 12, x + 12, y + 12, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 9, y + 13, x + 11, y + 13, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 9, y + 14, x + 11, y + 14, new Color(192, 192, 192, 255));
            Graphics.DrawLine(x + 10, y + 14, x + 10, y + 15, new Color(192, 192, 192, 255));
        }
    }

    public static Wallpaper? Wallpaper { get; set; }

    public static Image InnerWallpaper
    {
        get => _innerWallpaper;
        set
        {
            _innerWallpaper = value;
            InnerWallpaperTexture = Texture2D.LoadFromImage(value);
        }
    }

    public static Image OuterWallpaper
    {
        get => _outerWallpaper;
        set
        {
            _outerWallpaper = value;
            OuterWallpaperTexture = Texture2D.LoadFromImage(value);
        }
    }

    public static Image OuterTitleTopWallpaper
    {
        get => _outerTitleTopWallpaper;
        set
        {
            _outerTitleTopWallpaper = value;
            OuterTitleTopWallpaperTexture = Texture2D.LoadFromImage(value);
        }
    }

    public static Texture2D InnerWallpaperTexture { get; private set; }
    public static Texture2D OuterWallpaperTexture { get; private set; }
    public static Texture2D OuterTitleTopWallpaperTexture { get; private set; }

    public static void SetLook(IUserInterface active)
    {
        Wallpaper = new Wallpaper();
        _look = active.Look;
        if (_look.Outer is null)  // TOT
        {
            Wallpaper.OuterTitleTop = _look.OuterTitleTop.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterThinTop = _look.OuterThinTop.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterBottom = _look.OuterBottom.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterMiddle = _look.OuterMiddle.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterLeft = _look.OuterLeft.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterRight = _look.OuterRight.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.OuterTitleTopLeft = Images.ExtractBitmap(_look.OuterTitleTopLeft, active);
            Wallpaper.OuterTitleTopRight = Images.ExtractBitmap(_look.OuterTitleTopRight, active);
            Wallpaper.OuterThinTopLeft = Images.ExtractBitmap(_look.OuterThinTopLeft, active);
            Wallpaper.OuterThinTopRight = Images.ExtractBitmap(_look.OuterThinTopRight, active);
            Wallpaper.OuterMiddleLeft = Images.ExtractBitmap(_look.OuterMiddleLeft, active);
            Wallpaper.OuterMiddleRight = Images.ExtractBitmap(_look.OuterMiddleRight, active);
            Wallpaper.OuterBottomLeft = Images.ExtractBitmap(_look.OuterBottomLeft, active);
            Wallpaper.OuterBottomRight = Images.ExtractBitmap(_look.OuterBottomRight, active);
            Wallpaper.Inner = _look.Inner.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.InnerAlt = Images.ExtractBitmap(_look.InnerAlt, active);
            Wallpaper.Button = _look.Button.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            Wallpaper.ButtonClicked = _look.ButtonClicked.Select(img => Images.ExtractBitmap(img, active)).ToArray();
        }
        else    // MGE
        {
            Wallpaper.Outer = Images.ExtractBitmap(_look.Outer, active);
            Wallpaper.Inner = new[] { Images.ExtractBitmap(_look.Inner[0], active) };
        }

    }

    private static InterfaceStyle _look;

    public static Image[] GetScrollImages(int dim, bool vertical)
    {
        var image = Image.GenColor(dim, dim, Color.Black);
        var color1 = new Color(227, 227, 227, 255);
        image.DrawLine(0, 0, image.Width - 1, 0, color1);
        image.DrawLine(0, 0, 0, image.Height - 1, color1);

        var color2 = Color.White;
        image.DrawLine(1, 1, image.Width - 2, 1, color2);
        image.DrawLine(1, 1, 1, image.Height - 2, color2);

        var color3 = new Color(240, 240, 240, 255);
        image.DrawLine(3, 3, image.Width - 4, image.Height - 4, color3);

        var color4 = new Color(160, 160, 160, 255);
        image.DrawLine(image.Width - 2, 1, image.Width - 2, image.Height - 2, color4);
        image.DrawLine(1, image.Height - 2, image.Width - 2, image.Height - 2, color4);

        var color5 = new Color(105, 105, 105, 255);
        image.DrawLine(0, image.Height - 1, image.Width - 1, image.Height - 1, color5);
        image.DrawLine(image.Width - 1, 0, image.Width - 1, image.Height - 1, color5);

        var left = image.Copy();
        if (vertical)
        {
            left.DrawPixel(9, 6, Color.Black);
            left.DrawLine(8, 7, 10, 7, Color.Black);
            left.DrawLine(7, 8, 11, 8, Color.Black);
            left.DrawLine(6, 9, 12, 9, Color.Black);

        }
        else
        {
            left.DrawPixel(6, 9, Color.Black);
            left.DrawLine(7, 8, 7, 10, Color.Black);
            left.DrawLine(8, 7, 8, 11, Color.Black);
            left.DrawLine(9, 6, 9, 12, Color.Black);

        }
        var right = image.Copy();
        if (vertical)
        {
            right.DrawPixel(9, dim - 6, Color.Black);
            right.DrawLine(8, dim - 7, 10, dim - 7, Color.Black);
            right.DrawLine(7, dim - 8, 11, dim - 8, Color.Black);
            right.DrawLine(6, dim - 9, 12, dim - 9, Color.Black);
        }
        else
        {
            right.DrawPixel(dim - 6, 9, Color.Black);
            right.DrawLine(dim - 7, 8, dim - 7, 10, Color.Black);
            right.DrawLine(dim - 8, 7, dim - 8, 11, Color.Black);
            right.DrawLine(dim - 9, 6, dim - 9, 12, Color.Black);
        }

        return new[] { left, image, right };
    }

    public static Vector2 GetUnitTextures(IUnit unit, IUserInterface active, IGame game,
        List<IViewElement> viewElements, Vector2 loc,
        bool noStacking = false)
    {
        var unitTexture = TextureCache.GetImage(active.UnitImages.Units[(int)unit.Type].Image);
        var shieldTexture = TextureCache.GetImage(active.UnitImages.Shields, active, unit.Owner.Id);

        var shield = active.UnitShield((int)unit.Type);

        var tile = unit.CurrentLocation;

        if (shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile, isShaded: unit.Order == (int)OrderType.Sleep));
        }

        // Stacked shield
        if (unit.IsInStack && !noStacking)
        {
            if (shield.DrawShadow)
            {
                var stackShadowOffset = shield.StackingOffset + shield.ShadowOffset;
                viewElements.Add(new TextureElement(
                    location: loc,
                    texture: TextureCache.GetImage(active.UnitImages.ShieldShadow, active, unit.Owner.Id),
                    tile: tile, offset: shield.Offset + stackShadowOffset));
            }
            viewElements.Add(new TextureElement(
                location: loc,
                texture: TextureCache.GetImage(active.UnitImages.ShieldBack, active, unit.Owner.Id),
                tile: tile, offset: shield.Offset + shield.StackingOffset));
        }

        // Shield shadow
        if (shield.DrawShadow)
        {
            viewElements.Add(new TextureElement(location: loc,
                texture: TextureCache.GetImage(active.UnitImages.ShieldShadow, active, unit.Owner.Id),
                tile: tile, offset: shield.Offset + shield.ShadowOffset));
        }

        // Front shield
        viewElements.Add(new TextureElement(location: loc,
            texture: shieldTexture, tile: tile, offset: shield.Offset));

        // Health bar
        viewElements.Add(new HealthBar(location: loc,
            tile: tile, unit.RemainingHitpoints, unit.HitpointsBase,
            offset: shield.Offset + shield.HPbarOffset, shield));

        // Orders text
        var shieldText = (int)unit.Order <= 11 ? game.Rules.Orders[(int)unit.Order - 1].Key : "-";
        viewElements.Add(new TextElement(shieldText, loc, shield.OrderTextHeight,
            tile, shield.Offset + shield.OrderOffset));

        if (!shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile, isShaded: unit.Order == (int)OrderType.Sleep));
        }

        if (unit.Order == (int)OrderType.Fortified)
        {
            viewElements.Add(new TextureElement(location: loc, texture: TextureCache.GetImage(active.UnitImages.Fortify), tile: tile));
        }

        return new Vector2(unitTexture.Width, unitTexture.Height);
    }

    // public static Image GetUnitImage(IUserInterface active, Unit unit, bool noStacking = false)
    // {
    //     int w = (int)active.UnitImages.UnitRectangle.Width - 1;
    //     int h = (int)active.UnitImages.UnitRectangle.Height - 1;
    //     var image = NewImage(w, h);
    //     var rect = new Rectangle(0, 0, w, h);
    //     var flagLoc = active.UnitImages.Units[(int)unit.Type].FlagLoc;
    //     var shldSrc = new Rectangle(0, 0, active.UnitImages.ShieldShadow.Width, active.UnitImages.ShieldShadow.Height);
    //     var shldDes = new Rectangle(flagLoc.X, flagLoc.Y, shldSrc.Width, shldSrc.Height);
    //     int stackingDir = (int)active.UnitImages.Units[(int)unit.Type].FlagLoc.X < 32 ? -1 : 1;
    //     var shldShadowDes = new Rectangle(flagLoc.X + stackingDir, flagLoc.Y + 1, shldSrc.Width, shldSrc.Height);
    //     if (unit.IsInStack && !noStacking)
    //     {
    //         var shldStackShadowDes = new Rectangle(flagLoc.X + 5 * stackingDir, flagLoc.Y + 1, shldSrc.Width, shldSrc.Height);
    //         var shldStackDes = new Rectangle(flagLoc.X + 4 * stackingDir, flagLoc.Y, shldSrc.Width, shldSrc.Height);
    //         Raylib.ImageDraw(ref image, active.UnitImages.ShieldShadow, shldSrc, shldStackShadowDes, Color.WHITE);
    //         Raylib.ImageDraw(ref image, active.UnitImages.ShieldBack[unit.Owner.Id], shldSrc, shldStackDes, Color.WHITE);
    //     }
    //     Raylib.ImageDraw(ref image, active.UnitImages.ShieldShadow, shldSrc, shldShadowDes, Color.WHITE);
    //     Raylib.ImageDraw(ref image, GetShieldWithHP(active.UnitImages.Shields[unit.Owner.Id], unit), shldSrc, shldDes, Color.WHITE);
    //     var shldTxt = (int)unit.Order <= 11 ? Game.Instance.Rules.Orders[(int)unit.Order - 1].Key : string.Empty;
    //     if ((int)unit.Order == 255) shldTxt = "-";
    //     var txtMeasr = Raylib.MeasureTextEx(Fonts.AlternativeFont, shldTxt, 12, 0.0f);
    //     var txtLoc = new Vector2(shldDes.X + shldDes.Width / 2 - txtMeasr.X / 2 + 1, 
    //         shldDes.Y + shldDes.Height / 2 - txtMeasr.Y / 2 + 3);
    //     Raylib.ImageDrawTextEx(ref image, Fonts.AlternativeFont, shldTxt, txtLoc, 12, 0.0f, Color.Black);
    //     Raylib.ImageDraw(ref image, active.UnitImages.Units[(int)unit.Type].Image, rect, rect, Color.WHITE);
    //     return image;
    // }

    public static Image GetShieldWithHp(Image shield, Unit unit)
    {
        var hpShield = shield.Copy();
        var hpBarX = (int)Math.Floor((float)unit.RemainingHitpoints * 12 / unit.HitpointsBase);
        var hpColor = hpBarX switch
        {
            <= 3 => new Color(243, 0, 0, 255),
            >= 4 and <= 8 => new Color(255, 223, 79, 255),
            _ => new Color(87, 171, 39, 255)
        };
        hpShield.DrawRectangle(0, 2, hpBarX, 3, hpColor);
        hpShield.DrawRectangle(hpBarX, 2, hpShield.Width - hpBarX, 3, Color.Black);
        return hpShield;
    }

    public static Texture2D GetImpImage(IUserInterface activeInterface, IImageSource imageSource, int owner)
    {
        return TextureCache.GetImage(imageSource, activeInterface, owner);
    }

    public static int ZoomScale(this int i, int zoom)
    {
        return (int)((8.0 + zoom) / 8.0 * i);
    }

    public static int ZoomScale(this float i, int zoom)
    {
        return (int)((8.0 + zoom) / 8.0 * i);
    }

    public static float ZoomScale(int zoom)
    {
        return (float)((8.0 + zoom) / 8.0);
    }

    public static Rectangle ZoomScale(this Rectangle rect, int zoom)
    {
        return new Rectangle(0, 0, rect.Width.ZoomScale(zoom), rect.Height.ZoomScale(zoom));
    }

    public static Vector2 ZoomScale(this Vector2 coords, int zoom)
    {
        return new Vector2(coords.X.ZoomScale(zoom), coords.Y.ZoomScale(zoom));
    }
}