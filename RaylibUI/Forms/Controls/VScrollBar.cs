using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public class VScrollBar : Control
{
    bool _topArrowPressed = false;
    bool _btmArrowPressed = false;
    bool _barTopPressed = false;
    bool _barBtmPressed = false;

    public int Height { get; set; }
    public int Width { get; set; } = 17;

    /// <summary>
    /// Max no of items in scrollbar range
    /// </summary>
    public int Maximum { get; set; } = 100;

    /// <summary>
    /// Index of element on bar top position
    /// </summary>
    public int Value { get; set; } = 0;

    /// <summary>
    /// No of items visible next to scrollbar
    /// </summary>
    public int VisibleItems { get; set; }

    public void Draw(int x, int y)
    {
        Vector2 mousePos = Raylib.GetMousePosition();
        int sliderOffset = 17;
        if (Maximum != VisibleItems)
        {
            sliderOffset = 17 + (Height - 3 * 17) / (Maximum - VisibleItems) * Value;
        }

        // Determine user interaction
        _topArrowPressed = false;
        _btmArrowPressed = false;
        _barTopPressed = false;
        _barBtmPressed = false;

        if (Enabled)
        {
            _topArrowPressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y, Width, 17));
            _btmArrowPressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y + Height - 17, Width, 17));

            // Is bar (without the slider) pressed?
            if (Value == 0) // slider on top
            {
                if (Maximum > VisibleItems)
                {
                    _barBtmPressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y + 2 * 17, Width, Height - 3 * 17));
                }
            }
            else if (Value == Maximum - VisibleItems) // slider on bottom
            {
                _barTopPressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y + 17, Width, Height - 3 * 17));
            }
            else
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y + 17, Width, sliderOffset - 17)))
                {
                    _barTopPressed = true;
                }
                else if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT) && Raylib.CheckCollisionPointRec(mousePos, new Rectangle(x, y + sliderOffset + 17, Width, Height - sliderOffset - 2 * 17)))
                {
                    _barBtmPressed = true;
                }
            }
        }

        if (_barTopPressed)
        {
            Value = Math.Max(Value - VisibleItems + 1, 0);
        }
        if (_barBtmPressed)
        {
            Value = Math.Min(Value + VisibleItems - 1, Maximum - VisibleItems);
        }

        if (_topArrowPressed)
        {
            Value = Math.Max(0, Value - 1);
        }

        if (_btmArrowPressed)
        {
            if (Value + 1 <= Maximum - VisibleItems)
            {
                Value++;
            }
        }

        Raylib.DrawRectangle(x, y, Width, Height, new Color(240, 240, 240, 255));

        // Top arrow
        if (!_topArrowPressed)
        {
            Raylib.DrawLine(x, y, x + Width - 1, y, new Color(227, 227, 227, 255));
            Raylib.DrawLine(x + 1, y, x + 1, y + 16, new Color(227, 227, 227, 255));
            Raylib.DrawLine(x, y + 16, x + Width, y + 16, new Color(105, 105, 105, 255));
            Raylib.DrawLine(x + Width, y, x + Width, y + 16, new Color(105, 105, 105, 255));
            Raylib.DrawLine(x + 2, y + 1, x + 2, y + 15, new Color(255, 255, 255, 255));
            Raylib.DrawLine(x + 2, y + 1, x + Width - 2, y + 1, new Color(255, 255, 255, 255));
            Raylib.DrawLine(x + 1, y + 15, x + Width - 2, y + 15, new Color(160, 160, 160, 255));
            Raylib.DrawLine(x + Width - 1, y + 1, x + Width - 1, y + 16, new Color(160, 160, 160, 255));
        }
        else
        {
            Raylib.DrawRectangleLines(x, y, Width, 17, new Color(160, 160, 160, 255));
        }
        int offsetArrow = _topArrowPressed ? 1 : 0;
        Raylib.DrawLine(x + Width / 2 - 3 + offsetArrow, y + 9 + offsetArrow, x + Width / 2 + 4 + offsetArrow, y + 9 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 - 2 + offsetArrow, y + 8 + offsetArrow, x + Width / 2 + 3 + offsetArrow, y + 8 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 - 1 + offsetArrow, y + 7 + offsetArrow, x + Width / 2 + 2 + offsetArrow, y + 7 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 + offsetArrow, y + 6 + offsetArrow, x + Width / 2 + 1 + offsetArrow, y + 6 + offsetArrow, Color.BLACK);

        // Bottom arrow
        if (!_btmArrowPressed)
        {
            Raylib.DrawLine(x, y + Height - 17, x + Width - 1, y + Height - 17, new Color(227, 227, 227, 255));
            Raylib.DrawLine(x + 1, y + Height - 17, x + 1, y + Height - 1, new Color(227, 227, 227, 255));
            Raylib.DrawLine(x, y + Height - 1, x + Width, y + Height - 1, new Color(105, 105, 105, 255));
            Raylib.DrawLine(x + Width, y + Height - 17, x + Width, y + Height - 1, new Color(105, 105, 105, 255));
            Raylib.DrawLine(x + 2, y + Height - 16, x + 2, y + Height - 2, new Color(255, 255, 255, 255));
            Raylib.DrawLine(x + 2, y + Height - 16, x + Width - 2, y + Height - 16, new Color(255, 255, 255, 255));
            Raylib.DrawLine(x + 1, y + Height - 2, x + Width - 2, y + Height - 2, new Color(160, 160, 160, 255));
            Raylib.DrawLine(x + Width - 1, y + Height - 16, x + Width - 1, y + Height - 1, new Color(160, 160, 160, 255));
        }
        offsetArrow = _btmArrowPressed ? 1 : 0;
        Raylib.DrawLine(x + Width / 2 - 3 + offsetArrow, y + Height - 10 + offsetArrow, x + Width / 2 + 4 + offsetArrow, y + Height - 10 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 - 2 + offsetArrow, y + Height - 9 + offsetArrow, x + Width / 2 + 3 + offsetArrow, y + Height - 9 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 - 1 + offsetArrow, y + Height - 8 + offsetArrow, x + Width / 2 + 2 + offsetArrow, y + Height - 8 + offsetArrow, Color.BLACK);
        Raylib.DrawLine(x + Width / 2 + offsetArrow, y + Height - 7 + offsetArrow, x + Width / 2 + 1 + offsetArrow, y + Height - 7 + offsetArrow, Color.BLACK);

        // Slider
        Raylib.DrawLine(x, y + sliderOffset, x + Width - 1, y + sliderOffset, new Color(227, 227, 227, 255));
        Raylib.DrawLine(x + 1, y + sliderOffset, x + 1, y + 16 + sliderOffset, new Color(227, 227, 227, 255));
        Raylib.DrawLine(x, y + 16 + sliderOffset, x + Width, y + 16 + sliderOffset, new Color(105, 105, 105, 255));
        Raylib.DrawLine(x + Width, y + sliderOffset, x + Width, y + 16 + sliderOffset, new Color(105, 105, 105, 255));
        Raylib.DrawLine(x + 2, y + 1 + sliderOffset, x + 2, y + 15 + sliderOffset, new Color(255, 255, 255, 255));
        Raylib.DrawLine(x + 2, y + 1 + sliderOffset, x + Width - 2, y + 1 + sliderOffset, new Color(255, 255, 255, 255));
        Raylib.DrawLine(x + 1, y + 15 + sliderOffset, x + Width - 2, y + 15 + sliderOffset, new Color(160, 160, 160, 255));
        Raylib.DrawLine(x + Width - 1, y + 1 + sliderOffset, x + Width - 1, y + 16 + sliderOffset, new Color(160, 160, 160, 255));

        if (_barTopPressed)
        {
            Raylib.DrawRectangle(x, y + 18, Width, sliderOffset - 17, Color.BLACK);
        }
        if (_barBtmPressed)
        {
            Raylib.DrawRectangle(x, y + sliderOffset + 18, Width, Height - sliderOffset - 2 * 17, Color.BLACK);
        }
    }
}
