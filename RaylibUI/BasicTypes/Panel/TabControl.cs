using Model;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.BasicTypes;

public class TabControl : BaseControl
{
    private readonly IUserInterface? _active;
    private readonly List<LabelControl> _labels = [];
    private readonly Dictionary<string, IControl> _panels;
    private string _selected;
    private Rectangle _panelArea;

    public TabControl(IControlLayout controller, Dictionary<string, IControl> panels) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _panels = panels;

        var xOffset = 0;
        foreach (var entry in panels)
        {
            var label = new LabelControl(controller, entry.Key, false, font: Fonts.Arial,
                fontSize: 16, padding: new(0, 4, 0, 4))
            {
                Location = new(xOffset, 2),
            };
            xOffset += label.Width;
            _labels.Add(label);
        }
        _selected = _labels[2].Text;

        foreach (var label in _labels)
        {
            label.Click += (_,_) => 
            {
                _selected = label.Text;
                OnResize();
            };
        }
    }

    public override void OnResize()
    {
        _panelArea = new(Bounds.X, Bounds.Y + _labels[0].Height, Bounds.Width, Bounds.Height - _labels[0].Height);

        Controls = [];

        for (var i = 0; i < _labels.Count; i++)
        {
            Controls.Add(_labels[i]);
            _labels[i].BackgroundColor = _labels[i].Text == _selected ?
                new Color(207, 207, 207, 255) : Color.Gray;
        }

        Controls.Add(_panels[_selected]);
        _panels[_selected].Location = new(2, _labels[0].Height + 2);
        _panels[_selected].Width = (int)_panelArea.Width - 4;
        _panels[_selected].Height = (int)_panelArea.Height - 4;
        _panels[_selected].OnResize();
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangleRec(_panelArea, new Color(207, 207, 207, 255));
        Graphics.DrawRectangleLinesEx(_panelArea, 1f, new Color(67, 67, 67, 255));

        base.Draw(pulse);

        foreach (var label in _labels)
        {
            Graphics.DrawRectangleLinesEx(label.Bounds, 1f, new Color(67, 67, 67, 255));
            if (label.Text == _selected)
            {
                Graphics.DrawRectangleLines((int)label.Bounds.X + 1, (int)label.Bounds.Y + label.Height,
                    label.Width - 2, 1, new Color(207, 207, 207, 255));
            }
        }
    }
}
