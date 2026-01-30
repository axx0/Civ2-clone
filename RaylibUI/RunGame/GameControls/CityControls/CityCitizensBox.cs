using Civ2engine;
using Model;
using Model.Controls;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CityCitizensBox : BaseControl
{
    private readonly CityWindow _cityWindow;
    private readonly CityWindowLayout _props;
    private readonly IUserInterface _active;
    private readonly ImageBox[] _icons;
    private readonly City _city;
    private readonly int _epoch, _specialistsStart;
    private readonly int[] _citizenIndex;
    
    public CityCitizensBox(CityWindow cityWindow) : base(cityWindow)
    {
        _cityWindow = cityWindow;
        _city = _cityWindow.City;
        _active = cityWindow.MainWindow.ActiveInterface;
        _props = _cityWindow.CityWindowProps;

        _epoch = _city.Owner.Epoch;
        _specialistsStart = _city.Size - (_city.NoOfSpecialistsx4 / 4);
        _citizenIndex = new int[_city.Size];
        _icons = new ImageBox[_city.Size];
        for (var i = 0; i < _icons.Length; i++)
        {
            if (i >= _specialistsStart)
            {
                _citizenIndex[i] = 8;    // initialize specialists icon. This is probably read from somewhere in sav file.
            }

            _icons[i] = new ImageBox(_cityWindow, _active.PicSources["people"][0], eventTransparent: false);
            _icons[i].Click += OnClick;
            Controls.Add(_icons[i]);
        }
    }

    public override void OnResize()
    {
        var pos = _props.CitizensBox.ScaleAll(_cityWindow.Scale);
        Location = new(_cityWindow.LayoutPadding.Left + pos.X, _cityWindow.LayoutPadding.Top + pos.Y);
        Width = (int)pos.Width;
        Height = (int)pos.Height;
        base.OnResize();

        var iconWidth = Images.GetImageWidth(_active.PicSources["people"][0], _active);
        int spacing = 0;
        if (_city.Size > 2)
        {
            spacing = Math.Min(((int)_props.CitizensBox.Width - 4 - iconWidth) / (_city.Size - 1), iconWidth + 1);
        }

        for (var i = 0; i < _icons.Length; i++)
        {
            if (i < _specialistsStart)
            {
                _citizenIndex[i] = 2 + i % 2;   // show just workers for now
            }
            _icons[i].Image = [_active.PicSources["people"][_citizenIndex[i] + 11 * _epoch]];

            _icons[i].Location = new((2 + i * spacing) * _cityWindow.Scale, 7 * _cityWindow.Scale);
            _icons[i].Scale = _cityWindow.Scale;
        }

        foreach (var child in Controls)
        {
            child.OnResize();
        }
    }

    private void OnClick(object? sender, MouseEventArgs e) 
    {
        // Change specialist
        var index = Array.IndexOf(_icons, sender);
        if (index >= _specialistsStart)
        {
            _citizenIndex[index]++;
            if (_citizenIndex[index] > 10)
            {
                _citizenIndex[index] = 8;
            }
        }
        OnResize();
    }
}