using Civ2engine;
using Civ2engine.Terrains;
using Model;
using Model.Controls;
using Model.Core.Advances;
using Model.Core.Units;
using Model.Images;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;
using System.Numerics;
using Civ2engine.IO;

namespace RaylibUI.RunGame.GameControls;

public class CivilopediaWindow : BaseDialog
{
    private readonly IUserInterface _active;
    private readonly GameScreen _gameScreen;
    private const int InnerWidth = 624;
    private const int InnerHeight = 318;
    private Civilopedia _pedia;
    private readonly Button _exitIcon;
    private readonly Rules _rules;
    private readonly List<Advance> _advances;
    private readonly List<Improvement> _improvements, _wonders;
    private readonly List<UnitDefinition> _units;
    private readonly List<Government> _govs;
    private readonly List<ITerrain> _terrains = [];
    private List<Civilopedia> _ctrlHistory = [];
    private readonly List<string> _concepts;

    public CivilopediaWindow(GameScreen gameScreen, Civilopedia civilopedia) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;
        _active = gameScreen.MainWindow.ActiveInterface;
        _pedia = civilopedia;
        _rules = _gameScreen.Game.Rules;

        _advances = _rules.Advances.Take(89).OrderBy(x => x.Name).ToList();
        _improvements = _rules.Improvements.Skip(1).Take(_rules.FirstWonderIndex - 1).OrderBy(x => x.Name).ToList();
        _wonders = _rules.Improvements.Skip(_rules.FirstWonderIndex).OrderBy(x => x.Name).ToList();
        _units = _rules.UnitTypes.Take(51).OrderBy(x => x.Name).ToList();
        _govs = _rules.Governments.OrderBy(x => x.Name).ToList();
        for (var i = 0; i < _rules.Terrains[0].Length; i++)
        {
            var t = _rules.Terrains[0][i];
            _terrains.Add(t);
            if (i == 2) // Grassland has duplicate specials, skip one
            {
                _terrains.Add(t.Specials[0]);
            }
            else
            {
                _terrains.Add(t.Specials[0]);
                _terrains.Add(t.Specials[1]);
            }
        }
        _terrains = _terrains.OrderBy(x => x.Name).ToList();
        _concepts = CivilopediaLoader.ReadConceptsList().OrderBy(s => s[0]).ToList();  // Read a list of concepts from describe file

        LayoutPadding = new(45, 11, 42, 11);

        BackgroundImage = ImageUtils.PaintDialogBase(_active, Width, Height, LayoutPadding);

        _exitIcon = new Button(this, String.Empty, backgroundImage: _active.PicSources["close"][0], imageScale: 28f / 16f)
        {
            Location = new(11, 10)
        };
        _exitIcon.Click += (_, _) => _gameScreen.CloseDialog(this);

