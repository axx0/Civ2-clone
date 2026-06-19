using Civ2engine;
using Model;
using Model.Controls;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ResourceProductionBar : BaseControl
{
    private const int LabelFontSize = 11;
    private const int LabelSpacing = 0;
    private const int LabelInset = 2;
    private readonly CityWindow _cityWindow;
    private readonly ResourceArea _resource;
    private int _spacing;
    private int _mid;
    private List<ProdSection> _sections = [];
    private int _iconStep;
    private float _iconScale = 1f;
    private int _iconTargetHeight;

    public ResourceProductionBar(CityWindow cityWindow, ResourceArea resource) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _resource = resource;
        _cityWindow.ResourceProductionChanged += (_, _) => CalculateContents();
        Controls = [];
    }

    public override void OnResize()
    {
        Location = new(_cityWindow.LayoutPadding.Left + _resource.Bounds.X * _cityWindow.Scale,
            _cityWindow.LayoutPadding.Top + _resource.Bounds.Y * _cityWindow.Scale);
        Width = (int)(_resource.Bounds.Width * _cityWindow.Scale);
        Height = (int)(_resource.Bounds.Height * _cityWindow.Scale);

        //var pos = _resource.Bounds.ScaleAll(_cityWindow.Scale);
        //Location = new(pos.X, pos.Y);
        //Width = (int)pos.Width;
        //Height = (int)pos.Height;
        base.OnResize();
        CalculateContents();
    }

    private void CalculateContents()
    {
        var sections = new List<ProdSection>();
        if (_resource is ConsumableResourceArea consumableResource)
        {
            var resourceImage =
                _cityWindow.CurrentGameScreen.Main.ActiveInterface.ResourceImages.First(i => i.Name == consumableResource.Name);

            var mainImage = TextureCache.GetImage(resourceImage.LargeImage);
            var lossImage = TextureCache.GetImage(resourceImage.LossImage);
            
            var values = _cityWindow.City.GetConsumableResourceValues(consumableResource.Name);

            sections.Add(
                new ProdSection(label: consumableResource.GetDisplayDetails(values.Consumption, OutputType.Consumption), value: values.Consumption,
                    icon: mainImage));

            if (values.Loss != 0 || consumableResource.NoSurplus)
            {
                sections.Add(
                    new ProdSection(label: consumableResource.GetDisplayDetails(values.Loss, OutputType.Loss), value: Math.Abs(values.Loss), icon: lossImage));
            }

            if (values.Surplus > 0 || (values.Loss == 0 && !consumableResource.NoSurplus))
            {
                sections.Add(new ProdSection(label: consumableResource.GetDisplayDetails(values.Surplus, OutputType.Surplus), value: values.Surplus,
                    icon: mainImage));
            }
        }
        else if (_resource is SharedResourceArea sharedResourceArea)
        {
            foreach (var resource in sharedResourceArea.Resources)
            {
                var value = _cityWindow.City.GetResourceValues(resource.Name);
                sections.Add(new ProdSection(label: resource.GetResourceLabel(value, _cityWindow.City),
                    value: value,
                    icon: TextureCache.GetImage(resource.Icon)));
            }
        }
        else
        {
            throw new NotImplementedException("Unknown area type " + _resource.GetType());
        }

        if (sections.Count == 0)
        {
            _sections = [];
            return;
        }

        CalculateIconMetrics(sections);

        var requiredSpace = sections.Sum(s => Math.Max(0, s.Value) * _iconStep) + 2 * _iconStep;
        if (requiredSpace < Width)
        {
            _spacing = _iconStep;
            _mid = sections.Count < 3
                ? -1
                : (Width - requiredSpace) / 2 + Math.Max(0, sections[0].Value) * _iconStep;
        }
        else
        {
            var minSpace = (sections.Count * 2 - 1) * _iconStep;
            var remainingSpace = Width - minSpace;
            var points = sections.Sum(s => s.Value > 0 ? s.Value - 1 : 0);
            _spacing = points <= 0
                ? _iconStep
                : Math.Max(1, Math.DivRem(Math.Max(1, remainingSpace), points, out remainingSpace));

            _mid = sections.Count < 3 ? -1 : 2 * _iconStep + Math.Max(0, sections[0].Value) * _spacing + remainingSpace / 2;
        }

        _sections = sections;
    }

    private void CalculateIconMetrics(IReadOnlyCollection<ProdSection> sections)
    {
        var maxIconWidth = Math.Max(1, sections.Max(s => s.Icon.Width));
        var maxIconHeight = Math.Max(1, sections.Max(s => s.Icon.Height));
        var targetHeight = Math.Max(8, Height - 2);

        _iconScale = Math.Min(1.35f, targetHeight / (float)maxIconHeight);
        _iconTargetHeight = Math.Max(1, (int)Math.Ceiling(maxIconHeight * _iconScale));
        _iconStep = Math.Max(1, (int)Math.Ceiling(maxIconWidth * _iconScale) + 1);
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);

        if (_sections.Count == 0)
        {
            return;
        }
        
        var fontSize = Math.Max(10, (int)Math.Round(LabelFontSize * Math.Min(1.25f, _cityWindow.Scale * 0.85f)));
        var textDim = TextManager.MeasureTextEx(Fonts.Arial, _sections[0].Label, fontSize, LabelSpacing);
        var labely = GetLabelY(textDim);
            
        DrawResourceLabel(_sections[0].Label, new Vector2(Bounds.X + LabelInset * _cityWindow.Scale, labely), fontSize);
        var pos = new Vector2(Bounds.X + 1, Bounds.Y + Math.Max(0, (Bounds.Height - _iconTargetHeight) / 2f));
        for (int i = 0; i < _sections[0].Value; i++)
        {
            DrawIcon(_sections[0].Icon, pos);
            pos.X += _spacing;
        }

        if (_sections.Count == 1)
        {
            return;
        }

        var final = 1;
        if (_mid != -1)
        {
            pos = new Vector2(Bounds.X + 1, Bounds.Y + Math.Max(0, (Bounds.Height - _iconTargetHeight) / 2f));
            pos.X += _mid;
            for (int i = 0; i < _sections[1].Value; i++)
            {
                DrawIcon(_sections[1].Icon, pos);
                pos.X += _spacing;
            }
            var midText = _sections[1].Label;
            var midSize = TextManager.MeasureTextEx(Fonts.Arial, midText, fontSize, LabelSpacing);
            DrawResourceLabel(midText, new Vector2(Bounds.X + Width / 2f - midSize.X / 2, labely), fontSize);

            final = 2;
        }
        pos = new Vector2(Bounds.X + 1, Bounds.Y + Math.Max(0, (Bounds.Height - _iconTargetHeight) / 2f));
        pos.X += Width - _iconStep - 2;
        for (int i = 0; i < _sections[final].Value; i++)
        {
            DrawIcon(_sections[final].Icon, pos);
            pos.X -= _spacing;
        }

        var finalText = _sections[final].Label;
        var finalLabelInset = (_resource.LabelBelow ? 8 : LabelInset) * _cityWindow.Scale;
        DrawRightAlignedResourceLabel(finalText, Bounds.X + Width - finalLabelInset, labely, fontSize);

    }

    private float GetLabelY(Vector2 textDim)
    {
        if (_resource.LabelBelow)
        {
            return Bounds.Y + Bounds.Height + Math.Max(1, _cityWindow.Scale);
        }

        return Bounds.Y + 1 - textDim.Y;
    }

    private static void DrawResourceLabel(string text, Vector2 position, int fontSize)
    {
        global::RaylibUI.TextRendering.DrawWithShadow(Fonts.Arial, text, position, fontSize, LabelSpacing, Color.White, Color.Black, Vector2.One);
    }

    private static void DrawRightAlignedResourceLabel(string text, float rightEdge, float y, int fontSize)
    {
        var textSize = TextManager.MeasureTextEx(Fonts.Arial, text, fontSize, LabelSpacing);
        DrawResourceLabel(text, new Vector2(rightEdge - textSize.X, y), fontSize);
    }

    private void DrawIcon(Texture2D icon, Vector2 slotPosition)
    {
        var drawWidth = icon.Width * _iconScale;
        var drawHeight = icon.Height * _iconScale;
        var x = slotPosition.X + Math.Max(0, (_iconStep - drawWidth) / 2f);
        var y = slotPosition.Y + Math.Max(0, (_iconTargetHeight - drawHeight) / 2f);
        Graphics.DrawTextureEx(icon, new Vector2(x, y), 0, _iconScale, Color.White);
    }
}

internal class ProdSection
{
    public ProdSection(string label, int value, Texture2D icon)
    {
        Label = label;
        Value = value;
        Icon = icon;
    }

    public string Label { get; init; }
    public int Value { get; init; }
    public Texture2D Icon { get; init; }
}
