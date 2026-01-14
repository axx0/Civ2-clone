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

    public Cell(IControl control, int row, int col, Padding? padding = null)
    {
        Control = control;
        Row = row;
        Column = col;
        if (padding != null)
        {
            Padding = (Padding)padding;
        }
    }
}