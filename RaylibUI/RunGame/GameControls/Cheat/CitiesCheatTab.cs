using Model;
using Model.Controls;
using Model.Core.Cities;
using Model.Interface;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
using Raylib_CSharp.Colors;

namespace RaylibUI.RunGame.GameControls;

public class CitiesCheatTab : BaseControl
{
    private readonly Listbox _civBox, _cityBox;
    private readonly GameScreen _gameScreen;
    private readonly IUserInterface? _active;
    private readonly ImageBox _cityImage;
    private List<City> _citiesList;
    private City? _selectedCity;
    private readonly LabelControl _cityName;
    private readonly OptionsPanel _ownersPanel;
    private readonly MasterCheatEntries _cheatEntries;

    public CitiesCheatTab(MasterCheatDialog dialog, GameScreen gameScreen) : base(dialog)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        _cheatEntries = dialog.CheatEntries;

        _cityImage = new ImageBox(dialog, null, 2.0f, true);
        Controls.Add(_cityImage);

        _cityName = new(dialog, string.Empty, true, fontSize: 32, font: Fonts.Tnr,
            colorShadow: Color.Black, shadowOffset: new System.Numerics.Vector2(1, 1));
        Controls.Add(_cityName);

        var civs = gameScreen.Game.AllCivilizations.Where(c => c.Alive).Select(c => c.TribeName);
        civs = civs.Prepend("All civs");
        _civBox = new Listbox(dialog)
        {
            Type = ListboxType.Small,
            Groups = civs.Select(t => new ListboxGroup(t)).ToList(),
        };
        _civBox.Width = _civBox.Controls.OfType<ListboxControlGroup>().Max(w => w.Width) + 15;
        Controls.Add(_civBox);

        _cityBox = new Listbox(dialog)
        {
            Type = ListboxType.Small,
            Groups = gameScreen.Game.AllCities.Select(c => c.Name).Select(t => new ListboxGroup(t)).ToList()
        };
        _cityBox.Width = _cityBox.Controls.OfType<ListboxControlGroup>().Max(w => w.Width) + 15;
        Controls.Add(_cityBox);

        _ownersPanel = new OptionsPanel(dialog);
        _ownersPanel.Texts = gameScreen.Game.AllCivilizations.Select(c => c.TribeName).ToList();
        _ownersPanel.Type = OptionsType.Small;
        _ownersPanel.ItemSelected += (_, args) => 
        {
            _cheatEntries.Cities[_cityBox.SelectedId].OwnerId = args.Index;
        };
        
        Controls.Add(_ownersPanel);

        _civBox.ItemSelected += CivSelected;
        _cityBox.ItemSelected += CitySelected;
        _citiesList = _gameScreen.Game.AllCities;

        dialog.Focused = _civBox;
    }

    public override void OnResize()
    {
        _civBox.Height = this.Height;
        _civBox.Location = new(2, 2);
        _cityBox.Location = new(_civBox.Location.X + _civBox.Width, _civBox.Location.Y);
        _cityName.Location = new(_cityBox.Location.X + _cityBox.Width, _cityBox.Location.Y);
        _cityImage.Location = new(_cityBox.Location.X + _cityBox.Width, _cityName.Location.Y + _cityName.Height);
        _ownersPanel.Location = new(_cityImage.Location.X + _cityImage.Width, _cityImage.Location.Y);
        _ownersPanel.Width = 100;

        _ownersPanel.Visible = _selectedCity != null;

        foreach (var c in Controls)
        {
            c.OnResize();
        }
    }

    private void CivSelected(object? sender, ListboxSelectionEventArgs args)
    {
        _citiesList = args.Index == 0 ?
            _gameScreen.Game.AllCities :
            _gameScreen.Game.AllCities.Where(c => c.OwnerId == args.Index - 1 && c.Owner.Alive).ToList();
        _cityBox.Groups = _citiesList.Select(c => c.Name).Select(t => new ListboxGroup(t)).ToList();


        if (_citiesList.Count == 0)
        {
            _selectedCity = null;
            _cityImage.Image = [null];
            _cityName.Text = string.Empty;
        }

        OnResize();
    }

    private void CitySelected(object? sender, ListboxSelectionEventArgs args)
    {
        if (_citiesList.Count > 0)
        {
            _selectedCity = _citiesList[_cityBox.SelectedId];
            var civ = _selectedCity.Owner;
            var cityStyleIndex = _active.GetCityStyleIndexFromEpoch(civ.CityStyle, civ.Epoch);
            var sizeIncrement = _active.GetCityIndexForStyle(cityStyleIndex, _selectedCity, _selectedCity.Size);
            var cityImage = _active.CityImages.Sets[cityStyleIndex][sizeIncrement];
            var cityImgWidth = Images.GetImageWidth(cityImage.Image, _active);

            _cityName.Text = _selectedCity.Name;
            _cityName.ColorFront = _active.PlayerColours[civ.Id].TextColour;

            _cityImage.Image = [cityImage.Image, _active.PlayerColours[civ.Id].Image];
            var flagH = Images.GetImageHeight(_active.PlayerColours[civ.Id].Image, _active, 1.0f);
            _cityImage.Coords = new int[,] { { 0, 0 }, 
                { 2 * (int)cityImage.FlagLoc.X, 2 * (int)cityImage.FlagLoc.Y - flagH } };

            _ownersPanel.SelectedId = _cheatEntries.Cities[_cityBox.SelectedId].OwnerId;
        }
        
        OnResize();
    }

    public override void Draw(bool pulse)
    {
        //Graphics.DrawRectangleRec(Bounds, Raylib_CSharp.Colors.Color.Red);

        base.Draw(pulse);
    }
}
