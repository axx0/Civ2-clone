﻿using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using System.Diagnostics;
using System.Numerics;

namespace RaylibUI.BasicTypes.Controls;

public class ScrollBar : BaseControl
{
    public const int ScrollbarDimDefault = 17;
    private int _scrollbarDim;
    private readonly Action<int> _scrollAction;
    private readonly Texture2D[] _images;
    private readonly bool _vertical;
    private int _scrollPos;
    private double _increment;

    public ScrollBar(IControlLayout controller, Action<int> scrollAction, bool vertical = true, int? scrollbarDim = null) : base(controller)
    {
        _scrollAction = scrollAction;
        _vertical = vertical;
        _scrollbarDim = scrollbarDim == null ? ScrollbarDimDefault : (int)scrollbarDim;
        _images = ImageUtils.GetScrollImages(_scrollbarDim, _vertical).Select(Texture2D.LoadFromImage).ToArray();
        if (_vertical)
        {
            Width = _scrollbarDim;
        }
        else
        {
            Height = _scrollbarDim;
        }
        _scrollPos = 0;
        Click += OnClick;
    }

    /// <summary>
    /// Upper value of scrollable range.
    /// </summary>
    public int Maximum { get; set; } = 0;

    public int Position => _scrollPos;

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        var pos = GetRelativeMousePosition();

        if ((_vertical && pos.Y < _scrollbarDim) ||
            (!_vertical && pos.X < _scrollbarDim))   // Up or left arrow clicked
        {
            SetScrollPosition(Math.Max(_scrollPos - 1, 0));
        }
        else if ((_vertical && pos.Y > Height - _scrollbarDim) ||
            (!_vertical && pos.X > Width - _scrollbarDim)) // Down or right arrow clicked
        {
            SetScrollPosition(Math.Min(_scrollPos + 1, Maximum));
        }
    }

    public void SetScrollPosition(int position)
    {
        _increment = _vertical ? (Height - 3 * ScrollbarDimDefault) / (double)Maximum : 
            (Width - 3 * ScrollbarDimDefault) / (double)Maximum;
        _scrollPos = position;
        _scrollAction(_scrollPos);
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        Graphics.DrawRectangleRec(Bounds, Color.White);

        if (_vertical)
        {
            Graphics.DrawTexture(_images[0], (int)Bounds.X, (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[1], (int)Bounds.X, (int)Bounds.Y + ScrollbarDimDefault + (int)(_scrollPos * _increment), Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X, (int)Bounds.Y + Height - ScrollbarDimDefault, Color.White);
        }
        else
        {
            Graphics.DrawTexture(_images[0], (int)Bounds.X, (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[1], (int)Bounds.X + ScrollbarDimDefault + (int)(_scrollPos * _increment), (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X + Width - ScrollbarDimDefault, (int)Bounds.Y, Color.White);
        }
    }
}