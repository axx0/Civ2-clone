﻿using Model;
using System.Numerics;

namespace RaylibUI.BasicTypes;

public class Cell
{
    public IControl? Control;
    public int Row { get; }
    public int Column { get; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Vector2 Location { get; set; }
    public Padding Padding { get; set; } = Padding.None;

    /// <summary>
    /// Control fills the column's full width (minus padding) instead of keeping its own width.
    /// </summary>
    public bool Stretch { get; set; }

    public Cell(IControl control, int row, int col, Padding? padding = null, bool stretch = false)
    {
        Control = control;
        Row = row;
        Column = col;
        Stretch = stretch;
        if (padding != null)
        {
            Padding = (Padding)padding;
        }
    }
}