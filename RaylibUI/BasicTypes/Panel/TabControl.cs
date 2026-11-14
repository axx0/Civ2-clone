using Model;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using System.Numerics;

namespace RaylibUI.BasicTypes;

/// <summary>
/// Tab pages with a row of tabs on top. Only the selected page is a child control,
/// so the other pages get no input, aren't drawn and aren't in the tab order.
/// - a press on a tab selects it and focuses the tab control,
/// - with the tab control focused: Left/Right (and Home/End) switch tabs,
/// - Ctrl+Tab / Ctrl+Shift+Tab switch tabs from anywhere inside it (see BaseLayoutController).
/// </summary>
public class TabControl : BaseControl
{
    private const int TabPaddingX = 6, TabPaddingY = 3;
    private const int Border = 2;           // page frame thickness
    private const int SelectedRaise = 2;    // the selected tab is this much bigger on each side
    private const int FontSize = 16;

    private static readonly Color Face = new(207, 207, 207, 255);
    private static readonly Color Light = Color.White;
    private static readonly Color Shadow = new(135, 135, 135, 255);
    private static readonly Color Dark = new(67, 67, 67, 255);

    private readonly List<(string Title, IControl? Page)> _tabs = [];
    private readonly List<Rectangle> _tabRects = [];    // relative to the tab control
    private readonly Font _font = Fonts.Arial;
    private readonly int _tabHeight;
    private int _selectedIndex;
    private bool _focused;

    public event EventHandler? SelectedIndexChanged;

    public override bool CanFocus => _tabs.Count > 0;

    public TabControl(IControlLayout controller) : base(controller)
    {
        _tabHeight = (int)TextManager.MeasureTextEx(_font, "Ag", FontSize, 0f).Y + 2 * TabPaddingY;
    }

    /// <summary>
    /// Tabs in the dictionary's order; a page may be null (an empty tab).
    /// </summary>
    public TabControl(IControlLayout controller, Dictionary<string, IControl?> pages) : this(controller)
    {
        foreach (var (title, page) in pages)
        {
            AddTab(title, page);
        }
    }

    public void AddTab(string title, IControl? page)
    {
        _tabs.Add((title, page));
        LayoutTabs();
        if (_tabs.Count == 1)
        {
            ShowSelectedPage();
        }
    }

