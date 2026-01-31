﻿using Raylib_CSharp.Colors;
using Raylib_CSharp.Rendering;
using Raylib_CSharp.Textures;
using System.Diagnostics;
using System.Numerics;

namespace RaylibUI.BasicTypes.Controls;

public class ScrollBar : BaseControl
{
    public const int ScrollBarDim = 17;
    private readonly Action<int> _scrollAction;
    private readonly Texture2D[] _images;
    private readonly bool _vertical;
    private int _scrollPos;
    private double _increment;

    public ScrollBar(IControlLayout controller, Action<int> scrollAction, bool vertical = true) : base(controller)
    {
        _scrollAction = scrollAction;
        _vertical = vertical;
        _images = ImageUtils.GetScrollImages(ScrollBarDim, _vertical).Select(Texture2D.LoadFromImage).ToArray();
        if (_vertical)
        {
            Width = ScrollBarDim;
        }
        else
        {
            Height = ScrollBarDim;
        }
        _scrollPos = 0;
        Click += OnClick;
    }

    /// <summary>
    /// Upper value of scrollable range.
    /// </summary>
    public int Maximum { get; set; } = 0;

    public void OnClick(object? sender, MouseEventArgs mouseEventArgs)
    {
        var pos = GetRelativeMousePosition();

        if ((_vertical && pos.Y < ScrollBarDim) ||
            (!_vertical && pos.X < ScrollBarDim))   // Up or left arrow clicked
        {
            SetScrollPosition(Math.Max(_scrollPos - 1, 0));
        }
        else if ((_vertical && pos.Y > Height - ScrollBarDim) ||
            (!_vertical && pos.X > Width - ScrollBarDim)) // Down or right arrow clicked
        {
            SetScrollPosition(Math.Min(_scrollPos + 1, Maximum));
        }
    }

    public void SetScrollPosition(int position)
    {
        _increment = _vertical ? (Height - 3 * ScrollBarDim) / (double)Maximum : (Width - 3 * ScrollBarDim) / (double)Maximum;
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
            Graphics.DrawTexture(_images[1], (int)Bounds.X, (int)Bounds.Y + ScrollBarDim + (int)(_scrollPos * _increment), Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X, (int)Bounds.Y + Height - ScrollBarDim, Color.White);
        }
        else
        {
            Graphics.DrawTexture(_images[0], (int)Bounds.X, (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[1], (int)Bounds.X + ScrollBarDim + (int)(_scrollPos * _increment), (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X + Width - ScrollBarDim, (int)Bounds.Y, Color.White);
        }
    }
}