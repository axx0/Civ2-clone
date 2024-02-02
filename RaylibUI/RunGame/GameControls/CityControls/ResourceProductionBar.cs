using System.Numerics;
using Model;
using Model.Interface;
using Raylib_cs;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class ResourceProductionBar : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly ResourceArea _resource;
    private int _spacing;
    private int _mid;
    private List<ProdSection> _sections;
    private int _iconWidth;

    public ResourceProductionBar(CityWindow cityWindow, ResourceArea resource) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _resource = resource;
        AbsolutePosition = resource.Bounds;
        _cityWindow.ResourceProductionChanged += (_, _) => CalculateContents();

    }

    public override void OnResize()
    {
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
             _iconWidth = mainImage.Width;

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
        }else if (_resource is SharedResourceArea sharedResourceArea)
        {
            foreach (var resource in sharedResourceArea.Resources)
            {
                var value = _cityWindow.City.GetResourceValues(resource.Name);
                sections.Add(new ProdSection(label: resource.GetResourceLabel(value, _cityWindow.City),
                    value: value,
                    icon: TextureCache.GetImage(resource.Icon)));
            }

            _iconWidth = sections[0].Icon.Width;
        }
        else
        {
            throw new NotImplementedException("Unknown area type " + _resource.GetType());
        }

        var requiredSpace = sections.Sum(s => s.Value * _iconWidth) + 2 * _iconWidth;
        if (requiredSpace < Width)
        {
            _spacing = _iconWidth;
            _mid = sections.Count < 3
                ? -1
                : (Width - requiredSpace)/2 + sections[0].Value * _iconWidth;
        }
        else
        {
            var minSpace = (sections.Count * 2 - 1) * _iconWidth;
            var remainingSpace = Width - minSpace;
            var points = sections.Sum(s => s.Value > 0 ? s.Value - 1 : 0);
            _spacing = Math.DivRem(remainingSpace, points, out remainingSpace);

            _mid = sections.Count < 3 ? -1 : 2 * _iconWidth + sections[0].Value * _spacing + remainingSpace / 2;
        }

        _sections = sections;
    }

    public override void Draw(bool pulse)
    {
        base.Draw(pulse);
        
        var textDim = Raylib.MeasureTextEx(Fonts.Arial, _sections[0].Label, 14, 1);
        var labely = Location.Y + (_resource.LabelBelow ? Bounds.Height : 1-textDim.Y);
            
        Raylib.DrawTextEx(Fonts.Arial, _sections[0].Label, new Vector2(Location.X + 1, labely),14,1,Color.WHITE);
        var pos = Location + Vector2.One;
        for (int i = 0; i < _sections[0].Value; i++)
        {
            Raylib.DrawTextureEx(_sections[0].Icon, pos,0,1,Color.WHITE);
            pos.X += _spacing;
        }

        var final = 1;
        if (_mid != -1)
        {
            pos = Location + Vector2.One;
            pos.X += _mid;
            for (int i = 0; i < _sections[1].Value; i++)
            {
                Raylib.DrawTextureEx(_sections[1].Icon, pos,0,1,Color.WHITE);
                pos.X += _spacing;
            }
            var midText = _sections[1].Label;
            var midSize = Raylib.MeasureTextEx(Fonts.Arial, midText, 14, 1);
            Raylib.DrawTextEx(Fonts.Arial, midText, new Vector2(Location.X + Width/2f - midSize.X/2, labely),14,1,Color.WHITE);

            final = 2;
        }
        pos = Location + Vector2.One;
        pos.X += Width - _iconWidth -2;
        for (int i = 0; i < _sections[final].Value; i++)
        {
            Raylib.DrawTextureEx(_sections[final].Icon, pos,0,1,Color.WHITE);
            pos.X -= _spacing;
        }

        var finalText = _sections[final].Label;
        var finalSize = Raylib.MeasureTextEx(Fonts.Arial, finalText, 14, 1);
        Raylib.DrawTextEx(Fonts.Arial, finalText, new Vector2(Location.X + Width - finalSize.X -1, labely),14,1,Color.WHITE);

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