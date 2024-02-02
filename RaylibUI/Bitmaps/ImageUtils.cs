using Model.Images;
using Raylib_cs;
using System.Numerics;
using Civ2engine;
using Civ2engine.Enums;
using Model;
using Color = Raylib_cs.Color;
using Font = Raylib_cs.Font;
using Image = Raylib_cs.Image;
using Rectangle = Raylib_cs.Rectangle;
using Civ2engine.Units;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;
using Model.ImageSets;
using Model.Interface;
using Civ2;

namespace RaylibUI;

public static class ImageUtils
{
    private static Image _innerWallpaper;
    private static Image _outerWallpaper;
    private static Image _outerTitleTopWallpaper;

    public static void PaintPanelBorders(IUserInterface active, ref Image image, int Width, int Height, Padding padding)
    {
        active.DrawBorderWallpaper(Wallpaper, ref image, Height, Width, padding);
        active.DrawBorderLines(ref image, Height, Width, padding);
    }

    public static void DrawTiledImage(Image source,ref Image destination, int Height, int Width)
    {  
        int rows = Height / source.Height + 1;
        var columns = Width / source.Width + 1;
        var sourceRec = new Rectangle { Height = source.Height, Width = source.Width };
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Raylib.ImageDraw(ref destination, source, sourceRec, new Rectangle(col* source.Width, row* source.Height, source.Width, source.Height), Color.WHITE);
            }
        }
    }
    
    public static void DrawTiledImage(Image source,ref Image destination, int Height, int Width, Padding padding)
    {
        var totalColumns = (Width - padding.Left - padding.Right) / source.Width;
        var totalRows = (Height - padding.Top - padding.Bottom) / source.Height;

        var rightEdge = padding.Left + (source.Width * totalColumns);
        var rightWidth = Width - padding.Right - rightEdge;
        var rightSrcRect = new Rectangle { Height = source.Height, Width = rightWidth };

        var srcRec = new Rectangle { Height = source.Height, Width = source.Width };
        
        for (int row = 0; row < totalRows; row++)
        {
            var rowPos = source.Height * row + padding.Top;
            for (int col = 0; col < totalColumns ; col++)
            {
                Raylib.ImageDraw(ref destination, source, srcRec,
                    new Rectangle(col * source.Width + padding.Left, rowPos, source.Width, source.Height),
                    Color.WHITE);
            }
            Raylib.ImageDraw(ref destination, source, rightSrcRect,
                new Rectangle(rightEdge, rowPos, rightWidth, source.Height),
                Color.WHITE);
        }

        var bottomEdge = padding.Top + totalRows * source.Height;
        var bottomWidth = Height - padding.Bottom - bottomEdge;
        if (bottomWidth > 0)
        {
            var bottomSourceRect = new Rectangle { Height = bottomWidth, Width = source.Width };
            for (int col = 0; col < totalColumns; col++)
            {
                Raylib.ImageDraw(ref destination, source, bottomSourceRect,
                    new Rectangle(col * source.Width + padding.Left, bottomEdge, source.Width, bottomWidth),
                    Color.WHITE);
            }
            
            Raylib.ImageDraw(ref destination, source, new Rectangle { Height = bottomWidth, Width = rightWidth},
                new Rectangle(rightEdge, bottomEdge, rightWidth, bottomWidth),
                Color.WHITE);
        }
    }

    public static Image NewImage(int Width, int h)
    {
        var image = Raylib.LoadImage("blank.png");
        
        Raylib.ImageResize(ref image, Width, h);
        return image;
    }

    /// <summary>
    /// Paint base screen of a dialog
    /// </summary>
    /// <param name="Width">Width of dialog</param>
    /// <param name="Height"></param>
    /// <param name="padding"></param>
    /// <param name="centerImage">Image to place in centre of dialog</param>
    /// <param name="noWallpaper">true to not draw inner wallpaper defaults to false</param>
    //public static Texture2D? PaintDialogBase(IUserInterface active, int Width, int Height, int top, int paddingBtm, int paddingSide, Image? centerImage = null, bool noWallpaper = false)
    public static Texture2D? PaintDialogBase(IUserInterface active, int Width, int Height, Padding padding, Image? centerImage = null, bool noWallpaper = false)
    {
        // Outer wallpaper
        var image = NewImage(Width, Height);
        PaintPanelBorders(active, ref image, Width, Height, padding);
        if (centerImage != null)
        {
            var innerWidth = Math.Min(Width - padding.Left - padding.Right, centerImage.Value.Width);
            var innerHeight = Math.Min(Height - padding.Top - padding.Bottom, centerImage.Value.Height);
            Raylib.ImageDraw( ref image, centerImage.Value, new Rectangle(0,0,innerWidth,innerHeight), new Rectangle(padding.Left, padding.Top, innerWidth,innerHeight), Color.WHITE);
        }
        else if(!noWallpaper)
        {
            DrawTiledImage(Wallpaper.Inner, ref image, Height, Width, padding);
        }

        return Raylib.LoadTextureFromImage(image);
    }

    public static void PaintRadioButton(int X, int Y, bool isSelected)
    {
        Raylib.DrawCircle(X + 8, Y + 8, 8.0f, new Color(128, 128, 128, 255));
        Raylib.DrawCircleLines(X + 8 + 1, Y + 8 + 1, 8.0f, Color.BLACK);
        Raylib.DrawRectangle(X + 1, Y + 4, 2, 3, Color.BLACK);
        Raylib.DrawRectangle(X + 3, Y + 2, 2, 2, Color.BLACK);
        Raylib.DrawRectangle(X + 6, Y + 1, 1, 1, Color.BLACK);
        Raylib.DrawRectangle(X + 11, Y + 15, 3, 2, Color.BLACK);
        Raylib.DrawRectangle(X + 14, Y + 13, 2, 2, Color.BLACK);
        Raylib.DrawRectangle(X + 16, Y + 11, 1, 1, Color.BLACK);
        Raylib.DrawCircleLines(X + 8, Y + 8, 8.0f, Color.WHITE);

        if (!isSelected)
        {
            Raylib.DrawRectangle(X + 6, Y + 4, 5, 9, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 4, Y + 6, 9, 5, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 5, Y + 11, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 4, Y + 6, 1, 5, Color.WHITE);
            Raylib.DrawRectangle(X + 5, Y + 5, 1, 2, Color.WHITE);
            Raylib.DrawRectangle(X + 6, Y + 4, 1, 2, Color.WHITE);
            Raylib.DrawRectangle(X + 7, Y + 4, 4, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 11, Y + 5, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 11, Y + 11, 1, 1, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 7, Y + 13, 4, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 11, Y + 12, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 12, Y + 11, 1, 1, Color.WHITE);
            Raylib.DrawRectangle(X + 13, Y + 7, 1, 4, Color.WHITE);
        }
        else
        {
            Raylib.DrawRectangle(X + 7, Y + 4, 4, 10, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 4, Y + 7, 10, 4, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 6, Y + 5, 6, 8, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 5, Y + 6, 8, 6, new Color(192, 192, 192, 255));
            Raylib.DrawRectangle(X + 7, Y + 6, 4, 6, Color.BLACK);
            Raylib.DrawRectangle(X + 6, Y + 7, 6, 4, Color.BLACK);
        }
    }

    public static void PaintCheckbox(int X, int Y, bool isChecked)
    {
        Raylib.DrawRectangle(X + 3, Y + 2, 15, 17, Color.WHITE);
        Raylib.DrawRectangle(X + 2, Y + 3, 17, 15, Color.WHITE);
        Raylib.DrawRectangle(X + 4, Y + 3, 13, 15, new Color(128, 128, 128, 255));
        Raylib.DrawRectangle(X + 3, Y + 4, 15, 13, new Color(128, 128, 128, 255));
        Raylib.DrawLine(X + 4, Y + 3, X + 16, Y + 3, Color.BLACK);
        Raylib.DrawLine(X + 3, Y + 4, X + 3, Y + 16, Color.BLACK);
        Raylib.DrawLine(X + 3, Y + 4, X + 4, Y + 4, Color.BLACK);
        Raylib.DrawLine(X + 4, Y + 19, X + 18, Y + 19, Color.BLACK);
        Raylib.DrawLine(X + 18, Y + 18, X + 19, Y + 18, Color.BLACK);
        Raylib.DrawLine(X + 19, Y + 4, X + 19, Y + 17, Color.BLACK);

        if (isChecked)
        {
            Raylib.DrawLine(X + 21, Y + 3, X + 25, Y + 3, Color.BLACK);
            Raylib.DrawLine(X + 20, Y + 4, X + 23, Y + 4, Color.BLACK);
            Raylib.DrawLine(X + 19, Y + 5, X + 21, Y + 5, Color.BLACK);
            Raylib.DrawLine(X + 18, Y + 6, X + 20, Y + 6, Color.BLACK);
            Raylib.DrawLine(X + 17, Y + 7, X + 19, Y + 7, Color.BLACK);
            Raylib.DrawLine(X + 16, Y + 8, X + 18, Y + 8, Color.BLACK);
            Raylib.DrawLine(X + 15, Y + 9, X + 17, Y + 9, Color.BLACK);
            Raylib.DrawLine(X + 5, Y + 10, X + 6, Y + 10, Color.BLACK);
            Raylib.DrawLine(X + 14, Y + 10, X + 16, Y + 10, Color.BLACK);
            Raylib.DrawLine(X + 6, Y + 11, X + 7, Y + 11, Color.BLACK);
            Raylib.DrawLine(X + 14, Y + 11, X + 16, Y + 11, Color.BLACK);
            Raylib.DrawLine(X + 7, Y + 12, X + 8, Y + 12, Color.BLACK);
            Raylib.DrawLine(X + 13, Y + 12, X + 15, Y + 12, Color.BLACK);
            Raylib.DrawLine(X + 8, Y + 13, X + 14, Y + 13, Color.BLACK);
            Raylib.DrawLine(X + 12, Y + 13, X + 15, Y + 13, Color.BLACK);
            Raylib.DrawLine(X + 12, Y + 14, X + 14, Y + 14, Color.BLACK);
            Raylib.DrawLine(X + 9, Y + 15, X + 12, Y + 15, Color.BLACK);
            Raylib.DrawLine(X + 10, Y + 16, X + 12, Y + 16, Color.BLACK);
            Raylib.DrawLine(X + 11, Y + 16, X + 11, Y + 17, Color.BLACK);
            Raylib.DrawLine(X + 20, Y + 1, X + 22, Y + 1, Color.WHITE);
            Raylib.DrawLine(X + 19, Y + 2, X + 20, Y + 2, Color.WHITE);
            Raylib.DrawLine(X + 20, Y + 2, X + 22, Y + 2, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 18, Y + 3, X + 19, Y + 3, Color.WHITE);
            Raylib.DrawLine(X + 19, Y + 3, X + 20, Y + 3, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 17, Y + 4, X + 18, Y + 4, Color.WHITE);
            Raylib.DrawLine(X + 18, Y + 4, X + 19, Y + 4, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 16, Y + 5, X + 17, Y + 5, Color.WHITE);
            Raylib.DrawLine(X + 17, Y + 5, X + 18, Y + 5, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 15, Y + 6, X + 16, Y + 6, Color.WHITE);
            Raylib.DrawLine(X + 16, Y + 6, X + 17, Y + 6, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 14, Y + 7, X + 15, Y + 7, Color.WHITE);
            Raylib.DrawLine(X + 15, Y + 7, X + 16, Y + 7, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 4, Y + 8, X + 5, Y + 8, Color.WHITE);
            Raylib.DrawLine(X + 5, Y + 8, X + 5, Y + 9, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 13, Y + 8, X + 14, Y + 8, Color.WHITE);
            Raylib.DrawLine(X + 14, Y + 8, X + 15, Y + 8, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 6, Y + 9, X + 6, Y + 10, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 13, Y + 9, X + 14, Y + 9, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 7, Y + 10, X + 7, Y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 12, Y + 10, X + 13, Y + 10, Color.WHITE);
            Raylib.DrawLine(X + 13, Y + 10, X + 13, Y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 8, Y + 11, X + 8, Y + 12, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 11, Y + 11, X + 12, Y + 11, Color.WHITE);
            Raylib.DrawLine(X + 12, Y + 11, X + 13, Y + 11, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 9, Y + 12, X + 9, Y + 13, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 11, Y + 12, X + 12, Y + 12, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 9, Y + 13, X + 11, Y + 13, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 9, Y + 14, X + 11, Y + 14, new Color(192, 192, 192, 255));
            Raylib.DrawLine(X + 10, Y + 14, X + 10, Y + 15, new Color(192, 192, 192, 255));
        }
    }

    public static Wallpaper Wallpaper { get; set; }

    public static Image InnerWallpaper
    {
        get => _innerWallpaper;
        set
        {
            _innerWallpaper = value;
            InnerWallpaperTexture = Raylib.LoadTextureFromImage(value);
        }
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

    public static Image OuterTitleTopWallpaper
    {
        get => _outerTitleTopWallpaper;
        set
        {
            _outerTitleTopWallpaper = value;
            OuterTitleTopWallpaperTexture = Raylib.LoadTextureFromImage(value);
        }
    }

    public static Texture2D InnerWallpaperTexture { get; private set; }
    public static Texture2D OuterWallpaperTexture { get; private set; }
    public static Texture2D OuterTitleTopWallpaperTexture { get; private set; }

    public static Texture2D[] GetOptionImages(bool checkbox)
    {
        var images = checkbox ? Look.CheckBoxes : Look.RadioButtons;
        return images.Select(TextureCache.GetImage).ToArray();
    }
    
    public static void SetLook(IUserInterface active)
    {
        Wallpaper = new Wallpaper();
        if (active.Look.Outer is null)
        {
            Wallpaper.OuterTitleTop = Images.ExtractBitmap(active.Look.OuterTitleTop[0]);
            Wallpaper.OuterThinTop = Images.ExtractBitmap(active.Look.OuterThinTop[0]);
            Wallpaper.OuterBottom = Images.ExtractBitmap(active.Look.OuterBottom[0]);
            Wallpaper.OuterMiddle = Images.ExtractBitmap(active.Look.OuterMiddle[0]);
            Wallpaper.OuterLeft = Images.ExtractBitmap(active.Look.OuterLeft[0]);
            Wallpaper.OuterRight = Images.ExtractBitmap(active.Look.OuterRight[0]);
            Wallpaper.OuterTitleTopLeft = Images.ExtractBitmap(active.Look.OuterTitleTopLeft);
            Wallpaper.OuterTitleTopRight = Images.ExtractBitmap(active.Look.OuterTitleTopRight);
            Wallpaper.OuterThinTopLeft = Images.ExtractBitmap(active.Look.OuterThinTopLeft);
            Wallpaper.OuterThinTopRight = Images.ExtractBitmap(active.Look.OuterThinTopRight);
            Wallpaper.OuterMiddleLeft = Images.ExtractBitmap(active.Look.OuterMiddleLeft);
            Wallpaper.OuterMiddleRight = Images.ExtractBitmap(active.Look.OuterMiddleRight);
            Wallpaper.OuterBottomLeft = Images.ExtractBitmap(active.Look.OuterBottomLeft);
            Wallpaper.OuterBottomRight = Images.ExtractBitmap(active.Look.OuterBottomRight);
            Wallpaper.Inner = Images.ExtractBitmap(active.Look.Inner);
        }
        else
        {
            Wallpaper.Outer = Images.ExtractBitmap(active.Look.Outer);
            Wallpaper.Inner = Images.ExtractBitmap(active.Look.Inner);
        }

        Look = active.Look;
    }

    private static InterfaceStyle Look;

    public static Image[] GetScrollImages(int dim)
    {
        var image = NewImage(dim, dim);
        var color1 = new Color(227, 227, 227, 255);
        Raylib.ImageDrawLine(ref image, 0,0, image.Width -1, 0, color1);
        Raylib.ImageDrawLine(ref image, 0,0, 0, image.Height -1, color1);
        
        var color2 = Color.WHITE;
        Raylib.ImageDrawLine(ref image, 1,1, image.Width -2, 1, color2);
        Raylib.ImageDrawLine(ref image, 1,1, 1, image.Height -2, color2);

        var color3 = new Color(240, 240, 240, 255);
        Raylib.ImageDrawRectangle(ref image, 3,3,image.Width -4, image.Height - 4, color3);

        var color4 = new Color(160, 160, 160, 255);
        Raylib.ImageDrawLine(ref image, image.Width -2,1, image.Width -2, image.Height -2, color4);
        Raylib.ImageDrawLine(ref image, 1, image.Height -2, image.Width-2, image.Height -2, color4);
        
        var color5 =new Color(105,105,105,255);
        Raylib.ImageDrawLine(ref image, 0, image.Height -1, image.Width -1, image.Height -1, color5);
        Raylib.ImageDrawLine(ref image, image.Width -1, 0, image.Width -1, image.Height -1, color5);

        var left = Raylib.ImageCopy(image);
        Raylib.ImageDrawPixel(ref left, 6,9,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 7,8,7,10,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 8,7,8,11,Color.BLACK);
        Raylib.ImageDrawLine(ref left, 9,6,9,12,Color.BLACK);
        var right = Raylib.ImageCopy(image);
        Raylib.ImageDrawPixel(ref right, dim - 6,9,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-7,8,dim-7,10,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-8,7,dim-8,11,Color.BLACK);
        Raylib.ImageDrawLine(ref right, dim-9,6,dim-9,12,Color.BLACK);
        return new[] { left, image, right };
    }

    public static Vector2 GetUnitTextures(IUnit unit, IUserInterface active, List<IViewElement> viewElements, Vector2 loc,
        bool noStacking = false)
    {
        var unitTexture = active.UnitImages.Units[(int)unit.Type].Texture;
        var shieldTexture = TextureCache.GetImage(active.UnitImages.Shields, active, unit.Owner.Id);
        
        var shield = active.UnitShield((int)unit.Type);
        var shieldLoc = loc + shield.Offset;
        
        var tile = unit.CurrentLocation;

        if (shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile));
        }

        // Stacked shield
        if (unit.IsInStack && !noStacking)
        {
            if (shield.DrawShadow)
            {
                var stackShadowOffset = shield.StackingOffset + shield.ShadowOffset;
                viewElements.Add(new TextureElement(
                    location: shieldLoc + stackShadowOffset,
                    texture: TextureCache.GetImage(active.UnitImages.ShieldShadow, active, unit.Owner.Id),
                    tile: tile, offset: shield.Offset + stackShadowOffset));
            }
            viewElements.Add(new TextureElement(
                location: shieldLoc + shield.StackingOffset,
                texture: TextureCache.GetImage(active.UnitImages.ShieldBack, active, unit.Owner.Id),
                tile: tile, offset: shield.Offset + shield.StackingOffset));
        }

        // Shield shadow
        if (shield.DrawShadow)
        {
            viewElements.Add(new TextureElement(location: shieldLoc + shield.ShadowOffset,
                texture: TextureCache.GetImage(active.UnitImages.ShieldShadow, active, unit.Owner.Id), 
                tile: tile, offset: shield.Offset + shield.ShadowOffset));
        }

        // Front shield
        viewElements.Add(new TextureElement(location: shieldLoc,
            texture: shieldTexture, tile: tile, offset:shield.Offset));

        // Health bar
        viewElements.Add(new HealthBar(location: shieldLoc + shield.HPbarOffset,
            tile: tile, unit.RemainingHitpoints, unit.HitpointsBase, 
            offset: shield.Offset + shield.HPbarOffset, shield));

        // Orders text
        var shieldText = (int)unit.Order <= 11 ? Game.Instance.Rules.Orders[(int)unit.Order - 1].Key : "-";
        viewElements.Add(new TextElement(shieldText, shieldLoc + shield.OrderOffset, shield.OrderTextHeight,
            tile, shield.Offset + shield.OrderOffset));

        if (!shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile));
        }

        if (unit.Order == OrderType.Fortified)
        {
            viewElements.Add(new TextureElement(location: loc, texture: active.UnitImages.Fortify, tile: tile));
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
    //     Raylib.ImageDrawTextEx(ref image, Fonts.AlternativeFont, shldTxt, txtLoc, 12, 0.0f, Color.BLACK);
    //     Raylib.ImageDraw(ref image, active.UnitImages.Units[(int)unit.Type].Image, rect, rect, Color.WHITE);
    //     return image;
    // }

    public static Image GetShieldWithHP(Image shield, Unit unit)
    {
        var hpShield = Raylib.ImageCopy(shield);
        var hpBarX = (int)Math.Floor((float)unit.RemainingHitpoints * 12 / unit.HitpointsBase);
        var hpColor = hpBarX switch
        {
            <= 3 => new Color(243, 0, 0, 255),
            >= 4 and <= 8 => new Color(255, 223, 79, 255),
            _ => new Color(87, 171, 39, 255)
        };
        Raylib.ImageDrawRectangle(ref hpShield, 0, 2, hpBarX, 3, hpColor);
        Raylib.ImageDrawRectangle(ref hpShield, hpBarX, 2, hpShield.Width - hpBarX, 3, Color.BLACK);
        return hpShield;
    }

    public static Texture2D GetImpImage(IUserInterface activeInterface, IImageSource imageSource, int owner)
    {
        return TextureCache.GetImage(imageSource, activeInterface, owner);
    }
    
}