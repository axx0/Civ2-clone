using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Controls;
using Model.Core;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls;

public class MasterCheatDialog : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly GameScreen _gameScreen;
    private const int InnerWidth = 1000;
    private const int InnerHeight = 600;
    private readonly IGame _game;

    public MasterCheatEntries CheatEntries { get; set; } = new();

    public MasterCheatDialog(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        _game = _gameScreen.Game;

        // Copy properties to new object
        for (int i = 0; i < _game.AllCities.Count(); i++)
        {
            CheatEntries.Cities.Add(new()
            {
                OwnerId = _game.AllCities.ElementAt(i).OwnerId
            });
        }

        LayoutPadding = new(45, 11, 42, 11);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding);

        var headerLabel = new HeaderLabel(this, _active.Look, "Mastercheat", _active.Look.HeaderLabelFontSizeLarge);
        headerLabel.Location = new Vector2(InnerWidth / 2 - headerLabel.Width / 2, headerLabel.Location.Y);
        Controls.Add(headerLabel);

        var btnWidth = (Width - PaddingSide - 6) / 2;
        var cancelBtn = new Button(this, Labels.For(LabelIndex.Cancel), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left, Height - LayoutPadding.Bottom + 4),
            Width = btnWidth,
            Height = LayoutPadding.Bottom - 12
        };
        cancelBtn.Click += (_, _) => _gameScreen.CloseDialog(this);
        Controls.Add(cancelBtn);

        var okBtn = new Button(this, Labels.For(LabelIndex.OK), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 4 + btnWidth, Height - LayoutPadding.Bottom + 4),
            Width = btnWidth,
            Height = LayoutPadding.Bottom - 12
        };
        okBtn.Click += (_, _) => 
        {
            ChangeCityOwners();
            _gameScreen.CloseDialog(this);
            _gameScreen.MapControl.ForceRedraw = true;
        };
        Controls.Add(okBtn);

        var tabs = new TabControl(this, new Dictionary<string, IControl> { 
            { "General", null },
            { "Civilizations", null },
            { "Cities", new CitiesCheatTab(this, gameScreen) },
        })
        {
            Width = InnerWidth - 4,
            Height = InnerHeight - 4,
            Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 2)
        };
        Controls.Add(tabs);
    }

    public override int Width => InnerWidth + PaddingSide;
    public override int Height => InnerHeight + LayoutPadding.Top + LayoutPadding.Bottom;

    public override void Resize(int width, int height)
    {
        SetLocation(width, Width, height, Height);

        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    private void ChangeCityOwners()
    {
        for (var i = 0; i < _game.AllCities.Count(); i++)
        {
            var city = _game.AllCities.ElementAt(i);
            var currentCityOwnerId = city.Owner.Id;
            var newOwner = _game.AllCivilizations[CheatEntries.Cities[i].OwnerId];
            var newOwnerId = CheatEntries.Cities[i].OwnerId;
            
            if (currentCityOwnerId == newOwnerId) continue;

            var cityTile = city.Location;

            // Change city's owner
            city.Owner.Cities.Remove(city);
            city.Owner = newOwner;
            newOwner.Cities.Add(city);

            // Current & new owner should have knowledge of this event
            cityTile.PlayerKnowledge[currentCityOwnerId].CityHere.OwnerId = newOwnerId;
            cityTile.PlayerKnowledge[newOwnerId].CityHere = new()
            {
                Name = city.Name,
                OwnerId = newOwnerId,
                Size = city.Size,
            };

            // Supported units
            foreach (var supportedUnit in city.SupportedUnits.Where(u => u.CurrentLocation != cityTile))
            {
                supportedUnit.HomeCity = null;
            }

            // Units in city also swap owners
            foreach (var unitInCity in city.UnitsInCity)
            {
                unitInCity.Owner = newOwner;
            }

        }
    }
}
