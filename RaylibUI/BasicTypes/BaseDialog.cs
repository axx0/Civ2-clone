using System.Numerics;
using Model;
using Raylib_cs;

namespace RaylibUI;

public abstract class BaseDialog : BaseLayoutController
{
    protected Vector2 _location;
    protected Texture2D? BackgroundImage;

    protected BaseDialog(Main main, Point? position = null) : base(main)
    {
        Position = position ?? new Point(0,0);
    }

    public Point Position { get; set; }

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

    protected void SetLocation(int screenWidth, int dialogWidth, int screenHeight, int dialogHeight)
    {
        _location.X = Position.X switch
        {
            0 => (screenWidth - dialogWidth) / 2f,
            < 0 => screenWidth + (int)Position.X,
            _ => _location.X
        };

        _location.Y = Position.Y switch
        {
            0 => (screenHeight - dialogHeight) / 2f,
            < 0 => screenHeight + (int)Position.Y,
            _ => _location.Y
        };
    }


    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(BackgroundImage.Value,(int)_location.X, (int)_location.Y, Color.WHITE);
        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }
    }
}