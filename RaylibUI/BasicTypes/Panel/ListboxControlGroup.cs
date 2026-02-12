using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls;
using RaylibUI.RunGame.GameControls.CityControls;
using RaylibUtils;
using System.Diagnostics;
using System.Numerics;

namespace RaylibUI.BasicTypes;

public class ListboxControlGroup : ControlGroup
{
    public Action<ListboxControlGroup, bool> Selected { get; set; }
    private readonly List<ListboxGroupElement> _elements;
    private bool _softSelection;    // true = don't make final selection based on this
    private readonly IUserInterface _active;

    public ListboxControlGroup(IControlLayout controller, ListboxDefinition def, int index) :
        base(controller, eventTransparent: false)
    {
        _active = controller.MainWindow.ActiveInterface;
        var group = def.Groups[index];
        _elements = group.Elements;

        if (group.Height != null)
        {
            Height = (int)group.Height;
        }

        // Make controls
        for (int i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            if (element.Text != string.Empty)
            {
                var fontSize = element.TextSizeOverride != null ? element.TextSizeOverride : def.Looks.FontSize;
                var colorFront = element.FrontColorOverride != null ? element.FrontColorOverride : def.Looks.TextColorFront;
                var colorShadow = element.ShadowColorOverride != null ? element.ShadowColorOverride : def.Looks.TextColorShadow;

                var label = new LabelControl(controller, element.Text, true, font: def.Looks.Font, fontSize: (int)fontSize,
                    colorFront: colorFront, colorShadow: colorShadow, shadowOffset: def.Looks.TextShadowOffset, 
                    horizontalAlignment: element.HorizontalAlignment, verticalAlignment: element.VerticalAlignment);
                if (group.Height != null)
                {
                    label.Height = (int)group.Height;
                }
                Controls.Add(label);
            }
            else if (element.Icon is not null)
            {
                var imagebox = new ImageBox(controller, element.Icon, element.ScaleIcon);
                int coordX = 0;
                if (element.Width != null)
                {
                    imagebox.Width = (int)element.Width;
                }
                if (group.Height != null)
                {
                    imagebox.Height = (int)group.Height;
                }
                if (def.ImageShift && index % 2 == 1)
                {
                    coordX = imagebox.Width / 2 + 1;
                }
                var imageHeight = Images.GetImageHeight(imagebox.Image[0], _active, imagebox.Scale);
                int coordY = imagebox.Height / 2 - imageHeight / 2;
                imagebox.Coords = new int[,] { { coordX, coordY } };
                Controls.Add(imagebox);
            }
            else if (element.Unit is not null)
            {
                Controls.Add(new UnitDisplay(controller, element.Unit, element.Game, new(0, 0), _active, element.ScaleIcon, true));
            }
        }

        _softSelection = true;

        Click += OnClick;
    }

    public override bool CanFocus => false;

    private int _width;
    public override int Width
    {
        get
        {
            if (_width == 0)
            {
                _width = base.GetPreferredWidth();
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
                _height = base.GetPreferredHeight();
            }

            return _height;
        }
        set { _height = value; }
    }


    private void OnClick(object? sender, MouseEventArgs e)
    {
        SelectThis(false);
    }

    public void SelectThis(bool softSelection)
    {
        _softSelection = softSelection;
        Selected(this, _softSelection);
    }

    public override void OnResize()
    {
        int offset = 0;
        for (int i = 0; i < Controls.Count; i++)
        {
            var child = Controls[i];
            offset = _elements[i].Xoffset ?? offset;
            child.Location = new Vector2(offset, 0);
            child.Width = _elements[i].Width ?? child.GetPreferredWidth();
            offset += child.Width;
        }

        // Stretch last control to get desired width
        if (Width > offset && Controls.Count > 0 && Controls[^1] is LabelControl)
        {
            Controls[^1].Width += Width - offset;
        }
    }

    public override void Draw(bool pulse)
    {
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }
}