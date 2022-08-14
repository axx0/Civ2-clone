using System;
using Eto.Drawing;
using Eto.Forms;

namespace EtoFormsUI.Menu;

public class Civ2GoldMenu : InterfaceStyle
{
    private PicturePanel _sinaiPanel;

    public Civ2GoldMenu(string path) : base("Civilization II Multiplayer Gold", path)
    {
    }

    public override void DrawIntroScreen(PixelLayout layout)
    {
        var imgV = new ImageView { Image = Images.ExtractBitmap(DLLs.Tiles, "introScreenSymbol") };
        layout.Add(imgV, (int)Screen.PrimaryScreen.Bounds.Width / 2 - imgV.Image.Width / 2, 
            (int)Screen.PrimaryScreen.Bounds.Height / 2 - imgV.Image.Height / 2);
    }

    public override void ShowMainMenuDecoration(PixelLayout layout)
    {
        _sinaiPanel = new PicturePanel(this, Images.ExtractBitmap(DLLs.Intro, "sinaiPic"));
        layout.Add(_sinaiPanel, new Point(160, 76));
    }

    public override void ClearMainMenuDecoration()
    {
        _sinaiPanel.Dispose();
    }
}