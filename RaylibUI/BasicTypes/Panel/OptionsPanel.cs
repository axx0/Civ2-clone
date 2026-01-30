using Civ2engine.Scripting;
using Model;
using Model.Controls;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using RaylibUtils;
using static System.Net.Mime.MediaTypeNames;

namespace RaylibUI.BasicTypes;

public class OptionsPanel : BaseControl
{
    private readonly IUserInterface _active;
    private readonly IControlLayout _controller;
    private readonly TableLayout _tableLayout;
    private readonly List<OptionControl> _optionControls = [];
    private readonly int _columns, _rows, _maxVisibleColumns, _maxVisibleRows;
    private readonly ScrollBar _VscrollBar, _HscrollBar;
    private int _startingControlRow, _startingControlCol;
    private readonly OptionsDefinition _def;

    public override bool CanFocus => true;

    public Color BackgroundColour { get; set; } = Color.Blank;

    public int SelectedId { get; set; }

    public IList<bool>? CheckboxStates { get; set; }

    public OptionsPanel(IControlLayout controller, OptionsDefinition def) : base(controller)
    {
        _controller = controller;
        _active = controller.MainWindow.ActiveInterface;
        _def = def;

        // Make controls
        var images = def.IsCheckbox ? _active.Look.CheckBoxes : _active.Look.RadioButtons;
        for (int i = 0; i < (def.Icons == null ? def.Texts.Count : def.Icons.Length); i++)
        {
            _optionControls.Add(
                new OptionControl(controller, this, def.ReplacedTexts[i], i,
                def.CheckboxStates?[i] ?? false, i < (def.Icons?.Length ?? 0) ? [def.Icons[i]] : images));
        }

        SelectedId = def.SelectedId;
        CheckboxStates = def.CheckboxStates;
        if (!def.IsCheckbox)
        {
            _optionControls[SelectedId].Checked = true;
        }
        _columns = def.Columns;
        _maxVisibleColumns = def.MaxVisibleCols;
        _maxVisibleRows = def.MaxVisibleRows;

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

        var optionAction = def.IsCheckbox ? (Action<OptionControl>)ToggleCheckBox : SetSelectedOption;
        foreach (var option in _optionControls)
        {
            option.Click += (_, _) => optionAction(option);
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
            option.Width = Width / Math.Min(_columns, _maxVisibleColumns);
        }

        _tableLayout?.CalculateDimensions(_startingControlRow, _startingControlCol, _maxVisibleRows, _maxVisibleColumns);

        Height = _tableLayout.GetHeight(_startingControlRow, _maxVisibleRows);

        _VscrollBar.Visible = _maxVisibleRows < _tableLayout.RowCount;
        _VscrollBar.Maximum = _tableLayout.RowCount - _maxVisibleRows;
        _VscrollBar.Location = new(Width - _VscrollBar.Width - 2, 2);
        _VscrollBar.Height = Height - 4;

        _HscrollBar.Visible = _maxVisibleColumns < _tableLayout.ColumnCount;
        if (_HscrollBar.Visible)
        {
            _HscrollBar.Location = new(2, Height - _HscrollBar.Height - 2);
            _HscrollBar.Maximum = _tableLayout.ColumnCount - _maxVisibleColumns;
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
        if (_optionControls[SelectedId] == newSelection) return;
        if (!_def.IsCheckbox)
        {
            newSelection.Checked = true;
            _optionControls[SelectedId]?.Clear();
        }
        SelectedId = newSelection.Index;
    }

    private void ToggleCheckBox(OptionControl checkBox)
    {
        SelectedId = checkBox.Index;
        checkBox.Checked = !checkBox.Checked;
        if (CheckboxStates == null || CheckboxStates.Count < checkBox.Index)
        {
            var old = CheckboxStates ?? new List<bool>();
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
                newId = SelectedId == 0 ? _optionControls.Count - 1 : SelectedId - 1;
                SetSelectedOption(_optionControls[newId]);
                return true;
            case KeyboardKey.Down or KeyboardKey.Kp2:
                newId = SelectedId < _optionControls.Count - 1 ? SelectedId + 1 : 0;
                SetSelectedOption(_optionControls[newId]);
                return true;
            case KeyboardKey.Left or KeyboardKey.Kp4:
                if (!_def.IsCheckbox)
                {
                    if (_def.Columns == 1)
                    {
                        newId = SelectedId == 0 ? _optionControls.Count - 1 : SelectedId - 1;
                    }
                    else
                    {
                        newId = SelectedId - GetRows();
                        if (newId < 0)
                        {
                            newId += _optionControls.Count;
                        }
                    }
                    SetSelectedOption(_optionControls[newId]);
                }
                return true;
            case KeyboardKey.Right or KeyboardKey.Kp6:
                if (!_def.IsCheckbox)
                {
                    if (_def.Columns == 1)
                    {
                        newId = SelectedId == _optionControls.Count - 1 ? 0 : SelectedId + 1;
                    }
                    else
                    {
                        newId = SelectedId + GetRows();
                        if (newId >= _optionControls.Count)
                        {
                            newId -= _optionControls.Count;
                        }
                    }
                    SetSelectedOption(_optionControls[newId]);
                }
                return true;
            case KeyboardKey.Space:
                if (_def.IsCheckbox)
                {
                    ToggleCheckBox(_optionControls[SelectedId]);
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