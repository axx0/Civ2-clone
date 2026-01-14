﻿using Model;
using Model.Interface;
using Neo.IronLua;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Transformations;
using RaylibUI.BasicTypes.Controls;
using System.Diagnostics;

namespace RaylibUI.BasicTypes;

public class Listbox : BaseControl
{
    private readonly IControlLayout _controller;
    private TableLayout _tableLayout;
    private List<ListboxControlGroup> _controls;
    private int _totalColumns, _totalRows;   // visible & non-visible rows & columns
    private ScrollBar _VscrollBar, _HscrollBar;
    private int _viewStartingRow, _viewStartingCol;
    private readonly IUserInterface? _active;
    private readonly ListboxDefinition _def;
    private int _selectedId;

    public event EventHandler<ListboxSelectionEventArgs>? ItemSelected;

    public override bool CanFocus => true;

    public Listbox(IControlLayout controller, ListboxDefinition def) : base(controller)
    {
        _active = controller.MainWindow.ActiveInterface;
        _controller = controller;
        _def = def;
        _selectedId = def.SelectedId;

        if (_def.Type != null)
        {
            _def.Looks = _active.GetListboxLooks(_def.Type);
        }

        Update();
    }

    public void Update(bool resetSelection = false)
    {
        Controls = [];
        _controls = [];

        if (resetSelection)
        {
            _selectedId = 0;
        }

        for (int i = 0; i < _def.Groups.Count; i++)
        {
            _controls.Add(new ListboxControlGroup(_controller, _def, i));
        }

        // Get total rows & columns (visible & non-visible)
        if (_def.VerticalScrollbar)
        {
            _totalColumns = _def.Columns;
            _totalRows = (int)Math.Ceiling((double)_controls.Count / (double)_totalColumns);
        }
        else
        {
            _totalRows = _def.Rows;
            _totalColumns = (int)Math.Ceiling((double)_controls.Count / (double)_totalRows);
        }

        _viewStartingRow = 0;
        _viewStartingCol = 0;

        if (_def.VerticalScrollbar)
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

        foreach (var control in _controls)
        {
            control.Selected = Selected;
            Controls.Add(control);
        }

        _tableLayout = new TableLayout();
        for (int col = 0; col < _totalColumns; col++)
        {
            for (int row = 0; row < _totalRows; row++)
            {
                var index = _totalRows * col + row;

                if (_controls.Count < index + 1)
                {
                    break;
                }

                _tableLayout.Add(_controls[index], row, col);
            }
        }

        // Select control at start
        if (_selectedId == -1)  // If this is true there's something wrong
        {
            _selectedId = 0;
        }
        _controls[_selectedId].SelectThis(true);

        OnResize();
    }

    public override void OnResize()
    {
        foreach (var control in _controls)
        {
            control.Width = Width / _def.Columns;

            // Change width of controls so they don't overlap with scrollbar
            if (_def.VerticalScrollbar && _controls.Count > _def.Rows)
            {
                control.Width -= _VscrollBar.Width;
            }
            control.Width -= 4;

            control.OnResize();
        }

        _tableLayout?.CalculateDimensions(_viewStartingRow, _viewStartingCol, _def.Rows, _def.Columns);

        Height = _def.Rows * _controls[0].Height + (_def.VerticalScrollbar ? 0 : _HscrollBar.Height);

        if (_def.VerticalScrollbar)
        {
            _VscrollBar.Maximum = _tableLayout.RowCount - _def.Rows;
            _VscrollBar.Location = new(Width - _VscrollBar.Width - 2, 2);
            _VscrollBar.Height = Height - 4;
            _VscrollBar.Visible = _def.Rows < _tableLayout.RowCount;
        }
        else
        {
            _HscrollBar.Location = new(2, Height - _HscrollBar.Height - 2);
            _HscrollBar.Maximum = _tableLayout.ColumnCount - _def.Columns;
            _HscrollBar.Width = Width - 4;
            _HscrollBar.Visible = _def.Columns < _tableLayout.ColumnCount;
            _HscrollBar.Location = new(_HscrollBar.Location.X, Height - _HscrollBar.Height);
        }
    }

