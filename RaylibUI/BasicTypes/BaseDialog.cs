using System.Numerics;
using Model;
using Raylib_cs;
using RaylibUI.Controls;
using RaylibUI.Forms;

namespace RaylibUI;

public class BaseDialog : BaseLayoutController
{
    private Size _size;
    private Vector2 _location;
    public Point Position { get; }
    
    protected BaseDialog(string title, Point? position = null) 
    {
        Position = position ?? new Point(0,0);
        if (!string.IsNullOrWhiteSpace(title))
        {
            Controls.Add(new HeaderLabel(this, title));
        }

        _location = Location;
    }

    public Vector2 Location { get; }

    public override void Resize(int width, int height)
    {
        var heights = new int[Controls.Count];
        var maxWidth = 0;
        var totalHeight = 0;
        for (var index = 0; index < Controls.Count; index++)
        {
            var control = Controls[index];
            var size = control.GetPreferredSize(width, height);
            if (size.Width > maxWidth )
            {
                maxWidth = size.Width;
            }
            heights[index] = size.Height;
            totalHeight += size.Height;
        }

        _location.X = Position.X switch
        {
            0 => (width - maxWidth) / 2f,
            < 0 => width + (int)Position.X,
            _ => _location.X
        };

        _location.Y = Position.Y switch
        {
            0 => (height - totalHeight) / 2f,
            < 0 => height + (int)Position.Y,
            _ => _location.Y
        };

        int left = 11 + (int)_location.X;
        int top = (int)_location.Y;
        for (int index = 0; index < Controls.Count; index++)
        {
            Controls[index].Bounds = new Rectangle(left, top, maxWidth, heights[index]);
            top += heights[index];
            Controls[index].OnResize();
        }

        _size = new Size(maxWidth + 11 * 2, totalHeight + 22);

    }

    public override void Draw(bool pulse)
    {
        ImageUtils.PaintDialogBase((int)_location.X, (int)_location.Y, _size.Width, _size.Height,
             new Padding(11, 11, 38, 46));
        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }
    }

    public override void Move(Vector2 moveAmount)
    {
        _location += moveAmount;
        MoveChildren(moveAmount, Controls);
    }


    private static void MoveChildren(Vector2 moveAmount, IEnumerable<IControl> controls)
    {
        foreach (var control in controls)
        {
            control.Location += moveAmount;
            if (control.Children != null)
            {
                MoveChildren(moveAmount, control.Children);
            }
        }
    }
}