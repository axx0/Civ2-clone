using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using Model.Images;
using RaylibUtils;
using Model.Controls;
using Model;

namespace RaylibUI.BasicTypes.Controls;

public class ImageBox : BaseControl
{
    private readonly IUserInterface _active;

    public ImageBox(IControlLayout controller, DialogImageElements image, bool eventTransparent = true) : base(controller, eventTransparent)
    {
        _active = controller.MainWindow.ActiveInterface;
        Image = image.Image ?? [];
        Scale = image.Scale;
        Coords = image.Coords;
    }

    public ImageBox(IControlLayout controller, IImageSource? image, float scale = 1.0f, bool eventTransparent = true) : base(controller, eventTransparent)
    {
        _active = controller.MainWindow.ActiveInterface;
        Image = [image];
        Scale = scale;
    }

    private IImageSource?[] _image = [];
    public IImageSource?[] Image 
    {
        get => _image;
        set 
        {
            _image = value;
            Width = GetPreferredWidth();
            Height = GetPreferredHeight();
        }
    }

    public int[,] Coords { get; set; } = new int[,] { { 0, 0 } };

    private float _scale = 1.0f;
    public float Scale 
    {
        get => _scale;
        set
        {
            if (value != _scale)
            {
                _scale = value;
                Width = GetPreferredWidth();
                Height = GetPreferredHeight();
            }
        }
    }

    public override int GetPreferredWidth()
    {
        return Image.Length == 0 ? 0 : Image.Select(img => Images.GetImageWidth(img, _active, _scale)).Max();
    }

    public override int GetPreferredHeight()
    {
        return Image.Length == 0 ? 0 : Image.Select(img => Images.GetImageHeight(img, _active, _scale)).Max();
    }

    public void FitWithin(int maxWidth, int maxHeight, int padding = 0, float maxScale = 1f)
    {
        maxWidth = Math.Max(1, maxWidth);
        maxHeight = Math.Max(1, maxHeight);

        var sources = Image.Where(img => img is not null).ToList();
        if (sources.Count == 0)
        {
            _scale = 1f;
            Width = maxWidth;
            Height = maxHeight;
            Coords = new int[Math.Max(1, Image.Length), 2];
            return;
        }

        var sourceSizes = sources
            .Select(img => new
            {
                Width = Math.Max(1, Images.GetImageWidth(img, _active)),
                Height = Math.Max(1, Images.GetImageHeight(img, _active))
            })
            .ToList();

        var maxSourceWidth = sourceSizes.Max(size => size.Width);
        var maxSourceHeight = sourceSizes.Max(size => size.Height);
        var availableWidth = Math.Max(1, maxWidth - 2 * padding);
        var availableHeight = Math.Max(1, maxHeight - 2 * padding);
        var fitScale = Math.Min(availableWidth / (float)maxSourceWidth, availableHeight / (float)maxSourceHeight);

        _scale = Math.Max(0.01f, Math.Min(maxScale, fitScale));

        var drawWidth = (int)Math.Ceiling(maxSourceWidth * _scale);
        var drawHeight = (int)Math.Ceiling(maxSourceHeight * _scale);
        Width = Math.Min(maxWidth, drawWidth + 2 * padding);
        Height = Math.Min(maxHeight, drawHeight + 2 * padding);

        Coords = new int[Math.Max(1, Image.Length), 2];
        for (var i = 0; i < Image.Length; i++)
        {
            var image = Image[i];
            if (image is null)
            {
                continue;
            }

            var imageWidth = Images.GetImageWidth(image, _active, _scale);
            var imageHeight = Images.GetImageHeight(image, _active, _scale);
            Coords[i, 0] = padding + Math.Max(0, (Width - 2 * padding - imageWidth) / 2);
            Coords[i, 1] = padding + Math.Max(0, (Height - 2 * padding - imageHeight) / 2);
        }
    }

    public void FitIntoSlot(int slotWidth, int slotHeight, int padding = 0, float maxScale = 1f)
    {
        FitWithin(slotWidth, slotHeight, padding, maxScale);
        Width = Math.Max(1, slotWidth);
        Height = Math.Max(1, slotHeight);

        for (var i = 0; i < Image.Length; i++)
        {
            var image = Image[i];
            if (image is null)
            {
                continue;
            }

            var imageWidth = Images.GetImageWidth(image, _active, _scale);
            var imageHeight = Images.GetImageHeight(image, _active, _scale);
            Coords[i, 0] = padding + Math.Max(0, (Width - 2 * padding - imageWidth) / 2);
            Coords[i, 1] = padding + Math.Max(0, (Height - 2 * padding - imageHeight) / 2);
        }
    }

    private int _width;
    public override int Width
    {
        get
        {
            if (_width == 0)
            {
                _width = GetPreferredWidth();
            }

            return _width;
        }
        set { _width = value; }
    }

    private int _height;
    public override int Height
    {
        get
        {
            if (_height == 0)
            {
                _height = GetPreferredHeight();
            }

            return _height;
        }
        set { _height = value; }
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        if (_image != null)
        {
            for (int i = 0; i < _image.Length; i++)
            {
                var image = _image[i];
                if (image is null)
                {
                    continue;
                }

                DrawImageInsideBounds(image, i);
            }
        }

        // Draw control's bounds
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }

    private void DrawImageInsideBounds(IImageSource image, int index)
    {
        var texture = TextureCache.GetImage(image);
        if (texture.Width <= 0 || texture.Height <= 0)
        {
            return;
        }

        var coordX = index < Coords.GetLength(0) ? Coords[index, 0] : 0;
        var coordY = index < Coords.GetLength(0) ? Coords[index, 1] : 0;
        var boxWidth = Math.Max(1, Width);
        var boxHeight = Math.Max(1, Height);
        var drawScale = Math.Max(0.01f, _scale);
        var drawWidth = texture.Width * drawScale;
        var drawHeight = texture.Height * drawScale;

        // A lot of older UI code constrains ImageBox.Width/Height after construction.
        // Keep that contract: once a box has a smaller slot, never let high-resolution
        // FOSS art spill outside it and cover neighboring buttons or panels.
        if (drawWidth > boxWidth || drawHeight > boxHeight)
        {
            drawScale = Math.Max(0.01f, Math.Min(boxWidth / (float)texture.Width, boxHeight / (float)texture.Height));
            drawWidth = texture.Width * drawScale;
            drawHeight = texture.Height * drawScale;
            coordX = Math.Max(0, (int)Math.Round((boxWidth - drawWidth) / 2f));
            coordY = Math.Max(0, (int)Math.Round((boxHeight - drawHeight) / 2f));
        }

        Graphics.DrawTexturePro(texture,
            new Rectangle(0, 0, texture.Width, texture.Height),
            new Rectangle(Bounds.X + coordX, Bounds.Y + coordY, drawWidth, drawHeight),
            new System.Numerics.Vector2(0, 0), 0f, Color.White);
    }
}
