﻿using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
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
    }

    /// <summary>
    /// Upper value of scrollable range.
    /// </summary>
    public int Maximum { get; set; } = 0;

    public int Position => _scrollPos;

    // Length of the arrows and the thumb along the bar. The images are always this long;
    // _scrollbarDim is only the thickness of the bar.
    private const int ButtonLength = ScrollbarDimDefault;

    /// <summary>
    /// How far a click on the track (beside the thumb) scrolls: one page. The owner sets it,
    /// e.g. a listbox to its number of visible rows.
    /// </summary>
    public int LargeChange { get; set; } = 1;

    // Holding an arrow or the track: one step on press, then after a delay one step per interval (as in Windows)
    private const string RepeatEventName = "ScrollBarRepeat";   // only one scrollbar can be held at a time
    private static readonly TimeSpan RepeatDelay = TimeSpan.FromSeconds(0.4);
    private static readonly TimeSpan RepeatInterval = TimeSpan.FromSeconds(0.05);

    private enum HeldPart { None, Arrow, Track }
    private HeldPart _held;
    private int _heldDirection;     // -1 = up/left, +1 = down/right
    private Vector2 _mousePos;      // last mouse position while held; the repeat pauses while it's off the held part

    /// <summary>
    /// Whether the held arrow/track part is still under the mouse. For the track this also stops
    /// the paging once the thumb reaches the mouse.
    /// </summary>
    private bool OverHeldPart() => _held switch
    {
        HeldPart.Arrow => ArrowAt(_mousePos) == _heldDirection,
        HeldPart.Track => TrackAt(_mousePos) == _heldDirection,
        _ => false
    };

    /// <summary>
    /// Which side of the thumb on the track is at this position: -1 = before the thumb, +1 = after it, 0 = neither.
    /// </summary>
    private int TrackAt(Vector2 pos)
    {
        var across = _vertical ? pos.X : pos.Y;
        if (across < 0 || across >= _scrollbarDim || ArrowAt(pos) != 0 || Maximum <= 0) return 0;

        var along = Along(pos);
        var thumbStart = ButtonLength + ThumbOffset();
        if (along >= ButtonLength && along < thumbStart) return -1;
        if (along >= thumbStart + ButtonLength && along < (_vertical ? Height : Width) - ButtonLength) return 1;
        return 0;
    }

    /// <summary>
    /// Scroll for a mouse wheel movement (positive = away from the user = up/left): up to 3 rows per notch,
    /// but never more than a page. Returns false if there is nothing to scroll, so the wheel can go elsewhere.
    /// </summary>
    public bool ScrollByWheel(float wheelDelta)
    {
        if (!Visible || Maximum <= 0 || wheelDelta == 0) return false;

        var perNotch = Math.Min(3, Math.Max(1, LargeChange));
        var notches = Math.Max(1, (int)Math.Round(Math.Abs(wheelDelta)));
        ScrollBy(-Math.Sign(wheelDelta) * perNotch * notches);
        return true;
    }

    public override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (ScrollByWheel(e.WheelDelta))
        {
            e.Handled = true;
        }
    }

    private void StepHeldPart()
    {
        ScrollBy(_heldDirection * (_held == HeldPart.Track ? Math.Max(1, LargeChange) : 1));
    }

    /// <summary>
    /// Which arrow is at this position (relative to the scrollbar): -1 = up/left, +1 = down/right, 0 = none.
    /// </summary>
    private int ArrowAt(Vector2 pos)
    {
        var along = _vertical ? pos.Y : pos.X;
        var across = _vertical ? pos.X : pos.Y;
        var length = _vertical ? Height : Width;

        if (across < 0 || across >= _scrollbarDim) return 0;
        if (along >= 0 && along < ButtonLength) return -1;
        if (along >= length - ButtonLength && along < length) return 1;
        return 0;
    }

    public override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (e.Button != MouseButton.Left) return;

        _mousePos = e.Location;
        var arrow = ArrowAt(e.Location);
        var track = TrackAt(e.Location);
        if (arrow != 0 || track != 0)
        {
            _held = arrow != 0 ? HeldPart.Arrow : HeldPart.Track;
            _heldDirection = arrow != 0 ? arrow : track;
            StepHeldPart();
            ScheduleRepeat(RepeatDelay);
        }
        else if (IsOnThumb(e.Location))
        {
            _dragging = true;
            _dragGrab = Along(e.Location) - (ButtonLength + ThumbOffset());
        }
    }

    public override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _mousePos = e.Location;

        if (_dragging)
        {
            if (!Capture)   // the press ended without a mouse up reaching us
            {
                _dragging = false;
                return;
            }
            DragTo(e.Location);
        }
    }

    public override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (e.Button == MouseButton.Left)
        {
            StopRepeat();
            _dragging = false;
        }
    }

    // Dragging the thumb: it jumps from row to row (no pixel scrolling), to the row nearest the mouse
    private bool _dragging;
    private float _dragGrab;    // where on the thumb it was grabbed, so the thumb doesn't jump on press

    private float Along(Vector2 pos) => _vertical ? pos.Y : pos.X;

    /// <summary>
    /// Room the thumb can move in, in pixels (between the two arrows, minus the thumb itself).
    /// </summary>
    private int TrackLength => Math.Max(0, (_vertical ? Height : Width) - 3 * ButtonLength);

    private bool IsOnThumb(Vector2 pos)
    {
        if (Maximum <= 0) return false;

        var start = ButtonLength + ThumbOffset();
        var along = Along(pos);
        return along >= start && along < start + ButtonLength;
    }

    private void DragTo(Vector2 pos)
    {
        if (Maximum <= 0 || TrackLength == 0) return;

        var thumbStart = Along(pos) - _dragGrab - ButtonLength;
        var position = (int)Math.Round(thumbStart * Maximum / TrackLength);
        ScrollBy(Math.Clamp(position, 0, Maximum) - _scrollPos);
    }

    private void ScheduleRepeat(TimeSpan delay)
    {
        Controller.MainWindow.Schedule(RepeatEventName, delay, () =>
        {
            // Stop if the press ended without a mouse up reaching us (e.g. the dialog closed)
            if (_held == HeldPart.None || !Capture)
            {
                _held = HeldPart.None;
                return;
            }

            if (OverHeldPart())
            {
                StepHeldPart();
            }
            ScheduleRepeat(RepeatInterval);
        });
    }

    private void StopRepeat()
    {
        if (_held == HeldPart.None) return;

        _held = HeldPart.None;
        Controller.MainWindow.ClearSchedule(RepeatEventName);
    }

    /// <summary>
    /// Scroll by this many rows/columns, staying within 0..Maximum. Does nothing at either end.
    /// </summary>
    private void ScrollBy(int steps)
    {
        var position = Math.Clamp(_scrollPos + steps, 0, Math.Max(0, Maximum));
        if (position == _scrollPos) return;

        SetScrollPosition(position);
    }

    /// <summary>
    /// Set the scroll position and tell the owner (also used by the owner to keep the scrollbar in sync).
    /// </summary>
    public void SetScrollPosition(int position)
    {
        _scrollPos = Math.Max(0, position);
        _scrollAction(_scrollPos);
    }

    /// <summary>
    /// Offset of the thumb from the end of the up/left arrow, in pixels.
    /// </summary>
    private int ThumbOffset()
    {
        if (Maximum <= 0) return 0;

        return (int)((double)Math.Min(_scrollPos, Maximum) * TrackLength / Maximum);
    }

    public override void Draw(bool pulse)
    {
        if (!Visible) return;

        Graphics.DrawRectangleRec(Bounds, Color.White);

        if (_vertical)
        {
            Graphics.DrawTexture(_images[0], (int)Bounds.X, (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[1], (int)Bounds.X, (int)Bounds.Y + ButtonLength + ThumbOffset(), Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X, (int)Bounds.Y + Height - ButtonLength, Color.White);
        }
        else
        {
            Graphics.DrawTexture(_images[0], (int)Bounds.X, (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[1], (int)Bounds.X + ButtonLength + ThumbOffset(), (int)Bounds.Y, Color.White);
            Graphics.DrawTexture(_images[2], (int)Bounds.X + Width - ButtonLength, (int)Bounds.Y, Color.White);
        }
    }
}