using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.BasicTypes;

/// <summary>
/// Radio buttons or checkboxes, one <see cref="OptionControl"/> per option, filled column by column.
/// Shows up to <see cref="MaxVisibleRows"/> x <see cref="MaxVisibleCols"/> options; the rest is reached by scrolling.
/// </summary>
public class OptionsPanel : BaseControl
{
    private readonly IUserInterface _active;
    private readonly IControlLayout _controller;
    private readonly ScrollBar _vScrollBar, _hScrollBar;
    private List<OptionControl> _optionControls = [];
    private bool _built;    // option controls are (re)made only when needed, after the setters are done
    private int _startingControlRow, _startingControlCol;

    public event EventHandler<OptionsSelectionEventArgs>? ItemSelected;
    public override bool CanFocus => true;

    private IImageSource[]? _icons;
    public IImageSource[]? Icons
    {
        get => _icons;
        set
        {
            _icons = value;
            _built = false;
        }
    }

    public Color BackgroundColour { get; set; } = Color.Blank;

    private bool _isCheckbox;
    public bool IsCheckbox
    {
        get => _isCheckbox;
        set
        {
            _isCheckbox = value;
            _built = false;
        }
    }

    private int _selectedId;
    public int SelectedId
    {
        get => _selectedId;
        set
        {
            _selectedId = value;
            if (_built && !_isCheckbox)
            {
                for (var i = 0; i < _optionControls.Count; i++)
                {
                    _optionControls[i].Checked = i == _selectedId;
                }
            }
        }
    }

    private IList<string> _texts = [];
    public IList<string> Texts
    {
        get => _texts;
        set
        {
            _texts = value;
            _built = false;
        }
    }

    private int _columns = 1;
    /// <summary>
    /// Total option columns (visible+invisible).
    /// </summary>
    public int Columns
    {
        get => _columns;
        set
        {
            _columns = value;
            _built = false;
        }
    }

    /// <summary>
    /// No of option columns that are visible on panel.
    /// </summary>
    public int MaxVisibleCols { get; set; } = 20;

    /// <summary>
    /// No of option rows that are visible on panel (the panel is always as tall as these rows).
    /// </summary>
    public int MaxVisibleRows { get; set; } = 20;

    private IList<bool>? _checkboxStates;
    public IList<bool>? CheckboxStates
    {
        get => _checkboxStates;
        set
        {
            _checkboxStates = value;
            _built = false;
        }
    }

    private OptionsType? _type;
    /// <summary>
    /// Defines looks of options.
    /// </summary>
    public OptionsType? Type
    {
        get => _type;
        set
        {
            _type = value;
            Looks = _active.GetOptionsLooks(_type);
        }
    }

    private OptionsLooks _looks = new();
    public OptionsLooks Looks
    {
        get => _looks;
        set
        {
            _looks = value;
            _built = false;     // the option controls take their font and colours when made
        }
    }

    public OptionsPanel(IControlLayout controller) : base(controller)
    {
        _controller = controller;
        _active = controller.MainWindow.ActiveInterface;

        // Made once; scrolling only moves the options
        _vScrollBar = new ScrollBar(_controller, position =>
        {
            _startingControlRow = position;
            LayoutOptions();
        });
        _hScrollBar = new ScrollBar(_controller, position =>
        {
            _startingControlCol = position;
            LayoutOptions();
        }, false);
        Controls = [_vScrollBar, _hScrollBar];
    }

    /// <summary>
    /// Make the option controls if a setter changed what they show.
    /// </summary>
    private void EnsureBuilt()
    {
        if (_built) return;
        _built = true;

        _optionControls = [];
        var images = _isCheckbox ? _active.Look.CheckBoxes : _active.Look.RadioButtons;
        var optionAction = _isCheckbox ? (Action<OptionControl>)ToggleCheckBox : SetSelectedOption;
        for (int i = 0; i < (_icons == null ? _texts.Count : _icons.Length); i++)
        {
            var entry = new OptionControl(_controller, this, _texts[i], i,
                    _checkboxStates?.ElementAtOrDefault(i) ?? false, i < (_icons?.Length ?? 0) ? [_icons![i]] : images)
            {
                Selected = optionAction
            };
            _optionControls.Add(entry);
        }

        _selectedId = Math.Clamp(_selectedId, 0, Math.Max(0, _optionControls.Count - 1));
        if (!_isCheckbox && _optionControls.Count > 0)
        {
            _optionControls[_selectedId].Checked = true;
        }

        Controls = [_vScrollBar, _hScrollBar, .. _optionControls];

        _vScrollBar.SetScrollPosition(0);
        _hScrollBar.SetScrollPosition(0);
    }

