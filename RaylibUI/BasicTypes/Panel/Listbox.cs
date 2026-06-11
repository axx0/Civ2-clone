using Model;
using Model.Controls;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.BasicTypes.Controls;
using System.ComponentModel;

namespace RaylibUI.BasicTypes;

public class Listbox : BaseControl
{
    private bool _verticalScrollbar;
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
    /// If true, controls are stacked left-to-right.
    /// </summary>
    public bool HorizontalStacking { get; set; } = false;

    /// <summary>
    /// Selected control id.
    /// </summary>
    public int SelectedId { get; set; } = 0;

    /// <summary>
    /// Meaning you can also move through the controls with keys.
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

    private readonly IControlLayout _controller;
    private TableLayout _tableLayout;
    private List<ListboxControlGroup> _entries = [];
    private int _totalColumns, _totalRows;   // visible & non-visible rows & columns
    private ScrollBar _VscrollBar, _HscrollBar;
    private int _viewStartingRow, _viewStartingCol;
    private readonly IUserInterface? _active;

    private List<ListboxGroup> _groups = [];
    public List<ListboxGroup> Groups 
    {
        get => _groups;
        set
        {
            if (value.Count == 0) return;

            _groups = value;
            Update();
        }
    }

    public event EventHandler<ListboxSelectionEventArgs>? ItemSelected;

    public override bool CanFocus => true;

