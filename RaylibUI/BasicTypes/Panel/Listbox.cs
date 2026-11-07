using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Fonts;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.RunGame.GameControls.Mapping;
using RaylibUtils;
using System.Numerics;

namespace RaylibUI.BasicTypes;

/// <summary>
/// List of items (rows of texts, icons and units) that the listbox draws itself.
/// Items are laid out on a grid of <see cref="Rows"/> x <see cref="Columns"/> visible cells; the rest is reached by scrolling.
/// The only child control is the scrollbar.
/// </summary>
public class Listbox : BaseControl
{
    private bool _verticalScrollbar = true;
    /// <summary>
    /// True = the list scrolls up/down (vertical scrollbar), false = it scrolls left/right (horizontal scrollbar).
    /// </summary>
    public bool VerticalScrollbar
    {
        get => _verticalScrollbar;
        set
        {
            if (_verticalScrollbar != value)
            {
                _verticalScrollbar = value;
                Update();
            }
        }
    }

    /// <summary>
    /// No of rows visible in the box.
    /// </summary>
    public int Rows { get; set; } = 10;

    /// <summary>
    /// Max no of columns that will be visible in the box.
    /// </summary>
    public int Columns { get; set; } = 1;

    /// <summary>
    /// If true, items are stacked left-to-right (then the next row), otherwise top-to-bottom (then the next column).
    /// </summary>
    public bool HorizontalStacking { get; set; } = false;

    private int _selectedId;
    /// <summary>
    /// Selected item index.
    /// </summary>
    public int SelectedId
    {
        get => _selectedId;
        set
        {
            _selectedId = value;
            Update();
        }
    }

    /// <summary>
    /// true = the selected item is highlighted and the selection can be moved with keys
    /// false = there's no highlight and no keys, but a click on an item still reports it
    /// </summary>
    public bool Selectable { get; set; } = true;

    private ListboxType? _type;
    /// <summary>
    /// Defines looks of listbox.
    /// </summary>
    public ListboxType? Type
    {
        get => _type;
        set
        {
            _type = value;
            Looks = _active.GetListboxLooks(_type);
            Update();
        }
    }

    public ListboxLooks Looks { get; set; } = new();

    private List<ListboxGroup> _groups = [];
    public List<ListboxGroup> Groups
    {
        get => _groups;
        set
        {
            _groups = value;
            _selectedId = 0;
            Update();
        }
    }

    /// <summary>
    /// An item was selected. Soft = moved there with keys; not soft = clicked or Enter pressed (a final choice).
    /// Each dialog decides what to do with each kind.
    /// </summary>
    public event EventHandler<ListboxSelectionEventArgs>? ItemSelected;

    /// <summary>
    /// Only a selectable list takes part in the tab order and gets focus on a click.
    /// </summary>
    public override bool CanFocus => Selectable;

    private readonly IUserInterface _active;
    private readonly ScrollBar _vScrollBar, _hScrollBar;
    private List<ItemLayout> _items = [];
    private int _rowHeight;
    private int _viewStartingRow, _viewStartingCol;
    private int _pressedIndex = -1;

    public Listbox(IControlLayout controller) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;

