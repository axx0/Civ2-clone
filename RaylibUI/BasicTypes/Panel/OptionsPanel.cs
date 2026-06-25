using Model;
using Model.Controls;
using Model.Images;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using System.Data;

namespace RaylibUI.BasicTypes;

public class OptionsPanel : BaseControl
{
    private readonly IUserInterface _active;
    private readonly IControlLayout _controller;
    private TableLayout _tableLayout;
    private List<OptionControl> _optionControls;
    private ScrollBar _VscrollBar, _HscrollBar;
    private int _rows, _startingControlRow, _startingControlCol;

    public event EventHandler<OptionsSelectionEventArgs>? ItemSelected;
    public override bool CanFocus => true;

    private IImageSource[]? _icons;
    public IImageSource[]? Icons
    {
        get => _icons;
        set
        {
            _icons = value;
            Update();
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
            Update();
        }
    }

    private int _selectedId;
    public int SelectedId
    {
        get => _selectedId;
        set
        {
            _selectedId = value;
            Update();
        }
    }

    private IList<string> _texts = [];
    public IList<string> Texts
    {
        get => _texts;
        set
        {
            _texts = value;
            Update();
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
            Update();
        }
    }

    private int _maxVisibleCols = 20;
    /// <summary>
    /// No of option columns that are visible on panel.
    /// </summary>
    public int MaxVisibleCols
    {
        get => _maxVisibleCols;
        set
        {
            _maxVisibleCols = value;
            Update();
        }
    }

    private int _maxVisibleRows = 20;
    /// <summary>
    /// No of option rows that are visible on panel.
    /// </summary>
    public int MaxVisibleRows
    {
        get => _maxVisibleRows;
        set
        {
            _maxVisibleRows = value;
            Update();
        }
    }

    public IList<bool> CheckboxStates { get; set; }

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
            Update();
        }
    }

    public OptionsLooks Looks { get; set; } = new();

    public OptionsPanel(IControlLayout controller) : base(controller)
    {
        _controller = controller;
        _active = controller.MainWindow.ActiveInterface;
    }

    private void Update() 
    {
        // Make controls
        Controls = [];
        _optionControls = [];
        var images = _isCheckbox ? _active.Look.CheckBoxes : _active.Look.RadioButtons;
        for (int i = 0; i < (_icons == null ? _texts.Count : _icons.Length); i++)
        {
            var entry = new OptionControl(_controller, this, _texts[i], i,
                    CheckboxStates?[i] ?? false, i < (_icons?.Length ?? 0) ? [_icons[i]] : images);
            entry.Selected = SetSelectedOption;
            _optionControls.Add(entry);
        }

        if (!_isCheckbox)
        {
            _optionControls[_selectedId].Checked = true;
        }

        _startingControlRow = 0;
        _startingControlCol = 0;

        _VscrollBar = new ScrollBar(_controller, (position) =>
        {
            _startingControlRow = position;
            OnResize();
        });
        Controls.Add(_VscrollBar);

        _HscrollBar = new ScrollBar(_controller, (position) =>
        {
            _startingControlCol = position;
            OnResize();
        }, false);
        Controls.Add(_HscrollBar);

        var optionAction = _isCheckbox ? (Action<OptionControl>)ToggleCheckBox : SetSelectedOption;
        foreach (var option in _optionControls)
        {
            Controls.Add(option);
        }

        _tableLayout = new TableLayout();
        _rows = (int)Math.Ceiling((double)_optionControls.Count / (double)_columns);
        for (int col = 0; col < _columns; col++)
        {
            for (int row = 0; row < _rows; row++)
            {
                var index = _rows * col + row;
                _tableLayout.Add(_optionControls[index], row, col);
            }
        }
    }

    public override void OnResize()
    {
        foreach (var option in _optionControls)
        {
            option.Width = Width / Math.Min(_columns, _maxVisibleCols);
        }

        _tableLayout?.CalculateDimensions(_startingControlRow, _startingControlCol, _maxVisibleRows, _maxVisibleCols);

        Height = _tableLayout.GetHeight(_startingControlRow, _maxVisibleRows);

        _VscrollBar.Visible = _maxVisibleRows < _tableLayout.RowCount;
        _VscrollBar.Maximum = _tableLayout.RowCount - _maxVisibleRows;
        _VscrollBar.Location = new(Width - _VscrollBar.Width - 2, 2);
        _VscrollBar.Height = Height - 4;

        _HscrollBar.Visible = _maxVisibleCols < _tableLayout.ColumnCount;
        if (_HscrollBar.Visible)
        {
            _HscrollBar.Location = new(2, Height - _HscrollBar.Height - 2);
            _HscrollBar.Maximum = _tableLayout.ColumnCount - _maxVisibleCols;
            _HscrollBar.Width = Width - 4;
        }

        // Change width of controls so they don't overlap with scrollbar
        if (_VscrollBar.Visible)
        {
            for (int i = 0; i < _optionControls.Count; i++)
            {
                if (_optionControls[i].Location.X + _optionControls[i].Width > _VscrollBar.Location.X)
                {
                    _optionControls[i].Width = (int)(_VscrollBar.Location.X - _optionControls[i].Location.X);
                }
            }
        }

        // Adjust height of panel so controls don't overlap with scrollbar
        if (_HscrollBar.Visible && Controls.Where(c => c.Location.Y + c.Height > _HscrollBar.Location.Y).Any())
        {
            Height += _HscrollBar.Height;
            _HscrollBar.Location = new(_HscrollBar.Location.X, Height - _HscrollBar.Height);
        }
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

        ItemSelected?.Invoke(newSelection,
            new OptionsSelectionEventArgs(_selectedId));
    }

    private void ToggleCheckBox(OptionControl checkBox)
    {
        _selectedId = checkBox.Index;
        checkBox.Checked = !checkBox.Checked;
        if (CheckboxStates == null || CheckboxStates.Count < checkBox.Index)
        {
            var old = CheckboxStates ?? [];
            CheckboxStates = old.Concat(Enumerable.Repeat(false, checkBox.Index + 1)).ToList();
        }

        CheckboxStates[checkBox.Index] = checkBox.Checked;
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
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