    public Listbox(IControlLayout controller) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _controller = controller;
        _verticalScrollbar = true;
    }

    public void Update()
    {
        Controls = [];

        for (int i = 0; i < _groups.Count; i++)
        {
            Controls.Add(new ListboxControlGroup(_controller, _groups[i], Looks));
        }
        _entries = Controls.OfType<ListboxControlGroup>().ToList();

        // Get total rows & columns (visible & non-visible)
        if (_verticalScrollbar)
        {
            _totalColumns = Columns;
            _totalRows = (int)Math.Ceiling((double)_entries.Count / (double)_totalColumns);
        }
        else
        {
            _totalRows = Rows;
            _totalColumns = (int)Math.Ceiling((double)_entries.Count / (double)_totalRows);
        }

        _viewStartingRow = 0;
        _viewStartingCol = 0;

        if (_verticalScrollbar)
        {
            _VscrollBar = new ScrollBar(_controller, (position) =>
            {
                _viewStartingRow = position;
                OnResize();
            });
            Controls.Add(_VscrollBar);
        }
        else
        {
            _HscrollBar = new ScrollBar(_controller, (position) =>
            {
                _viewStartingCol = position;
                OnResize();
            }, false);
            Controls.Add(_HscrollBar);
        }

        foreach (var control in _entries)
        {
            control.Selected = Selected;
            Controls.Add(control);
        }

        _tableLayout = new TableLayout();
        for (var dir1 = 0; dir1 < (HorizontalStacking ? _totalRows : _totalColumns); dir1++)
        {
            for (var dir2 = 0; dir2 < (HorizontalStacking ? _totalColumns : _totalRows); dir2++)
            {
                var index = (HorizontalStacking ? _totalColumns : _totalRows) * dir1 + dir2;

                if (_entries.Count < index + 1)
                {
                    break;
                }

                _tableLayout.Add(_entries[index], HorizontalStacking ? dir1 : dir2, HorizontalStacking ? dir2 : dir1);
            }
        }

        // Select control at start
        if (_entries.Count > 0 && Selectable)
        {
            if (SelectedId == -1)  // If this is true there's something wrong
            {
                SelectedId = 0;
            }
            _entries[SelectedId].SelectThis(true);
        }
    }

    public override void OnResize()
    {
        if (_entries.Count == 0) return;

        int scrollBarWidth = 0;
        if (_verticalScrollbar && _entries.Count > Rows * Columns)
        {
            scrollBarWidth = _VscrollBar.Width;
        }

        foreach (var control in _entries)
        {
            control.Width = (Width - scrollBarWidth) / Columns;
            control.OnResize();
        }

        _tableLayout?.CalculateDimensions(_viewStartingRow, _viewStartingCol, Rows, Columns);

        Height = Rows * _entries[0].Height + (_verticalScrollbar ? 0 : _HscrollBar.Height);

        if (_verticalScrollbar)
        {
            _VscrollBar.Maximum = _tableLayout.RowCount - Rows;
            _VscrollBar.Location = new(Width - _VscrollBar.Width - 2, 2);
            _VscrollBar.Height = Height - 4;
            _VscrollBar.Visible = Rows < _tableLayout.RowCount;
        }
        else
        {
            _HscrollBar.Location = new(2, Height - _HscrollBar.Height - 2);
            _HscrollBar.Maximum = _tableLayout.ColumnCount - Columns;
            _HscrollBar.Width = Width - 4;
            _HscrollBar.Visible = Columns < _tableLayout.ColumnCount;
            _HscrollBar.Location = new(_HscrollBar.Location.X, Height - _HscrollBar.Height);
        }
    }

    private void Selected(ListboxControlGroup control, bool soft)
    {
        SelectedId = _entries.IndexOf(control);

        // If the selected control is beyond the view
        int selectedRow, selectedCol;
        if (HorizontalStacking)
        {
            selectedRow = SelectedId / _totalColumns;
            selectedCol = SelectedId % _totalColumns;
        }
        else
        {
            selectedRow = SelectedId % _totalRows;
            selectedCol = SelectedId / _totalRows;
        }

        if (_viewStartingRow + Rows <= selectedRow)
        {
            _viewStartingRow = selectedRow - Rows + 1;
            _VscrollBar?.SetScrollPosition(_viewStartingRow);
        }
        else if (selectedRow < _viewStartingRow)
        {
            _viewStartingRow = selectedRow;
            _VscrollBar?.SetScrollPosition(_viewStartingRow);
        }
        if (_viewStartingCol + Columns <= selectedCol)
        {
            _viewStartingCol = selectedCol - Columns + 1;
            _HscrollBar?.SetScrollPosition(_viewStartingCol);
        }
        else if (selectedCol < _viewStartingCol)
        {
            _viewStartingCol = selectedCol;
            _HscrollBar?.SetScrollPosition(_viewStartingCol);
        }

        // If a control has been selected, change its appearance
        if (Selectable)
        {
            foreach (var groupElement in _entries)
            {
                foreach (var text in groupElement.Controls.Where(c => c is LabelControl))
                {
                    var label = (LabelControl)text;
                    label.BackgroundColor = groupElement == control ? Looks.SelectedTextBackgroundColor : null;
                    label.Font = groupElement == control ? Looks.SelectedTextFont : Looks.Font;
                    label.ColorFront = groupElement == control ? Looks.SelectedTextColorFront : Looks.TextColorFront;
                    label.ColorShadow = groupElement == control ? Looks.SelectedTextColorShadow : Looks.TextColorShadow;
                    label.ShadowOffset = groupElement == control ? Looks.SelectedTextShadowOffset : Looks.TextShadowOffset;
                }
            }
        }

        ItemSelected?.Invoke(control,
            new ListboxSelectionEventArgs(SelectedId, soft));
    }

    public void ScrollToEnd()
    {
        _viewStartingRow = Math.Max(0, _totalRows - Rows);
        _VscrollBar.SetScrollPosition(_viewStartingRow);
    }

    public void EnterPressed()
    {
        _entries[SelectedId].SelectThis(false);
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        if (!Selectable) return base.OnKeyPressed(key);

        int selectedCol;
        switch (key)
        {
            case KeyboardKey.Down or KeyboardKey.Kp2:
                SelectedId = Math.Min(SelectedId + 1, _entries.Count - 1);
                _entries[SelectedId].SelectThis(true);
                return true;
            case KeyboardKey.Up or KeyboardKey.Kp8:
                SelectedId = Math.Max(SelectedId - 1, 0);
                _entries[SelectedId].SelectThis(true);
                return true;
            case KeyboardKey.End:
                SelectedId = _entries.Count - 1;
                _entries[SelectedId].SelectThis(true);
                return true;
            case KeyboardKey.Home:
                SelectedId = 0;
                _entries[SelectedId].SelectThis(true);
                return true;
            case KeyboardKey.Left or KeyboardKey.Kp4:
                selectedCol = SelectedId / _totalRows;
                if (selectedCol > 0)
                {
                    SelectedId -= Rows;
                    _entries[SelectedId].SelectThis(true);
                }
                return true;
            case KeyboardKey.Right or KeyboardKey.Kp6:
                selectedCol = SelectedId / _totalRows;
                if (selectedCol < _totalColumns - 1 && SelectedId + Rows <= _entries.Count - 1)
                {
                    SelectedId += Rows;
                    _entries[SelectedId].SelectThis(true);
                }
                return true;
            case KeyboardKey.PageDown:
                SelectedId = Math.Min(SelectedId + Rows, _entries.Count - 1);
                _entries[SelectedId].SelectThis(true);
                return true;
            case KeyboardKey.PageUp:
                SelectedId = Math.Max(SelectedId - Rows, 0);
                _entries[SelectedId].SelectThis(true);
                return true;
        }

        return base.OnKeyPressed(key);
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangleRec(Bounds, Looks.BoxBackgroundColor);
        Graphics.DrawRectangleLinesEx(Bounds, 1f, Looks.BoxLineColor);
        //Graphics.DrawRectangleLinesEx(Bounds, 1f, Color.Magenta);

        base.Draw(pulse);
    }
}