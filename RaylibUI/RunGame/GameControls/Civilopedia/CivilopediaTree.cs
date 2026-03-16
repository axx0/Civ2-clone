using Civ2engine;
using Model;
using Model.Controls;
using Model.Core.Advances;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
using System.Numerics;

namespace RaylibUI.RunGame.GameControls.CityControls;

public class CivilopediaTree : BaseControl
{
    private readonly CivilopediaWindow _window;
    private readonly IUserInterface _active;
    private readonly Civilopedia _pedia;
    private readonly List<Advance> _advances;
    private readonly Advance? _prereq1, _prereq2, _prereq3;
    private readonly int _mainLabelW, _mainLabelH, _imgW, _imgH;
    private readonly Rules _rules;
    private readonly Color _color = new(225, 223, 79, 255);

    public CivilopediaTree(CivilopediaWindow window, GameScreen gameScreen, List<Advance> advances, Civilopedia pedia) : base(window)
    {
        _window = window;
        _pedia = pedia;
        _advances = advances;
        _active = gameScreen.MainWindow.ActiveInterface;
        _rules = gameScreen.Game.Rules;

        Width = window.Width - window.LayoutPadding.Left - window.LayoutPadding.Right;
        Height = window.Height - window.LayoutPadding.Top - window.LayoutPadding.Bottom;
        Location = new(window.LayoutPadding.Left, window.LayoutPadding.Top);

        var advance = advances[pedia.Id];
        var label = new PediaLinkLabel(_window, advance.Name, 0, 0, true);
        _mainLabelH = label.Height;
        _mainLabelW = label.Width;
        var img = _active.PicSources["advanceCategories"][5 * advance.Epoch + advance.KnowledgeCategory];
        _imgW = Images.GetImageWidth(img, _active);
        _imgH = Images.GetImageHeight(img, _active);
        MakeControl(advance, 0, 0);

        _prereq2 = null;
        _prereq3 = null;
        int i, j, k;
        for (i = 0; i < 2; i++)
        {
            _prereq1 = i == 0 ?
                advance.Prereq1 != -1 ? _rules.Advances[advance.Prereq1] : null :
                advance.Prereq2 != -1 ? _rules.Advances[advance.Prereq2] : null;
            MakeControl(_prereq1, 1, i);

            for (j = 0; j < 2; j++)
            {
                if (_prereq1 != null)
                {
                    _prereq2 = j == 0 ?
                        _prereq1?.Prereq1 != -1 ? _rules.Advances[_prereq1.Prereq1] : null :
                        _prereq1?.Prereq2 != -1 ? _rules.Advances[_prereq1.Prereq2] : null;
                }
                MakeControl(_prereq2, 2, 2 * i + j);

                for (k = 0; k < 2; k++)
                {
                    if (_prereq2 != null)
                    {
                        _prereq3 = k == 0 ?
                        _prereq2?.Prereq1 != -1 ? _rules.Advances[_prereq2.Prereq1] : null :
                        _prereq2?.Prereq2 != -1 ? _rules.Advances[_prereq2.Prereq2] : null;
                    }
                    MakeControl(_prereq3, 3, 4 * i + 2 * j + k);
                }
            }
        }
    }

    private void MakeControl(Advance? advance, int level, int seq)
    {
        if (advance == null) return;

        var label = new PediaLinkLabel(_window, advance.Name, 0, 0, true);
        var img = _active.PicSources["advanceCategories"][5 * advance.Epoch + advance.KnowledgeCategory];
        label.Click += (_, _) =>
        {
            _pedia.WindowType = CivilopediaWindowType.Info;
            _pedia.Id = _advances.IndexOf(advance);
            _window.UpdateControls();
        };
        var icon = new ImageBox(_window, new(img), true);

        var center = GetCenter(level, seq);
        label.Location = new(center.X + _imgW / 2 + 1, center.Y - _mainLabelH / 2);
        icon.Location = new(center.X - _imgW / 2, center.Y - _imgH / 2);

        Controls.Add(label);
        Controls.Add(icon);
    }

    private Vector2 GetCenter(int level, int seq)
    {
        Vector2 c = new();

        if (level == 0)
        {
            c.X = Width - _imgW / 2 - _mainLabelW - 5;
        }
        else if (level == 1)
        {
            c.X = Width * 2 / 3;
        }
        else if (level == 2)
        {
            c.X = Width / 3;
        }
        else
        {
            c.X = 22;
        }

        var yStep = (int)(Height / Math.Pow(2, level));
        c.Y = yStep / 2 + yStep * seq;

        return c;
    }

    public override void Draw(bool pulse)
    {
        var advance = _advances[_pedia.Id];

        for (var i = 0; i < 2; i++)
        {
            var c0 = GetCenter(0, 0);
            var prereq1 = i == 0 ?
                advance.Prereq1 != -1 ? _rules.Advances[advance.Prereq1] : null :
                advance.Prereq2 != -1 ? _rules.Advances[advance.Prereq2] : null;

            var c1 = GetCenter(1, i);
            if (prereq1 != null)
            {
                DrawLines(c0, c1);
            }

            c0 = c1;
            for (var j = 0; j < 2; j++)
            {
                if (prereq1 == null) continue;
                 
                var prereq2 = j == 0 ?
                    prereq1?.Prereq1 != -1 ? _rules.Advances[prereq1.Prereq1] : null :
                    prereq1?.Prereq2 != -1 ? _rules.Advances[prereq1.Prereq2] : null;

                c1 = GetCenter(2, 2 * i + j);
                if (prereq2 != null)
                {
                    DrawLines(c0, c1);
                }

                c0 = c1;
                for (var k = 0; k < 2; k++)
                {
                    if (prereq2 == null) continue;

                    var prereq3 = k == 0 ?
                        prereq2?.Prereq1 != -1 ? _rules.Advances[prereq2.Prereq1] : null :
                        prereq2?.Prereq2 != -1 ? _rules.Advances[prereq2.Prereq2] : null;

                    c1 = GetCenter(3, 4 * i + 2 * j + k);
                    if (prereq3 != null)
                    {
                        DrawLines(c0, c1);
                    }
                }
            }
        }

        base.Draw(pulse);
    }

    private void DrawLines(Vector2 c0, Vector2 c1)
    {
        var x1 = (int)(Bounds.X + c0.X);
        var y1 = (int)(Bounds.Y + c0.Y);
        var x2 = (int)(Bounds.X + c1.X);
        var y2 = (int)(Bounds.Y + c0.Y);
        Graphics.DrawLine(x1, y1 + 1, x2, y2 + 1, Color.Black);
        Graphics.DrawLine(x1, y1 - 1, x2, y2 - 1, Color.Black);
        Graphics.DrawLine(x1, y1, x2, y2, _color);

        x1 = (int)(Bounds.X + c1.X);
        y1 = (int)(Bounds.Y + c0.Y);
        x2 = (int)(Bounds.X + c1.X);
        y2 = (int)(Bounds.Y + c1.Y);
        Graphics.DrawLine(x1 + 1, y1, x2 + 1, y2, Color.Black);
        Graphics.DrawLine(x1 - 1, y1, x2 - 1, y2, Color.Black);
        Graphics.DrawLine(x1, y1, x2, y2, _color);
    }
}