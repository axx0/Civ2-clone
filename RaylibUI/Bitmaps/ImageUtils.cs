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

    public static void PaintPanelBorders(IUserInterface active, ref Image image, int width, int height, Padding padding)
    {
        active.DrawBorderWallpaper(Wallpaper, ref image, height, width, padding);
        active.DrawBorderLines(ref image, height, width, padding);
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
    
    public static void DrawTiledImage(Image source,ref Image destination, int height, int width, Padding padding)
    {
        var totalColumns = (width - padding.Left - padding.Right) / source.width;
        var totalRows = (height - padding.Top - padding.Bottom) / source.height;

        var rightEdge = padding.Left + (source.width * totalColumns);
        var rightWidth = width - padding.Right - rightEdge;
        var rightSrcRect = new Rectangle { height = source.height, width = rightWidth };

        var srcRec = new Rectangle { height = source.height, width = source.width };
        
        for (int row = 0; row < totalRows; row++)
        {
            var rowPos = source.height * row + padding.Top;
            for (int col = 0; col < totalColumns ; col++)
            {
                Raylib.ImageDraw(ref destination, source, srcRec,
                    new Rectangle(col * source.width + padding.Left, rowPos, source.width, source.height),
                    Color.WHITE);
            }
            Raylib.ImageDraw(ref destination, source, rightSrcRect,
                new Rectangle(rightEdge, rowPos, rightWidth, source.height),
                Color.WHITE);
        }

        var bottomEdge = padding.Top + totalRows * source.height;
        var bottomWidth = height - padding.Bottom - bottomEdge;
        if (bottomWidth > 0)
        {
            var bottomSourceRect = new Rectangle { height = bottomWidth, width = source.width };
            for (int col = 0; col < totalColumns; col++)
            {
                Raylib.ImageDraw(ref destination, source, bottomSourceRect,
                    new Rectangle(col * source.width + padding.Left, bottomEdge, source.width, bottomWidth),
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
    /// <param name="width">Width of dialog</param>
    /// <param name="height"></param>
    /// <param name="padding"></param>
    /// <param name="centerImage">Image to place in centre of dialog</param>
    /// <param name="noWallpaper">true to not draw inner wallpaper defaults to false</param>
    //public static Texture2D? PaintDialogBase(IUserInterface active, int width, int height, int top, int paddingBtm, int paddingSide, Image? centerImage = null, bool noWallpaper = false)
    public static Texture2D? PaintDialogBase(IUserInterface active, int width, int height, Padding padding, Image? centerImage = null, bool noWallpaper = false)
    {
        // Outer wallpaper
        var image = NewImage(width, height);
        PaintPanelBorders(active, ref image, width, height, padding);
        if (centerImage != null)
        {
            var innerWidth = Math.Min(width - padding.Left - padding.Right, centerImage.Value.width);
            var innerHeight = Math.Min(height - padding.Top - padding.Bottom, centerImage.Value.height);
            Raylib.ImageDraw( ref image, centerImage.Value, new Rectangle(0,0,innerWidth,innerHeight), new Rectangle(padding.Left, padding.Top, innerWidth,innerHeight), Color.WHITE);
        }
        else if(!noWallpaper)
        {
            DrawTiledImage(Wallpaper.Inner, ref image, height, width, padding);
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

        return new Vector2(unitTexture.width, unitTexture.height);
    }

    // public static Image GetUnitImage(IUserInterface active, Unit unit, bool noStacking = false)
    // {
    //     int w = (int)active.UnitImages.UnitRectangle.width - 1;
    //     int h = (int)active.UnitImages.UnitRectangle.height - 1;
    //     var image = NewImage(w, h);
    //     var rect = new Rectangle(0, 0, w, h);
    //     var flagLoc = active.UnitImages.Units[(int)unit.Type].FlagLoc;
    //     var shldSrc = new Rectangle(0, 0, active.UnitImages.ShieldShadow.width, active.UnitImages.ShieldShadow.height);
    //     var shldDes = new Rectangle(flagLoc.X, flagLoc.Y, shldSrc.width, shldSrc.height);
    //     int stackingDir = (int)active.UnitImages.Units[(int)unit.Type].FlagLoc.X < 32 ? -1 : 1;
    //     var shldShadowDes = new Rectangle(flagLoc.X + stackingDir, flagLoc.Y + 1, shldSrc.width, shldSrc.height);
    //     if (unit.IsInStack && !noStacking)
    //     {
    //         var shldStackShadowDes = new Rectangle(flagLoc.X + 5 * stackingDir, flagLoc.Y + 1, shldSrc.width, shldSrc.height);
    //         var shldStackDes = new Rectangle(flagLoc.X + 4 * stackingDir, flagLoc.Y, shldSrc.width, shldSrc.height);
    //         Raylib.ImageDraw(ref image, active.UnitImages.ShieldShadow, shldSrc, shldStackShadowDes, Color.WHITE);
    //         Raylib.ImageDraw(ref image, active.UnitImages.ShieldBack[unit.Owner.Id], shldSrc, shldStackDes, Color.WHITE);
    //     }
    //     Raylib.ImageDraw(ref image, active.UnitImages.ShieldShadow, shldSrc, shldShadowDes, Color.WHITE);
    //     Raylib.ImageDraw(ref image, GetShieldWithHP(active.UnitImages.Shields[unit.Owner.Id], unit), shldSrc, shldDes, Color.WHITE);
    //     var shldTxt = (int)unit.Order <= 11 ? Game.Instance.Rules.Orders[(int)unit.Order - 1].Key : string.Empty;
    //     if ((int)unit.Order == 255) shldTxt = "-";
    //     var txtMeasr = Raylib.MeasureTextEx(Fonts.AlternativeFont, shldTxt, 12, 0.0f);
    //     var txtLoc = new Vector2(shldDes.x + shldDes.width / 2 - txtMeasr.X / 2 + 1, 
    //         shldDes.y + shldDes.height / 2 - txtMeasr.Y / 2 + 3);
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
        Raylib.ImageDrawRectangle(ref hpShield, hpBarX, 2, hpShield.width - hpBarX, 3, Color.BLACK);
        return hpShield;
    }

    public static Texture2D GetImpImage(IUserInterface activeInterface, IImageSource imageSource, int owner)
    {
        return TextureCache.GetImage(imageSource, activeInterface, owner);
    }
    
}