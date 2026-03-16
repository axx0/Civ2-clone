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
using Model.Core.Units;
using Model.ImageSets;
using Model.Interface;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;
using RaylibUtils;
using RaylibUI.BasicTypes.Controls;

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

    ///// <summary>
    ///// Draw tiles within a rectangle (chose tiles randomly)
    ///// </summary>
    ///// <param name="tiles">Wallpaper tile images</param>
    ///// <param name="dest">Destination image</param>
    ///// <param name="rect">Rectangle within the image where tiles are to be drawn</param>
    //public static void DrawTiledRectangle(Image[] tiles, ref Image dest, Rectangle rect)
    //{
    //    var rnd = new Random();
    //    var len = tiles.Length;

    //    var totalColumns = Math.Ceiling(rect.Width / tiles[0].Width);
    //    var totalRows = Math.Ceiling(rect.Height / tiles[0].Height);

    //    for (int row = 0; row < totalRows; row++)
    //    {
    //        for (int col = 0; col < totalColumns; col++)
    //        {
    //            var srcRec = new Rectangle { Height = tiles[0].Height, Width = tiles[0].Width };
    //            var destRec = new Rectangle(rect.X + col * tiles[0].Width, rect.Y + row * tiles[0].Height, tiles[0].Width, tiles[0].Height);

    //            if (col == totalColumns - 1)
    //            {
    //                srcRec.Width = rect.Width - tiles[0].Width * col;
    //                destRec.Width = srcRec.Width;
    //            }

    //            if (row == totalRows - 1)
    //            {
    //                srcRec.Height = rect.Height - tiles[0].Height * row;
    //                destRec.Height = srcRec.Height;
    //            }

    //            dest.Draw(tiles[rnd.Next(len)], srcRec, destRec, Color.White);
    //        }
    //    }
    //}


    public static void DrawTiledImage(Wallpaper wp, ref Image destination, int height, int width, Padding padding, bool statusPanel = false, bool ToTStatusPanelLayout = false)
    {
        // MGE uses inner wallpaper from ICONS for all dialogs
        // TOT uses inner wallpaper from ICONS only for status panel, otherwise uses tiles from dialog image file
        var tiles = wp != null ? (statusPanel && wp.InnerAlt.Width > 0 ? new[] { wp.InnerAlt } : wp.Inner) : new[] { InnerWallpaper };

        if (!statusPanel)
        {
            DrawUtils.TileFill(tiles, ref destination,
                new Rectangle(padding.Left, padding.Top, width - padding.Left - padding.Right, height - padding.Top - padding.Bottom));
        }
        else
        {
            if (ToTStatusPanelLayout)
            {
                DrawUtils.TileFill(tiles, ref destination,
                    new Rectangle(padding.Left, padding.Top, 0.25f * width - padding.Left - padding.Right, height - padding.Top - padding.Bottom));
                DrawUtils.TileFill(tiles, ref destination,
                    new Rectangle(padding.Left + (0.25f * width - padding.Left - padding.Right) + 8, padding.Top, width - 0.25f * width - 8, height - padding.Top - padding.Bottom));
            }
            else
            {
                DrawUtils.TileFill(tiles, ref destination,
                    new Rectangle(padding.Left, padding.Top, width - padding.Left - padding.Right, 60));
                DrawUtils.TileFill(tiles, ref destination,
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
            //var innerWidth = Math.Min(width - padding.Left - padding.Right, centerImage.Value.Width);
            //var innerHeight = Math.Min(height - padding.Top - padding.Bottom, centerImage.Value.Height);
            var innerWidth = width - padding.Left - padding.Right;
            var innerHeight = height - padding.Top - padding.Bottom;
            image.Draw(centerImage.Value, new Rectangle(0, 0, innerWidth, innerHeight), new Rectangle(padding.Left, padding.Top, innerWidth, innerHeight), Color.White);
            image.Draw(centerImage.Value, new Rectangle(0, 0, centerImage.Value.Width, centerImage.Value.Height), new Rectangle(padding.Left, padding.Top, innerWidth, innerHeight), Color.White);
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
        Image image = vertical ? 
            Image.GenColor(dim, ScrollBar.ScrollbarDimDefault, new Color(240, 240, 240, 255)) :
            Image.GenColor(ScrollBar.ScrollbarDimDefault, dim, new Color(240, 240, 240, 255));
        var w = image.Width;
        var h = image.Height;

        var color1 = new Color(227, 227, 227, 255);
        image.DrawLine(0, 0, w - 1, 0, color1);
        image.DrawLine(0, 0, 0, h - 1, color1);

        var color2 = Color.White;
        image.DrawLine(1, 1, w - 2, 1, color2);
        image.DrawLine(1, 1, 1, h - 2, color2);

        var color3 = new Color(240, 240, 240, 255);
        image.DrawLine(3, 3, w - 4, h - 4, color3);

        var color4 = new Color(160, 160, 160, 255);
        image.DrawLine(w - 2, 1, w - 2, h - 1, color4);
        image.DrawLine(1, h - 2, w - 2, h - 2, color4);

        var color5 = new Color(105, 105, 105, 255);
        image.DrawLine(0, h - 1, w, h - 1, color5);
        image.DrawLine(w - 1, 0, w - 1, h - 1, color5);

        var left = image.Copy();
        if (vertical)
        {
            left.DrawPixel(w / 2, 6, Color.Black);
            left.DrawLine(w / 2 - 1, 7, w / 2 + 2, 7, Color.Black);
            left.DrawLine(w / 2 - 2, 8, w / 2 + 3, 8, Color.Black);
            left.DrawLine(w / 2 - 3, 9, w / 2 + 4, 9, Color.Black);
        }
        else
        {
            left.DrawPixel(6, h / 2, Color.Black);
            left.DrawLine(7, h / 2 - 1, 7, h / 2 + 2, Color.Black);
            left.DrawLine(8, h / 2 - 2, 8, h / 2 + 3, Color.Black);
            left.DrawLine(9, h / 2 - 3, 9, h / 2 + 4, Color.Black);
        }

        var right = image.Copy();
        if (vertical)
        {
            right.DrawPixel(w / 2, h - 7, Color.Black);
            right.DrawLine(w / 2 - 1, h - 8, w / 2 + 2, h - 8, Color.Black);
            right.DrawLine(w / 2 - 2, h - 9, w / 2 + 3, h - 9, Color.Black);
            right.DrawLine(w / 2 - 3, h - 10, w / 2 + 4, h - 10, Color.Black);
        }
        else
        {
            right.DrawPixel(w - 7, h / 2, Color.Black);
            right.DrawLine(w - 8, h / 2 - 1, w - 8, h / 2 + 2, Color.Black);
            right.DrawLine(w - 9, h / 2 - 2, w - 9, h / 2 + 3, Color.Black);
            right.DrawLine(w - 10, h / 2 - 3, w - 10, h / 2 + 4, Color.Black);
        }

        return [left, image, right];
    }

    public static Vector2 GetUnitTextures(IUnit unit, IUserInterface active, IGame game,
        List<IViewElement> viewElements, Vector2 loc, bool noStacking = false)
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

    /// <summary>
    /// Scale location & dimensions of a rectangle
    /// </summary>
    /// <param name="rect">Rectangle to be scaled</param>
    /// <param name="scale">Scale factor</param>
    /// <returns></returns>
    public static Rectangle ScaleAll(this Rectangle rect, float scale) =>
        new Rectangle(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
}