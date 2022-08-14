using System;
using Eto.Forms;

namespace EtoFormsUI.Menu;

public class TestOfTimeMenu : InterfaceStyle
{
    public TestOfTimeMenu(string path) : base("Test of Time", path)
    {
    }

    public override void DrawIntroScreen(PixelLayout pixelLayout)
    {
        //TODO: TOT intro image
    }

    public override void ShowMainMenuDecoration(PixelLayout layout)
    {
        //TODO: TOT main menu images
    }

    public override void ClearMainMenuDecoration()
    {
    }
}