    private void Selected(ListboxControlGroup control, bool soft)
    {
        _selectedId = _controls.IndexOf(control);

        // If the selected control is beyond the view
        var selectedRow = _selectedId % _totalRows;
        var selectedCol = _selectedId / _totalRows;
        if (_viewStartingRow + _def.Rows <= selectedRow)
        {
            _viewStartingRow = selectedRow - _def.Rows + 1;
            _VscrollBar?.SetScrollPosition(_viewStartingRow);
        }
        else if (selectedRow < _viewStartingRow)
        {
            _viewStartingRow = selectedRow;
            _VscrollBar?.SetScrollPosition(_viewStartingRow);
        }
        if (_viewStartingCol + _def.Columns <= selectedCol)
        {
            _viewStartingCol = selectedCol - _def.Columns + 1;
            _HscrollBar?.SetScrollPosition(_viewStartingCol);
        }
        else if (selectedCol < _viewStartingCol)
        {
            _viewStartingCol = selectedCol;
            _HscrollBar?.SetScrollPosition(_viewStartingCol);
        }

        // If a control has been selected, change its appearance
        if (_def.Selectable)
        {
            foreach (var groupElement in _controls)
            {
                foreach (var text in groupElement.Controls.Where(c => c is LabelControl))
                {
                    var label = (LabelControl)text;
                    label.BackgroundColor = groupElement == control ? _def.Looks.SelectedTextBackgroundColor : null;
                    label.Font = groupElement == control ? _def.Looks.SelectedTextFont : _def.Looks.Font;
                    label.ColorFront = groupElement == control ? _def.Looks.SelectedTextColorFront : _def.Looks.TextColorFront;
                    label.ColorShadow = groupElement == control ? _def.Looks.SelectedTextColorShadow : _def.Looks.TextColorShadow;
                    label.ShadowOffset = groupElement == control ? _def.Looks.SelectedTextShadowOffset : _def.Looks.TextShadowOffset;
                }
            }
        }

        ItemSelected?.Invoke(control,
            new ListboxSelectionEventArgs(_selectedId, soft));
    }

    public void ScrollToEnd()
    {
        _viewStartingRow = Math.Max(0, _totalRows - _def.Rows);
        _VscrollBar.SetScrollPosition(_viewStartingRow);
    }

    public void EnterPressed()
    {
        _controls[_selectedId].SelectThis(false);
    }

    public override bool OnKeyPressed(KeyboardKey key)
    {
        if (!_def.Selectable) return base.OnKeyPressed(key);

        int selectedCol;
        switch (key)
        {
            case KeyboardKey.Down or KeyboardKey.Kp2:
                _selectedId = Math.Min(_selectedId + 1, _controls.Count - 1);
                _controls[_selectedId].SelectThis(true);
                return true;
            case KeyboardKey.Up or KeyboardKey.Kp8:
                _selectedId = Math.Max(_selectedId - 1, 0);
                _controls[_selectedId].SelectThis(true);
                return true;
            case KeyboardKey.End:
                _selectedId = _controls.Count - 1;
                _controls[_selectedId].SelectThis(true);
                return true;
            case KeyboardKey.Home:
                _selectedId = 0;
                _controls[_selectedId].SelectThis(true);
                return true;
            case KeyboardKey.Left or KeyboardKey.Kp4:
                selectedCol = _selectedId / _totalRows;
                if (selectedCol > 0)
                {
                    _selectedId -= _def.Rows;
                    _controls[_selectedId].SelectThis(true);
                }
                return true;
            case KeyboardKey.Right or KeyboardKey.Kp6:
                selectedCol = _selectedId / _totalRows;
                if (selectedCol < _totalColumns - 1 && _selectedId + _def.Rows <= _controls.Count - 1)
                {
                    _selectedId += _def.Rows;
                    _controls[_selectedId].SelectThis(true);
                }
                return true;
            case KeyboardKey.PageDown:
                _selectedId = Math.Min(_selectedId + _def.Rows, _controls.Count - 1);
                _controls[_selectedId].SelectThis(true);
                return true;
            case KeyboardKey.PageUp:
                _selectedId = Math.Max(_selectedId - _def.Rows, 0);
                _controls[_selectedId].SelectThis(true);
                return true;
        }

        return base.OnKeyPressed(key);
    }

    public override void Draw(bool pulse)
    {
        Graphics.DrawRectangleRec(Bounds, _def.Looks.BoxBackgroundColor);
        Graphics.DrawRectangleLinesEx(Bounds, 1f, _def.Looks.BoxLineColor);

        base.Draw(pulse);
    }
}