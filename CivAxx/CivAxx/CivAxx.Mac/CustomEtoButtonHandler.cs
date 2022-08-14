using Eto.Drawing;
using Eto.Forms;
using EtoFormsUI;

namespace CivAxx.Mac;

public class CustomEtoButtonHandler : Eto.Mac.Forms.Controls.ButtonHandler, CustomEtoButton.IHandler
{
    public Image BackgroundImage 
    {
        get => Widget.Image;
        set
        {
            //These should be set in constructor but widget is null, should check for init method
            Widget.TextColor = Colors.Black;
            Widget.ImagePosition = ButtonImagePosition.Overlay;
            Widget.Image = value;
        }
    }
}