        // Both scrollbars are made once; only the one in use is a child
        _vScrollBar = new ScrollBar(controller, position => _viewStartingRow = position);
        _hScrollBar = new ScrollBar(controller, position => _viewStartingCol = position, false);
        Controls.Add(_vScrollBar);
    }

    private ScrollBar ActiveScrollBar => _verticalScrollbar ? _vScrollBar : _hScrollBar;

    // Grid of all items (visible and not)
    private int TotalColumns => _verticalScrollbar ? Math.Max(1, Columns) : (int)Math.Ceiling(_items.Count / (double)Math.Max(1, Rows));
    private int TotalRows => _verticalScrollbar ? (int)Math.Ceiling(_items.Count / (double)Math.Max(1, Columns)) : Math.Max(1, Rows);

    private (int row, int col) CellOf(int index) => HorizontalStacking
        ? (index / TotalColumns, index % TotalColumns)
        : (index % TotalRows, index / TotalRows);

    private int IndexOf(int row, int col)
    {
        if (row < 0 || col < 0 || row >= TotalRows || col >= TotalColumns) return -1;

        var index = HorizontalStacking ? row * TotalColumns + col : col * TotalRows + row;
        return index < _items.Count ? index : -1;
    }

    private int ScrollBarWidth => _verticalScrollbar && _items.Count > Rows * Columns ? _vScrollBar.Width : 0;

    private int ColumnWidth => (Width - ScrollBarWidth) / Math.Max(1, Columns);

    /// <summary>
    /// Width an item needs (its last element's end), for lists that size themselves to their items.
    /// </summary>
    public int PreferredItemWidth => _items.Count == 0 ? 0 : _items.Max(i => i.NaturalWidth);

    /// <summary>
    /// Rebuild the items after Groups/Looks/options changed: work out each item's elements once,
    /// reset the scrolling and (if selectable) report the selected item.
    /// </summary>
    public void Update()
    {
        if (Controls.Count == 0 || Controls[0] != ActiveScrollBar)
        {
            Controls = [ActiveScrollBar];
        }

        _items = _groups.Select(MakeItemLayout).ToList();
        _rowHeight = _items.Count == 0 ? 0 : _items.Max(i => i.Height);

        // Keep the selection inside the list (0 when the list is empty)
        _selectedId = Math.Clamp(_selectedId, 0, Math.Max(0, _items.Count - 1));

        _vScrollBar.SetScrollPosition(0);
        _hScrollBar.SetScrollPosition(0);

        // Not the virtual OnResize: the city boxes override it and set Groups from it
        LayoutBox();

        if (_items.Count > 0 && Selectable)
        {
            Select(_selectedId, true);
        }
    }

    public override void OnResize()
    {
        LayoutBox();
    }

    /// <summary>
    /// Box height from the rows, and the scrollbar's range and place.
    /// </summary>
    private void LayoutBox()
    {
        if (_items.Count == 0) return;

        Height = Rows * _rowHeight + (_verticalScrollbar ? 0 : _hScrollBar.Height);

        if (_verticalScrollbar)
        {
            _vScrollBar.Maximum = Math.Max(0, TotalRows - Rows);
            _vScrollBar.LargeChange = Rows;     // a click on the track pages by the visible rows
            _vScrollBar.Location = new(Width - _vScrollBar.Width - 2, 2);
            _vScrollBar.Height = Height - 4;
            _vScrollBar.Visible = Rows < TotalRows;
        }
        else
        {
            _hScrollBar.Maximum = Math.Max(0, TotalColumns - Columns);
            _hScrollBar.LargeChange = Columns;  // a click on the track pages by the visible columns
            _hScrollBar.Width = Width - 4;
            _hScrollBar.Location = new(2, Height - _hScrollBar.Height);
            _hScrollBar.Visible = Columns < TotalColumns;
        }
    }

    /// <summary>
    /// Select an item, scroll it into view, and report it.
    /// </summary>
    private void Select(int index, bool soft)
    {
        if (_items.Count == 0) return;

        _selectedId = Math.Clamp(index, 0, _items.Count - 1);
        EnsureVisible(_selectedId);
        ItemSelected?.Invoke(this, new ListboxSelectionEventArgs(_selectedId, soft));
    }

    private void EnsureVisible(int index)
    {
        var (row, col) = CellOf(index);

        if (row >= _viewStartingRow + Rows)
        {
            _vScrollBar.SetScrollPosition(row - Rows + 1);
        }
        else if (row < _viewStartingRow)
        {
            _vScrollBar.SetScrollPosition(row);
        }

        if (col >= _viewStartingCol + Columns)
        {
            _hScrollBar.SetScrollPosition(col - Columns + 1);
        }
        else if (col < _viewStartingCol)
        {
            _hScrollBar.SetScrollPosition(col);
        }
    }

    public void ScrollToEnd()
    {
        _vScrollBar.SetScrollPosition(Math.Max(0, TotalRows - Rows));
    }

    public void EnterPressed()
    {
        if (_items.Count == 0) return;

        Select(_selectedId, false);
    }

    /// <summary>
    /// Item at this position (relative to the listbox), or -1.
    /// </summary>
    public int IndexFromPoint(Vector2 pos)
    {
        if (_rowHeight <= 0 || ColumnWidth <= 0 || pos.X < 0 || pos.Y < 0) return -1;

        var visibleCol = (int)(pos.X / ColumnWidth);
        var visibleRow = (int)(pos.Y / _rowHeight);
        if (visibleCol >= Columns || visibleRow >= Rows) return -1;

        return IndexOf(_viewStartingRow + visibleRow, _viewStartingCol + visibleCol);
    }

    // A click selects the item on release, if it was also pressed on that item (as with buttons)
    public override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        _pressedIndex = e.Button == MouseButton.Left ? IndexFromPoint(e.Location) : -1;
    }

    public override void OnClick(MouseEventArgs e)
    {
        base.OnClick(e);
        if (e.Button != MouseButton.Left) return;

        var index = IndexFromPoint(e.Location);
        if (index < 0 || index != _pressedIndex) return;

        if (Selectable)
        {
            Controller.Focused = this;
        }
        Select(index, false);
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        // Nothing to move through in a read-only or empty list
        if (!Selectable || _items.Count == 0) return base.OnKeyPressed(key);

        // Moving along the stacking direction goes item by item; across it, a whole row/column at a time
        var along = 1;
        var across = HorizontalStacking ? TotalColumns : TotalRows;
        var page = HorizontalStacking ? Rows * TotalColumns : Rows;
        int? target = key switch
        {
            KeyboardKey.Down or KeyboardKey.Kp2 => _selectedId + (HorizontalStacking ? across : along),
            KeyboardKey.Up or KeyboardKey.Kp8 => _selectedId - (HorizontalStacking ? across : along),
            KeyboardKey.Right or KeyboardKey.Kp6 => _selectedId + (HorizontalStacking ? along : across),
            KeyboardKey.Left or KeyboardKey.Kp4 => _selectedId - (HorizontalStacking ? along : across),
            KeyboardKey.PageDown => Math.Min(_selectedId + page, _items.Count - 1),
            KeyboardKey.PageUp => Math.Max(_selectedId - page, 0),
            KeyboardKey.Home => 0,
            KeyboardKey.End => _items.Count - 1,
            _ => null
        };

        if (target == null) return base.OnKeyPressed(key);

        // Up/Down in a vertical list stop at the ends; other moves past the ends do nothing
        if (!HorizontalStacking && key is KeyboardKey.Down or KeyboardKey.Kp2 or KeyboardKey.Up or KeyboardKey.Kp8)
        {
            target = Math.Clamp(target.Value, 0, _items.Count - 1);
        }

        if (target >= 0 && target < _items.Count && target != _selectedId)
        {
            Select(target.Value, true);
        }
        return true;
    }

    /// <summary>
    /// The wheel over the list scrolls it, like its scrollbar.
    /// </summary>
    public override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (ActiveScrollBar.ScrollByWheel(e.WheelDelta))
        {
            e.Handled = true;
        }
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        Graphics.DrawRectangleRec(Bounds, Looks.BoxBackgroundColor);
        Graphics.DrawRectangleLinesEx(Bounds, 1f, Looks.BoxLineColor);

        if (_items.Count > 0 && _rowHeight > 0)
        {
            var columnWidth = ColumnWidth;

            // Tall icons may reach into neighbouring rows; they're only cut off at the edge of the box
            Graphics.BeginScissorMode((int)Bounds.X, (int)Bounds.Y, columnWidth * Columns, _rowHeight * Rows);

            for (var visibleRow = 0; visibleRow < Rows; visibleRow++)
            {
                for (var visibleCol = 0; visibleCol < Columns; visibleCol++)
                {
                    var index = IndexOf(_viewStartingRow + visibleRow, _viewStartingCol + visibleCol);
                    if (index < 0) continue;

                    var origin = new Vector2((int)Bounds.X + visibleCol * columnWidth, (int)Bounds.Y + visibleRow * _rowHeight);
                    DrawItem(_items[index], origin, columnWidth, Selectable && index == _selectedId);
                }
            }

            Graphics.EndScissorMode();
        }

        base.Draw(pulse);
    }

    // ---- Items ----

    private enum ElementKind { Text, Icon, Unit }

    /// <summary>
    /// One element of an item, worked out once in Update: where it goes and what it draws.
    /// </summary>
    private sealed class ElementLayout
    {
        public ElementKind Kind;
        public int X, Width, Height;

        public string Text = string.Empty;
        public int FontSize;
        public Vector2 TextSize;
        public Color? FrontColor, ShadowColor;
        public HorizontalAlignment HorizontalAlignment;
        public VerticalAlignment VerticalAlignment;

        public IImageSource? Icon;
        public float Scale = 1f;
        public int IconY;

        public List<IViewElement> UnitPieces = [];
    }

    private sealed class ItemLayout
    {
        public List<ElementLayout> Elements = [];
        public int Height;
        public int NaturalWidth;
    }

    private ItemLayout MakeItemLayout(ListboxGroup group)
    {
        var item = new ItemLayout();
        var offset = 0;

        foreach (var element in group.Elements ?? [])
        {
            offset = element.Xoffset ?? offset;
            ElementLayout? layout = null;

            if (element.Text != string.Empty)
            {
                var fontSize = element.TextSizeOverride ?? Looks.FontSize;
                var textSize = TextManager.MeasureTextEx(Looks.Font, element.Text, fontSize, 0f);
                layout = new ElementLayout
                {
                    Kind = ElementKind.Text,
                    Text = element.Text,
                    FontSize = fontSize,
                    TextSize = textSize,
                    FrontColor = element.FrontColorOverride,
                    ShadowColor = element.ShadowColorOverride,
                    HorizontalAlignment = element.HorizontalAlignment,
                    VerticalAlignment = element.VerticalAlignment,
                    Width = element.Width ?? (int)textSize.X + (element.HorizontalAlignment == HorizontalAlignment.Center ? 10 : 0),
                    Height = group.Height ?? (int)textSize.Y
                };
            }
            else if (element.Icon is not null)
            {
                var imageHeight = Images.GetImageHeight(element.Icon, _active, element.ScaleIcon);
                var height = group.Height ?? imageHeight;
                layout = new ElementLayout
                {
                    Kind = ElementKind.Icon,
                    Icon = element.Icon,
                    Scale = element.ScaleIcon,
                    Width = element.Width ?? Images.GetImageWidth(element.Icon, _active, element.ScaleIcon),
                    Height = height,
                    IconY = height / 2 - imageHeight / 2    // centred in the row
                };
            }
            else if (element.Unit is not null && element.Game is not null)
            {
                var pieces = new List<IViewElement>();
                var size = ImageUtils.GetUnitTextures(element.Unit, _active, element.Game, pieces, Vector2.Zero, true);
                layout = new ElementLayout
                {
                    Kind = ElementKind.Unit,
                    UnitPieces = pieces,
                    Scale = element.ScaleIcon,
                    Width = (int)(size.X * element.ScaleIcon),
                    Height = (int)(size.Y * element.ScaleIcon)
                };
            }

            if (layout == null) continue;

            layout.X = offset;
            offset += layout.Width;
            item.Elements.Add(layout);
        }

        item.Height = group.Height ?? (item.Elements.Count == 0 ? 0 : item.Elements.Max(e => e.Height));
        item.NaturalWidth = offset;
        return item;
    }

    private void DrawItem(ItemLayout item, Vector2 origin, int columnWidth, bool selected)
    {
        // The last element, if it's a text, stretches to the end of the item (so the highlight reaches the edge)
        int WidthOf(ElementLayout e) =>
            e == item.Elements[^1] && e.Kind == ElementKind.Text ? Math.Max(e.Width, columnWidth - e.X) : e.Width;

        // Highlight behind the texts only (not the icons)
        if (selected)
        {
            foreach (var e in item.Elements.Where(e => e.Kind == ElementKind.Text))
            {
                Graphics.DrawRectangle((int)origin.X + e.X, (int)origin.Y, WidthOf(e), e.Height, Looks.SelectedTextBackgroundColor);
            }
        }

        foreach (var e in item.Elements)
        {
            switch (e.Kind)
            {
                case ElementKind.Icon:
                    Graphics.DrawTextureEx(TextureCache.GetImage(e.Icon!), new Vector2(origin.X + e.X, origin.Y + e.IconY), 0f, e.Scale, Color.White);
                    break;
                case ElementKind.Unit:
                    foreach (var piece in e.UnitPieces)
                    {
                        piece.Draw(origin + new Vector2(e.X, 0) + piece.Location, scale: e.Scale, isShaded: piece.IsShaded);
                    }
                    break;
            }
        }

        foreach (var e in item.Elements.Where(e => e.Kind == ElementKind.Text))
        {
            var font = selected ? Looks.SelectedTextFont : Looks.Font;
            var front = selected ? Looks.SelectedTextColorFront : e.FrontColor ?? Looks.TextColorFront;
            var shadow = selected ? Looks.SelectedTextColorShadow : e.ShadowColor ?? Looks.TextColorShadow;
            var shadowOffset = selected ? Looks.SelectedTextShadowOffset : Looks.TextShadowOffset;

            var position = new Vector2(origin.X + e.X, origin.Y);
            position.X += e.HorizontalAlignment switch
            {
                HorizontalAlignment.Center => WidthOf(e) / 2f - e.TextSize.X / 2f,
                HorizontalAlignment.Right => WidthOf(e) - e.TextSize.X,
                _ => 0
            };
            position.Y += e.VerticalAlignment switch
            {
                VerticalAlignment.Center => e.Height / 2f - e.TextSize.Y / 2f,
                VerticalAlignment.Bottom => e.Height - e.TextSize.Y,
                _ => 0
            };

            Graphics.DrawTextEx(font, e.Text, position + shadowOffset, e.FontSize, 0f, shadow);
            Graphics.DrawTextEx(font, e.Text, position, e.FontSize, 0f, front);
        }
    }
}