    // Grid of all options (filled column by column) and the part of it that is shown
    private int TotalRows => (int)Math.Ceiling(_optionControls.Count / (double)Math.Max(1, _columns));
    private int VisibleRows => Math.Max(1, Math.Min(TotalRows, MaxVisibleRows));
    private int VisibleCols => Math.Max(1, Math.Min(_columns, MaxVisibleCols));
    private bool NeedsVScrollBar => MaxVisibleRows < TotalRows;
    private bool NeedsHScrollBar => MaxVisibleCols < _columns;
    private int VScrollBarSpace => NeedsVScrollBar ? _vScrollBar.Width + 2 : 0;
    private int HScrollBarSpace => NeedsHScrollBar ? _hScrollBar.Height : 0;

    private int OptionWidth => _optionControls.Count == 0 ? 0 : _optionControls.Max(o => o.GetPreferredWidth());
    private int RowHeight => _optionControls.Count == 0 ? 0 : _optionControls.Max(o => o.GetPreferredHeight());

    /// <summary>
    /// Width = widest option (icon + text) in each visible column, plus the vertical scrollbar.
    /// A dialog widens to fit it.
    /// </summary>
    public override int GetPreferredWidth()
    {
        EnsureBuilt();
        return OptionWidth * VisibleCols + VScrollBarSpace;
    }

    /// <summary>
    /// Height = the visible rows, plus the horizontal scrollbar.
    /// </summary>
    public override int GetPreferredHeight()
    {
        EnsureBuilt();
        return RowHeight * VisibleRows + HScrollBarSpace;
    }

    /// <summary>
    /// The panel is always as tall as its visible rows; the options share the width it's given.
    /// </summary>
    public override void OnResize()
    {
        EnsureBuilt();
        Height = GetPreferredHeight();
        LayoutOptions();
    }

    /// <summary>
    /// Place the shown options in their cells and hide the others; place the scrollbars.
    /// </summary>
    private void LayoutOptions()
    {
        var columnWidth = Math.Max(0, Width - VScrollBarSpace) / VisibleCols;
        var rowHeight = RowHeight;
        var totalRows = Math.Max(1, TotalRows);

        for (var i = 0; i < _optionControls.Count; i++)
        {
            var option = _optionControls[i];
            var row = i % totalRows - _startingControlRow;
            var col = i / totalRows - _startingControlCol;

            option.Visible = row >= 0 && row < VisibleRows && col >= 0 && col < VisibleCols;
            option.Location = new(col * columnWidth, row * rowHeight);
            option.Width = columnWidth;
            option.Height = rowHeight;
        }

        _vScrollBar.Visible = NeedsVScrollBar;
        _vScrollBar.Maximum = Math.Max(0, TotalRows - MaxVisibleRows);
        _vScrollBar.LargeChange = VisibleRows;     // a click on the track pages by the visible rows
        _vScrollBar.Location = new(Width - _vScrollBar.Width - 2, 2);
        _vScrollBar.Height = Math.Max(0, Height - HScrollBarSpace - 4);

        _hScrollBar.Visible = NeedsHScrollBar;
        _hScrollBar.Maximum = Math.Max(0, _columns - MaxVisibleCols);
        _hScrollBar.LargeChange = VisibleCols;     // ... and by the visible columns
        _hScrollBar.Location = new(2, Height - _hScrollBar.Height);
        _hScrollBar.Width = Math.Max(0, Width - 4);
    }

