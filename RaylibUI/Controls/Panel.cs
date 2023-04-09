using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Controls;

public class Panel : Form, IForm
{
    Button btn;
    public Panel(int x, int y, Size size, string title)
    {
        X = x;
        Y = y;
        Size = size;
        _formPosX = X;
        _formPosY = Y;

        Title = new FormattedText
        {
            Text = title,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        btn = new Button() { Text = "OK", Width = 100, Height = 50 };
        Controls.Add(btn);
    }

    public void Disable()
    {
        Enabled = false;
        Controls.ForEach(c => c.Enabled = false);
    }

    public void Draw()
    {
        base.Draw();

        btn.Draw(_formPosX + 10, _formPosY + 10);
        if (btn.Pressed)
            Raylib.DrawText("Pressed", 100, 100, 20, Color.RED);
    }
}
