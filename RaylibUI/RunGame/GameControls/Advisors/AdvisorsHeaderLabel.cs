using Model.Controls;
using Raylib_CSharp.Colors;
using RaylibUI.BasicTypes.Controls;

namespace RaylibUI.RunGame.GameControls;

public class AdvisorsHeaderLabel(
    IControlLayout layout,
    string text)
    : LabelControl(layout, text, true, horizontalAlignment: HorizontalAlignment.Center,
        font: layout.MainWindow.ActiveInterface.Look.DefaultFont, fontSize: 20, spacing: 0f,
        colorFront: new Color(223, 223, 223, 255),
        colorShadow: new Color(67, 67, 67, 255),
        shadowOffset: new System.Numerics.Vector2(2, 1));