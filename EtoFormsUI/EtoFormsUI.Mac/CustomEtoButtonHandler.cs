using Eto;
using Eto.Mac;
using Eto.Drawing;

namespace EtoFormsUI.Mac
{   
    public class CustomEtoButtonHandler : Eto.Mac.Forms.Controls.ButtonHandler, CustomEtoButton.IHandler
    {
        public CustomEtoButtonHandler()
        {
        }

        public Image BackgroundImage 
        {
            get => Widget.Image;
            set => Widget.Image = value;
        }
    }
}
