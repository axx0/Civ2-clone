using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusLabel(
    IControlLayout layout,
    string text,
    TextAlignment alignment = TextAlignment.Left,
    Color[]? switchColors = null,
    int switchTime = 0,
    int fontSize = 18)
    : LabelControl(layout, text, true, alignment: alignment, defaultHeight: 18,
        font: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFont, fontSize: fontSize, spacing: 0f,
        colorFront: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColor,
        colorShadow: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColorShadow,
        shadowOffset: new System.Numerics.Vector2(1, 1), switchColors: switchColors, switchTime: switchTime);