    public int TabCount => _tabs.Count;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_tabs.Count == 0) return;

            var index = Math.Clamp(value, 0, _tabs.Count - 1);
            if (index == _selectedIndex) return;

            // Focus inside the page being hidden moves to the tabs
            if (IsInSelectedPage(Controller.Focused))
            {
                Controller.Focused = this;
            }

            _selectedIndex = index;
            ShowSelectedPage();
            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public IControl? SelectedPage => _tabs.Count == 0 ? null : _tabs[_selectedIndex].Page;

    /// <summary>
    /// Select the next (or previous) tab, wrapping around.
    /// </summary>
    public void SelectNextTab(bool forward = true)
    {
        if (_tabs.Count == 0) return;
        SelectedIndex = ((_selectedIndex + (forward ? 1 : -1)) % _tabs.Count + _tabs.Count) % _tabs.Count;
    }

    private bool IsInSelectedPage(IControl? control)
    {
        var page = SelectedPage;
        for (var c = control; c != null; c = c.Parent as IControl)
        {
            if (c == page) return true;
        }
        return false;
    }

    private void ShowSelectedPage()
    {
        var page = SelectedPage;
        Controls = page == null ? [] : [page];
        ArrangePage();
    }

    private void LayoutTabs()
    {
        _tabRects.Clear();
        var x = SelectedRaise;
        foreach (var (title, _) in _tabs)
        {
            var width = (int)TextManager.MeasureTextEx(_font, title, FontSize, 0f).X + 2 * TabPaddingX;
            _tabRects.Add(new Rectangle(x, SelectedRaise, width, _tabHeight - SelectedRaise));
            x += width;
        }
    }

    private int TabStripWidth => _tabRects.Count == 0 ? 0 : (int)(_tabRects[^1].X + _tabRects[^1].Width) + SelectedRaise;

    /// <summary>
    /// Room for the page, inside the frame and below the tabs (relative to the tab control).
    /// </summary>
    public Rectangle PageArea => new(Border, _tabHeight + Border,
        Math.Max(0, Width - 2 * Border), Math.Max(0, Height - _tabHeight - 2 * Border));

    /// <summary>
    /// Wide enough for the tabs and the widest page; tall enough for the tallest page,
    /// so switching tabs never changes the size.
    /// </summary>
    public override int GetPreferredWidth()
    {
        var pages = _tabs.Select(t => t.Page?.GetPreferredWidth() ?? 0).DefaultIfEmpty(0).Max();
        var width = Math.Max(TabStripWidth, pages + 2 * Border);
        return width > 0 ? width : -1;
    }

    public override int GetPreferredHeight()
    {
        var pages = _tabs.Select(t => t.Page?.GetPreferredHeight() ?? 0).DefaultIfEmpty(0).Max();
        return _tabHeight + pages + 2 * Border;
    }

    public override void OnResize()
    {
        ArrangePage();
    }

    private void ArrangePage()
    {
        SelectedPage?.Arrange(PageArea);
    }

    public override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButton.Left) return;

        for (var i = 0; i < _tabRects.Count; i++)
        {
            var rect = i == _selectedIndex ? Grow(_tabRects[i]) : _tabRects[i];
            if (e.Location.X >= rect.X && e.Location.X < rect.X + rect.Width &&
                e.Location.Y >= rect.Y && e.Location.Y < rect.Y + rect.Height)
            {
                SelectedIndex = i;
                Controller.Focused = this;
                return;
            }
        }
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Left or KeyboardKey.Kp4:
                if (_selectedIndex > 0) SelectedIndex = _selectedIndex - 1;
                return true;
            case KeyboardKey.Right or KeyboardKey.Kp6:
                if (_selectedIndex < _tabs.Count - 1) SelectedIndex = _selectedIndex + 1;
                return true;
            case KeyboardKey.Home:
                SelectedIndex = 0;
                return true;
            case KeyboardKey.End:
                SelectedIndex = _tabs.Count - 1;
                return true;
        }
        return base.OnKeyPressed(key);
    }

    public override void OnFocus()
    {
        _focused = true;
    }

    public override void OnBlur()
    {
        _focused = false;
    }

    // The selected tab: raised and wider, covering the frame line below it
    private static Rectangle Grow(Rectangle tab) =>
        new(tab.X - SelectedRaise, tab.Y - SelectedRaise, tab.Width + 2 * SelectedRaise, tab.Height + SelectedRaise + 1);

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        var origin = new Vector2(Bounds.X, Bounds.Y);

        // Page frame (raised)
        var frame = new Rectangle(origin.X, origin.Y + _tabHeight, Width, Height - _tabHeight);
        Graphics.DrawRectangleRec(frame, Face);
        DrawBevel(frame, withBottom: true);

        // Unselected tabs first, then the selected one over the frame
        for (var i = 0; i < _tabRects.Count; i++)
        {
            if (i != _selectedIndex) DrawTab(i, Offset(_tabRects[i], origin), false);
        }
        if (_tabRects.Count > 0)
        {
            DrawTab(_selectedIndex, Offset(Grow(_tabRects[_selectedIndex]), origin), true);
        }

        base.Draw(pulse);
    }

    private static Rectangle Offset(Rectangle r, Vector2 by) => new(r.X + by.X, r.Y + by.Y, r.Width, r.Height);

    private void DrawTab(int index, Rectangle rect, bool selected)
    {
        Graphics.DrawRectangleRec(rect, Face);
        DrawBevel(rect, withBottom: false);

        var title = _tabs[index].Title;
        var size = TextManager.MeasureTextEx(_font, title, FontSize, 0f);
        var textPos = new Vector2((int)(rect.X + rect.Width / 2 - size.X / 2), (int)(rect.Y + (rect.Height - size.Y) / 2));
        Graphics.DrawTextEx(_font, title, textPos, FontSize, 0f, Color.Black);

        // Focus rectangle around the selected tab's title
        if (selected && _focused)
        {
            Graphics.DrawRectangleLinesEx(new Rectangle(textPos.X - 2, textPos.Y, size.X + 4, size.Y), 1f, Dark);
        }
    }

    /// <summary>
    /// 3D edge: light top/left, dark right (and bottom), with an inner shadow line.
    /// </summary>
    private static void DrawBevel(Rectangle r, bool withBottom)
    {
        int x = (int)r.X, y = (int)r.Y, w = (int)r.Width, h = (int)r.Height;
        Graphics.DrawRectangle(x, y, w - 1, 1, Light);              // top
        Graphics.DrawRectangle(x, y, 1, h - (withBottom ? 1 : 0), Light);   // left
        Graphics.DrawRectangle(x + w - 1, y, 1, h, Dark);            // right
        Graphics.DrawRectangle(x + w - 2, y + 1, 1, h - 2, Shadow);  // right, inner
        if (withBottom)
        {
            Graphics.DrawRectangle(x, y + h - 1, w, 1, Dark);        // bottom
            Graphics.DrawRectangle(x + 1, y + h - 2, w - 2, 1, Shadow);
        }
    }
}