        UpdateControls();
    }

    public void UpdateControls()
    {
        _ctrlHistory.Add(_pedia.Clone());

        Controls.Clear();
        
        var props = _active.GetCivilopediaProperties(_pedia);
        var buttons = props.Buttons;
        
        switch (_pedia.WindowType)
        {
            case CivilopediaWindowType.Listbox:
                
                string[] names = [];
                IImageSource[][] icons = [];

                int iconOffset = 0;
                int iconWidth = 0;
                switch (_pedia.InfoType)
                {
                    case CivilopediaInfoType.Advances:
                        names = _advances.Select(a => a.Name).ToArray();
                        icons = new IImageSource[names.Length][];
                        for (var i = 0; i < names.Length; i++)
                        {
                            icons[i] = new IImageSource[1];
                            icons[i][0] = 
                                _active.PicSources["advanceCategories"][5 * _advances[i].Epoch + _advances[i].KnowledgeCategory];
                        }
                        break;
                    case CivilopediaInfoType.Improvements:
                        names = _improvements.Select(i => i.Name).ToArray();
                        icons = new IImageSource[names.Length][];
                        for (var i = 0; i < names.Length; i++)
                        {
                            icons[i] = new IImageSource[1];
                            icons[i][0] = _active.GetImprovementImage(_improvements[i], _rules.FirstWonderIndex);
                        }
                        break;
                    case CivilopediaInfoType.Wonders:
                        names = _wonders.Select(w => w.Name).ToArray();
                        icons = new IImageSource[names.Length][];
                        for (var i = 0; i < names.Length; i++)
                        {
                            icons[i] = new IImageSource[1];
                            icons[i][0] = _active.GetImprovementImage(_wonders[i], _rules.FirstWonderIndex);
                        }
                        break;
                    case CivilopediaInfoType.Units:
                        names = _units.Select(u => u.Name).ToArray();
                        icons = new IImageSource[names.Length][];
                        for (var i = 0; i < names.Length; i++)
                        {
                            icons[i] = new IImageSource[1];
                            icons[i][0] = _active.PicSources["unit"][_units[i].Type];
                        }
                        iconOffset = Images.GetImageWidth(icons[0][0], _active);
                        break;
                    case CivilopediaInfoType.Governments:
                        names = _govs.Select(g => g.Name).ToArray();
                        icons = new IImageSource[_govs.Count][];
                        break;
                    case CivilopediaInfoType.Terrains:
                        names = _terrains.Select(t => t.Name).ToArray();
                        icons = new IImageSource[_terrains.Count][];
                        for (var i = 0; i < _terrains.Count; i++)
                        {
                            var t = _terrains[i];
                            if (t is Terrain terrain)
                            {
                                icons[i] = new IImageSource[1];
                                icons[i][0] = _active.PicSources["base1"][(int)terrain.Type];
                            }
                            else
                            {
                                icons[i] = new IImageSource[2];
                                var s = (Special)t;
                                var baseTerrain = _rules.Terrains[0].FirstOrDefault(t => t.Specials[0] == s);
                                if (baseTerrain != null)
                                {
                                    icons[i][0] = _active.PicSources["base1"][(int)baseTerrain.Type];
                                    icons[i][1] = _active.PicSources["special1"][(int)baseTerrain.Type];
                                }
                                else
                                {
                                    baseTerrain = _rules.Terrains[0].FirstOrDefault(t => t.Specials[1] == s);
                                    icons[i][0] = _active.PicSources["base1"][(int)baseTerrain.Type];
                                    icons[i][1] = _active.PicSources["special2"][(int)baseTerrain.Type];
                                }

                                // Give grassland's special a custom name and icon
                                if (i > 0 && names[i] == names[i - 1])
                                {
                                    names[i] += $" ({Labels.For(LabelIndex.Shield)})";
                                    icons[i][0] = _active.PicSources["base1"][0];
                                    icons[i][1] = _active.PicSources["shield"][0];
                                }
                            }
                        }
                        iconOffset = Images.GetImageWidth(icons[0][0], _active) / 2;
                        break;
                    case CivilopediaInfoType.Concepts:
                        names = _concepts.ToArray();
                        icons = new IImageSource[_concepts.Count][];
                        break;
                }
                if (icons.Length != 0 && icons[0] != null)
                {
                    iconWidth = Images.GetImageWidth(icons[0][0], _active, props.Listbox.IconScale);
                }

                List<ListboxGroup> groups = [];
                for (var i = 0; i < names.Length; i++)
                {
                    List<ListboxGroupElement> elements = [];
                    for (var j = 0; j < icons[i]?.Length; j++)
                    {
                        elements.Add(new ListboxGroupElement
                        {
                            Icon = icons[i][j],
                            ScaleIcon = props.Listbox.IconScale,
                            Xoffset = (i % 2 == 1) ? iconOffset + 2 : 2
                        });
                    }
                    elements.Add(new ListboxGroupElement
                    {
                        Text = names[i],
                        Xoffset = 4 + iconWidth + iconOffset,
                        TextSizeOverride = _active.Look.CivilopediaFontSize,
                        VerticalAlignment = VerticalAlignment.Center
                    });

                    var group = new ListboxGroup() { Elements = elements, Height = props.Listbox.RowHeight };
                    groups.Add(group);
                }

                var def = new ListboxDefinition()
                {
                    Rows = props.Listbox.Rows,
                    Columns = props.Listbox.Columns,
                    Type = ListboxType.Default,
                    VerticalScrollbar = props.Listbox.VerticalScrollbar,
                    Groups = groups,
                    SelectedId = _pedia.Id
                };

                var listbox = new Listbox(this, def)
                {
                    Width = InnerWidth - 4,
                    Height = InnerHeight - 4,
                    Location = new(LayoutPadding.Left + 2, LayoutPadding.Top + 1)
                };
                listbox.ItemSelected += (_, i) =>
                {
                    _pedia.Id = i.Index;
                    _ctrlHistory.Last().Id = _pedia.Id;
                };
                Controls.Add(listbox);
                Focused = listbox;

                break;

            case CivilopediaWindowType.Info:

                if (_pedia.InfoType == CivilopediaInfoType.Governments)
                {
                    var _id = Array.FindIndex(_rules.Governments, row => row == _govs[_pedia.Id]);
                    Controls.Add(new CivilopediaDescription(this, _gameScreen, _pedia, _id));
                }
                else if (_pedia.InfoType == CivilopediaInfoType.Concepts)
                {
                    Controls.Add(new CivilopediaDescription(this, _gameScreen, _pedia, _pedia.Id));
                }
                else
                {
                    Controls.Add(new CivilopediaInfo(this, _gameScreen, _advances, _improvements, _wonders, 
                        _units, _terrains, _pedia));
                }

                break;

            case CivilopediaWindowType.Description:

                // Since we can't get index of special tiles from rules anymore (that info is lost), 
                // here's a non elegant solution that works
                int terrainsId = 0;
                if (_pedia.InfoType == CivilopediaInfoType.Terrains)
                {
                    if (_terrains[_pedia.Id] is Terrain)
                    {
                        terrainsId = Array.FindIndex(_rules.Terrains[0], row => row == _terrains[_pedia.Id]);
                    }
                    else if (_terrains[_pedia.Id] is Special)
                    {
                        var s = (Special)_terrains[_pedia.Id];
                        var baseTerrain = _rules.Terrains[0].FirstOrDefault(t => t.Specials[0] == s);
                        if (baseTerrain != null)
                        {
                            terrainsId = Array.FindIndex(_rules.Terrains[0], row => row == baseTerrain)
                                + _rules.Terrains[0].Length;
                        }
                        else
                        {
                            baseTerrain = _rules.Terrains[0].FirstOrDefault(t => t.Specials[1] == s);
                            terrainsId = Array.FindIndex(_rules.Terrains[0], row => row == baseTerrain)
                                + 2 * _rules.Terrains[0].Length;
                        }
                    }
                }

                int id = _pedia.InfoType switch
                {
                    CivilopediaInfoType.Advances => Array.FindIndex(_rules.Advances, row => row == _advances[_pedia.Id]),
                    CivilopediaInfoType.Improvements => Array.FindIndex(_rules.Improvements, row => row == _improvements[_pedia.Id]),
                    CivilopediaInfoType.Wonders => Array.FindIndex(_rules.Improvements.Skip(_rules.FirstWonderIndex).ToArray(), 
                        row => row == _wonders[_pedia.Id]),
                    CivilopediaInfoType.Units => Array.FindIndex(_rules.UnitTypes, row => row == _units[_pedia.Id]),
                    CivilopediaInfoType.Terrains => terrainsId,
                    _ => throw new NotImplementedException()
                };
                Controls.Add(new CivilopediaDescription(this, _gameScreen, _pedia, id));
                break;

            case CivilopediaWindowType.Tree:

                Controls.Add(new CivilopediaTree(this, _gameScreen, _advances, _pedia));

                break;

            default: throw new NotImplementedException();
        }

        var offset = 0;
        if (buttons.Contains("Go Back"))
        {
            var gobackBtn = new Button(this, "Go Back", _active.Look.ButtonFont, 18)
            {
                Location = new(LayoutPadding.Left + offset, Height - LayoutPadding.Bottom + 4),
                Width = (InnerWidth - (buttons.Length - 1)) / buttons.Length,
                Height = LayoutPadding.Bottom - 12
            };
            offset += InnerWidth / buttons.Length;
            gobackBtn.Click += (_, _) =>
            {
                _ctrlHistory.RemoveAt(_ctrlHistory.Count - 1);
                if (_ctrlHistory.Count != 0)
                {
                    _pedia = _ctrlHistory.Last();
                    _ctrlHistory.RemoveAt(_ctrlHistory.Count - 1);
                }
                UpdateControls();
            };
            Controls.Add(gobackBtn);
        }

        if (buttons.Contains("Info"))
        {
            var infoBtn = new Button(this, Labels.For(LabelIndex.Info), _active.Look.ButtonFont, 18)
            {
                Location = new(LayoutPadding.Left + offset, Height - LayoutPadding.Bottom + 4),
                Width = (InnerWidth - (buttons.Length - 1)) / buttons.Length,
                Height = LayoutPadding.Bottom - 12
            };
            offset += InnerWidth / buttons.Length;
            infoBtn.Click += (_, _) =>
            {
                _pedia.WindowType = CivilopediaWindowType.Info;
                UpdateControls();
            };
            Controls.Add(infoBtn);
        }

        if (buttons.Contains("Tree"))
        {
            var treeBtn = new Button(this, "Tree", _active.Look.ButtonFont, 18)
            {
                Location = new(LayoutPadding.Left + offset, Height - LayoutPadding.Bottom + 4),
                Width = (InnerWidth - (buttons.Length - 1)) / buttons.Length,
                Height = LayoutPadding.Bottom - 12
            };
            offset += InnerWidth / buttons.Length;
            treeBtn.Click += (_, _) =>
            {
                _pedia.WindowType = CivilopediaWindowType.Tree;
                UpdateControls();
            };
            Controls.Add(treeBtn);
        }

        if (buttons.Contains("Description"))
        {
            var descrBtn = new Button(this, Labels.For(LabelIndex.Description), _active.Look.ButtonFont, 18)
            {
                Location = new(LayoutPadding.Left + offset, Height - LayoutPadding.Bottom + 4),
                Width = (InnerWidth - (buttons.Length - 1)) / buttons.Length,
                Height = LayoutPadding.Bottom - 12
            };
            offset += InnerWidth / buttons.Length;
            descrBtn.Click += (_, _) =>
            {
                _pedia.WindowType = CivilopediaWindowType.Description;
                UpdateControls();
            };
            Controls.Add(descrBtn);
        }

        if (buttons.Contains("Close"))
        {
            var closeBtn = new Button(this, Labels.For(LabelIndex.Close), _active.Look.ButtonFont, 18)
            {
                Location = new(LayoutPadding.Left + offset, Height - LayoutPadding.Bottom + 4),
                Width = (InnerWidth - (buttons.Length - 1)) / buttons.Length,
                Height = LayoutPadding.Bottom - 12
            };
            offset += InnerWidth / buttons.Length;
            closeBtn.Click += (_, _) => _gameScreen.CloseDialog(this);
            Controls.Add(closeBtn);
        }

        var headerText = _pedia.WindowType switch
        {
            CivilopediaWindowType.Listbox or CivilopediaWindowType.Tree => _pedia.InfoType switch 
            {
                CivilopediaInfoType.Advances => Labels.For(LabelIndex.CivilizationAdvances),
                CivilopediaInfoType.Improvements => Labels.For(LabelIndex.CityImprovements),
                CivilopediaInfoType.Wonders => Labels.For(LabelIndex.WondersoftheWorld),
                CivilopediaInfoType.Units => Labels.For(LabelIndex.UnitTypes),
                CivilopediaInfoType.Governments => Labels.For(LabelIndex.FormsofGovernment),
                CivilopediaInfoType.Terrains => Labels.For(LabelIndex.TerrainandSpecialResources),
                CivilopediaInfoType.Concepts => Labels.For(LabelIndex.GameConcepts),
                _ => throw new NotImplementedException()
            },
            CivilopediaWindowType.Info or CivilopediaWindowType.Description => _pedia.InfoType switch
            {
                CivilopediaInfoType.Advances => _advances[_pedia.Id].Name,
                CivilopediaInfoType.Improvements => _improvements[_pedia.Id].Name,
                CivilopediaInfoType.Wonders => _wonders[_pedia.Id].Name,
                CivilopediaInfoType.Units => _units[_pedia.Id].Name,
                CivilopediaInfoType.Governments => _govs[_pedia.Id].Name,
                CivilopediaInfoType.Terrains => _terrains[_pedia.Id].Name,
                CivilopediaInfoType.Concepts => _concepts[_pedia.Id],
                _ => throw new NotImplementedException()
            },
            _ => throw new NotImplementedException()
        };
        var headerLabel = new HeaderLabel(this, _active.Look, headerText, _active.Look.HeaderLabelFontSizeLarge);
        headerLabel.Location = new Vector2(InnerWidth / 2 - headerLabel.Width / 2, headerLabel.Location.Y);
        Controls.Add(headerLabel);

        Controls.Add(_exitIcon);

        foreach (var control in Controls)
        {
            control.OnResize();
        }
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

    public override void OnKeyPress(KeyboardKey key)
    {
        if (key == KeyboardKey.Escape || key == KeyboardKey.Enter || key == KeyboardKey.KpEnter)
        {
            _gameScreen.CloseDialog(this);
        }
        base.OnKeyPress(key);
    }
}