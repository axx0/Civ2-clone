using System.Numerics;
using Model.Controls;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

/// <summary>
/// Status panel label whose text colour alternates between the given colours every <paramref name="blinkInterval"/> seconds
/// (e.g. the "End of Turn (Press ENTER)" message)
/// </summary>
public class BlinkingStatusLabel(
    IControlLayout layout,
    string text,
    Color[] colors,
    double blinkInterval,
    int fontSize = 18)
    : LabelControl(layout, text, true,
        font: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFont, fontSize: fontSize, spacing: 0f,
        colorFront: colors[0],
        colorShadow: layout.MainWindow.ActiveInterface.Look.MovingUnitsViewingPiecesLabelColorShadow,
        shadowOffset: new Vector2(1, 1))
{
    public override void Draw(bool pulse)
    {
        // Time based (not frame based) so the blink rate doesn't depend on the monitor refresh rate
        ColorFront = colors[(int)(Time.GetTime() / blinkInterval) % colors.Length];
        base.Draw(pulse);
    }
}
