using Civ2engine;
using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUtils;

namespace RaylibUI.RunGame.GameControls;

public class TaxRateWindow : BaseDialog
{
    private readonly HeaderLabel _headerLabel;
    private readonly IUserInterface _active;
    private readonly TaxLabel _govLabel, _taxLabel, _sciLabel, _luxLabel; 
    private readonly int _max, _width, _height;
    private readonly ScrollBar _barTax, _barSci, _barLux;
    private bool _taxLocked, _sciLocked, _luxLocked;
    private int _tax, _sci, _lux;
    private readonly GameScreen _gameScreen;
    private readonly IImageSource[] _checkboxImg;
    private readonly ImageBox[] _lockImages;
    private readonly Civilization _civ;

    public TaxRateWindow(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        _civ = gameScreen.Game.GetPlayerCiv;
        _tax = _civ.TaxRate;
        _sci = _civ.ScienceRate;
        _lux = _civ.LuxRate;
        _taxLocked = false;
        _sciLocked = false;
        _luxLocked = false;
        _max = _civ.Government switch
        {
            0 or 1 => 60,    // anarchy, despotism
            2 => 70,    // monarchy
            3 or 4 or 5 => 80,    // communism, fundam., republic
            6 => 90, // democracy
            _ => throw new ArgumentOutOfRangeException($"Not expected government value: {_civ.Government}")
        };

        _headerLabel = new HeaderLabel(this, _active.Look, $"How Shall We Distribute The Wealth",
                        fontSize: _active.Look.HeaderLabelFontSizeNormal);
        Controls.Add(_headerLabel);

        LayoutPadding = _active.GetPadding(_headerLabel?.TextSize.Y ?? 0, false);

        var back = _active.PicSources["taxRateBack"][0];
        _width = Images.GetImageWidth(back, _active) + PaddingSide;
        _height = Images.GetImageHeight(back, _active) + LayoutPadding.Top + LayoutPadding.Bottom;

        BackgroundImage = ImageUtils.PaintDialogBase(_active, _width, _height, LayoutPadding,
            Images.ExtractBitmap(back, _active));
        _headerLabel.Width = _width;

        _govLabel = new TaxLabel(this, $"{Labels.For(LabelIndex.Government)}: {gameScreen.Game.Rules.Governments[_civ.Government].Name}" +
            $"  {Labels.For(LabelIndex.MaximumRate)}: {_max}%", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 10), Width = 535 };
        Controls.Add(_govLabel);

