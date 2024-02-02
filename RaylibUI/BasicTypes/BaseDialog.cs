using System.Numerics;
using Model;
using Raylib_cs;

namespace RaylibUI;

public abstract class BaseDialog : BaseLayoutController
{
    protected Texture2D? BackgroundImage;
    private readonly Point _position;
    
    protected BaseDialog(Main main, Point? position = null) : base(main, main.ActiveInterface.DialogPadding)
    {
        _position = position ?? new Point(0,0);
    }

    public override void Move(Vector2 moveAmount)
    {
        Location += moveAmount;
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
        var X = _position.X switch
        {
            0 => (screenWidth - dialogWidth) / 2f,
            < -1 => screenWidth + _position.X - dialogWidth,
            < 0 =>  (1+ _position.X) * screenWidth - dialogWidth,
            > 1 => _position.X,
            _ => _position.X * screenWidth
        };
        
        var Y = _position.Y switch
        {
            0 => (screenHeight - dialogHeight) / 2f,
            < -1 => screenHeight + _position.Y - dialogHeight,
            < 0 => (1+ _position.Y) * screenHeight - dialogHeight,
            > 1 => _position.Y,
            _ => _position.Y * screenHeight
        };

        Location = new Vector2((float)Math.Max(0, Math.Min(screenWidth - dialogWidth, X)),
            (float)Math.Max(0, Math.Min(screenHeight - dialogHeight, Y)));
    }


    public override void Draw(bool pulse)
    {
        Raylib.DrawTexture(BackgroundImage.Value,(int)Location.X, (int)Location.Y, Color.WHITE);
        foreach (var control in Controls)
        {
            control.Draw(pulse);
        }
    }
    
    protected int PaddingSide => LayoutPadding.Left + LayoutPadding.Right;
}