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
using Model.Core.Mapping;
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
    private const int HighResolutionShieldScale = 4;
    private static readonly Dictionary<string, Texture2D> HighResolutionUnitShields = new();
    private static bool _shieldTemplateLoaded;
    private static Image _shieldTemplate;

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
            active.DrawBorderWallpaper(Wallpaper!, ref image, height, width, padding, statusPanel && !toTStatusPanelLayout);
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


    public static void DrawTiledImage(Wallpaper? wp, ref Image destination, int height, int width, Padding padding, bool statusPanel = false, bool ToTStatusPanelLayout = false)
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
        var btn = Wallpaper?.Button;
        if (btn is not { Length: >= 3 } || btn.Any(part => part.Width <= 0 || part.Height <= 0))
        {
            return PaintFallbackButtonBase(width, height);
        }

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

    private static Texture2D PaintFallbackButtonBase(int width, int height)
    {
        var image = Image.GenColor(width, height, new Color(192, 192, 192, 255));

        image.DrawLine(0, 0, width - 1, 0, Color.White);
        image.DrawLine(0, 0, 0, height - 1, Color.White);
        image.DrawLine(1, 1, width - 2, 1, new Color(224, 224, 224, 255));
        image.DrawLine(1, 1, 1, height - 2, new Color(224, 224, 224, 255));

        image.DrawLine(0, height - 1, width - 1, height - 1, new Color(64, 64, 64, 255));
        image.DrawLine(width - 1, 0, width - 1, height - 1, new Color(64, 64, 64, 255));
        image.DrawLine(1, height - 2, width - 2, height - 2, new Color(128, 128, 128, 255));
        image.DrawLine(width - 2, 1, width - 2, height - 2, new Color(128, 128, 128, 255));

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
        var wallpaper = new Wallpaper();
        Wallpaper = wallpaper;
        _look = active.Look;
        if (_look.Outer is null)  // TOT
        {
            wallpaper.OuterTitleTop = _look.OuterTitleTop!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterThinTop = _look.OuterThinTop!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterBottom = _look.OuterBottom!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterMiddle = _look.OuterMiddle!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterLeft = _look.OuterLeft!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterRight = _look.OuterRight!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.OuterTitleTopLeft = Images.ExtractBitmap(_look.OuterTitleTopLeft!, active);
            wallpaper.OuterTitleTopRight = Images.ExtractBitmap(_look.OuterTitleTopRight!, active);
            wallpaper.OuterThinTopLeft = Images.ExtractBitmap(_look.OuterThinTopLeft!, active);
            wallpaper.OuterThinTopRight = Images.ExtractBitmap(_look.OuterThinTopRight!, active);
            wallpaper.OuterMiddleLeft = Images.ExtractBitmap(_look.OuterMiddleLeft!, active);
            wallpaper.OuterMiddleRight = Images.ExtractBitmap(_look.OuterMiddleRight!, active);
            wallpaper.OuterBottomLeft = Images.ExtractBitmap(_look.OuterBottomLeft!, active);
            wallpaper.OuterBottomRight = Images.ExtractBitmap(_look.OuterBottomRight!, active);
            wallpaper.Inner = _look.Inner!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.InnerAlt = Images.ExtractBitmap(_look.InnerAlt!, active);
            wallpaper.Button = _look.Button!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
            wallpaper.ButtonClicked = _look.ButtonClicked!.Select(img => Images.ExtractBitmap(img, active)).ToArray();
        }
        else    // MGE
        {
            wallpaper.Outer = Images.ExtractBitmap(_look.Outer, active);
            wallpaper.Inner = new[] { Images.ExtractBitmap(_look.Inner![0], active) };
        }

    }

    private static InterfaceStyle _look = null!;

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
        List<IViewElement> viewElements, Vector2 loc, bool noStacking = false, bool useMapArt = false)
    {
        var unitImage = active.UnitImages.Units[(int)unit.Type];
        var sourceImage = GetUnitSourceImage(unit, active, unitImage, useMapArt);
        var unitTexture = TextureCache.GetImage(sourceImage);
        var baseShieldTexture = TextureCache.GetImage(active.UnitImages.Shields, active, unit.Owner.Id);
        var shieldTexture = GetHighResolutionUnitShieldTexture(active, unit.Owner.Id, baseShieldTexture);
        var shieldRenderScale = GetHighResolutionShieldRenderScale(baseShieldTexture, shieldTexture);
        var shieldLogicalSize = new Vector2(baseShieldTexture.Width, baseShieldTexture.Height);

        var logicalSize = GetLogicalUnitSize(active, unitImage, unitTexture);
        var unitRenderScale = GetUnitRenderScale(unitImage, unitTexture, logicalSize);
        var unitDrawOffset = GetUnitDrawOffset(unitImage, unitTexture, logicalSize, unitRenderScale);

        var shield = active.UnitShield((int)unit.Type);

        var tile = unit.CurrentLocation;

        if (shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile, isShaded: unit.Order == (int)OrderType.Sleep,
                offset: unitDrawOffset, renderScale: unitRenderScale, maxDrawSize: logicalSize));
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

        // Front shield. Draw a generated high-resolution shield scaled back to the
        // original Civ2 logical shield size so maximum zoom does not magnify the
        // original low-resolution shield pixels.
        viewElements.Add(new TextureElement(location: loc,
            texture: shieldTexture, tile: tile, offset: shield.Offset,
            renderScale: shieldRenderScale, maxDrawSize: shieldLogicalSize));

        // Health bar
        viewElements.Add(new HealthBar(location: loc,
            tile: tile, unit.RemainingHitpoints, unit.HitpointsBase,
            offset: shield.Offset + shield.HPbarOffset, shield));

        // Orders text
        var shieldText = GetOrderShieldText(unit, game);
        viewElements.Add(new TextElement(shieldText, loc, shield.OrderTextHeight,
            tile, shield.Offset + shield.OrderOffset));

        if (!shield.ShieldInFrontOfUnit)
        {
            // Unit
            viewElements.Add(new TextureElement(location: loc, texture: unitTexture,
                tile: tile, isShaded: unit.Order == (int)OrderType.Sleep,
                offset: unitDrawOffset, renderScale: unitRenderScale, maxDrawSize: logicalSize));
        }

        if (unit.Order == (int)OrderType.Fortified)
        {
            viewElements.Add(new TextureElement(location: loc, texture: TextureCache.GetImage(active.UnitImages.Fortify), tile: tile));
        }

        return logicalSize;
    }

    private static Texture2D GetHighResolutionUnitShieldTexture(IUserInterface active, int ownerId, Texture2D sourceShield)
    {
        var width = Math.Max(12, sourceShield.Width) * HighResolutionShieldScale;
        var height = Math.Max(12, sourceShield.Height) * HighResolutionShieldScale;
        var key = $"FossUnitShield-{ownerId}-{width}x{height}";

        if (HighResolutionUnitShields.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var fill = ownerId >= 0 && ownerId < active.PlayerColours.Length
            ? active.PlayerColours[ownerId].LightColour
            : Color.White;
        var shade = ownerId >= 0 && ownerId < active.PlayerColours.Length
            ? active.PlayerColours[ownerId].DarkColour
            : new Color(64, 64, 64, 255);

        var image = Image.GenColor(width, height, new Color(0, 0, 0, 0));
        PaintHighResolutionShield(ref image, fill, shade);

        var texture = Texture2D.LoadFromImage(image);
        texture.SetFilter(TextureFilter.Bilinear);
        HighResolutionUnitShields[key] = texture;
        return texture;
    }

    private static bool TryGetHighResolutionShieldTemplate(out Image template)
    {
        if (!_shieldTemplateLoaded)
        {
            _shieldTemplateLoaded = true;
            var path = FindHighResolutionShieldTemplatePath();
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                try
                {
                    _shieldTemplate = Images.LoadImageFromFile(path).Image;
                }
                catch
                {
                    _shieldTemplate = default;
                }
            }
        }

        template = _shieldTemplate;
        return template.Width > 0 && template.Height > 0;
    }

    private static string? FindHighResolutionShieldTemplatePath()
    {
        var names = new[]
        {
            "shield.png",
            "unitshield.png",
            "unit-shield.png",
            "unit_shield.png",
            "shields.png"
        };

        foreach (var root in Settings.SearchPaths.Where(Directory.Exists).Concat(CandidateArtRoots()))
        {
            foreach (var name in names)
            {
                var direct = System.IO.Path.Combine(root, name);
                if (File.Exists(direct))
                {
                    return direct;
                }

                var foss = System.IO.Path.Combine(root, "FOSSart", name);
                if (File.Exists(foss))
                {
                    return foss;
                }

                var raylibFoss = System.IO.Path.Combine(root, "RaylibUI", "FOSSart", name);
                if (File.Exists(raylibFoss))
                {
                    return raylibFoss;
                }
            }
        }

        return null;
    }

    private static IEnumerable<string> CandidateArtRoots()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var start in new[] { Settings.BasePath, Directory.GetCurrentDirectory() }.Where(Directory.Exists))
        {
            var directory = new DirectoryInfo(start);
            for (var i = 0; i < 8 && directory != null; i++, directory = directory.Parent)
            {
                if (seen.Add(directory.FullName))
                {
                    yield return directory.FullName;
                }
            }
        }
    }

    private static void PaintFossShield(ref Image image, Image template, Color fill, Color shade)
    {
        PaintShieldBody(ref image, fill, shade);
        var width = image.Width;
        var height = image.Height;
        for (var y = 0; y < height; y++)
        {
            var sourceY = Math.Clamp((int)MathF.Round(y * (template.Height - 1) / MathF.Max(1, height - 1)), 0, template.Height - 1);
            for (var x = 0; x < width; x++)
            {
                var sourceX = Math.Clamp((int)MathF.Round(x * (template.Width - 1) / MathF.Max(1, width - 1)), 0, template.Width - 1);
                var outline = template.GetColor(sourceX, sourceY);
                if (outline.A == 0)
                {
                    continue;
                }

                var current = image.GetColor(x, y);
                image.DrawPixel(x, y, AlphaBlend(current, outline));
            }
        }
    }

    private static void PaintShieldBody(ref Image image, Color fill, Color shade)
    {
        var width = image.Width;
        var height = image.Height;
        var border = Math.Max(1, Math.Min(width, height) / 22);
        for (var y = 0; y < height; y++)
        {
            var ny = (y + 0.5f) / height;
            var halfWidth = ShieldHalfWidthAt(ny);
            for (var x = 0; x < width; x++)
            {
                var nx = (x + 0.5f) / width;
                var distanceFromCentre = MathF.Abs(nx - 0.5f);
                if (distanceFromCentre > halfWidth - border / (float)Math.Max(1, width))
                {
                    continue;
                }

                var amount = Math.Clamp((ny - 0.20f) * 0.45f, 0f, 0.32f);
                image.DrawPixel(x, y, Blend(fill, shade, amount));
            }
        }
    }

    private static Color AlphaBlend(Color below, Color above)
    {
        var alpha = above.A / 255f;
        var inverse = 1f - alpha;
        return new Color(
            (byte)Math.Clamp(above.R * alpha + below.R * inverse, 0, 255),
            (byte)Math.Clamp(above.G * alpha + below.G * inverse, 0, 255),
            (byte)Math.Clamp(above.B * alpha + below.B * inverse, 0, 255),
            (byte)Math.Clamp(above.A + below.A * inverse, 0, 255));
    }

    private static float GetHighResolutionShieldRenderScale(Texture2D sourceShield, Texture2D highResolutionShield)
    {
        if (highResolutionShield.Width <= 0 || highResolutionShield.Height <= 0)
        {
            return 1f;
        }

        var widthScale = sourceShield.Width / (float)highResolutionShield.Width;
        var heightScale = sourceShield.Height / (float)highResolutionShield.Height;
        return MathF.Max(0.01f, MathF.Min(widthScale, heightScale));
    }

    private static void PaintHighResolutionShield(ref Image image, Color fill, Color shade)
    {
        var width = image.Width;
        var height = image.Height;
        var border = Math.Max(1.25f, Math.Min(width, height) / 22f);
        var rim = Math.Max(border * 2.2f, Math.Min(width, height) / 9f);
        var black = new Color(8, 8, 12, 255);
        var bright = new Color(255, 255, 255, 235);

        for (var y = 0; y < height; y++)
        {
            var ny = (y + 0.5f) / height;
            var halfWidth = ShieldHalfWidthAt(ny);
            for (var x = 0; x < width; x++)
            {
                var nx = (x + 0.5f) / width;
                var distanceFromCentre = MathF.Abs(nx - 0.5f);
                if (distanceFromCentre > halfWidth)
                {
                    continue;
                }

                var edgeDistance = (halfWidth - distanceFromCentre) * width;
                var topDistance = y;
                var bottomDistance = height - y - 1;
                var outline = Math.Min(Math.Min(edgeDistance, topDistance), bottomDistance);

                if (outline <= border)
                {
                    image.DrawPixel(x, y, black);
                }
                else if (outline <= rim)
                {
                    image.DrawPixel(x, y, bright);
                }
                else
                {
                    var verticalShade = Math.Clamp((ny - 0.22f) * 0.45f, 0f, 0.34f);
                    var highlight = MathF.Max(0f, 1f - MathF.Abs(nx - 0.38f) * 5f) * MathF.Max(0f, 1f - MathF.Abs(ny - 0.32f) * 6f);
                    var color = Blend(fill, shade, verticalShade);
                    if (highlight > 0)
                    {
                        color = Blend(color, Color.White, Math.Min(0.22f, highlight * 0.18f));
                    }
                    image.DrawPixel(x, y, color);
                }
            }
        }

        var barY = (int)(height * 0.22f);
        var barHeight = Math.Max(1, (int)MathF.Round(height * 0.09f));
        for (var y = barY; y < Math.Min(height, barY + barHeight); y++)
        {
            var ny = (y + 0.5f) / height;
            var halfWidth = ShieldHalfWidthAt(ny);
            var left = Math.Max(0, (int)MathF.Floor((0.5f - halfWidth) * width));
            var right = Math.Min(width - 1, (int)MathF.Ceiling((0.5f + halfWidth) * width));
            for (var x = left; x <= right; x++)
            {
                var outline = y == barY || y == barY + barHeight - 1 ? black : bright;
                if (image.GetColor(x, y).A != 0)
                {
                    image.DrawPixel(x, y, outline);
                }
            }
        }
    }

    private static float ShieldHalfWidthAt(float normalizedY)
    {
        if (normalizedY < 0.14f)
        {
            return 0.44f;
        }

        if (normalizedY < 0.68f)
        {
            return 0.44f - (normalizedY - 0.14f) * 0.08f;
        }

        return MathF.Max(0.02f, 0.40f * (1f - (normalizedY - 0.68f) / 0.32f));
    }

    private static Color Blend(Color start, Color end, float amount)
    {
        amount = Math.Clamp(amount, 0f, 1f);
        return new Color(
            (byte)Math.Clamp(start.R + (end.R - start.R) * amount, 0, 255),
            (byte)Math.Clamp(start.G + (end.G - start.G) * amount, 0, 255),
            (byte)Math.Clamp(start.B + (end.B - start.B) * amount, 0, 255),
            255);
    }

    private static IImageSource GetUnitSourceImage(IUnit unit, IUserInterface active, UnitImage unitImage, bool useMapArt)
    {
        if (useMapArt)
        {
            return unitImage.MapImage ?? unitImage.Image;
        }

        // UI controls need the small classic unit sheet even if map rendering uses
        // 1024px FOSS art. UnitLoader keeps UnitImage.Image/UiImage on the classic
        // Civ2 sprite and stores FOSS art separately in UnitImage.MapImage.
        if (unitImage.UiImage is { } uiImage)
        {
            return uiImage;
        }

        var unitIndex = (int)unit.Type;
        if (active.PicSources.TryGetValue("unit", out var unitIcons) &&
            unitIndex >= 0 && unitIndex < unitIcons.Length)
        {
            return unitIcons[unitIndex];
        }

        return unitImage.Image;
    }

    private static Vector2 GetLogicalUnitSize(IUserInterface active, UnitImage unitImage, Texture2D unitTexture)
    {
        if (unitImage.LogicalSize != Vector2.Zero)
        {
            return unitImage.LogicalSize;
        }

        var unitRectangle = active.UnitImages.UnitRectangle;
        if (unitRectangle.Width > 0 && unitRectangle.Height > 0)
        {
            return new Vector2(unitRectangle.Width, unitRectangle.Height);
        }

        return new Vector2(unitTexture.Width, unitTexture.Height);
    }

    private static float GetUnitRenderScale(UnitImage unitImage, Texture2D unitTexture, Vector2 logicalSize)
    {
        var renderScale = unitImage.DrawScale > 0 ? unitImage.DrawScale : 1f;
        if (unitTexture.Width <= 0 || unitTexture.Height <= 0 || logicalSize.X <= 0 || logicalSize.Y <= 0)
        {
            return renderScale;
        }

        var drawWidth = unitTexture.Width * renderScale;
        var drawHeight = unitTexture.Height * renderScale;
        if (drawWidth <= logicalSize.X && drawHeight <= logicalSize.Y)
        {
            return renderScale;
        }

        // Defensive fallback for any UI path that receives a high-resolution FOSS
        // texture before UnitLoader has populated DrawScale metadata.
        return MathF.Max(0.01f, MathF.Min(logicalSize.X / unitTexture.Width, logicalSize.Y / unitTexture.Height));
    }

    private static Vector2 GetUnitDrawOffset(UnitImage unitImage, Texture2D unitTexture, Vector2 logicalSize, float renderScale)
    {
        if (unitImage.DrawOffset != Vector2.Zero)
        {
            return unitImage.DrawOffset;
        }

        var drawWidth = unitTexture.Width * renderScale;
        var drawHeight = unitTexture.Height * renderScale;
        return new Vector2(
            MathF.Max(0, (logicalSize.X - drawWidth) / 2f),
            MathF.Max(0, logicalSize.Y - drawHeight));
    }

    private static string GetOrderShieldText(IUnit unit, IGame game)
    {
        if (unit is Unit { Building: > 0 } worker)
        {
            if (worker.Building == ImprovementTypes.Road)
            {
                return "R";
            }

            if (worker.Building == ImprovementTypes.Irrigation)
            {
                return "I";
            }

            if (worker.Building == ImprovementTypes.Mining)
            {
                return "M";
            }
        }

        if (unit.Order == (int)OrderType.Automate)
        {
            return "A";
        }

        return unit.Order > 0 && unit.Order <= game.Rules.Orders.Length
            ? game.Rules.Orders[unit.Order - 1].Key
            : "-";
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
