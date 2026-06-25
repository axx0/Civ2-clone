using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class StatusLabel(
    IControlLayout layout,
    string text,
    HorizontalAlignment alignment = HorizontalAlignment.Left, int fontSize = 18)
    : LabelControl(layout, text, true, horizontalAlignment: alignment,
        font: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelFont, fontSize: fontSize, spacing: 0f,
        colorFront: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColor,
        colorShadow: layout.MainWindow.ActiveInterface.Look.StatusPanelLabelColorShadow,
        shadowOffset: new System.Numerics.Vector2(1, 1));