        Controls.Add(new TaxLabel(this, $"{Labels.For(LabelIndex.TotalIncome)}: ??  {Labels.For(LabelIndex.TotalCost)}: ??", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 64), Width = 535 });
        Controls.Add(new TaxLabel(this, $"{Labels.For(LabelIndex.Discoveries)}: ?? {Labels.For(LabelIndex.Turns)}", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left, LayoutPadding.Top + 91), Width = 535 });

        Controls.Add(new TaxLabel(this, Labels.For(LabelIndex.Lock), HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left + 520, LayoutPadding.Top + 140), Width = 80 });

        var btn = new Button(this, Labels.For(LabelIndex.OK), _active.Look.ButtonFont, 18)
        {
            Location = new(LayoutPadding.Left + 2, _height - LayoutPadding.Bottom - 2 - 28),
            Width = _width - PaddingSide - 4,
            Height = 28
        };
        btn.Click += CloseButtonOnClick;
        Controls.Add(btn);

        _checkboxImg = _active.Look.CheckBoxes;
        _lockImages = new ImageBox[3];

        // Taxes
        _taxLabel = new TaxLabel(this, "", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140), Width = 505 };
        Controls.Add(new TaxLabel(this, "0%") { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140), Width = 505 });
        Controls.Add(new TaxLabel(this, "100%", HorizontalAlignment.Right)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140), Width = 505 });
        Controls.Add(_taxLabel);
        _lockImages[0] = new ImageBox(this, _checkboxImg[_taxLocked ? 0 : 1], eventTransparent: false)
        { Location = new(LayoutPadding.Left + 545, LayoutPadding.Top + 166) };
        _lockImages[0].Click += (_, _) => 
        {
            if (_taxLocked)
            {
                _taxLocked = false;
            }
            else
            {
                _taxLocked = true;
                _sciLocked = false;
                _luxLocked = false;
            }
            _lockImages[0].Image = [_checkboxImg[_taxLocked ? 0 : 1]]; 
            _lockImages[1].Image = [_checkboxImg[_sciLocked ? 0 : 1]]; 
            _lockImages[2].Image = [_checkboxImg[_luxLocked ? 0 : 1]]; 
        };
        Controls.Add(_lockImages[0]);
        _barTax = new ScrollBar(this, i =>
            {
                if (10 * i != _tax)
                {
                    ChangeRates(10 * i, _sci, _lux);
                }
            }, false, 25)
        {
            Width = 505,
            Maximum = 10,
            Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 171)
        };
        Controls.Add(_barTax);

        // Science
        var yOffset = 61;
        _sciLabel = new TaxLabel(this, "", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 };
        Controls.Add(new TaxLabel(this, "0%") { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 });
        Controls.Add(new TaxLabel(this, "100%", HorizontalAlignment.Right)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 });
        Controls.Add(_sciLabel);
        _lockImages[1] = new ImageBox(this, _checkboxImg[_sciLocked ? 0 : 1], eventTransparent: false)
        { Location = new(LayoutPadding.Left + 545, LayoutPadding.Top + 166 + yOffset) };
        _lockImages[1].Click += (_, _) =>
        {
            if (_sciLocked)
            {
                _sciLocked = false;
            }
            else
            {
                _taxLocked = false;
                _sciLocked = true;
                _luxLocked = false;
            }
            _lockImages[0].Image = [_checkboxImg[_taxLocked ? 0 : 1]];
            _lockImages[1].Image = [_checkboxImg[_sciLocked ? 0 : 1]];
            _lockImages[2].Image = [_checkboxImg[_luxLocked ? 0 : 1]];
        };
        Controls.Add(_lockImages[1]);
        _barSci = new ScrollBar(this, i =>
            {
                if (10 * i != _sci)
                {
                    ChangeRates(_tax, 10 * i, _lux);
                }
            }, false, 25)
        {
            Width = 505,
            Maximum = 10,
            Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 171 + yOffset)
        };
        Controls.Add(_barSci);

        // Luxuries
        yOffset += 61;
        _luxLabel = new TaxLabel(this, "", HorizontalAlignment.Center)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 };
        Controls.Add(new TaxLabel(this, "0%") { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 });
        Controls.Add(new TaxLabel(this, "100%", HorizontalAlignment.Right)
        { Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 140 + yOffset), Width = 505 });
        Controls.Add(_luxLabel);
        _lockImages[2] = new ImageBox(this, _checkboxImg[_luxLocked ? 0 : 1], eventTransparent: false)
        { Location = new(LayoutPadding.Left + 545, LayoutPadding.Top + 166 + yOffset) };
        _lockImages[2].Click += (_, _) =>
        {
            if (_luxLocked)
            {
                _luxLocked = false;
            }
            else
            {
                _taxLocked = false;
                _sciLocked = false;
                _luxLocked = true;
            }
            _lockImages[0].Image = [_checkboxImg[_taxLocked ? 0 : 1]];
            _lockImages[1].Image = [_checkboxImg[_sciLocked ? 0 : 1]];
            _lockImages[2].Image = [_checkboxImg[_luxLocked ? 0 : 1]];
        };
        Controls.Add(_lockImages[2]);
        _barLux = new ScrollBar(this, i =>
            {
                if (10 * i != _lux)
                {
                    ChangeRates(_tax, _sci, 10 * i);
                }
            }, false, 25)
        {
            Width = 505,
            Maximum = 10,
            Location = new(LayoutPadding.Left + 15, LayoutPadding.Top + 171 + yOffset)
        };
        Controls.Add(_barLux);

        ChangeRates(_tax, _sci, _lux); 
    }

    public override int Width => _width;
    public override int Height => _height;

    public override void Resize(int width, int height)
    {
        SetLocation(width, Width, height, Height);

        foreach (var control in Controls)
        {
            control.OnResize();
        }
    }

    private void CloseButtonOnClick(object? sender, MouseEventArgs e)
    {
        _gameScreen.CloseDialog(this);
    }

    private void ChangeRates(int newTax, int newSci, int newLux)
    {
        if (newTax > _max || newSci > _max || newLux > _max)
        {
            _govLabel.ColorFront = new Color(243, 0, 0, 255);
            _govLabel.ColorShadow = Color.Black;
        }
        else
        {
            _govLabel.ColorFront = new Color(223, 223, 223, 255);
            _govLabel.ColorShadow = new Color(67, 67, 67, 255);

            var dTax = newTax - _tax;
            var dSci = newSci - _sci;
            var dLux = newLux - _lux;

            if (dTax != 0)
            {
                if (_sci - dTax >= 0 && _sci - dTax <= _max && !_sciLocked)
                {
                    _tax = newTax;
                    _sci -= dTax;
                }
                else if (_lux - dTax >= 0 && _lux - dTax <= _max && !_luxLocked)
                {
                    _tax = newTax;
                    _lux -= dTax;
                }
            }
            else if (dSci != 0)
            {
                if (_lux - dSci >= 0 && _lux - dSci <= _max && !_luxLocked)
                {
                    _sci = newSci;
                    _lux -= dSci;
                }
                else if (_tax - dSci >= 0 && _tax - dSci <= _max && !_taxLocked)
                {
                    _sci = newSci;
                    _tax -= dSci;
                }
            }
            else if (dLux != 0)
            {
                if (_tax - dLux >= 0 && _tax - dLux <= _max && !_taxLocked)
                {
                    _lux = newLux;
                    _tax -= dLux;
                }
                else if (_sci - dLux >= 0 && _sci - dLux <= _max && !_sciLocked)
                {
                    _lux = newLux;
                    _sci -= dLux;
                }
            }
        }

        if (_barTax.Position != _tax)
        {
            _barTax.SetScrollPosition(_tax / 10);
        }

        if (_barSci.Position != _sci)
        {
            _barSci.SetScrollPosition(_sci / 10);
        }

        if (_barLux.Position != _lux)
        {
            _barLux.SetScrollPosition(_lux / 10);
        }

        _taxLabel.Text = $"{Labels.For(LabelIndex.Taxes)}: {_tax}%";
        _sciLabel.Text = $"{Labels.For(LabelIndex.Science)}: {_sci}%";
        _luxLabel.Text = $"{Labels.For(LabelIndex.Luxuries)}: {_lux}%";

        _civ.TaxRate = _tax;
        _civ.ScienceRate = _sci;
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Escape || key == KeyboardKey.Enter || key == KeyboardKey.KpEnter)
        {
            _gameScreen.CloseDialog(this);
        }
        base.OnKeyPress(key);
    }
}

public class TaxLabel(IControlLayout layout, string text, HorizontalAlignment alignment = HorizontalAlignment.Left)
    : LabelControl(layout, text, true, horizontalAlignment: alignment,
        font: layout.MainWindow.ActiveInterface.Look.DefaultFont, 
        fontSize: layout.MainWindow.ActiveInterface.Look.HeaderLabelFontSizeNormal,
        colorFront: new Color(223, 223, 223, 255),
        colorShadow: new Color(67, 67, 67, 255),
        shadowOffset: new System.Numerics.Vector2(1, 1));