    private void SetSelectedOption(OptionControl newSelection)
    {
        if (_optionControls[_selectedId] == newSelection) return;
        if (!_isCheckbox)
        {
            newSelection.Checked = true;
            _optionControls[_selectedId]?.Clear();
        }
        _selectedId = newSelection.Index;
        EnsureVisible(_selectedId);

        ItemSelected?.Invoke(newSelection,
            new OptionsSelectionEventArgs(_selectedId));
    }

    /// <summary>
    /// Scroll so that this option is shown (e.g. after moving the selection with keys).
    /// </summary>
    private void EnsureVisible(int index)
    {
        var totalRows = Math.Max(1, TotalRows);
        var row = index % totalRows;
        var col = index / totalRows;

        if (row < _startingControlRow)
        {
            _vScrollBar.SetScrollPosition(row);
        }
        else if (row >= _startingControlRow + VisibleRows)
        {
            _vScrollBar.SetScrollPosition(row - VisibleRows + 1);
        }

        if (col < _startingControlCol)
        {
            _hScrollBar.SetScrollPosition(col);
        }
        else if (col >= _startingControlCol + VisibleCols)
        {
            _hScrollBar.SetScrollPosition(col - VisibleCols + 1);
        }
    }

    /// <summary>
    /// The wheel over the options scrolls them (up/down if there are hidden rows, else left/right).
    /// </summary>
    public override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (_vScrollBar.ScrollByWheel(e.WheelDelta) || _hScrollBar.ScrollByWheel(e.WheelDelta))
        {
            e.Handled = true;
        }
    }

    private void ToggleCheckBox(OptionControl checkBox)
    {
        _selectedId = checkBox.Index;
        checkBox.Checked = !checkBox.Checked;

        // The field, not the property: the options don't need to be made again
        if (_checkboxStates == null || _checkboxStates.Count <= checkBox.Index)
        {
            var old = _checkboxStates ?? [];
            _checkboxStates = old.Concat(Enumerable.Repeat(false, checkBox.Index + 1 - old.Count)).ToList();
        }

        _checkboxStates[checkBox.Index] = checkBox.Checked;
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        EnsureBuilt();
        if (_optionControls.Count == 0) return base.OnKeyPressed(key);

        int newId;
        switch (key)
        {
            case KeyboardKey.Up or KeyboardKey.Kp8:
                newId = _selectedId == 0 ? _optionControls.Count - 1 : _selectedId - 1;
                SetSelectedOption(_optionControls[newId]);
                return true;
            case KeyboardKey.Down or KeyboardKey.Kp2:
                newId = _selectedId < _optionControls.Count - 1 ? _selectedId + 1 : 0;
                SetSelectedOption(_optionControls[newId]);
                return true;
            case KeyboardKey.Left or KeyboardKey.Kp4:
                if (!_isCheckbox)
                {
                    if (Columns == 1)
                    {
                        newId = _selectedId == 0 ? _optionControls.Count - 1 : _selectedId - 1;
                    }
                    else
                    {
                        newId = _selectedId - GetRows();
                        if (newId < 0)
                        {
                            newId += _optionControls.Count;
                        }
                    }
                    SetSelectedOption(_optionControls[newId]);
                }
                return true;
            case KeyboardKey.Right or KeyboardKey.Kp6:
                if (!_isCheckbox)
                {
                    if (Columns == 1)
                    {
                        newId = _selectedId == _optionControls.Count - 1 ? 0 : _selectedId + 1;
                    }
                    else
                    {
                        newId = _selectedId + GetRows();
                        if (newId >= _optionControls.Count)
                        {
                            newId -= _optionControls.Count;
                        }
                    }
                    SetSelectedOption(_optionControls[newId]);
                }
                return true;
            case KeyboardKey.Space:
                if (_isCheckbox)
                {
                    ToggleCheckBox(_optionControls[_selectedId]);
                }
                return true;
        }

        return base.OnKeyPressed(key);
    }

    private int GetRows()
    {
        var rows = Math.DivRem(_optionControls.Count, _columns, out var rem);
        if (rem != 0)
        {
            rows++;
        }
        return rows;
    }

    public override void Draw(bool pulse)
    {
        //Graphics.DrawRectangleRec(Bounds, BackgroundColour);

        base.Draw(pulse);
    }
}